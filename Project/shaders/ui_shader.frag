#version 330 core

in vec3 fPos;
in vec2 fTexCoords;

struct Material {
	vec4 color;
	bool useDiffuse;
	sampler2D diffuse;
};

uniform Material material;

out vec4 FragColor;

void main() {
	vec4 diffuseTexture = vec4(1);
	if (material.useDiffuse)
	diffuseTexture = texture(material.diffuse, fTexCoords);
	FragColor = material.color * diffuseTexture;
}
