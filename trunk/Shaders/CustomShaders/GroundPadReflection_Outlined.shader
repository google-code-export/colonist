Shader "Custom/PlanerReflection_Outlined" { //定义SHADER 名字
	Properties { //定义 Shader的外部属性,可以在Inspector 中查看到
	    _BaseColor("Base color", Color)= (.5, .5, .5, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cube ("Cube", Cube) = "" {}
		_CubeReflectionFactor("Cube reflection factor", Range(0,1)) = 0.3
		_2DReflect ("2D Reflection", 2D) = "" {}
		_2DReflectionFactor("2D reflection factor", Range(0,1)) = 0.3
		_Normal("Normal", 2D) = "bump" {}
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (.002, 0.03)) = .005
	}
CGINCLUDE	
#include "UnityCG.cginc"
//定义Vertex 传递给 Fragment的参数
	struct v2f_full
    {
	  half4 pos : SV_POSITION;  //Vertex顶点位置
	  half2 uv : TEXCOORD0;     //UV帖图坐标
	  half3 worldViewDir : TEXCOORD1; //UV贴图坐标1
	  half3 tsBase0 : TEXCOORD2;//UV贴图坐标2
	  half3 tsBase1 : TEXCOORD3;//UV贴图坐标3
	  half3 tsBase2 : TEXCOORD4;//UV贴图坐标4
	  #ifdef LIGHTMAP_ON	
  	  half2 uvLM : TEXCOORD5; //LightMap 贴图坐标, 使用宏定义是因为,如果场景没有应用Lightmap 则不需要使用这个变量
  	  #endif
  	  half2 uv2DReflection : TEXCOORD6;//UV贴图坐标6
  	  float4 color:COLOR;//颜色
    };

//Cg变量,和Properties同名//
     float4 _BaseColor; 
	 sampler2D _MainTex;
	 samplerCUBE _Cube;
	 sampler2D _2DReflect;
	 sampler2D _Normal;	
	 float _CubeReflectionFactor;
	 float _2DReflectionFactor;
	
	 //从AngryBot中抄的,不知道干嘛的// 
     void WriteTangentSpaceData (appdata_full v, out half3 ts0, out half3 ts1, out half3 ts2) {
	    TANGENT_SPACE_ROTATION;
	    ts0 = mul(rotation, _Object2World[0].xyz * unity_Scale.w);
	    ts1 = mul(rotation, _Object2World[1].xyz * unity_Scale.w);
	    ts2 = mul(rotation, _Object2World[2].xyz * unity_Scale.w);				
	 }
	 	 //从AngryBot中抄的,不知道干嘛的// 
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
		Name "TextureReflectionBase"
CGPROGRAM

// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
//#pragma exclude_renderers gles


//指定顶点处理方法叫 vert()
	  	#pragma vertex vert
//指定Fragment处理方法叫 frag()
		#pragma fragment frag
//指定该Shader编译要求是性能优先
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		//4个float = r,g,b,alpha,代表颜色
	  	half4 _MainTex_ST;
		half4 unity_LightmapST;
		
		//sampler2D 就是Texture, 在CG中一个2维位图的变量类型是sampler2D
		sampler2D unity_Lightmap;
		
		//这个vert的目的是:
		//1. 把计算主贴图的坐标值 -> o.pos = ....
		//2. 如果有LIGHTMAP , 计算LIGHTMAP的帖图坐标  o.uvLM = .....
		//3. 计算 WorldViewDir ,用于 反射帖图
		//4. 计算 uv2DReflection , 似乎也是用于反射,原理不详
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
		
		//这个fragment 的目的是:
		//1. 给OBJECT帖图 (主贴图)  -> fixed4 tex = tex2D (_MainTex, i.uv.xy);
		//2. 计算3D反射帖图 , 3D并不是指图是3D的,而是3D帖图的输入参数不是一个普通的2维帖图,而是一个CUBE MAP,也就是6个面各有一个对应的帖图(6个2D帖图,对应6个面)
		//3. 如果有LIGHTMAP , 计算LIGHTMAP的帖图像素,然后附加在上一个主帖图至上: -> tex.rgb *= lm;
		//4. 计算 2D反射帖图 : tex += refl_2D * _2DReflectionFactor;
		//注意一点: 计算LightMap的时候,用的是乘法,而计算帖图的时候,用的是加法
		//这是因为: 加法永远只会令OBJECT的材质显示更亮, 而乘法可以令OBJECT的材质显示根据LIGHTMAP的数值变暗. 这正是LIGHTMAP的作用所在.
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
		
		//这个PASS是用来画外框线的(Outline), 原理没有研究过.
		Pass {
			Name "OUTLINE"
			Tags { "LightMode" = "Always" }
			Cull Front
			ZWrite On
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vertOutline
			#pragma fragment fragOutline
			struct appdata {
		      float4 vertex : POSITION;
		      float3 normal : NORMAL;
		    };

		    struct v2f {
		      float4 pos : POSITION;
		      float4 color : COLOR;
		    };
	        uniform float _Outline;
            uniform float4 _OutlineColor;
     
		    v2f vertOutline(appdata v) {
		      v2f o;
		      o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
              float3 norm = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
		      float2 offset = TransformViewToProjection(norm.xy);
              o.pos.xy += offset * o.pos.z * _Outline;
              o.color = _OutlineColor;
              return o;
            }
			half4 fragOutline(v2f i) :COLOR { return i.color; }
			ENDCG
		}
 
	} 
	FallBack "Diffuse"
}
