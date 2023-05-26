Shader "LineTest/InstancedLineJoins" {
	SubShader {

		Pass {

			Tags {"LightMode"="ForwardBase"}
			
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.5

			#include "UnityCG.cginc"

			StructuredBuffer<float3> points;
			float thickness;
			float4 colour;

			struct v2f
			{
				float4 pos : SV_POSITION;
			};

			v2f vert (appdata_full v, uint instanceID : SV_InstanceID)
			{
				float3 worldCentre = points[instanceID];
				float3 worldVertPos = worldCentre + float3(v.vertex.xy,0) * thickness * 0.5;
				v2f o;
				o.pos = mul(UNITY_MATRIX_VP, float4(worldVertPos, 1.0f));
				return o;
			}

			float4 frag (v2f i) : SV_Target
			{
				return colour;
			}

			ENDCG
		}
	}
}