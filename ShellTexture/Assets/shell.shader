Shader "CustomRenderTexture/New Custom Render Texture"
{

    SubShader{
        Tags{
            "LightMode" = "ForwardBase"
        }

        Pass
        {
            Cull Off
            Name "Shell"

            CGPROGRAM
            #include "UnityPBSLighting.cginc"
            #include "AutoLight.cginc"
            #include "UnityCustomRenderTexture.cginc"

            #pragma vertex vert
			#pragma fragment frag



            float4      _Color;
            sampler2D   _MainTex;

            int _shellCount;
            int _shellIndex;
            float _length;
            float _density;
            float _thickness;
            float _minLength;
            float _maxLength;
            float3 _shellColor;

            //struct that is used for our vert()
            struct VertexData{
                float4 vertex: POSITION;
                float3 normal: NORMAL;
                float2 uv: TEXCORD0;
            }
            
            struct v2f_customrendertexture {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
            }

            v2f_customrendertexture vert(VertexData v){
                v2f_customrendertexture i;

                //
                float shellHeight = pow(((float)_shellIndex/(float)_shellCount),1.0f);
                
            }
            
            float4 frag(v2f_customrendertexture IN) : SV_Target
            {
                float2 uv = IN.localTexcoord.xy;
                float4 color = tex2D(_MainTex, uv) * _Color;

                // TODO: Replace this by actual code!
                uint2 p = uv.xy * 256;
                return countbits(~(p.x & p.y) + 1) % 2 * float4(uv, 1, 1) * color;
            }

            float Hash(int x) {
                x = (x << 13) ^ x;
                return frac((x * (x * x * 15731 + 789221) + 1376312589) * (1.0 / 1073741824.0));
            }

            ENDCG
        }
    }
}
