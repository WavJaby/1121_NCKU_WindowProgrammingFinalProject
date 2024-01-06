using System.Numerics;
using Project.GameHelper;
using Project.lib;
using Project.Lighting;
using Silk.NET.OpenGL;

namespace Project.defaultObjects.UI;

public class Image : UIGameObject {
    private static readonly float[] Vertices = {
        //X    Y      Z     UV
        +1.0f, +1.0f, 0.0f, 1.0f, 0.0f,
        +1.0f, -1.0f, 0.0f, 1.0f, 1.0f,
        -1.0f, -1.0f, 0.0f, 0.0f, 1.0f,
        -1.0f, +1.0f, 0.0f, 0.0f, 0.0f,
    };

    private static readonly uint[] Indices = {
        0, 1, 3,
        1, 2, 3
    };
    
    private readonly GL _gl;
    private readonly UIShader _shader;
    private readonly Mesh _mesh;
    private readonly Material _material;

    public Image(GL gl, Material material) {
        _gl = gl;
        _shader = new UIShader(gl);
        _mesh = new Mesh(_gl, Vertices, Indices, false);
        _material = material;
    }
    
    public override unsafe void Render(Scene scene) {
        _mesh.Bind();
        _shader.Use();
        _material.Bind();
        _shader.SetUniform("uModel", Matrix4x4.Identity);
        _shader.SetMaterial(_material);
        scene.ApplyCameraUI(_shader);
        _gl.DrawElements(PrimitiveType.Triangles, (uint) Indices.Length, DrawElementsType.UnsignedInt, (void*) 0);

        _material.Unbind();
        _shader.Unbind();
    }

    public override void Dispose() {
        _mesh.Dispose();
        _shader.Dispose();
    }
}