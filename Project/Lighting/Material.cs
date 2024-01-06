using System.Numerics;
using Silk.NET.OpenGL;
using Color = System.Drawing.Color;
using Texture = Project.lib.Texture;

namespace Project.Lighting;

public struct Material {
    public readonly uint LightMask;
    public readonly Vector4 Color;
    public readonly Texture? Diffuse;
    public readonly Texture? Specular;
    public readonly float Shininess;

    public Material(Texture? diffuse, Texture? specular = null, float shininess = 0) {
        LightMask = 0b11;
        Color = Vector4.One;
        Diffuse = diffuse;
        Specular = specular;
        Shininess = shininess;
    }

    public Material(Vector4 color, float shininess, uint lightMask) {
        LightMask = lightMask;
        Color = color;
        Diffuse = null;
        Specular = null;
        Shininess = shininess;
    }

    public Material(Color color, float shininess) :
        this(new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f), shininess, 0b11) { }

    public Material(Color color, float shininess, uint lightMask) :
        this(new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f), shininess, lightMask) { }

    public readonly void Bind() {
        Diffuse?.Bind(TextureUnit.Texture0);
        Specular?.Bind(TextureUnit.Texture1);
    }

    public readonly void Unbind() {
        Diffuse?.Unbind();
        Specular?.Unbind();
    }
}