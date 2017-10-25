Shader "Custom/Mask" {
	SubShader {
		ColorMask 0
		Stencil{
			Ref 1
			Pass replace
		}
		Pass{
        }
    }
}
