Shader "Custom/IronReflection" {
	Properties {
	    _AmbientColor("Ambient color", Color) = (0.3,0.3,0.3,1)
	    _MainColor("Main color", Color) = (0.3,0.3,0.3,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Reflections ("Reflection (RGB) Gloss (A)", Cube) = "skybox" { TexGen CubeReflect }
	}
	SubShader {
		Tags { "Queue" = "Transparent" }
		Pass {
            Material {
                Ambient (0.2,0.2,0.2,1)
                Diffuse [_MainColor]
            }
            Lighting On
            SetTexture [_MainTex] {
                combine texture + primary
            }
            SetTexture [_Reflections] {
                combine previous * primary + texture
                //Matrix [_Reflections]
                Matrix [_MainTex]
            }
		}

	} 
	FallBack "Diffuse"
}
