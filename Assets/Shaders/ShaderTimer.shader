Shader "Kelas11/Belajar/03_Waktu"
{
    // PELAJARAN 3 — GPU punya jam: _Time.y = detik sejak Play.
    //
    // Dua efek yang sama-sama memakai waktu:
    // 1. Denyut = sin() membuat terang-gelap berulang.
    // 2. Gulir UV = geser koordinat sampel, gambar seperti berjalan.
    //
    // Siswa: ubah _KecepatanDenyut dan _KecepatanGulir di Inspector saat Play.

    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _KecepatanDenyut ("Kecepatan Denyut", Float) = 3
        _KecepatanGulir ("Kecepatan Gulir UV", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "Unlit2D"
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Tint;
                float _KecepatanDenyut;
                float _KecepatanGulir;
            CBUFFER_END

            struct Atribut
            {
                float4 posisiObjek : POSITION;
                float2 uv : TEXCOORD0;
                float4 warnaVertex : COLOR;
            };

            struct KeFragment
            {
                float4 posisiClip : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 warnaVertex : COLOR;
            };

            KeFragment Vert(Atribut masuk)
            {
                KeFragment keluar;
                keluar.posisiClip = TransformObjectToHClip(masuk.posisiObjek.xyz);
                keluar.uv = TRANSFORM_TEX(masuk.uv, _MainTex);
                keluar.warnaVertex = masuk.warnaVertex;
                return keluar;
            }

            half4 Frag(KeFragment masuk) : SV_Target
            {
                float2 uv = masuk.uv;
                uv.x += _Time.y * _KecepatanGulir;

                half4 teks = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                half4 kolom = teks * (half4)_Tint * masuk.warnaVertex;

                // sin() hasilnya -1..1, kita geser jadi 0.6..1 supaya tidak sampai hitam.
                half denyut = 0.6h + 0.4h * (half)sin(_Time.y * _KecepatanDenyut);
                kolom.rgb *= denyut;
                return kolom;
            }
            ENDHLSL
        }
    }
}
