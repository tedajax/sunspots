  //--------------------------------
  // Toon
  //--------------------------------
  // By Evolved
  // http://www.vector3r.com/
  //--------------------------------

  //--------------------------------
  // un-tweaks
  //--------------------------------
   matrix WorldVP:WorldViewProjection; 
   matrix World:World;   
   matrix ViewInv:ViewInverse; 

  //--------------------------------
  // tweaks
  //--------------------------------
   float  EdgeDis = 0.15f;
   float4 LightPosition = {0.0f, 0.0f, 0.0f, 1.0f};   
   float  LightRange = 200.0f;	

  //--------------------------------
  // Textures
  //--------------------------------
   texture Toon 
      <
	string Name="";
      >;	
   sampler2D ToonSample = sampler_state 
      {
 	texture = <Toon>;
      };
   texture Shade 
      <
	string Name="";
      >;	
   sampler2D ShadeSample = sampler_state 
      {
 	texture = <Shade>;
      };

  //--------------------------------
  // structs
  //--------------------------------
   struct input
      {
    	float4 Pos : POSITION ; 
    	float2 UV : TEXCOORD ; 
 	float3 Normal : NORMAL;  
      };
   struct output
      {
    	float4 OPos : POSITION ; 
    	float2 uv : TEXCOORD0 ; 
     	float2 Shade : TEXCOORD1 ;
      };
   struct output_Edge
      {
    	float4 OPos : POSITION ;
     	float4 Edge : COLOR0 ; 
      };

  //--------------------------------
  // vertex shader
  //--------------------------------
   output VS(input IN) 
     {
    	output OUT;
    	OUT.OPos=mul(IN.Pos,WorldVP);
	OUT.uv=IN.UV;
	float3 WNor=mul(IN.Normal,World);  
	       WNor=normalize(WNor);
	float3 WPos=mul(IN.Pos,World); 
  	float3 LPos=LightPosition-WPos;
   	float4 Light=max(saturate(0.02f+mul(LPos,WNor)*0.02f),0.2)/2;
	float Dis=max(saturate(1-(length(LPos)/LightRange)),0.2);
	OUT.Shade.xy=Light*Dis;
  	return OUT;
     }
   output_Edge VS_Edge(input IN) 
     {
    	output_Edge OUT;
	float4 Vpos;
	Vpos.xyz=IN.Pos+(IN.Normal*EdgeDis);Vpos.w=IN.Pos.w;
    	OUT.OPos=mul(Vpos,WorldVP);
	OUT.Edge.xyzw=0;
  	return OUT;
     }

  //-----------------
  // techniques 
  //-----------------
    technique Toon
      {
 	pass p0
      {		
 	vertexShader = compile vs_1_1 VS(); 
        Sampler[0] = <ToonSample>;
  	ColorOp[0]   = MODULATE;
        ColorArg1[0] = TEXTURE;
        Sampler[1] = <ShadeSample>;
  	ColorOp[1]   = MODULATE;
        ColorArg1[1] = TEXTURE;
      }
  	pass p1
      {		
 	vertexShader = compile vs_1_1 VS_Edge(); 
        CullMode = Cw;
        Sampler[0] = null;
        Sampler[1] = null;
      }
      }