/*********************************************************************NVMH3****
Path: $Id: //sw/devtools/ShaderLibrary/1.0/HLSL/Ocean.fx#1 $
File:  ocean.fx

Copyright NVIDIA Corporation 2003-2007
TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, THIS SOFTWARE IS PROVIDED
*AS IS* AND NVIDIA AND ITS SUPPLIERS DISCLAIM ALL WARRANTIES, EITHER EXPRESS
OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS FOR A PARTICULAR PURPOSE.  IN NO EVENT SHALL NVIDIA OR ITS SUPPLIERS
BE LIABLE FOR ANY SPECIAL, INCIDENTAL, INDIRECT, OR CONSEQUENTIAL DAMAGES
WHATSOEVER (INCLUDING, WITHOUT LIMITATION, DAMAGES FOR LOSS OF BUSINESS PROFITS,
BUSINESS INTERRUPTION, LOSS OF BUSINESS INFORMATION, OR ANY OTHER PECUNIARY LOSS)
ARISING OUT OF THE USE OF OR INABILITY TO USE THIS SOFTWARE, EVEN IF NVIDIA HAS
BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.


% Simple ocean shader with animated bump map and geometric waves
% Based partly on "Effective Water Simulation From Physical Models", GPU Gems

keywords: material animation environment bumpmap

Sparkle Mod: C.Humphrey
Altered the shader so that the "HALO" sparkle effect could be switched on and off
rather than having either or.

******************************************************************************/

float Script : STANDARDSGLOBAL <
    string UIWidget = "none";
    string ScriptClass = "object";
    string ScriptOrder = "standard";
    string ScriptOutput = "color";
    string Script = "Technique=Main;";
> = 0.8;


///////// UNTWEAKABLES //////////////////////

float4x4 world : World < string UIWidget = "none";>;	           	// World or Model matrix
float4x4 wvp : WorldViewProjection < string UIWidget = "none";>;	// Model*View*Projection
float4x4 worldView : WorldView < string UIWidget = "none";>;
float4x4 viewI : ViewInverse < string UIWidget = "none";>;

bool sparkle;

float time : Time < string UIWidget = "none"; >;

//////////////// TEXTURES ///////////////////

texture normalMap : Normal <
	string UIName = "Normal Map";
	string ResourceName = "waves2.dds";
	string ResourceType = "2D";
>;

texture cubeMap : Environment <
	string UIName = "Environment Cube Map";
	string ResourceName = "CloudyHillsCubemap2.dds";
	string ResourceType = "Cube";
>;

sampler2D normalMapSamplerHALO = sampler_state
{
	Texture = <normalMap>;
	// this is a trick from Halo - use point sampling for sparkles
	MagFilter = Linear;	
	MinFilter = Point;
	MipFilter = None;
};
sampler2D normalMapSampler = sampler_state
{
	Texture = <normalMap>;
	MagFilter = Linear;	
	MinFilter = Linear;
	MipFilter = Linear;
};

samplerCUBE envMapSampler = sampler_state
{
	Texture = <cubeMap>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

///////// TWEAKABLE PARAMETERS //////////////////

float bumpHeight <
	string UIWidget = "slider";
	float UIMin = 0.0; float UIMax = 2.0; float UIStep = 0.01;
	string UIName = "Bump Height";
> = 0.1;

float2 textureScale <
    string UIName = "Texture Scale";
> = { 8.0, 4.0 };

float2 bumpSpeed <
    string UIName = "Bumpmap Translation Speed";
> = { -0.05, 0.0 };

float fresnelBias <
    string UIName = "Fresnel Bias";
	string UIWidget = "slider";
	float UIMin = 0.0; float UIMax = 1.0; float UIStep = 0.01;
> = 0.1;

float fresnelPower <
    string UIName = "Fresnel Exponent";
	string UIWidget = "slider";
	float UIMin = 1.0; float UIMax = 10.0; float UIStep = 0.01;
> = 4.0;

float hdrMultiplier <
    string UIName = "HDR Multiplier";
	string UIWidget = "slider";
	float UIMin = 0.0; float UIMax = 100.0; float UIStep = 0.01;
> = 3.0;

float4 deepColor : Diffuse <
    string UIName = "Deep Water";
> = {0.0f, 0.0f, 0.1f, 1.0f};

float4 shallowColor : Diffuse <
    string UIName = "Shallow Water";
> = {0.0f, 0.5f, 0.5f, 1.0f};

float4 reflectionColor : Specular <
    string UIName = "Reflection";
> = {1.0f, 1.0f, 1.0f, 1.0f};

// these are redundant, but makes the ui easier:
float reflectionAmount <
    string UIName = "Reflection Amount";
	string UIWidget = "slider";    
	float UIMin = 0.0; float UIMax = 2.0; float UIStep = 0.01;    
> = 1.0f;

float waterAmount <
    string UIName = "Water Color Amount";
	string UIWidget = "slider";    
	float UIMin = 0.0; float UIMax = 2.0; float UIStep = 0.01;    
> = 1.0f;

float waveAmp <
    string UIName = "Wave Amplitude";
	string UIWidget = "slider";
	float UIMin = 0.0; float UIMax = 10.0; float UIStep = 0.1;
> = 1.0;

float waveFreq <
    string UIName = "Wave frequency";
	string UIWidget = "slider";
	float UIMin = 0.0; float UIMax = 1.0; float UIStep = 0.001;
> = 0.1;

//////////// CONNECTOR STRUCTS //////////////////

struct AppData {
	float4 Position : POSITION;   // in object space
	float2 TexCoord : TEXCOORD0;
	float3 Tangent  : TEXCOORD1;
	float3 Binormal : TEXCOORD2;
	float3 Normal   : NORMAL;
};

struct VertexOutput {
	float4 Position  : POSITION;  // in clip space
	float2 TexCoord  : TEXCOORD0;
	float3 TexCoord1 : TEXCOORD1; // first row of the 3x3 transform from tangent to cube space
	float3 TexCoord2 : TEXCOORD2; // second row of the 3x3 transform from tangent to cube space
	float3 TexCoord3 : TEXCOORD3; // third row of the 3x3 transform from tangent to cube space

	float2 bumpCoord0 : TEXCOORD4;
	float2 bumpCoord1 : TEXCOORD5;
	float2 bumpCoord2 : TEXCOORD6;
	
	float3 eyeVector  : TEXCOORD7;
};

// wave functions ///////////////////////

struct Wave {
  float freq;  // 2*PI / wavelength
  float amp;   // amplitude
  float phase; // speed * 2*PI / wavelength
  float2 dir;
};

#define NWAVES 2
Wave wave[NWAVES] = {
	{ 1.0, 1.0, 0.5, float2(-1, 0) },
	{ 2.0, 0.5, 1.3, float2(-0.7, 0.7) }	
};

float evaluateWave(Wave w, float2 pos, float t)
{
  return w.amp * sin( dot(w.dir, pos)*w.freq + t*w.phase);
}

// derivative of wave function
float evaluateWaveDeriv(Wave w, float2 pos, float t)
{
  return w.freq*w.amp * cos( dot(w.dir, pos)*w.freq + t*w.phase);
}

// sharp wave functions
float evaluateWaveSharp(Wave w, float2 pos, float t, float k)
{
  return w.amp * pow(sin( dot(w.dir, pos)*w.freq + t*w.phase)* 0.5 + 0.5 , k);
}

float evaluateWaveDerivSharp(Wave w, float2 pos, float t, float k)
{
  return k*w.freq*w.amp * pow(sin( dot(w.dir, pos)*w.freq + t*w.phase)* 0.5 + 0.5 , k - 1) * cos( dot(w.dir, pos)*w.freq + t*w.phase);
}

///////// SHADER FUNCTIONS ///////////////

VertexOutput BumpReflectWaveVS(AppData IN,
					  uniform float4x4 WorldViewProj,
					  uniform float4x4 World,
					  uniform float4x4 ViewIT,
					  uniform float BumpScale,
					  uniform float2 textureScale,
					  uniform float2 bumpSpeed,
					  uniform float time,
					  uniform float waveFreq,
					  uniform float waveAmp
                	  )
{
	VertexOutput OUT;

    wave[0].freq = waveFreq;
    wave[0].amp = waveAmp;

    wave[1].freq = waveFreq*2.0;
    wave[1].amp = waveAmp*0.5;

    float4 P = IN.Position;

	// sum waves	
	P.y = 0.0;
	float ddx = 0.0, ddy = 0.0;
	for(int i=0; i<NWAVES; i++) {
    	P.y += evaluateWave(wave[i], P.xz, time);
    	float deriv = evaluateWaveDeriv(wave[i], P.xz, time);
    	ddx += deriv * wave[i].dir.x;
    	ddy += deriv * wave[i].dir.y;
    }

	// compute tangent basis
    float3 B = float3(1, ddx, 0);
    float3 T = float3(0, ddy, 1);
    float3 N = float3(-ddx, 1, -ddy);

	OUT.Position = mul(P, WorldViewProj);
	
	// pass texture coordinates for fetching the normal map
	OUT.TexCoord.xy = IN.TexCoord*textureScale;

	time = fmod(time, 100.0);
	OUT.bumpCoord0.xy = IN.TexCoord*textureScale + time*bumpSpeed;
	OUT.bumpCoord1.xy = IN.TexCoord*textureScale*2.0 + time*bumpSpeed*4.0;
	OUT.bumpCoord2.xy = IN.TexCoord*textureScale*4.0 + time*bumpSpeed*8.0;

	// compute the 3x3 tranform from tangent space to object space
	float3x3 objToTangentSpace;
	// first rows are the tangent and binormal scaled by the bump scale
	objToTangentSpace[0] = BumpScale * normalize(T);
	objToTangentSpace[1] = BumpScale * normalize(B);
	objToTangentSpace[2] = normalize(N);

	OUT.TexCoord1.xyz = mul(objToTangentSpace, World[0].xyz);
	OUT.TexCoord2.xyz = mul(objToTangentSpace, World[1].xyz);
	OUT.TexCoord3.xyz = mul(objToTangentSpace, World[2].xyz);

	// compute the eye vector (going from shaded point to eye) in cube space
	float4 worldPos = mul(P, World);
	OUT.eyeVector = ViewIT[3] - worldPos; // view inv. transpose contains eye position in world space in last row
	return OUT;
}


// Pixel Shaders

float4 BumpReflectMain(VertexOutput IN,					   
					   uniform samplerCUBE EnvironmentMap) : COLOR
{
	// fetch the bump normal from the normal map
	float4 N;
	
	if(!sparkle)
		N = tex2D(normalMapSamplerHALO, IN.TexCoord.xy)*2.0 - 1.0;
	else
		N = tex2D(normalMapSampler, IN.TexCoord.xy)*2.0 - 1.0;
		
	
	float3x3 m; // tangent to world matrix
	m[0] = IN.TexCoord1;
	m[1] = IN.TexCoord2;
	m[2] = IN.TexCoord3;
	float3 Nw = mul(m, N.xyz);
	
//	float3 E = float3(IN.TexCoord1.w, IN.TexCoord2.w, IN.TexCoord3.w);
	float3 E = IN.eyeVector;
    float3 R = reflect(-E, Nw);

	return texCUBE(EnvironmentMap, R);
}

float4 OceanMain(VertexOutput IN,				 
				 uniform samplerCUBE EnvironmentMap,
				 uniform half4 deepColor,
				 uniform half4 shallowColor,
				 uniform half4 reflectionColor,
				 uniform half4 reflectionAmount,
				 uniform half4 waterAmount,
				 uniform half fresnelPower,
				 uniform half fresnelBias,
				 uniform half hdrMultiplier
				 ) : COLOR
{
	// sum normal maps
	half4 t0;
    half4 t1;
    half4 t2;
    
	if(!sparkle)
	{
		t0 = tex2D(normalMapSampler, IN.bumpCoord0.xy)*2.0-1.0;
		t1 = tex2D(normalMapSampler, IN.bumpCoord1.xy)*2.0-1.0;
		t2 = tex2D(normalMapSampler, IN.bumpCoord2.xy)*2.0-1.0;
    }
    else
    {
		t0 = tex2D(normalMapSamplerHALO, IN.bumpCoord0.xy)*2.0-1.0;
		t1 = tex2D(normalMapSamplerHALO, IN.bumpCoord1.xy)*2.0-1.0;
		t2 = tex2D(normalMapSamplerHALO, IN.bumpCoord2.xy)*2.0-1.0;
    }
    half3 N = t0.xyz + t1.xyz + t2.xyz;
//    half3 N = t1.xyz;

    half3x3 m; // tangent to world matrix
    m[0] = IN.TexCoord1;
    m[1] = IN.TexCoord2;
    m[2] = IN.TexCoord3;
    half3 Nw = mul(m, N.xyz);
    Nw = normalize(Nw);

	// reflection
    float3 E = normalize(IN.eyeVector);
    half3 R = reflect(-E, Nw);

    half4 reflection = texCUBE(EnvironmentMap, R);
    // hdr effect (multiplier in alpha channel)
    reflection.rgb *= (1.0 + reflection.a*hdrMultiplier);

	// fresnel - could use 1D tex lookup for this
    half facing = 1.0 - max(dot(E, Nw), 0);
    half fresnel = fresnelBias + (1.0-fresnelBias)*pow(facing, fresnelPower);

    half4 waterColor = lerp(deepColor, shallowColor, facing);

	return waterColor*waterAmount + reflection*reflectionColor*reflectionAmount*fresnel;
//	return waterColor;
//	return fresnel;
//	return reflection;
}

//////////////////// TECHNIQUE ////////////////

technique Main <
	string Script = "Pass=p0;";
> {
	pass p0 <
		string Script = "Draw=geometry;";
	> {
		VertexShader = compile vs_1_1 BumpReflectWaveVS(wvp, world, viewI,
                                                        bumpHeight, textureScale, bumpSpeed, time,
                                                        waveFreq, waveAmp);
		
		Zenable = true;
		ZWriteEnable = true;
//		CullMode = None;

//		PixelShader = compile ps_2_0 BumpReflectMain(envMapSampler);
		PixelShader = compile ps_2_0 OceanMain(envMapSampler,
                                               deepColor, shallowColor, reflectionColor, reflectionAmount, waterAmount,
                                               fresnelPower, fresnelBias, hdrMultiplier);
	}
}

///////////////////////////////// eof ///
