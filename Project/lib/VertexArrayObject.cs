using Silk.NET.OpenGL;

namespace Project.lib;

public class VertexArrayObject<TVertexType> : IDisposable
    where TVertexType : unmanaged {
    private readonly GL _gl;
    private readonly uint _handle;

    public VertexArrayObject(GL gl) {
        _gl = gl;

        _handle = _gl.GenVertexArray();
        Bind();
    }

    public unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offSet) {
        _gl.VertexAttribPointer(index, count, type, false, vertexSize * (uint) sizeof(TVertexType), (void*) (offSet * sizeof(TVertexType)));
        _gl.EnableVertexAttribArray(index);
    }

    public void Bind() {
        _gl.BindVertexArray(_handle);
    }

    public void Unbind() {
        _gl.BindVertexArray(0);
    }

    public void Dispose() {
        _gl.DeleteVertexArray(_handle);
    }
}