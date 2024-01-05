// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Silk.NET.OpenGL;

namespace Project.lib;

public class Mesh : IDisposable {
    private readonly GL _gl;

    private readonly float[] _vertices;
    private readonly uint[] _indices;
    private VertexArrayObject<float> _vao;
    private BufferObject<float> _vbo;
    private BufferObject<uint> _ebo;
    public uint IndicesCount => (uint) _indices.Length;
    
    public Mesh(GL gl, float[] vertices, uint[] indices) {
        _gl = gl;
        _vertices = vertices;
        _indices = indices;
        SetupMesh();
    }

    private void SetupMesh() {
        _vao = new VertexArrayObject<float>(_gl);
        _vbo = new BufferObject<float>(_gl, _vertices, BufferTargetARB.ArrayBuffer);
        _ebo = new BufferObject<uint>(_gl, _indices, BufferTargetARB.ElementArrayBuffer);
        _vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);
        _vao.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);
        _vao.VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);

        _vao.Unbind();
        _vbo.Unbind();
        _ebo.Unbind();
    }

    public void Bind() {
        _vao.Bind();
    }

    public void Dispose() {
        _vao.Dispose();
        _vbo.Dispose();
        _ebo.Dispose();
    }
}