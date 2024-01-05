using System.Numerics;
using Silk.NET.OpenGL;

namespace Project.lib;

public class Shader : IDisposable {
    protected GL Gl { get; }

    private readonly uint _handle;

    public Shader(GL gl, string vertexShaderSrc, string fragmentShaderSrc) {
        Gl = gl;

        uint vertex = LoadShader(gl, ShaderType.VertexShader, vertexShaderSrc);
        uint fragment = LoadShader(gl, ShaderType.FragmentShader, fragmentShaderSrc);
        _handle = Gl.CreateProgram();
        Gl.AttachShader(_handle, vertex);
        Gl.AttachShader(_handle, fragment);
        Gl.LinkProgram(_handle);
        Gl.GetProgram(_handle, GLEnum.LinkStatus, out int status);
        if (status == 0) {
            throw new Exception($"Program failed to link with error: {Gl.GetProgramInfoLog(_handle)}");
        }
        Gl.DetachShader(_handle, vertex);
        Gl.DetachShader(_handle, fragment);

        Gl.DeleteShader(vertex);
        Gl.DeleteShader(fragment);
    }

    public Shader(GL gl, uint vertexShaderHandle, uint fragmentShaderHandle) {
        Gl = gl;

        _handle = Gl.CreateProgram();
        Gl.AttachShader(_handle, vertexShaderHandle);
        Gl.AttachShader(_handle, fragmentShaderHandle);
        Gl.LinkProgram(_handle);
        Gl.GetProgram(_handle, GLEnum.LinkStatus, out int status);
        if (status == 0) {
            throw new Exception($"Program failed to link with error: {Gl.GetProgramInfoLog(_handle)}");
        }
        Gl.DetachShader(_handle, vertexShaderHandle);
        Gl.DetachShader(_handle, fragmentShaderHandle);
    }

    public Shader(GL gl, uint handle) {
        Gl = gl;
        _handle = handle;
    }

    public void Use() {
        Gl.UseProgram(_handle);
    }

    public void Unbind() {
        Gl.UseProgram(0);
    }

    public unsafe void SetUniform(string name, Matrix4x4 value) {
        int location = GetUniformLocation(name);
        if (location == -1) return;
        Gl.UniformMatrix4(location, 1, false, (float*) &value);
    }

    public void SetUniform(string name, int value) {
        int location = GetUniformLocation(name);
        if (location == -1) return;
        Gl.Uniform1(location, value);
    }

    public void SetUniform(string name, float value) {
        int location = GetUniformLocation(name);
        if (location == -1) return;
        Gl.Uniform1(location, value);
    }

    public void SetUniform(string name, Vector3 value) {
        int location = GetUniformLocation(name);
        if (location == -1) return;
        Gl.Uniform3(location, value.X, value.Y, value.Z);
    }

    public void SetUniform(string name, Vector4 value) {
        int location = GetUniformLocation(name);
        if (location == -1) return;
        Gl.Uniform4(location, value.X, value.Y, value.Z, value.W);
    }

    public void SetUniform(string name, Vector2 value) {
        int location = GetUniformLocation(name);
        if (location == -1) return;
        Gl.Uniform2(location, value.X, value.Y);
    }

    public void SetUniform(string name, bool value) {
        int location = GetUniformLocation(name);
        if (location == -1) return;
        Gl.Uniform1(location, value ? 1 : 0);
    }

    public int GetUniformLocation(string name) {
        int location = Gl.GetUniformLocation(_handle, name);
        if (location == -1)
            Console.WriteLine($"WARN: {name} uniform not found on shader.");
        return location;
    }

    public static uint LoadShader(GL gl, ShaderType type, string src) {
        uint handle = gl.CreateShader(type);
        gl.ShaderSource(handle, src);
        gl.CompileShader(handle);
        string infoLog = gl.GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(infoLog)) {
            throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
        }

        return handle;
    }

    public void Dispose() {
        Gl.DeleteProgram(_handle);
    }
}