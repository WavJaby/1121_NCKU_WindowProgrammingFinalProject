using System.Numerics;
using Project.GameHelper;
using Project.lib;
using Project.Lighting;
using Silk.NET.OpenGL;
using Shader = Project.lib.Shader;
using Texture = Project.lib.Texture;

namespace Project.defaultObjects;

public class Cube : GameObject {
    private static readonly float[] Vertices = {
        //X    Y      Z      UV          Normal
        // Front face
        -0.5f, -0.5f, +0.5f, 0.0f, 1.0f, 0.0f, 0.0f, +1.0f,
        +0.5f, -0.5f, +0.5f, 1.0f, 1.0f, 0.0f, 0.0f, +1.0f,
        +0.5f, +0.5f, +0.5f, 1.0f, 0.0f, 0.0f, 0.0f, +1.0f,
        -0.5f, +0.5f, +0.5f, 0.0f, 0.0f, 0.0f, 0.0f, +1.0f,
        // Back face
        -0.5f, -0.5f, -0.5f, 1.0f, 1.0f, 0.0f, 0.0f, -1.0f,
        +0.5f, -0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, -1.0f,
        +0.5f, +0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f,
        -0.5f, +0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 0.0f, -1.0f,
        // Right face
        +0.5f, -0.5f, +0.5f, 0.0f, 1.0f, +1.0f, 0.0f, 0.0f,
        +0.5f, -0.5f, -0.5f, 1.0f, 1.0f, +1.0f, 0.0f, 0.0f,
        +0.5f, +0.5f, -0.5f, 1.0f, 0.0f, +1.0f, 0.0f, 0.0f,
        +0.5f, +0.5f, +0.5f, 0.0f, 0.0f, +1.0f, 0.0f, 0.0f,
        // Left face
        -0.5f, -0.5f, +0.5f, 1.0f, 1.0f, -1.0f, 0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f, -1.0f, 0.0f, 0.0f,
        -0.5f, +0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f,
        -0.5f, +0.5f, +0.5f, 1.0f, 0.0f, -1.0f, 0.0f, 0.0f,
        // Top face
        -0.5f, +0.5f, +0.5f, 0.0f, 1.0f, 0.0f, +1.0f, 0.0f,
        +0.5f, +0.5f, +0.5f, 1.0f, 1.0f, 0.0f, +1.0f, 0.0f,
        +0.5f, +0.5f, -0.5f, 1.0f, 0.0f, 0.0f, +1.0f, 0.0f,
        -0.5f, +0.5f, -0.5f, 0.0f, 0.0f, 0.0f, +1.0f, 0.0f,
        // Bottom face
        -0.5f, -0.5f, +0.5f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f,
        +0.5f, -0.5f, +0.5f, 1.0f, 0.0f, 0.0f, -1.0f, 0.0f,
        +0.5f, -0.5f, -0.5f, 1.0f, 1.0f, 0.0f, -1.0f, 0.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f, 0.0f, -1.0f, 0.0f,
    };

    private static readonly uint[] Indices = {
        // Front face
        0, 1, 2,
        2, 3, 0,

        // Back face
        4, 5, 6,
        6, 7, 4,

        // Right face
        8, 9, 10,
        10, 11, 8,

        // Left face
        12, 13, 14,
        14, 15, 12,

        // Top face
        16, 17, 18,
        18, 19, 16,

        // Bottom face
        20, 21, 22,
        22, 23, 20,
    };

    private readonly GL _gl;
    private readonly DefaultShader _shader;
    private readonly Mesh _mesh;
    private readonly Material _material;

    public Cube(GL gl, Material material) {
        _gl = gl;
        _shader = new DefaultShader(gl);
        _mesh = new Mesh(_gl, Vertices, Indices, true);
        _material = material;
    }

    public override unsafe void Render(Scene scene) {
        _mesh.Bind();
        _shader.Use();
        _material.Bind();
        _shader.SetUniform("uModel", TransformMatrix);
        _shader.SetMaterial(_material);
        scene.ApplyCameraAndLighting(_shader);
        _gl.DrawElements(PrimitiveType.Triangles, (uint) Indices.Length, DrawElementsType.UnsignedInt, (void*) 0);

        _material.Unbind();
        _shader.Unbind();
    }

    public override void Dispose() {
        _mesh.Dispose();
        _shader.Dispose();
    }
}