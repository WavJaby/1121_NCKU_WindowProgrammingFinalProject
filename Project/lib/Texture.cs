using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace Project.lib;

public class Texture : IDisposable {
    private readonly GL _gl;
    private readonly uint _handle;

    public readonly string Path;
    public readonly TextureType Type;

    public unsafe Texture(GL gl, string path, TextureType type = TextureType.None) {
        _gl = gl;
        Path = path;
        Type = type;
        _handle = _gl.GenTexture();
        Bind();

        using (var img = Image.Load<Rgba32>(path)) {
            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, (uint) img.Width, (uint) img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);

            img.ProcessPixelRows(accessor => {
                for (int y = 0; y < accessor.Height; y++) {
                    fixed (void* data = accessor.GetRowSpan(y)) {
                        gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, y, (uint) accessor.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                    }
                }
            });
        }

        SetParameters();
    }

    public unsafe Texture(GL gl, Span<byte> data, uint width, uint height) {
        _gl = gl;

        _handle = _gl.GenTexture();
        Bind();

        fixed (void* d = &data[0]) {
            _gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
            SetParameters();
        }
    }

    private void SetParameters() {
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.ClampToEdge);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.ClampToEdge);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.None);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.None);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
        _gl.GenerateMipmap(TextureTarget.Texture2D);
    }

    public void Bind(TextureUnit textureSlot = TextureUnit.Texture0) {
        _gl.ActiveTexture(textureSlot);
        _gl.BindTexture(TextureTarget.Texture2D, _handle);
    }

    public void Unbind() {
        _gl.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Dispose() {
        _gl.DeleteTexture(_handle);
    }
}