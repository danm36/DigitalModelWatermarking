#version 330

attribute vec3 aVertex;
attribute vec3 aNormal;

out vec3 vPosition;
out vec3 vNormal;
out vec3 vWNormal;

uniform mat4 uPVMMatrix;

void main(void)
{
	//gl_Position = uPVMMatrix * vec4(aVertex.x, aVertex.y, -aVertex.z, 1);
		
	vPosition = aVertex;
	vNormal = aNormal;
	vWNormal = normalize((uPVMMatrix * vec4(vNormal, 1)).xyz);
	gl_Position = uPVMMatrix * vec4(vPosition, 1);
}