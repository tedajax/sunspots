//////////////////////////////////////////////////////////////
//															//
// Dynamic sky sphere.										//
//															//
//	Writen by G. Subramanian     							//
//	9/11/2001												//
//  9:56 AM													//
//	At the WTC												//
//////////////////////////////////////////////////////////////

float4x4 World : World;
float4x4 View : View;
float4x4 Projection : Projection;

float4 SkyColor;		
float4 NightColor;		
float4 MorningTint;		
float4 EveningTint;		
float cloudTimer;
float cloudIntensity = 1;

float3 EyePosition : CameraPosition;

float timeOfDay;

Texture surfaceTexture;
samplerCUBE TextureSampler = sampler_state 
{ 
	texture = <surfaceTexture> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = Clamp;
	AddressV = Clamp;
};

texture cloud;
sampler cloudTexture = sampler_state
{ 
	texture = <cloud> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = Clamp;
	AddressV = Clamp;
};

float4 CubeMapLookup(float3 CubeTexcoord)
{    
    return texCUBE(TextureSampler, CubeTexcoord);
}

struct VS_INPUT 
{
	float2 TexCoord : TEXCOORD0;
	float4 Position	: POSITION0;
	float3 Normal : NORMAL;	
};

struct VS_OUTPUT 
{
	float2 TexCoord : TEXCOORD0;
	float4 Position	: POSITION0;
	float3 ViewDirection : TEXCOORD2;
	float3 Normal : TEXCOORD1;
};

VS_OUTPUT Transform(VS_INPUT Input)
{
	float4x4 WorldViewProjection = mul(mul(World, View), Projection);
	float3 ObjectPosition = mul(Input.Position, World);
	
	VS_OUTPUT Output;
	
	Output.Position	= mul(Input.Position, WorldViewProjection);		
	Output.ViewDirection = EyePosition - ObjectPosition;	
	
	Output.TexCoord = Input.TexCoord;
	Output.Normal = Input.Normal;
	
	return Output;
}

struct PS_INPUT 
{	
	float2 TexCoord : TEXCOORD0;
	float3 ViewDirection : TEXCOORD2;
	float3 Normal : TEXCOORD1;
};

float2 SwirlHoriz(float2 coord,bool clockwise)
{
	if(clockwise)
	{
		coord.x +=  cloudTimer;
		if(coord.x > 1)	
			coord.x = coord.x-1;
	}
	else
	{		
		coord.x -= cloudTimer;
		if(coord.x < 0)	
			coord.x = coord.x+1;
	}	
	
	return coord;
}
float2 SwirlVert(float2 coord,bool up)
{
	if(up)
	{
		coord.y -= cloudTimer;
		if(coord.y < 0)	
			coord.y = coord.y+1;
	}
	else
	{
		coord.y += cloudTimer;
		if(coord.y > 1)	
			coord.y = coord.y-1;
	}
	
	return coord;
}

float4 col;

float4 BasicShader(PS_INPUT Input) : COLOR0
{	
	float3 ViewDirection = normalize(Input.ViewDirection);	
	
	col = 0;
	float4 ncol = 0;
	float4 stars = CubeMapLookup(-ViewDirection);	
	
	col = (SkyColor + (1,1,1,1)) * Input.TexCoord.y;
	ncol = NightColor * (4 - Input.TexCoord.y) * .125f;

	stars *= .05;		
	
	if(timeOfDay <= 12)
	{
		col *= timeOfDay / 12;	
		stars *= ((timeOfDay - 24) / -24);		
		ncol *= timeOfDay / 12;
	}
	else
	{
		col *= (timeOfDay - 24) / -12;				
		stars *= timeOfDay / 24;
		ncol *= (timeOfDay - 24) / -12;
	}
	
	col += (MorningTint * .05) * ((24 - timeOfDay)/24);
	col += (EveningTint * .05) * (timeOfDay / 24);	
	col += ncol;
		
	float starGlow = .25;
	if(col.r <= starGlow && col.g <= starGlow && col.b <= starGlow)	
		col += stars * (80 * ((starGlow*3) - (col.r+col.g+col.b)));
		
	col += stars;
	
	float4 cloudCol0 = 0;
	float4 cloudCol1 = 0;
	float4 cloudCol2 = 0;

	float2 cloudTex0 = Input.TexCoord;
	float2 cloudTex1 = Input.TexCoord;
	float2 cloudTex2 = Input.TexCoord;	
	
	// Swirl the clouds	
	cloudTex0 = SwirlHoriz(cloudTex0,true);
	cloudTex1 = SwirlHoriz(cloudTex1,true);
	cloudTex1 = SwirlVert(cloudTex1,true);
	cloudTex2 = SwirlHoriz(cloudTex2,false);
	cloudTex2 = SwirlVert(cloudTex2,false);
	
	cloudCol0 = tex2D(cloudTexture, cloudTex0);	
	cloudCol1 = tex2D(cloudTexture, cloudTex1);
	cloudCol2 = tex2D(cloudTexture, cloudTex2);
	
	float4 cloudCol = (cloudCol0 - (cloudCol1 + cloudCol2));
	
	cloudCol *= Input.TexCoord.y;	
	col += cloudCol * cloudIntensity;
	
	return col;
}

technique BasicShader 
{
	pass P0
	{
		VertexShader = compile vs_2_0 Transform();
		PixelShader  = compile ps_2_0 BasicShader();
	}	
}





