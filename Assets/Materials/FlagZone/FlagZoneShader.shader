Shader "Custom/TransparentWithContactEdge"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_EdgeDistance("Edge Distance", Range(0,1)) = 0.2
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		Cull Off

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _CameraDepthTexture;

		struct Input
		{
			float2 uv_MainTex;
			float4 screenPos;
		};

		fixed4 _Color;
		half _EdgeDistance;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float depth = tex2D(_CameraDepthTexture, IN.screenPos.xy / IN.screenPos.w) * IN.screenPos.w;
			float4 depthPos = float4(IN.screenPos.xy, depth, IN.screenPos.w);
			float4 transDiff = depthPos - IN.screenPos;
			float edge = length(transDiff.xyz);
			float transparency = 1.0f - min(edge / _EdgeDistance, 1.0f);
			o.Emission.rgb = _Color * transparency;

			o.Metallic = 0.0f;
			o.Smoothness = 0.0f;
			o.Alpha = transparency;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
