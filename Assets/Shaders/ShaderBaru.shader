Shader "Custom/ShaderBaru"
{
    Properties
    {
        [MainColor] _Warna("Base Color", Color) = (27, 158, 194, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" "IgnoreProjector" = "True" }

        Pass
        {

            Name "Unlit2D"
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Warna;
            CBUFFER_END

            struct Attributes
            {
                float4 posisiObjek : POSITION;
            };

            struct keFragment
            {
                float4 posisiClip : SV_POSITION;
            };


            keFragment vert(Attributes IN)
            {
                keFragment OUT;
                OUT.posisiClip = TransformObjectToHClip(IN.posisiObjek.xyz);
                return OUT;
            }

            half4 frag(keFragment IN) : SV_Target
            {
                return (half4)_Warna;
            }
            ENDHLSL
        }
    }
}
