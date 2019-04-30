#version 330 core
layout (location = 0) in vec3 aPos;
uniform mat4 modelWorld;
uniform mat4 worldView;
uniform mat4 viewClip;
void main()
{
gl_Position = viewClip * worldView * modelWorld * vec4(aPos, 1.0);
}