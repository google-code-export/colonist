Shader "Custom/Simple Glass" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,0)
        _SpecColor ("Spec Color", Color) = (1,1,1,1)
        _Emission ("Emmisive Color", Color) = (0,0,0,0)
        _Shininess ("Shininess", Range (0.01, 1)) = 0.7
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _Reflections ("Reflection (RGB) Gloss (A)", Cube) = "skybox" { TexGen CubeReflect }
    }

    SubShader {
        // We use the material in many passes by defining them in the subshader.
        // Anything defined here becomes default values for all contained passes.
        Material {
            Diffuse [_Color]
            Ambient [_Color]
            Shininess [_Shininess]
            Specular [_SpecColor]
            Emission [_Emission]
        }
        Lighting On
        SeparateSpecular On

        //Glass effect1:
        Blend SrcAlpha OneMinusSrcAlpha
        //Glass effect2:
        //Blend DstColor Zero
        
        //Blend One One
        //Blend One OneMinusDstColor 
        
       //ALL BLACK:
       // Blend Zero Zero
        

        
        // Render the back facing parts of the object.
        // If the object is convex, these will always be further away
        // than the front-faces.
        Pass {
            Cull Front
            SetTexture [_MainTex] {
                Combine Primary * Texture
            }
        }

        // Render the parts of the object facing us.
        // If the object is convex, these will be closer than the
        // back-faces.
        Pass {
            Cull Back
            SetTexture [_MainTex] {
                Combine Primary * Texture
            }
        }
        Pass {
            Blend DstColor One 
            SetTexture [_Reflections] {
                combine texture
                Matrix [_Reflection]
            }
        }
    }
} 