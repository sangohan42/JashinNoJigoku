Shader "Custom/CustomDiffuse" { 

	Properties { _MainTex ("Main Texture", 2D) = "white" {} 

			}
     SubShader 
     {
         LOD 200
 
         Tags 
         { 
             "RenderType"="Opaque" 
             "Queue" = "Geometry" 
         }
         
         Pass
         {
             Cull Off
             Fog { Mode Off }
             AlphaTest Off
             Blend Off
 
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma glsl_no_auto_normalization
             #include "UnityCG.cginc"
                 
             sampler2D _MainTex;
             float4 _MainTex_ST;
 
             struct Vertex
             {
                 float4 vertex : POSITION;
                 float4 uv : TEXCOORD0;
             };
 
             struct Fragment
             {
                 float4 vertex : POSITION;
                 float4 uv : TEXCOORD0;
             };
 
             Fragment vert(Vertex v)
             {
                 Fragment o;
 
                 o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                 o.uv.xy = v.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                 
                 return o;
             }
                                                 
             fixed4 frag(Fragment IN) : COLOR
             {
                 fixed4 output = fixed4(0, 0, 0, 1);
                 
                 output.rgb = tex2D(_MainTex, IN.uv.xy).rgb;
                 return saturate(output);
             }
             
             ENDCG
         }
     }
     

 
     Fallback Off
 }