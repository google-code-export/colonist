Shader "Toon/Custom/Basic Toon (Complex)" {
	Properties {
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_FogColor("Fog color", Color) = (.5,.5,.5,1)
		_FogDensity("Fog density", Range (0, 0.3)) = 0.03
		_RedCutoff("Red cut off", Range(0,1))=0
		_GreenCutoff("Green cut off", Range(0,1))=0
		_BlueCutoff("Blue cut off", Range(0,1))=0
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ToonShade ("ToonShader Cubemap(RGB)", CUBE) = "" { Texgen CubeNormal }
	}


	SubShader {
		Tags { "RenderType"="Opaque" }
		
		Pass {
			Name "BASE"

			//ZWrite Off

			//ZTest Always  

			
			Fog {
			  Mode Exp2
			  //Color (.25,.35,.5,.4)
			  Color [_FogColor]
		      Density [_FogDensity]
			}
			 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			samplerCUBE _ToonShade;
			float4 _MainTex_ST;
			float4 _Color;
			float _RedCutoff;
			float _GreenCutoff;
			float _BlueCutoff;
			
			struct appdata {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};
			
			struct v2f {
				float4 pos : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 cubenormal : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.cubenormal = mul (UNITY_MATRIX_MV, float4(v.normal,0));
				
				return o;
			}

			float4 frag (v2f i) : COLOR
			{
			    float4 maintex = tex2D(_MainTex, i.texcoord);
			    if(_RedCutoff > 0)
			    {
			       clip(maintex.r - _RedCutoff);
			    }
			    if(_GreenCutoff > 0)
			    {
			       clip(maintex.g - _GreenCutoff);
			    }
			    if(_BlueCutoff > 0)
			    {
			       clip(maintex.b - _BlueCutoff);
			    }
				float4 col = _Color * maintex;//tex2D(_MainTex, i.texcoord);
				float4 cube = texCUBE(_ToonShade, i.cubenormal);
				return float4(2.0f * cube.rgb * col.rgb, col.a);
			}
			ENDCG			
		}
	} 

	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
			Name "BASE"
			Cull Off
			SetTexture [_MainTex] {
				constantColor [_Color]
				Combine texture * constant
			} 
			SetTexture [_ToonShade] {
				combine texture * previous DOUBLE, previous
			}
		}
	} 
	
	Fallback "VertexLit"
}
