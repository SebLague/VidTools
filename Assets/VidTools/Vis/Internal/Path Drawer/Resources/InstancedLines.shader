Shader "LineTest/InstancedLines" {
	SubShader {

		Pass {

			Tags {"LightMode"="ForwardBase"}

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.5

			#include "UnityCG.cginc"

			
			struct LineSegment {
				float3 pointA;
				float3 pointB;
			};

			StructuredBuffer<LineSegment> lineSegments;
			float thickness;
			float4 colour;

			struct v2f
			{
				float4 pos : SV_POSITION;
			};
			
			
			v2f vert (appdata_full v, uint instanceID : SV_InstanceID)
			{
				LineSegment data = lineSegments[instanceID];
				float3 lineOffset = data.pointB - data.pointA;
				float3 linePerpDir = normalize(cross(lineOffset, float3(0,0,-1)));
				float3 vertexPos = v.vertex.xyz;
				float3 pos = data.pointA + lineOffset * vertexPos.x + linePerpDir * vertexPos.y * thickness;

				v2f o;
				o.pos = mul(UNITY_MATRIX_VP, float4(pos, 1.0f));
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