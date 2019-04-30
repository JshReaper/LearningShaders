#include <glad/glad.h>
#include <GLFW/glfw3.h>
#include <stb_image.h>

#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

#include "shader_s.h"

#pragma once
void InitializeGlfw();
void Render(_Notnull_ GLFWwindow* window, _Notnull_ Shader shaderProgram, _Notnull_ unsigned int VAO, unsigned int texture1, unsigned int texture2);
void Update(_Notnull_ GLFWwindow* window);
void framebuffer_size_callback(GLFWwindow* window, int width, int height);
void processInput(GLFWwindow* window);