Shader "Custom/CoinCollectorShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags {"RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"}
        LOD 200

        Cull Back
        ZWrite On

        Pass
        {
            Tags {"LightMode"="UniversalForward"}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Metallic;
            float _Smoothness;

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);

                output.positionCS = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.normalWS = normalInput.normalWS;
                output.positionWS = vertexInput.positionWS;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                Light mainLight = GetMainLight();
                float3 normalWS = normalize(input.normalWS);
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - input.positionWS.xyz);

                half4 albedo = tex2D(_MainTex, input.uv) * _Color;

                // Lambert diffuse
                float3 diffuse = mainLight.color * max(dot(normalWS, mainLight.direction), 0.0);

                // Simple specular
                float3 halfVec = normalize(mainLight.direction + viewDir);
                float spec = pow(max(dot(normalWS, halfVec), 0.0), 64.0) * _Metallic;

                float3 color = albedo.rgb * (0.3 + diffuse) + spec;
                return half4(color, albedo.a);
            }
            ENDHLSL
        }
    }
}