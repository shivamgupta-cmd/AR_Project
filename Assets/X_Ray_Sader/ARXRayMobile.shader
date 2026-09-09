Shader "AR/XRayVisible"
{
    Properties
    {
        _XRayColor ("X-Ray Color", Color) = (0.0, 0.8, 1.0, 1.0)

        _BaseAlpha ("Body Visibility", Range(0,1)) = 0.45

        _RimPower ("Rim Power", Range(0.5,8)) = 2.0
        _RimIntensity ("Rim Intensity", Range(0,5)) = 2.0
        _RimAlpha ("Rim Alpha", Range(0,1)) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "XRay"

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
            };


            CBUFFER_START(UnityPerMaterial)

                half4 _XRayColor;
                half _BaseAlpha;
                half _RimPower;
                half _RimIntensity;
                half _RimAlpha;

            CBUFFER_END


            Varyings vert(Attributes input)
            {
                Varyings output;

                float3 positionWS =
                    TransformObjectToWorld(input.positionOS.xyz);

                output.positionHCS =
                    TransformWorldToHClip(positionWS);

                output.normalWS =
                    TransformObjectToWorldNormal(input.normalOS);

                output.viewDirWS =
                    GetWorldSpaceViewDir(positionWS);

                return output;
            }


            half4 frag(Varyings input) : SV_Target
            {
                half3 normal =
                    normalize(input.normalWS);

                half3 viewDir =
                    normalize(input.viewDirWS);


                half fresnel =
                    1.0h - abs(dot(normal, viewDir));


                fresnel =
                    pow(fresnel, _RimPower);


                half glow =
                    fresnel * _RimIntensity;


                half3 color =
                    _XRayColor.rgb *
                    (0.6h + glow);


                half alpha =
                    _BaseAlpha +
                    fresnel * _RimAlpha;


                alpha = saturate(alpha);


                return half4(
                    color,
                    alpha
                );
            }

            ENDHLSL
        }
    }

    FallBack Off
}




// Shader "AR/XRayVisible"
// {
//     Properties
//     {
//         [Header(X Ray Colors)]
//         [HDR] _BodyColor ("Body Color", Color) = (0.01, 0.18, 0.55, 1)
//         [HDR] _EdgeColor ("Edge Color", Color) = (0.25, 0.75, 1.0, 1)

//         [Header(Transparency)]
//         _BodyAlpha ("Body Visibility", Range(0,1)) = 0.22
//         _BackAlpha ("Inside Visibility", Range(0,1)) = 0.16

//         [Header(Edge)]
//         _RimPower ("Edge Sharpness", Range(0.5,8)) = 2.2
//         _RimIntensity ("Edge Brightness", Range(0,8)) = 3.5

//         [Header(Internal)]
//         _InsideIntensity ("Inside Brightness", Range(0,5)) = 1.2
//     }

//     SubShader
//     {
//         Tags
//         {
//             "Queue" = "Transparent"
//             "RenderType" = "Transparent"
//             "RenderPipeline" = "UniversalPipeline"
//             "IgnoreProjector" = "True"
//         }

//         // ====================================================
//         // PASS 1
//         // BACK FACES / INTERNAL X-RAY LOOK
//         // ====================================================

//         Pass
//         {
//             Name "XRayInside"

//             Cull Front
//             ZWrite Off
//             ZTest LEqual

//             // Additive blending gives glowing internal surfaces
//             Blend SrcAlpha One

//             HLSLPROGRAM

//             #pragma vertex vert
//             #pragma fragment fragBack
//             #pragma target 2.0

//             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

//             struct Attributes
//             {
//                 float4 positionOS : POSITION;
//                 float3 normalOS : NORMAL;
//             };

//             struct Varyings
//             {
//                 float4 positionHCS : SV_POSITION;
//                 float3 normalWS : TEXCOORD0;
//                 float3 viewDirWS : TEXCOORD1;
//             };

//             CBUFFER_START(UnityPerMaterial)

//             half4 _BodyColor;
//             half4 _EdgeColor;

//             half _BodyAlpha;
//             half _BackAlpha;

//             half _RimPower;
//             half _RimIntensity;

//             half _InsideIntensity;

//             CBUFFER_END


//             Varyings vert(Attributes input)
//             {
//                 Varyings output;

//                 float3 positionWS =
//                     TransformObjectToWorld(input.positionOS.xyz);

//                 output.positionHCS =
//                     TransformWorldToHClip(positionWS);

//                 output.normalWS =
//                     TransformObjectToWorldNormal(input.normalOS);

//                 output.viewDirWS =
//                     GetWorldSpaceViewDir(positionWS);

//                 return output;
//             }


//             half4 fragBack(Varyings input) : SV_Target
//             {
//                 half3 normalWS =
//                     normalize(input.normalWS);

//                 half3 viewDir =
//                     normalize(input.viewDirWS);


//                 half fresnel =
//                     1.0h -
//                     saturate(
//                         abs(dot(normalWS, viewDir))
//                     );


//                 fresnel =
//                     pow(fresnel, _RimPower);


//                 half3 color =
//                     _BodyColor.rgb *
//                     _InsideIntensity;


//                 color +=
//                     _EdgeColor.rgb *
//                     fresnel *
//                     _RimIntensity *
//                     0.35h;


//                 half alpha =
//                     _BackAlpha +
//                     fresnel * 0.2h;


//                 return half4(
//                     color,
//                     saturate(alpha)
//                 );
//             }

//             ENDHLSL
//         }



//         // ====================================================
//         // PASS 2
//         // FRONT TRANSPARENT BODY + BRIGHT X-RAY EDGES
//         // ====================================================

//         Pass
//         {
//             Name "XRaySurface"

//             Cull Back
//             ZWrite Off
//             ZTest LEqual

//             Blend SrcAlpha OneMinusSrcAlpha

//             HLSLPROGRAM

//             #pragma vertex vert
//             #pragma fragment fragFront
//             #pragma target 2.0

//             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


//             struct Attributes
//             {
//                 float4 positionOS : POSITION;
//                 float3 normalOS : NORMAL;
//             };


//             struct Varyings
//             {
//                 float4 positionHCS : SV_POSITION;
//                 float3 normalWS : TEXCOORD0;
//                 float3 viewDirWS : TEXCOORD1;
//             };


//             CBUFFER_START(UnityPerMaterial)

//             half4 _BodyColor;
//             half4 _EdgeColor;

//             half _BodyAlpha;
//             half _BackAlpha;

//             half _RimPower;
//             half _RimIntensity;

//             half _InsideIntensity;

//             CBUFFER_END


//             Varyings vert(Attributes input)
//             {
//                 Varyings output;


//                 float3 positionWS =
//                     TransformObjectToWorld(
//                         input.positionOS.xyz
//                     );


//                 output.positionHCS =
//                     TransformWorldToHClip(
//                         positionWS
//                     );


//                 output.normalWS =
//                     TransformObjectToWorldNormal(
//                         input.normalOS
//                     );


//                 output.viewDirWS =
//                     GetWorldSpaceViewDir(
//                         positionWS
//                     );


//                 return output;
//             }


//             half4 fragFront(Varyings input) : SV_Target
//             {
//                 half3 normalWS =
//                     normalize(input.normalWS);


//                 half3 viewDir =
//                     normalize(input.viewDirWS);


//                 // -----------------------------------------
//                 // Fresnel
//                 // Camera-facing surface dark rahegi,
//                 // side edges bright hongi.
//                 // -----------------------------------------

//                 half fresnel =
//                     1.0h -
//                     saturate(
//                         abs(dot(normalWS, viewDir))
//                     );


//                 fresnel =
//                     pow(
//                         fresnel,
//                         _RimPower
//                     );


//                 // -----------------------------------------
//                 // Dark blue transparent body
//                 // -----------------------------------------

//                 half3 body =
//                     _BodyColor.rgb;


//                 // -----------------------------------------
//                 // White / cyan glowing outline
//                 // -----------------------------------------

//                 half3 edge =
//                     _EdgeColor.rgb *
//                     fresnel *
//                     _RimIntensity;


//                 half3 finalColor =
//                     body + edge;


//                 // Center transparent,
//                 // edges more solid
//                 half alpha =
//                     _BodyAlpha +
//                     fresnel * 0.75h;


//                 return half4(
//                     finalColor,
//                     saturate(alpha)
//                 );
//             }

//             ENDHLSL
//         }
//     }

//     FallBack Off
// }