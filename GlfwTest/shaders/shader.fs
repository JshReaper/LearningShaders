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

in vec3 FragPos;  
in vec3 Normal;  
  
uniform vec3 viewPos;
uniform Material material;
uniform PointLight pointLight;
uniform bool blinn = true;

vec3 PointLightColor(
PointLight light,
vec3 viewDir,
vec3 FragPos,
vec3 Normal
);

void main()
{
	vec3 viewDir = normalize(viewPos - FragPos);
	vec3 result = PointLightColor(pointLight,viewDir,FragPos,Normal);
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
	return result;
}