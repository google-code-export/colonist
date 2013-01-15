Shader "Custom/PlanerReflection" {
	Properties {
	    _BaseColor("Base color", Color)= (.5, .5, .5, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cube ("Cube", Cube) = "" {}
		_CubeReflectionFactor("Cube reflection factor", Range(0,1)) = 0.3
		_2DReflect ("2D Reflection", 2D) = "" {}
		_2DReflectionFactor("2D reflection factor", Range(0,1)) = 0.3
		_Normal("Normal", 2D) = "bump" {}
	}
CGINCLUDE	
#include "UnityCG.cginc"	
	struct v2f_full
    {
	  half4 pos : SV_POSITION;
	  half2 uv : TEXCOORD0;
	  half3 worldViewDir : TEXCOORD1;
	  half3 tsBase0 : TEXCOORD2;
	  half3 tsBase1 : TEXCOORD3;
	  half3 tsBase2 : TEXCOORD4;
	  #ifdef LIGHTMAP_ON
  	  half2 uvLM : TEXCOORD5;
  	  #endif
  	  half2 uv2DReflection : TEXCOORD6;
    };

     float4 _BaseColor;
	 sampler2D _MainTex;
	 samplerCUBE _Cube;
	 sampler2D _2DReflect;
	 sampler2D _Normal;	
	 float _CubeReflectionFactor;
	 float _2DReflectionFactor;
	 	
     
     void WriteTangentSpaceData (appdata_full v, out half3 ts0, out half3 ts1, out half3 ts2) {
	    TANGENT_SPACE_ROTATION;
	    ts0 = mul(rotation, _Object2World[0].xyz * unity_Scale.w);
	    ts1 = mul(rotation, _Object2World[1].xyz * unity_Scale.w);
	    ts2 = mul(rotation, _Object2World[2].xyz * unity_Scale.w);				
	 }
	 half2 EthansFakeReflection (half4 vtx) {
	    half3 worldSpace = mul(_Object2World, vtx).xyz;
		worldSpace = (-_WorldSpaceCameraPos * 0.6 + worldSpace) * 0.07;
	    return worldSpace.xz;
	 }
ENDCG 

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass
		{
CGPROGRAM

      //Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
      //#pragma exclude_renderers gles
	  	#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		
	  	half4 _MainTex_ST;
	  	 
		half4 unity_LightmapST;
		sampler2D unity_Lightmap;	
		 
		v2f_full vert(appdata_full v) 
		{
			v2f_full o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			
			o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
			#ifdef LIGHTMAP_ON
			o.uvLM = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
			#endif
			o.worldViewDir = normalize(WorldSpaceViewDir(v.vertex));
			
			WriteTangentSpaceData(v, o.tsBase0, o.tsBase1, o.tsBase2);	
		    
		    o.uv2DReflection = EthansFakeReflection (v.vertex);
		    
			return o; 
		}	
		
		fixed4 frag (v2f_full i) : COLOR0 
		{
		    fixed4 tex = tex2D (_MainTex, i.uv.xy);
		    
		    //Cubemap reflection:
 
			half3 nrml = UnpackNormal(tex2D(_Normal, i.uv.xy));
			half3 bumpedNormal = half3(dot(i.tsBase0,nrml), dot(i.tsBase1,nrml), dot(i.tsBase2,nrml));
			
			half3 reflectVector = reflect(normalize(-i.worldViewDir.xyz), normalize(bumpedNormal.xyz));
			
			
			//tex += refl_3D * tex.a;
				
 			half4 refl_3D = texCUBE(_Cube, (reflectVector)); 
			tex += refl_3D * _CubeReflectionFactor;
			

			//Lightmap:
			#ifdef LIGHTMAP_ON
			fixed3 lm = DecodeLightmap (tex2D(unity_Lightmap, i.uvLM.xy));
			tex.rgb *= lm;
			#endif
			
			//2D Reflection:
			fixed4 refl_2D = tex2D (_2DReflect, i.uv2DReflection);
            tex += refl_2D * _2DReflectionFactor;
            tex.rgba *= _BaseColor;
			return tex;
			
		}
		
ENDCG
		}
 
	} 
	FallBack "Diffuse"
}
