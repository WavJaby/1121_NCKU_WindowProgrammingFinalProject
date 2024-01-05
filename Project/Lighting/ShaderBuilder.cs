using Silk.NET.OpenGL;
using Shader = Project.lib.Shader;

namespace Project.Lighting;

public class ShaderBuilder {
    private struct ShaderObject {
        public string? Source;
        public uint? Handle;
        public ShaderType ShaderType;
    }

    private readonly GL _gl;
    private List<ShaderObject> _shaders = new();

    public ShaderBuilder(GL gl) {
        _gl = gl;
    }

    public ShaderBuilder AddFile(ShaderType shaderType, string sourcePath) {
        _shaders.Add(new ShaderObject {
            Source = File.ReadAllText(sourcePath),
            Handle = null,
            ShaderType = shaderType
        });
        return this;
    }

    public ShaderBuilder AddSource(ShaderType shaderType, string source) {
        _shaders.Add(new ShaderObject {
            Source = source,
            Handle = null,
            ShaderType = shaderType
        });
        return this;
    }

    public ShaderBuilder AddHandle(ShaderType shaderType, string source) {
        _shaders.Add(new ShaderObject {
            Source = source,
            Handle = null,
            ShaderType = shaderType
        });
        return this;
    }

    public Shader Build() {
        uint handle = _gl.CreateProgram();

        List<uint> freeList = new ();
        foreach (var shaderObject in _shaders) {
            uint shaderHandle;
            if (shaderObject.Source != null) {
                shaderHandle = Shader.LoadShader(_gl, shaderObject.ShaderType, shaderObject.Source);
                freeList.Add(shaderHandle);
            } else
                shaderHandle = (uint) shaderObject.Handle!;
            _gl.AttachShader(handle, shaderHandle);
        }
        _gl.LinkProgram(handle);
        _gl.GetProgram(handle, GLEnum.LinkStatus, out int status);
        if (status == 0) {
            throw new Exception($"Program failed to link with error: {_gl.GetProgramInfoLog(handle)}");
        }
        foreach (uint shaderHandle in freeList) {
            _gl.DeleteShader(shaderHandle);
        }

        return new Shader(_gl, handle);
    }
}