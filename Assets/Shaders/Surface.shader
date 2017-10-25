Shader "Custom/SurfaceShader" {
	Properties {
		_MainTex ("Texture", 2D) = "white"
        [Toggle] useSpriteRendererColor("use SpriteRenderer Color", Float) = 0
        alpha ("Alpha", Float) = 1
	}
	SubShader {
	 	Tags { "Queue"="Transparent" "RenderType"="Transparent" }
	 	Blend SrcAlpha OneMinusSrcAlpha

		Stencil{
			Ref 1
			Comp equal
		}
		
		 Pass
         {
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag

             sampler2D _MainTex;
             float useSpriteRendererColor;
             float alpha;
 
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
                 Fragment fragment;
                 fragment.vertex = UnityObjectToClipPos(v.vertex);
                 fragment.uv_MainTex = v.uv_MainTex;
                 fragment.uv2 = v.uv2;
                 fragment.color = v.color;
                 return fragment;
             }

             float4 frag(Fragment fragment) : COLOR
             {
                 float4 colorOut = fragment.color;
                 if(useSpriteRendererColor == 0){
                 	half4 c = tex2D (_MainTex, fragment.uv_MainTex);
				 	colorOut.rgb = c.rgb;
				 	colorOut.a = c.a;
                 }else{
                 	colorOut.rgb = fragment.color;
                 	colorOut.a = alpha;
                 }
                 return colorOut;
             }
 
             ENDCG
         }
    }
}
