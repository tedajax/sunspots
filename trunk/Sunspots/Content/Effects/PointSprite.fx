//--------------Constants
uniform extern texture SpriteTexture;
uniform extern float4x4 WorldViewProj;
uniform extern float3x3 Rotation;
struct PS_INPUT
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float4 Color : COLOR0;
	
};

sampler Sampler = sampler_state
{
	Texture = <SpriteTexture>;
};

PS_INPUT VertexShader(float4 pos : POSITION0, float4 color : COLOR0, float2 texCoord : TEXCOORD0)
{
	PS_INPUT Output = (PS_INPUT)0;
    
	Output.Position = mul(pos, WorldViewProj);
	Output.TexCoord = texCoord;
	Output.Color = color;
	return Output;
}

float4 PixelShader(PS_INPUT input) : COLOR0
{
	float2 texCoord;
	float4 newpos = float4(input.TexCoord.x,input.TexCoord.y, 0,0);
	float3 newposo = mul(input.TexCoord, Rotation);
	texCoord = input.TexCoord.xy;
	float4 Color = tex2D(Sampler, texCoord);
	Color.a *= input.Color.a;
	return clamp(Color,0,1);
}

technique PointSpriteTechnique
{
	pass P0
	{
		vertexShader = compile vs_2_0 VertexShader();
		pixelShader = compile ps_2_0 PixelShader();
	}

}


