texture particleTexture;
float4x4 WorldViewProj : WorldViewProjection;
struct PS_INPUT
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float4 Color : COLOR0;
};
sampler Sampler = sampler_state
{
	Texture = <particleTexture>;
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
	texCoord = input.TexCoord.xy;
	float4 Color = tex2D(Sampler, texCoord);
	Color *= input.Color;
	return Color;	
}
technique PointSpriteTechnique
{
    pass P0
    {
        vertexShader = compile vs_2_0 VertexShader();
        pixelShader = compile ps_2_0 PixelShader();
    }
}