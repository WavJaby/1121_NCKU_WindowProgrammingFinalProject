using System.Numerics;
using Project.GameHelper;
using Project.lib;
using Silk.NET.OpenGL;
using Shader = Project.lib.Shader;

namespace Project.Lighting;

public class DefaultShader : Shader {
    public const string DefaultVertexShaderSourcePath = "shaders/object_shader.vert";
    public const string DefaultFragmentShaderSourcePath = "shaders/object_shader.frag";

    private static uint _defaultVertexShader, _defaultFragmentShader;

    // Setup the coordinate systems for our view
    private readonly int _uViewLocation;
    private readonly int _uProjectionLocation;

    private readonly int _viewPosLocation;

    // Configure the materials variables.
    private readonly int _materialColorLocation;
    private readonly int _materialUseDiffuseLocation;
    private readonly int _materialDiffuseLocation;
    private readonly int _materialUseSpecularLocation;
    private readonly int _materialSpecularLocation;
    private readonly int _materialShininessLocation;

    // // Add light sources
    // var pointLightColor = new Vector3(0, 1, 1);
    // int i = 0;
    //     foreach (var pointLight in _pointLightSources) {
    //     string pointPos = "pointLights[" + i + "]";
    //     shader.SetUniform(pointPos + ".localPos", pointLight);
    //     shader.SetUniform(pointPos + ".base.color", pointLightColor);
    //     shader.SetUniform(pointPos + ".base.ambientIntensity", 0.01f);
    //     shader.SetUniform(pointPos + ".base.diffuseIntensity", 1.0f);
    //     shader.SetUniform(pointPos + ".atten.constant", 0.7f);
    //     shader.SetUniform(pointPos + ".atten.linear", 0.0f);
    //     shader.SetUniform(pointPos + ".atten.exp", 0.5f);
    //     if (++i >= MaxPointLights)
    //         break;
    // }
    private int _pointLightsLengthLocation;
    private int _lightMaskLocation;

    // Sun light
    private readonly int _dirLightDirectionLocation;
    private readonly int _dirLightBaseColorLocation;
    private readonly int _dirLightBaseAmbientIntensityLocation;
    private readonly int _dirLightBaseDiffuseIntensityLocation;

    public DefaultShader(GL gl) : base(gl, _defaultVertexShader, _defaultFragmentShader) {
        _uViewLocation = GetUniformLocation("uView");
        _uProjectionLocation = GetUniformLocation("uProjection");
        _viewPosLocation = GetUniformLocation("viewPos");

        _materialColorLocation = GetUniformLocation("material.color");
        _materialUseDiffuseLocation = GetUniformLocation("material.useDiffuse");
        _materialDiffuseLocation = GetUniformLocation("material.diffuse");
        _materialUseSpecularLocation = GetUniformLocation("material.useSpecular");
        _materialSpecularLocation = GetUniformLocation("material.specular");
        _materialShininessLocation = GetUniformLocation("material.shininess");

        _lightMaskLocation = GetUniformLocation("lightMask");
        _pointLightsLengthLocation = GetUniformLocation("pointLightsLength");

        _dirLightDirectionLocation = GetUniformLocation("dirLight.direction");
        _dirLightBaseColorLocation = GetUniformLocation("dirLight.base.color");
        _dirLightBaseAmbientIntensityLocation = GetUniformLocation("dirLight.base.ambientIntensity");
        _dirLightBaseDiffuseIntensityLocation = GetUniformLocation("dirLight.base.diffuseIntensity");
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
        Gl.Uniform1(_materialUseSpecularLocation, material.Specular == null ? 0 : 1);
        Gl.Uniform1(_materialSpecularLocation, 1);
        Gl.Uniform1(_materialShininessLocation, material.Shininess);
        Gl.Uniform1(_lightMaskLocation, material.LightMask);
    }

    public void SetSun(Vector3 direction, Vector4 color, float ambient, float diffuse) {
        Gl.Uniform3(_dirLightDirectionLocation, direction.X, direction.Y, direction.Z);
        Gl.Uniform4(_dirLightBaseColorLocation, color.X, color.Y, color.Z, color.W);
        Gl.Uniform1(_dirLightBaseAmbientIntensityLocation, ambient);
        Gl.Uniform1(_dirLightBaseDiffuseIntensityLocation, diffuse);
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