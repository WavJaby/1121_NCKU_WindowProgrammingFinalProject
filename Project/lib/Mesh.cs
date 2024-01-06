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

    public Mesh(GL gl, float[] vertices, uint[] indices, bool normal) {
        _gl = gl;
        _vertices = vertices;
        _indices = indices;
        SetupMesh(normal);
    }

    private void SetupMesh(bool normal) {
        _vao = new VertexArrayObject<float>(_gl);
        _vbo = new BufferObject<float>(_gl, _vertices, BufferTargetARB.ArrayBuffer);
        _ebo = new BufferObject<uint>(_gl, _indices, BufferTargetARB.ElementArrayBuffer);

        uint vertexSize = normal ? 8u : 5u;
        _vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, vertexSize, 0);
        _vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, vertexSize, 3);
        if (normal)
            _vao.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, vertexSize, 5);

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