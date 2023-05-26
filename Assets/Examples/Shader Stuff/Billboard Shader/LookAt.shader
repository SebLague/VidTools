Shader "Unlit/LookAt"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;

			v2f vert (appdata v)
			{
				v2f o;
				float3 vertexWorldPos = mul(unity_ObjectToWorld, v.vertex);
				float3 worldOffset = float3(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13, unity_ObjectToWorld._m23);
				float4 viewPos = mul(UNITY_MATRIX_V, float4(worldOffset, 1)) + float4(vertexWorldPos, 0);

				o.vertex = mul(UNITY_MATRIX_P, viewPos);//
				o.uv = v.uv;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{	
				return 1;
			}
			ENDCG
		}
	}
}
