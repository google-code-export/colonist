Shader "Custom/Stealth(surf)" {
	Properties {
        _Alpha ("Alpha" , Range(0,1)) = 0.05
        _ClipValve("Clip valve",Range(0,1)) = 1
       // _offsetRate("Offset rate", Range(-10,10))=0
		_MainTex ("Base (RGB)", 2D) = "white" {}
        _Bumpmap ("Bumpmap", 2D) = "white" {}      
//        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
//		_Outline ("Outline width", Range (.002, 0.03)) = .005 
	}
	SubShader {
            Tags 
            { 
                "RenderType" = "Opaque"
                "LightMode" = "Always" 
                "Queue" = "Transparent+1"
            }
            Cull Front
            ZWrite On
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha


		CGPROGRAM
		#pragma surface surf Lambert

        float _Alpha;
        float _ClipValve;
		sampler2D _MainTex;
        sampler2D _Bumpmap;
//        float _offsetRate;
		struct Input {
			float2 uv_MainTex;
            float2 uv_Bumpmap;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);	
			//IN.uv_Bumpmap.x += _SinTime * _offsetRate;
			//IN.uv_Bumpmap.y += _CosTime * _offsetRate;
            if((c.r-_ClipValve)>0)
            {
				o.Albedo = c.rgb;
		    }
			else
			{
				o.Albedo = half3(1,1,1)*0.02;
			}
			o.Alpha = _Alpha; 
//           o.Normal = UnpackNormal (tex2D (_Bumpmap, IN.uv_Bumpmap));
//           o.Normal = UnpackNormal(c);
		}
		ENDCG
		
//		Pass {
//			Name "OUTLINE"
//			Tags { "LightMode" = "Always" }
//			Cull Front
//			ZWrite On
//			ColorMask RGB
//			Blend SrcAlpha OneMinusSrcAlpha
//
//			CGPROGRAM
//			#include "UnityCG.cginc"
//			#pragma vertex vertOutline
//			#pragma fragment fragOutline
//			struct appdata {
//		      float4 vertex : POSITION;
//		      float3 normal : NORMAL;
//		    };
//
//		    struct v2f {
//		      float4 pos : POSITION;
//		      float4 color : COLOR;
//		    };
//	        uniform float _Outline;
//            uniform float4 _OutlineColor;
//     
//		    v2f vertOutline(appdata v) {
//		      v2f o;
//		      o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
//              float3 norm = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
//		      float2 offset = TransformViewToProjection(norm.xy);
//              o.pos.xy += offset * o.pos.z * _Outline;
//              o.color = _OutlineColor;
//              return o;
//            }
//			half4 fragOutline(v2f i) :COLOR { return i.color; }
//			ENDCG
//		}
		
	} 
	FallBack "Diffuse"
}
