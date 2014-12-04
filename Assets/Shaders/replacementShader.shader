Shader "Custom/replacementShader" {
	Properties {
		 _MainTex ("Base (RGB)", 2D) = "white" {}
	}
        SubShader {
            Tags { "RenderType" ="Opaque" }
        
        Pass {
            CGPROGRAM
                #pragma fragment frag
                #pragma vertex vert
                #include "UnityCG.cginc"

				struct v2f {
		        float4 pos : SV_POSITION;
		    };
				v2f vert (appdata_base v)
			    {
			        v2f o;
			        o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			        return o;
			    }
			    
			    half4 frag (v2f i) : COLOR
			    {
			        return half4 (.016,.537,.384, 1);
			    }
            ENDCG
        }
    }
    
    SubShader {
      Tags { "RenderType" = "Transparent" 
                   "Queue" = "Transparent" 
		}
         // draw after all opaque geometry has been drawn
      Pass {
         ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects
 
         Blend SrcAlpha OneMinusSrcAlpha // use alpha blending

         CGPROGRAM 
 
         #pragma vertex vert 
         #pragma fragment frag
         
         uniform sampler2D _MainTex;
		 // vertex input: position, UV
        struct appdata {
            float4 vertex : POSITION;
            float4 texcoord : TEXCOORD0;
        };
        
		 struct v2f {
		        float4 pos : SV_POSITION;
		        float4 uv : TEXCOORD0;
		    };
 
         v2f vert(appdata v) 
         {
	          v2f o;
	          o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
	          o.uv = float4( v.texcoord.xy, 0, 0 );
	          return o;
         }
 
         float4 frag(v2f i) : COLOR 
         {
            return tex2D (_MainTex, i.uv.xy);
               // the fourth component (alpha) is important: 
               // this is semitransparent green
         }
 
         ENDCG  
      }
   }
    
}

