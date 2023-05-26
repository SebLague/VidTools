using UnityEngine;
using VidTools.Helpers;

namespace VidTools.Examples
{
	public class InstancingExample : MonoBehaviour
	{
		public int numInstances = 100000;
		public Mesh mesh;
		public Shader instanceShader;
		public ComputeShader animateCompute;

		Material material;
		ComputeBuffer particleBuffer;
		ComputeBuffer argsBuffer;
		Bounds bounds;

		const float displaySize = 1;

		void Start()
		{
			material = new Material(instanceShader);
			material.enableInstancing = true;
			bounds = new Bounds(Vector3.zero, Vector3.one * 1000);

			// Create args buffer
			argsBuffer = ComputeHelper.CreateArgsBuffer(mesh, numInstances);

			Particle[] particles = new Particle[numInstances];
			for (int i = 0; i < numInstances; i++)
			{
				Particle p = new Particle();
				p.position = Random.insideUnitSphere * 10;
				p.colour = Vector3.one;
				particles[i] = p;
			}

			ComputeHelper.CreateStructuredBuffer<Particle>(ref particleBuffer, numInstances);
			particleBuffer.SetData(particles);
			material.SetBuffer("Particles", particleBuffer);

			animateCompute.SetBuffer(0, "Particles", particleBuffer);
			animateCompute.SetInt("numParticles", particleBuffer.count);
		}

		void Update()
		{
			animateCompute.SetFloat("deltaTime", Time.deltaTime);
			ComputeHelper.Dispatch(animateCompute, particleBuffer.count);

			Graphics.DrawMeshInstancedIndirect(mesh, 0, material, bounds, argsBuffer);
		}

		void OnDestroy()
		{
			ComputeHelper.Release(particleBuffer, argsBuffer);
		}

		public struct Particle
		{
			public Vector3 position;
			public Vector3 colour;
		}
	}
}