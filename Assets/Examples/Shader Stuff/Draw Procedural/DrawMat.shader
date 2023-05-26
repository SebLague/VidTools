Shader "Unlit/DrawMat"
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

			StructuredBuffer<float3> VertexBuffer;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				uint vertexID : SV_VertexID;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(float4(VertexBuffer[v.vertexID], 1.0));
				o.uv = v.uv;
				return o;
			}

			float4 frag (v2f i) : SV_Target
			{
				return float4(1,0,0,1);
			}
			ENDCG
		}
	}
}
