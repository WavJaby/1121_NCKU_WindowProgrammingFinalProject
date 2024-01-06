#version 330 core
layout (location = 0) in vec3 vPos;
layout (location = 1) in vec2 vTexCoords;

uniform mat4 uProjection;
uniform mat4 uModel;

out vec3 fPos;
out vec2 fTexCoords;

void main() {
	gl_Position = uProjection * uModel * vec4(vPos, 1.0);
	fPos = vec3(uModel * vec4(vPos, 1.0));
	fTexCoords = vTexCoords;
}