Shader "Custom/PrismShader"
{
    Properties
    {
        _CapEmission ("Cap Emission Color", Color) = (1, 1, 1, 1)
        _Side ("Side Color", Color) = (1, 1, 1, 1)
        _SmoothnessMask ("Smoothness Mask", 2D) = "white" {}

        _Color ("Base Color", Color) = (1,1,1,1)
        _Metallic ("Metallic", Range(0, 1)) = 0.5
        _GlossinessMin ("Min Smoothness", Range(0,1)) = 0.0
        _GlossinessMax("Max Smoothness", Range(0, 1)) = 1.0
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

        sampler2D _SmoothnessMask;

        struct Input
        {
            float2 uv_SmoothnessMask;
            float3 worldNormal;
            float3 worldPos;
        };

        fixed4 _Side;

        float _GlossinessMin;
        float _GlossinessMax;
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
            float smoothness = tex2D(_SmoothnessMask, IN.uv_SmoothnessMask).x;

            // Albedo comes from a texture tinted by color
            if (caps > 0.5f) {
                o.Albedo = UNITY_ACCESS_INSTANCED_PROP(Props, _CapEmission);
            } else {
                o.Albedo = _Side.rgb;
            }
            o.Smoothness = lerp(_GlossinessMin, _GlossinessMax, smoothness.r);

            o.Metallic = _Metallic;
            o.Alpha = 1.0f;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
