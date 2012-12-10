Shader "Custom/HelloVertxShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Offset_X("X Offset", Range(1,1.5)) = 1
		_Offset_Y("Y Offset", Range(1,1.5)) = 1
		_Offset_Z("Z Offset", Range(1,1.5)) = 1
		_Offset_Normal("Normal Offset", Range(-1,1)) = 1
	}
	SubShader {
    Pass {

    CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
    #include "UnityCG.cginc"

    sampler2D _MainTex;
    
		struct v2f {
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			float3 color : COLOR0;
		};
		
    half4 _MainTex_ST;
    float _Offset_X;
    float _Offset_Y;
    float _Offset_Z;
    float _Offset_Normal;
    v2f vert (appdata_full v)
    {
        v2f o;
        o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
       // o.pos.x *= _Offset_X;
       // o.pos.y *= _Offset_Y;
       // o.pos.z *= _Offset_Z;
       o.pos.xyz += v.normal * _Offset_Normal;
        o.color = v.normal * 0.5;
        //#define TRANSFORM_TEX(tex,name) (tex.xy * name##_ST.xy + name##_ST.zw)
        o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
        WorldSpaceViewDir(v.vertex);
        return o;
    }

    half4 frag (v2f i) : COLOR
    {
        //return half4 (i.color, 1);
        half4 ret = tex2D (_MainTex, i.uv.xy);
        ret[0] += i.color[0];
        ret[1] += i.color[1];
        ret[2] += i.color[2];
        return ret;
    }
ENDCG

    }
}

	FallBack "Diffuse"
}
