Shader "Unlit/Apearing"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}

        _Color("Color", Color) = (1,1,1,1)
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Emission ("Emission", Range(0,1)) = 0.0

        _MinMeshY("Min mesh Y", float) = -1
        _MaxMeshY("Max mesh Y", float) = -1
        _ClipBorderMax("Clip border max", Range(0, 1)) = 0
        _ClipBorderMin("Clip border min", Range(0, 1)) = 0

        _BorderRangePercentage("Border Range Percentage", Range(0,1)) = 0.03

        _BorderColor("BorderColor", Color) = (1,1,1,1)
        _BorderEmission ("Border Emission", Range(0,1)) = 0.0

        _AppearingRotationQuaternion("Appearing Axis Rotation", vector) = (0,0,0,1)
        _AppearingOriginPoint("Appearing Origin Point", vector) = (0,0,0)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGPROGRAM
        #include "UnityCG.cginc"
        #pragma surface surf Standard fullforwardshadows

            
        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };
            
        half _Smoothness;
        half _Metallic;
        half _Emission;
        float _MinMeshY;
        float _MaxMeshY;
        float _ClipBorderMax;
        float _ClipBorderMin;
        float _BorderRangePercentage;
        float4 _Color;
        float4 _BorderColor;
        float _BorderEmission;
        sampler2D _MainTex;
        float4 _AppearingRotationQuaternion;
        float3 _AppearingOriginPoint;

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

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // position of the rendering pixel in local coordinates
            float3 localPos = IN.worldPos - _AppearingOriginPoint;
            // a total length of the area where pixels should be
            float length = (_MaxMeshY - _MinMeshY);

            float4 q = _AppearingRotationQuaternion;
            float4 q_ = float4(-q.x, -q.y, -q.z, q.w);
            float4 pos = float4(localPos.x, localPos.y, localPos.z, 0);
            float4 rotatedPos = hamiltonProduct(hamiltonProduct(q, pos), q_);
            // position of the rendering pixel relatively the length
            float level = rotatedPos.y - _MinMeshY;


            // range of the area where the rendering pixel should be colored as a border of appearing
            // it includes half of itself before min and after max
            float borderRange = length * _BorderRangePercentage;
            float max = (length + borderRange) * _ClipBorderMax - borderRange * 0.5;
            float min = (length + borderRange) * _ClipBorderMin - borderRange * 0.5;

            clip(  level - (min - 0.5 * borderRange)  );
            clip(  (max + 0.5 * borderRange) - level  );

            fixed4 col = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            float3 emission = _Emission;

            float tmp = min + borderRange * 0.5;
            if (level < tmp) col = _BorderColor;
            if (level < tmp) emission = _BorderEmission * _BorderColor.rgb;

            tmp = max - borderRange * 0.5;
            if (level > tmp) col = _BorderColor;
            if (level > tmp) emission = _BorderEmission * _BorderColor.rgb;


            o.Emission = emission;
            o.Albedo = col.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Alpha = col.a;
        }
        ENDCG
    }
}