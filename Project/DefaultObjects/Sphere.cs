using Project.GameHelper;
using Project.lib;
using Project.Lighting;
using Silk.NET.OpenGL;

namespace Project.defaultObjects;

public class Sphere : GameObject {
    private readonly GL _gl;
    private readonly DefaultShader _shader;
    private readonly Mesh _mesh;
    private readonly Material _material;

    private readonly uint[] _indices;
    private readonly float[] _vertices;

    public Sphere(GL gl, Material material, int segmentX, int segmentY) {
        _gl = gl;
        _shader = new DefaultShader(gl);
        CreateSphereVerticesAndIndices(out _vertices, out _indices, Math.Max(segmentX, 3), Math.Max(segmentY, 1) + 1);
        _mesh = new Mesh(_gl, _vertices, _indices, true);
        _material = material;
    }

    public override unsafe void Render(Scene scene) {
        _mesh.Bind();
        _shader.Use();
        _material.Bind();
        _shader.SetUniform("uModel", TransformMatrix);
        _shader.SetMaterial(_material);
        scene.ApplyCameraAndLighting(_shader);
        _gl.DrawElements(PrimitiveType.Triangles, (uint) _indices.Length, DrawElementsType.UnsignedInt, (void*) 0);
        
        _material.Unbind();
        _shader.Unbind();
    }

    public override void Dispose() {
        _mesh.Dispose();
        _shader.Dispose();
    }

    static void CreateSphereVerticesAndIndices(out float[] sphereVertices, out uint[] sphereIndices, int segments, int rings) {
        List<float> vertexList = new List<float>();
        List<uint> indexList = new List<uint>();

        for (int i = 0; i <= rings; i++) {
            float phi = MathF.PI * i / rings;
            for (int j = 0; j <= segments; j++) {
                float theta = 2 * MathF.PI * j / segments;

                float x = 0.5f * MathF.Sin(phi) * MathF.Cos(theta);
                float y = 0.5f * MathF.Cos(phi);
                float z = 0.5f * MathF.Sin(phi) * MathF.Sin(theta);

                float u = (float) j / segments;
                float v = (float) i / rings;

                // Vertex
                vertexList.Add(x);
                vertexList.Add(y);
                vertexList.Add(z);
                // uv
                vertexList.Add(u);
                vertexList.Add(v);
                // Normal
                vertexList.Add(x);
                vertexList.Add(y);
                vertexList.Add(z);
            }
        }

        for (uint i = 0; i < rings; i++) {
            for (uint j = 0; j < segments; j++) {
                uint a = i * ((uint) segments + 1) + j;
                uint b = a + (uint) segments + 1;
                uint c = b + 1;
                uint d = a + 1;

                indexList.Add(a);
                indexList.Add(b);
                indexList.Add(d);

                indexList.Add(b);
                indexList.Add(c);
                indexList.Add(d);
            }
        }

        sphereVertices = vertexList.ToArray();
        sphereIndices = indexList.ToArray();
    }
}