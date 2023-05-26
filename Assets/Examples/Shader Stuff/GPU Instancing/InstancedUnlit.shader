Shader "Instanced/InstancedUnlit" {
	Properties {
		
	}
	SubShader {

		Tags { "Queue"="Geometry" }

		Pass {

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.5

			#include "UnityCG.cginc"
			
			struct Particle {
				float3 position;
				float3 colour;
			};

			StructuredBuffer<Particle> Particles;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 colour : TEXCOORD0;
			};

			v2f vert (appdata_full v, uint instanceID : SV_InstanceID)
			{
				Particle particle = Particles[instanceID];
				float3 worldVertPos = particle.position + mul(unity_ObjectToWorld, v.vertex);
				float3 objectVertPos = mul(unity_WorldToObject, float4(worldVertPos.xyz, 1));
				v2f o;

				o.pos = UnityObjectToClipPos(objectVertPos);
				o.colour = particle.colour;
				return o;
			}

			float4 frag (v2f i) : SV_Target
			{
				float3 colour = i.colour;
				return float4(colour.rgb, 1);
			}

			ENDCG
		}
	}
}