/*
% Simple shiny lighting shader
*/

string description = "A Simple Shader that displays the object with a texture and basic directional lighting";

float4x4 WorldViewProj : WorldViewProjection;
float4x4 world : World;
float4x4 viewInverse : ViewInverse;

float3 lightDir : Direction
<
    string Object = "DirectionalLight";
    string Space = "World";
> = { 0, 1, 0 };

float4 ambientColor : Ambient = { 0.4f, 0.4f, 0.4f, 1.0f };
float4 diffuseColor : Diffuse = { 0.5f, 0.5f, 0.5f, 1.0f };
float4 specularColor : Specular = { 1.0f, 1.0f, 1.0f, 1.0f };
float shininess : SpecularPower = 48.0f;

texture diffuseTexture : Diffuse
<
    string ResourceName = "marble.dds";
>;

sampler DiffuseTextureSampler = sampler_state
{
    Texture = <diffuseTexture>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};

struct VertexInput
{
    float3 pos : POSITION;
    float2 texCoord : TEXCOORD0;
    float3 normal : NORMAL;
};

struct VertexOutput
{
    float4 pos : POSITION;
    float2 texCoord : TEXCOORD0;
    float3 normal : TEXCOORD1;
    float3 halfVec : TEXCOORD2;
};

VertexOutput VS_SpecularPerPixel(VertexInput In)
{
    VertexOutput Out = (VertexOutput)0;
    float4 pos = float4(In.pos, 1);
    Out.pos = mul(pos, WorldViewProj);
    Out.texCoord = In.texCoord;
    Out.normal = mul(In.normal, world);
    
    //Eye pos
    float3 eyePos = viewInverse[3];
    //World pos
    float3 worldPos = mul(pos, world);
    //Eye vector
    float3 eyeVector = normalize(eyePos - worldPos);
    //Half vector
    Out.halfVec = normalize(eyeVector + lightDir);
    
    return Out;
}

float4 PS_SpecularPerPixel(VertexOutput In) : COLOR
{
    float4 textureColor = tex2D(DiffuseTextureSampler, In.texCoord);
    float3 normal = normalize(In.normal);
    float brightness = dot(normal, lightDir);
    float specular = pow(dot(normal, In.halfVec), shininess);
    return textureColor *
           (ambientColor + brightness * diffuseColor) +
           specular * specularColor;
}

technique SpecularPerPixel {
	pass p0 
  {
	    VertexShader = compile vs_2_0 VS_SpecularPerPixel();
	    PixelShader = compile ps_2_0 PS_SpecularPerPixel();
	}
}
