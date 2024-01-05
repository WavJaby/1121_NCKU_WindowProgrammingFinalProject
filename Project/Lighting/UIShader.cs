using System.Numerics;
using Project.GameHelper;
using Project.lib;
using Silk.NET.OpenGL;
using Shader = Project.lib.Shader;

namespace Project.Lighting;

public class UIShader : Shader {
    public const string DefaultVertexShaderSourcePath = "shaders/ui_shader.vert";
    public const string DefaultFragmentShaderSourcePath = "shaders/ui_shader.frag";

    private static uint _defaultVertexShader, _defaultFragmentShader;

    // Setup the coordinate systems for our view
    private readonly int _uViewLocation;
    private readonly int _uProjectionLocation;

    private readonly int _viewPosLocation;

    // Configure the materials variables.
    private readonly int _materialColorLocation;
    private readonly int _materialUseDiffuseLocation;
    private readonly int _materialDiffuseLocation;

    public UIShader(GL gl) : base(gl, _defaultVertexShader, _defaultFragmentShader) {
        _uViewLocation = GetUniformLocation("uView");
        _uProjectionLocation = GetUniformLocation("uProjection");
        _viewPosLocation = GetUniformLocation("viewPos");

        _materialColorLocation = GetUniformLocation("material.color");
        _materialUseDiffuseLocation = GetUniformLocation("material.useDiffuse");
        _materialDiffuseLocation = GetUniformLocation("material.diffuse");
    }

    public unsafe void SetViewport(Camera camera) {
        Matrix4x4 viewMatrix = camera.ViewMatrix;
        Matrix4x4 projectionMatrix = camera.ProjectionMatrix;
        Gl.UniformMatrix4(_uViewLocation, 1, false, (float*) &viewMatrix);
        Gl.UniformMatrix4(_uProjectionLocation, 1, false, (float*) &projectionMatrix);
        Vector3 pos = camera.Position;
        Gl.Uniform3(_viewPosLocation, pos.X, pos.Y, pos.Z);
    }

    public void SetMaterial(Material material) {
        Gl.Uniform4(_materialColorLocation, material.Color.X, material.Color.Y, material.Color.Z, material.Color.W);
        Gl.Uniform1(_materialUseDiffuseLocation, material.Diffuse == null ? 0 : 1);
        Gl.Uniform1(_materialDiffuseLocation, 0);
    }

    public static void LoadDefaultShader(GL gl) {
        _defaultVertexShader = LoadShader(gl, ShaderType.VertexShader, File.ReadAllText(DefaultVertexShaderSourcePath));
        _defaultFragmentShader = LoadShader(gl, ShaderType.FragmentShader, File.ReadAllText(DefaultFragmentShaderSourcePath));
    }

    public static void DisposeDefaultShader(GL gl) {
        gl.DeleteShader(_defaultVertexShader);
        gl.DeleteShader(_defaultFragmentShader);
    }
}