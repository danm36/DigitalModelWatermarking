#version 330

in vec3 vNormal;
in vec3 vWNormal;

uniform int isWireframe;

void main(void)
{
	//vec3 col = abs(vNormal);
	vec3 col = vec3(dot(vWNormal, normalize(vec3(-0.75, 0.75, 1))));

	if(isWireframe != 0)
		col *= -1;

	gl_FragColor = vec4(col, 1);
}