using Project.GameHelper;
using Project.lib;
using Project.Lighting;
using Silk.NET.OpenGL;
using Camera = Project.GameHelper.Camera;
using Shader = Project.lib.Shader;
using Texture = Project.lib.Texture;

namespace Project.defaultObjects;

public class ModelObject : GameObject {
    private readonly GL _gl;
    private readonly Model _model;
    private readonly DefaultShader _shader;
    private readonly Material _material;

    public ModelObject(GL gl, string modelPath, Texture? texture) {
        _gl = gl;
        _model = new Model(gl, modelPath);
        _shader = new DefaultShader(gl);
        _material = new Material(texture, texture, 32f);
    }
    public ModelObject(GL gl, string modelPath, Texture? texture, Texture? specular) {
        _gl = gl;
        _model = new Model(gl, modelPath);
        _shader = new DefaultShader(gl);
        _material = new Material(texture, specular, 32f);
    }

    public override void Render(Scene scene) {
        _material.Bind();
        foreach (var mesh in _model.Meshes) {
            mesh.Bind();
            _shader.Use();
            _shader.SetUniform("uModel", TransformMatrix);
            _shader.SetMaterial(_material);
            scene.ApplyCameraAndLighting(_shader);
            _gl.DrawArrays(PrimitiveType.Triangles, 0, mesh.IndicesCount);

            _shader.Unbind();
        }
        _material.Unbind();
    }

    public override void Dispose() {
        _shader.Dispose();
        _model.Dispose();
    }
}