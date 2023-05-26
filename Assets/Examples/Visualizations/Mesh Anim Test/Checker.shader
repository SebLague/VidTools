Shader "Example/Checker"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Cull Off
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
			int faceDir : VFACE;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float2 mod2(float2 x, float2 y)
		{
			return x - y * floor(x/y);
		}

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float3 col = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			o.Albedo = col * max(0, IN.faceDir);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
