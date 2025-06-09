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
            float _density;
            float _thickness;
            float _minLength;
            float _maxLength;
            float3 _shellColor;

            float Hash(int x) {
                x = (x << 13) ^ x;
                return frac((x * (x * x * 15731 + 789221) + 1376312589) * (1.0 / 1073741824.0));
            }

            //struct that is used for our vert()
            struct VertexData{
                float4 vertex: POSITION;
                float3 normal: NORMAL;
                float2 uv: TEXCOORD0;
            };
            
            struct v2f {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 normal : TEXCOORD2;
            };

            v2f vert(VertexData v){
                v2f i;
´
                float shellHeight = (float)_shellIndex / (float)_shellCount;
                
                v.vertex.xyz += v.normal * shellHeight;
                i.normal = normalize(UnityObjectToWorldNormal(v.normal));
                i.uv = v.uv;
                i.worldPos = mul(unity_ObjectToWorld, v.vertex);
                i.position = UnityObjectToClipPos(v.vertex);
            
                return i;
            }
            
            float4 frag(v2f IN) : SV_Target
            {
                // Creates new UVs that are used the create more strands.
                float2 newUV = IN.uv * _density;
                
                // The UV range is set between 0 and 1. To get a wider range for the UV coordinate
                // we take a fractional part of the UV range and multiply it by 2 and take -1
                // so that the rage is set to -1 and 1.
                float2 localUV = frac(newUV) * 2 -1;
                float localDistfromCenter = length(localUV);

                // We convert the newUV to uint2 to make it easier to use when calculating the seed
                // and using the hash function. This was recommended to do by the guides.
                uint2 uintUV = newUV;
                uint seed = uintUV.x + 100 * uintUV.y * 100 * 10;

                // We use the seed as a value that we linear interpolate between the min and max length
                float randFloat = lerp(_minLength, _maxLength, Hash(seed));
            
                // This is the normalized shellhight, the same as in v2f  
                float h = (float)_shellIndex / (float)_shellCount;

                if (localDistfromCenter > (_thickness *(randFloat-h))) discard;

                // _WorldSpaceLightPos0 is a build in shader variable in Unity and refers to the main light source
                float lightdir = DotClamped(IN.normal, _WorldSpaceLightPos0) *0.5f + 0.5f;
                lightdir=lightdir*lightdir; 

                float ambientOcclusion = pow(h, 2);
                return float4(_shellColor * lightdir * ambientOcclusion, 1.0);
            }

            ENDCG
        }
    }
}
