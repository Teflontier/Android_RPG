Shader "Custom/Overlay" {
	Properties {
        _MainTex ("Sprite Texture", 2D) = "white" {}
	}
	SubShader {
		Stencil{
			Ref 1
			Comp equal
		}
		
		 Pass
         {
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma multi_compile DUMMY PIXELSNAP_ON
  
             sampler2D _MainTex;
 
             struct Vertex {
                 float4 vertex : POSITION;
                 float2 uv_MainTex : TEXCOORD0;
                 float2 uv2 : TEXCOORD1;
                 float4 color : COLOR;
             };
     
             struct Fragment {
                 float4 vertex : POSITION;
                 float2 uv_MainTex : TEXCOORD0;
                 float2 uv2 : TEXCOORD1;
                 float4 color : COLOR;
             };

             Fragment vert(Vertex v)
             {
                 Fragment o;
                 o.vertex = UnityObjectToClipPos(v.vertex);
                 o.uv_MainTex = v.uv_MainTex;
                 o.uv2 = v.uv2;
                 o.color = v.color;
                 return o;
             }

             float4 frag(Fragment fragment) : COLOR
             {
                 float4 o = float4(1, 0, 0, 0.2);
                 half4 c = tex2D (_MainTex, fragment.uv_MainTex);
                 o.rgb = fragment.color;
                 return o;
             }
 
             ENDCG
         }
    }
}
