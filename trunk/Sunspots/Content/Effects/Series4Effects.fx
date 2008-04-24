//----------------------------------------------------
//--                                                --
//--             www.riemers.net                 --
//--         Series 4: Advanced terrain             --
//--                 Shader code                    --
//--                                                --
//----------------------------------------------------

//------- Constants --------
float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;
float3 xLightDirection;
float xAmbient;
bool xEnableLighting;

float4x4 xReflectionView;
float xWaveLength;
float xWaveHeight;

float3 xCamPos;



//------- Texture Samplers --------

Texture xTexture;

sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};


 Texture xGrassTexture;

sampler GrassTextureSampler = sampler_state { texture = <xGrassTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};
Texture xSandTexture;

sampler SandTextureSampler = sampler_state { texture = <xSandTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};
Texture xRockTexture;

sampler RockTextureSampler = sampler_state { texture = <xRockTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};
Texture xSnowTexture;

sampler SnowTextureSampler = sampler_state { texture = <xSnowTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xReflectionMap;
sampler ReflectionSampler = sampler_state { texture = <xReflectionMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xRefractionMap;
sampler RefractionSampler = sampler_state { texture = <xRefractionMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xWaterBumpMap;
sampler WaterBumpMapSampler = sampler_state { texture = <xWaterBumpMap>; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};


//------- Technique: Colored --------

struct ColVertexToPixel
{
    float4 Position     : POSITION;    
    float4 Color        : COLOR0;
    float LightingFactor: TEXCOORD0;
};

struct ColPixelToFrame
{
    float4 Color : COLOR0;
};

ColVertexToPixel ColoredVS( float4 inPos : POSITION, float4 inColor: COLOR, float3 inNormal: NORMAL)
{
    ColVertexToPixel Output = (ColVertexToPixel)0;
    float4x4 preViewProjection = mul (xView, xProjection);
    float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
    
    Output.Position = mul(inPos, preWorldViewProjection);
    Output.Color = inColor;

    float3 Normal = normalize(mul(normalize(inNormal), xWorld));
    Output.LightingFactor = 1;
    if (xEnableLighting)
        Output.LightingFactor = saturate(dot(Normal, -xLightDirection)+xAmbient);
    
    return Output;    
}

ColPixelToFrame ColoredPS(ColVertexToPixel PSIn)
{
    ColPixelToFrame Output = (ColPixelToFrame)0;
    
    Output.Color = PSIn.Color*PSIn.LightingFactor;

    return Output;
}

technique Colored
{
    pass Pass0
    {
        VertexShader = compile vs_1_1 ColoredVS();
        PixelShader = compile ps_1_1 ColoredPS();
    }
}

//------- Technique: Textured --------

struct TexVertexToPixel
{
    float4 Position         : POSITION;    
    float4 Color            : COLOR0;
    float3 Normal            : TEXCOORD0;
    float2 TextureCoords    : TEXCOORD1;
    float4 LightDirection    : TEXCOORD2;

};

struct TexPixelToFrame
{
    float4 Color : COLOR0;
};

TexVertexToPixel TexturedVS( float4 inPos : POSITION, float3 inNormal: NORMAL, float2 inTexCoords: TEXCOORD0)
{
    TexVertexToPixel Output = (TexVertexToPixel)0;
    float4x4 preViewProjection = mul (xView, xProjection);
    float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
    
    Output.Position = mul(inPos, preWorldViewProjection);
    Output.Normal = mul(normalize(inNormal), xWorld);
    Output.TextureCoords = inTexCoords;
    Output.LightDirection.xyz = -xLightDirection;
    Output.LightDirection.w = 1;
    
    return Output;    
}

TexPixelToFrame TexturedPS(TexVertexToPixel PSIn)
{
    TexPixelToFrame Output = (TexPixelToFrame)0;
    
    float lightingFactor = 1;
    if (xEnableLighting)
        lightingFactor = saturate(saturate(dot(PSIn.Normal, PSIn.LightDirection)) + xAmbient);

    Output.Color = tex2D(TextureSampler, PSIn.TextureCoords)*lightingFactor;

    return Output;
}

technique Textured
{
    pass Pass0
    {
        VertexShader = compile vs_1_1 TexturedVS();
        PixelShader = compile ps_1_1 TexturedPS();
    }
}



 //------- Technique: MultiTextured --------
 
 struct MultiTexVertexToPixel
 {
     float4 Position         : POSITION;    
     float4 Color            : COLOR0;
     float3 Normal            : TEXCOORD0;
     float4 TextureCoords    : TEXCOORD1;
     float4 LightDirection    : TEXCOORD2;
     float4 TextureWeights    : TEXCOORD3;
 };
 
 struct MultiTexPixelToFrame
 {
     float4 Color : COLOR0;
 };
 
 MultiTexVertexToPixel MultiTexturedVS( float4 inPos : POSITION, float3 inNormal: NORMAL, float4 inTexCoords: TEXCOORD0, float4 inTexWeights: TEXCOORD1)
 {
     MultiTexVertexToPixel Output = (MultiTexVertexToPixel)0;
     float4x4 preViewProjection = mul (xView, xProjection);
     float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
     
     Output.Position = mul(inPos, preWorldViewProjection);
     Output.Normal = mul(normalize(inNormal), xWorld);
     Output.TextureCoords = inTexCoords;
     Output.LightDirection.xyz = -xLightDirection;
     Output.LightDirection.w = 1;
     Output.TextureWeights = inTexWeights;
     
     return Output;    
 }
 
 MultiTexPixelToFrame MultiTexturedPS(MultiTexVertexToPixel PSIn)
 {
     MultiTexPixelToFrame Output = (MultiTexPixelToFrame)0;
     
     float lightingFactor = 1;
     if (xEnableLighting)
         lightingFactor = saturate(saturate(dot(PSIn.Normal, PSIn.LightDirection)) + xAmbient);
 
     Output.Color = tex2D(SandTextureSampler, PSIn.TextureCoords)*PSIn.TextureWeights.x;
     Output.Color += tex2D(GrassTextureSampler, PSIn.TextureCoords)*PSIn.TextureWeights.y;
     Output.Color += tex2D(RockTextureSampler, PSIn.TextureCoords)*PSIn.TextureWeights.z;
     Output.Color += tex2D(SnowTextureSampler, PSIn.TextureCoords)*PSIn.TextureWeights.w;
 
     Output.Color *= saturate(lightingFactor + xAmbient);
 
     return Output;
 }
 
 technique MultiTextured
 {
     pass Pass0
     {
         VertexShader = compile vs_1_1 MultiTexturedVS();
         PixelShader = compile ps_2_0 MultiTexturedPS();
     }
 }
 
 //------- Technique: Water --------

struct WaterVertexToPixel
{
    float4 Position                 : POSITION;    
    float4 ReflectionMapSamplingPos    : TEXCOORD1;
    float2 BumpMapSamplingPos        : TEXCOORD2;
    float4 RefractionMapSamplingPos : TEXCOORD3;
    float4 Position3D                : TEXCOORD4;
};

struct WaterPixelToFrame
{
    float4 Color : COLOR0;
};

WaterVertexToPixel WaterVS(float4 inPos : POSITION, float2 inTex: TEXCOORD)
{
    WaterVertexToPixel Output = (WaterVertexToPixel)0;
    float4x4 preViewProjection = mul (xView, xProjection);
    float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
    float4x4 preReflectionViewProjection = mul (xReflectionView, xProjection);
    float4x4 preWorldReflectionViewProjection = mul (xWorld, preReflectionViewProjection);
    
    Output.Position = mul(inPos, preWorldViewProjection);
    Output.ReflectionMapSamplingPos = mul(inPos, preWorldReflectionViewProjection);
	Output.BumpMapSamplingPos = inTex/xWaveLength;
	
	Output.RefractionMapSamplingPos = mul(inPos, preWorldViewProjection);
	Output.Position3D = inPos;

	
    return Output;    
}

WaterPixelToFrame WaterPS(WaterVertexToPixel PSIn)
{
    WaterPixelToFrame Output = (WaterPixelToFrame)0;
    
    float2 ProjectedTexCoords;
    ProjectedTexCoords.x = PSIn.ReflectionMapSamplingPos.x/PSIn.ReflectionMapSamplingPos.w/2.0f + 0.5f;
    ProjectedTexCoords.y = -PSIn.ReflectionMapSamplingPos.y/PSIn.ReflectionMapSamplingPos.w/2.0f + 0.5f;

	float4 bumpColor = tex2D(WaterBumpMapSampler, PSIn.BumpMapSamplingPos);
	float2 perturbation = xWaveHeight*(bumpColor.rg - 0.5f);
	
	float2 perturbatedTexCoords = ProjectedTexCoords + perturbation;
	
	float4 reflectiveColor = tex2D(ReflectionSampler, perturbatedTexCoords);

	float2 ProjectedRefrTexCoords;
	ProjectedRefrTexCoords.x = PSIn.RefractionMapSamplingPos.x/PSIn.RefractionMapSamplingPos.w/2.0f + 0.5f;
	ProjectedRefrTexCoords.y = -PSIn.RefractionMapSamplingPos.y/PSIn.RefractionMapSamplingPos.w/2.0f + 0.5f;

	float2 perturbatedRefrTexCoords = ProjectedRefrTexCoords + perturbation;

	float4 refractiveColor = tex2D(RefractionSampler, perturbatedRefrTexCoords);

	float3 eyeVector = normalize(xCamPos - PSIn.Position3D);
	
	float3 normalVector = float3(0,1,0);
	
	float fresnelTerm = dot(eyeVector, normalVector);

	float4 combinedcolor = refractiveColor*fresnelTerm + reflectiveColor*(1-fresnelTerm);
	
	float4 dullcolor = float4(0.3f, 0.3f, 0.5f, 1.0f);
	
	float4 dullwaterblendfactor = 0.4f;
	
	Output.Color = dullwaterblendfactor *dullcolor +(1-dullwaterblendfactor)*combinedcolor;
    return Output;
}

technique Water
{
    pass Pass0
    {
        VertexShader = compile vs_1_1 WaterVS();
        PixelShader = compile ps_2_0 WaterPS();
    }
}
