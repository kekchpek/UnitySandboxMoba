Shader "URP/Apearing"
{
    Properties
    { 
        _MainTex("Texture", 2D) = "white" {}

        [HDR]_Color("Color", Color) = (1,1,1,1)
        _SmoothnessVal ("Smoothness", Range(0,1)) = 0.5
        _MetallicVal ("Metallic", Range(0,1)) = 0.0
        _Emission ("Emission", Range(0,1)) = 0.0

        _MinMeshY("Min mesh Y", float) = -1
        _MaxMeshY("Max mesh Y", float) = -1
        _ClipBorderMax("Clip border max", Range(0, 1)) = 0
        _ClipBorderMin("Clip border min", Range(0, 1)) = 0

        _BorderRangePercentage("Border Range Percentage", Range(0,1)) = 0.03

        [HDR]_BorderColor("BorderColor", Color) = (1,1,1,1)
        _BorderEmission ("Border Emission", float) = 0.0

        _AppearingRotationQuaternion("Appearing Axis Rotation", vector) = (0,0,0,1)
        _AppearingOriginPoint("Appearing Origin Point", vector) = (0,0,0)
    }


    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalRenderPipeline" 
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            
            half _SmoothnessVal;
            half _MetallicVal;
            half _Emission;
            float _MinMeshY;
            float _MaxMeshY;
            float _ClipBorderMax;
            float _ClipBorderMin;
            float _BorderRangePercentage;
            float4 _Color;
            float4 _BorderColor;
            float _BorderEmission;
            float4 _AppearingRotationQuaternion;
            float3 _AppearingOriginPoint;

            TEXTURE2D(_MainTex);
            
            SAMPLER(sampler_MainTex);
               
            float4 _MainTex_ST;
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 uv           : TEXCOORD0;
                float2 uvLM         : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID          
            };
            
            struct SurfaceInput
            {
                float2 uv                       : TEXCOORD0;
                float2 uvLM                     : TEXCOORD1;
                float4 positionWSAndFogFactor   : TEXCOORD2; // xyz: positionWS, w: vertex fog factor
                half3  normalWS                 : TEXCOORD3;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 pos : CUSTOM_POS;
                SurfaceInput surfaceInput : CUSTOM_SURF;
            };     
            
            float4 hamiltonProduct(float4 q1, float4 q2)
            {
                float4 res =
                {
                    q1.w*q2.x + q1.x*q2.w + q1.y*q2.z - q1.z*q2.y,
                    q1.w*q2.y - q1.x*q2.z + q1.y*q2.w + q1.z*q2.x,
                    q1.w*q2.z + q1.x*q2.y - q1.y*q2.x + q1.z*q2.w,
                    q1.w*q2.w - q1.x*q2.x - q1.y*q2.y - q1.z*q2.z
                };
                return res;
            }

            

            SurfaceInput surfaceVert(Attributes input)
            {
                SurfaceInput output;

                // VertexPositionInputs contains position in multiple spaces (world, view, homogeneous clip space)
                // Our compiler will strip all unused references (say you don't use view space).
                // Therefore there is more flexibility at no additional cost with this struct.
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

                // Similar to VertexPositionInputs, VertexNormalInputs will contain normal, tangent and bitangent
                // in world space. If not used it will be stripped.
                VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                // Computes fog factor per-vertex.
                float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

                // TRANSFORM_TEX is the same as the old shader library.
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);

                output.positionWSAndFogFactor = float4(vertexInput.positionWS, fogFactor);
                output.normalWS = vertexNormalInput.normalWS;
                return output;
            }

            float4 surfaceColor(SurfaceInput input, float3 emission, float4 colorArg)
            {
                half3 normalWS = input.normalWS;
                normalWS = normalize(normalWS);
                half3 bakedGI = SampleSH(normalWS);

                float3 positionWS = input.positionWSAndFogFactor.xyz;
                half3 viewDirectionWS = SafeNormalize(GetCameraPositionWS() - positionWS);

                half4 alb = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * colorArg;
                BRDFData brdfData;
                InitializeBRDFData(alb, _MetallicVal, 0, _SmoothnessVal, 0, brdfData);

                Light mainLight = GetMainLight();

                half3 color = GlobalIllumination(brdfData, bakedGI, 0, normalWS, viewDirectionWS);

                // LightingPhysicallyBased computes direct light contribution.
                color += LightingPhysicallyBased(brdfData, mainLight, normalWS, viewDirectionWS);

                color += emission;

                float fogFactor = input.positionWSAndFogFactor.w;

                color = MixFog(color, fogFactor);

                return half4(color, 0);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.pos = IN.positionOS.xyz;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.surfaceInput = surfaceVert(IN);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                // position of the rendering pixel in local coordinates
                float3 localPos = TransformObjectToWorld(IN.pos)  - _AppearingOriginPoint;
                // a total length of the area where pixels should be
                float length = (_MaxMeshY - _MinMeshY);

                float4 q = _AppearingRotationQuaternion;
                float4 q_ = float4(-q.x, -q.y, -q.z, q.w);
                float4 pos = float4(localPos, 0);
                float4 rotatedPos = hamiltonProduct(hamiltonProduct(q_, pos), q);
                // position of the rendering pixel relatively the length
                float level = rotatedPos.y - _MinMeshY;


                // range of the area where the rendering pixel should be colored as a border of appearing
                // it includes half of itself before min and after max
                float borderRange = length * _BorderRangePercentage;
                float max = (length + borderRange) * _ClipBorderMax - borderRange * 0.5;
                float min = (length + borderRange) * _ClipBorderMin - borderRange * 0.5;

                clip(  level - (min - 0.5 * borderRange)  );
                clip(  (max + 0.5 * borderRange) - level  );

                float4 col = surfaceColor(IN.surfaceInput, _Emission * _Color, _Color);

                float tmp = min + borderRange * 0.5;
                if (level < tmp) col = surfaceColor(IN.surfaceInput, _BorderEmission * _BorderColor, _BorderColor);

                tmp = max - borderRange * 0.5;
                if (level > tmp) col = surfaceColor(IN.surfaceInput, _BorderEmission * _BorderColor, _BorderColor);

                return col;
            }
            ENDHLSL
        }
    }
}