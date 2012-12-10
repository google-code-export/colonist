Shader "Custom/TestGrabShader" {
    SubShader {
        // Draw ourselves after all opaque geometry
        Tags { "Queue" = "Transparent" }

        // Grab the screen behind the object into _GrabTexture
        GrabPass { "Text" }

        // Render the object with the texture generated above, and invert it's colors
        Pass { 
            SetTexture [_GrabTexture] { combine one-texture }
        }
    }
 
	
}
