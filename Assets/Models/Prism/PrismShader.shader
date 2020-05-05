Shader "Custom/PrismShader"
{
    Properties
    {
        _CapEmission ("Cap Emission Color", Color) = (1, 1, 1, 1)
        _SideEmission ("Side Emission Color", Color) = (1, 1, 1, 1)
        _EmissionMask ("Emission Mask", 2D) = "white" {}

        _Color ("Base Color", Color) = (1,1,1,1)
        _Metallic ("Metallic", Range(0, 1)) = 0.5
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _EmissionMask;

        struct Input
        {
            float2 uv_EmissionMask;
            float3 worldNormal;
            float3 worldPos;
        };

        fixed4 _SideEmission;

        fixed4 _Color;
        float _Glossiness;
        float _Metallic;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _CapEmission)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float caps = abs(dot(IN.worldNormal, float3(0.0f, 1.0f, 0.0f)));
            float emission = tex2D(_EmissionMask, IN.uv_EmissionMask).x;

            // Albedo comes from a texture tinted by color
            if (caps > 0.5f) {
                o.Emission = emission * UNITY_ACCESS_INSTANCED_PROP(Props, _CapEmission);
            } else {
                o.Emission = emission * _SideEmission;
            }
            o.Albedo = (1.0f - emission) * _Color.rgb;
            o.Smoothness = lerp(_Glossiness, 1.0f, emission);

            o.Metallic = _Metallic;
            o.Alpha = 1.0f;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
