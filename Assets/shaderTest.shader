Shader "Shaders/shaderTest"
{
    Properties
    {
        _Color("Test Color", color) = (1,1,1,1)
        _Threshold("Threshold", Range(0,1)) = 0.5
        _MainTex ("Noise Texture (RGB)", 2D) = "white" {}
        _ShellCount("Shell Count", Int) = 10
        _ShellStep("Shell offset", Float) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling
            
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            int _ShellCount;
            float _ShellStep;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0; //pass coords to frag-shader
                float shellIndex : TEXCOORD1; 
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Threshold;
            float4 _Color;
            
            v2f vert (appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);

                v2f output;
                float offset = v.instanceID * _ShellStep;
                float3 offsetPos = float3(v.vertex.x, v.vertex.y * offset, v.vertex.z);
                output.vertex = UnityObjectToClipPos(float4(offsetPos, 1.0)); //Model view Projection
                
                output.uv = TRANSFORM_TEX(v.uv, _MainTex);
                output.shellIndex = v.instanceID;
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
