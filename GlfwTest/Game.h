#include "shader_s.h"
#include <glad/glad.h>
#include <GLFW/glfw3.h>
#pragma once
void InitializeGlfw();
void Render(_Notnull_ GLFWwindow* window, _Notnull_ Shader shaderProgram, _Notnull_ Shader vnShader, _Notnull_ unsigned int VAO, unsigned int vVAO);
void Update(_Notnull_ GLFWwindow* window);
void framebuffer_size_callback(GLFWwindow* window, int width, int height);
void processInput(GLFWwindow* window);

// settings
const unsigned int SCR_WIDTH = 800;
const unsigned int SCR_HEIGHT = 600;

