using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VidTools.Helpers;

public class ProceduralTest : MonoBehaviour
{

	public ComputeShader compute;
	public Shader drawShader;
	ComputeBuffer vertBuffer;
	Material testMat;

	void Start()
	{
		ComputeHelper.CreateStructuredBuffer<Vector3>(ref vertBuffer, 6);
		compute.SetBuffer(0, "VertexBuffer", vertBuffer);
		ComputeHelper.Dispatch(compute, 1);

		testMat = new Material(drawShader);
		testMat.SetBuffer("VertexBuffer", vertBuffer);
	}
	
	void Update()
	{
		testMat.SetPass(0);
		Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 100);
		Graphics.DrawProcedural(testMat, bounds, MeshTopology.Triangles, 6, 1, null);
	}

	void OnDestroy()
	{
		ComputeHelper.Release(vertBuffer);
	}
}
