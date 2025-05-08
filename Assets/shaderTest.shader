Shader "Shaders/shaderTest"
{
    Properties
    {
        _Color("Test Color", color) = (1,1,1,1)
        _Threshold("Threshold", Range(0,1)) = 0.5
        _MainTex ("Noise Texture (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0; //pass coords to frag-shader

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Threshold;
            float4 _Color;
            
            v2f vert (appdata v)
            {
                v2f output;
                output.vertex = UnityObjectToClipPos(v.vertex); //Model view Projection
                
                output.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return output;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);

                clip(texColor.r - _Threshold);

                
                if (texColor.a < _Threshold) {
                    discard; // Explicitly discard the pixel
                }
                return _Color; // Return transparent black
                
            }
            ENDCG
        }
    }
}
