#version 330 core
out vec4 FragColor;

struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;    
    float shininess;
}; 

struct PointLight {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

	// attenuation parameters
	float constant; // a
	float linear; // b
	float quadratic; // c
};
struct DirectionalLight {
vec3 direction;
vec3 ambient;
vec3 diffuse;
vec3 specular;
};

struct SpotLight {
vec3 position;
    vec3 direction;
    float cutOff;
    float outerCutOff;
  
    float constant;
    float linear;
    float quadratic;
  
    vec3 ambient;
    vec3 diffuse;
    vec3 specular; 
};


in vec3 FragPos;  
in vec3 Normal;  
  
uniform vec3 viewPos;
uniform Material material;
#define MAX_POINT_LIGHTS 10
uniform PointLight pointLights[MAX_POINT_LIGHTS];
uniform DirectionalLight dirLight;
uniform SpotLight spotLight;
uniform bool blinn = true;

vec3 PointLightColor(
PointLight light,
vec3 viewDir,
vec3 FragPos,
vec3 Normal
);
vec3 DirectionalLightColor(
DirectionalLight light,
vec3 viewDirection,
vec3 fragmentNormal
);

vec3 SpotLightColor(
SpotLight light,
vec3 viewDirection,
vec3 fragmentPosition,
vec3 fragmentNormal
);

//vec3 SoftSpotLightColor(
//SoftSpotLight light,
//vec3 viewDirection,
//vec3 fragmentPosition,
//vec3 fragmentNormal
//);

void main()
{
	vec3 viewDir = normalize(viewPos - FragPos);
	vec3 result = vec3(0.0);
	for(int i = 0; i < MAX_POINT_LIGHTS; i++)
	{
		result += PointLightColor(pointLights[i], viewDir,
		FragPos, Normal);
	}
	// result = DirectionalLightColor(dirLight,viewDir,Normal);
	// result = SpotLightColor(spotLight,viewDir,FragPos,Normal);
	//result += PointLightColor(pointLights[0], viewDir,
	//	FragPos, Normal);
	FragColor =  vec4(result,1.0);
}

vec3 PointLightColor(PointLight light,vec3 viewDir,vec3 FragPos,vec3 Normal)
{
	const float pi = 3.14159265;
	float distance = length(light.position - FragPos);
	float attenuation = 1.0 /
	(
		light.constant +
		light.linear * distance +
		light.quadratic * (distance * distance)
	);
	vec3 alAmbient = light.ambient * attenuation;
	vec3 alDiffuse = light.diffuse * attenuation;
	vec3 alSpecular = light.specular * attenuation;
    // ambient
    vec3 ambient = alAmbient * material.ambient;
  	
    // diffuse 
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(light.position - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = alDiffuse * (diff * material.diffuse);
	//specular
	float specular = 0;
	//blinn-Phong specular
    if( blinn )
	{
		float energyFactor = ( 8.0 + material.shininess ) / ( 8.0 * pi );
		vec3 halfwayDirection = normalize(lightDir + viewDir);
		float spec = max(dot(Normal, halfwayDirection), 0.0);
		specular = energyFactor * pow(spec,material.shininess);
	}
	//Phong specular
	else
	{
		float energyFactor = ( 8.0 + material.shininess ) / ( 8.0 * pi );
		vec3 reflectDirection = reflect(-lightDir, Normal);
		float spec = max(dot(viewDir, reflectDirection), 0.0);
		specular = energyFactor * pow(spec, material.shininess);
		
	}
	vec3 specularColor = specular * material.specular * alSpecular;
    vec3 result = ambient + diffuse + specularColor;
	//return vec3(attenuation);
	return result;
}
vec3 DirectionalLightColor(
DirectionalLight light,
vec3 viewDirection,
vec3 fragmentNormal
)
{
	vec3 lightDir = normalize(-light.direction);
	float diff = max(dot(fragmentNormal,lightDir),0.0);
	vec3 reflectDir = reflect(-lightDir,fragmentNormal);
	float spec = pow(max(dot(viewDirection,reflectDir),0.0),material.shininess);
	vec3 ambient = light.ambient * material.ambient;
	vec3 diffuse = light.diffuse * diff * material.diffuse;
	vec3 specular =  light.specular * spec * material.specular;
	return (ambient+diffuse+specular);
}
vec3 SpotLightColor(
SpotLight light,
vec3 viewDirection,
vec3 fragmentPosition,
vec3 fragmentNormal
){
	vec3 lightDir = normalize(light.position - fragmentPosition);
	float diff = max(dot(fragmentNormal,lightDir),0.0);
	vec3 reflectDir = reflect(-lightDir,fragmentNormal);
	float spec = pow(max(dot(viewDirection,reflectDir),0.0),material.shininess);
	float distance = length(light.position - fragmentPosition);
	float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
	float theta = dot(lightDir,normalize(-light.direction));
	float epsilon = light.cutOff - light.outerCutOff;
	float intensity = clamp((theta-light.outerCutOff)/ epsilon,0.0,1.0);
	vec3 ambient = light.ambient * material.ambient;
	vec3 diffuse = light.diffuse * diff * material.diffuse;
	vec3 specular = light.specular * spec * material.specular;
	ambient *= attenuation * intensity;
	diffuse *= attenuation * intensity;
	specular *= attenuation * intensity;
	return (ambient + diffuse + specular);
}