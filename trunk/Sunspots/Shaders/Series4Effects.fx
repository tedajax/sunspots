//----------------------------------------------------
//--                                                --
//--               www.riemers.net                  --
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

//------- Texture Samplers --------

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

//------- Technique: Colored --------

struct ColVertexToPixel
{
    float4 Position   	: POSITION;    
    float4 Color		: COLOR0;
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
        PixelShader  = compile ps_1_1 ColoredPS();
    }
}

//------- Technique: Textured --------

struct TexVertexToPixel
{
    float4 Position   		: POSITION;    
    float4 Color			: COLOR0;
    float3 Normal			: TEXCOORD0;
    float2 TextureCoords	: TEXCOORD1;
    float4 LightDirection	: TEXCOORD2;

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
        PixelShader  = compile ps_1_1 TexturedPS();
    }
}

