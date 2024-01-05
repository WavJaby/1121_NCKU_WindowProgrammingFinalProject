using System.Numerics;
using Project.GameHelper;
using Project.lib;
using Project.Lighting;
using Silk.NET.OpenGL;
using Shader = Project.lib.Shader;

namespace Project.defaultObjects;

public class Line : GameObject {
    private const string FragmentShader = @"
#version 330 core
out vec4 FragColor;
uniform vec3 lineColor;
void main() {
    FragColor = vec4(lineColor, 1.0f);
}";

    private const string GeometryShader = @"
#version 330 core
layout(lines) in;
layout(triangle_strip, max_vertices = 4) out;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform float lineWidth; // Width of the line

void main() {
    vec3 dir = gl_in[1].gl_Position.xyz - gl_in[0].gl_Position.xyz;
    vec3 normal = normalize(vec3(-dir.y, dir.x, 0.0)); // Perpendicular to the line direction
    normal *= lineWidth / 2.0;

    gl_Position = projection * view * model * vec4(gl_in[0].gl_Position.xyz + normal, 1.0);
    EmitVertex();
    gl_Position = projection * view * model * vec4(gl_in[0].gl_Position.xyz - normal, 1.0);
    EmitVertex();
    gl_Position = projection * view * model * vec4(gl_in[1].gl_Position.xyz + normal, 1.0);
    EmitVertex();
    gl_Position = projection * view * model * vec4(gl_in[1].gl_Position.xyz - normal, 1.0);
    EmitVertex();

    EndPrimitive();
}";

    private readonly GL _gl;
    private readonly Shader _shader;
    private readonly Mesh _mesh;

    public Line(GL gl, Vector3 begin, Vector3 end) {
        _gl = gl;
        _shader = new ShaderBuilder(gl)
            .AddFile(ShaderType.VertexShader, DefaultShader.DefaultVertexShaderSourcePath)
            .AddSource(ShaderType.FragmentShader, FragmentShader)
            .AddSource(ShaderType.GeometryShader, GeometryShader)
            .Build();
        float[] vertices = {
            begin.X, begin.Y, begin.Z,
            //Normal    UV
            0.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            end.X, end.Y, end.Z,
            0.0f, 0.0f, 0.0f, 1.0f, 0.0f,
        };

        _mesh = new Mesh(_gl, vertices, null);
    }

    public override void Render(Scene scene) {
        _mesh.Bind();
        _shader.Use();
        _shader.SetUniform("uModel", TransformMatrix);
        _shader.SetUniform("uView", scene.Camera.ViewMatrix);
        _shader.SetUniform("uProjection", scene.Camera.ProjectionMatrix);
        _shader.SetUniform("lineColor", new Vector3(1, 0, 0));
        _shader.SetUniform("lineWidth", 1);

        _gl.DrawArrays(PrimitiveType.Lines, 0, 2);

        _shader.Unbind();
    }

    public override void Dispose() {
        _mesh.Dispose();
        _shader.Dispose();
    }
}