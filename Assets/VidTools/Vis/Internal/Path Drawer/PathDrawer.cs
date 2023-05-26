using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using VidTools.Helpers;
using UnityEngine.Rendering;

public class PathDrawer
{
	Shader lineShader;
	Shader lineJoinsShader;

	const int circleJoinResolution = 32;

	Vector3[] points;
	Bounds bounds;
	ComputeBuffer lineSegmentsBuffer;
	ComputeBuffer pointsBuffer;
	ComputeBuffer lineArgsBuffer;
	ComputeBuffer joinsArgsBuffer;
	Material lineMat;
	Material joinsMat;
	Mesh lineSegmentMesh;
	Mesh circleJoinMesh;

	readonly int thicknessID = Shader.PropertyToID("thickness");
	readonly int colourID = Shader.PropertyToID("colour");

	void Init()
	{
		if (lineSegmentMesh == null)
		{
			CreateLineMesh();
		}
		if (circleJoinMesh == null)
		{
			CreateCircleJoinMesh();
		}
		if (lineMat == null)
		{
			lineMat = new Material(Shader.Find("LineTest/InstancedLines"));
			joinsMat = new Material(Shader.Find("LineTest/InstancedLineJoins"));
		}
	}

	public void Create(Vector3[] points, bool closed = false)
	{

		Init();

		this.points = points;
		if (points.Length > 1)
		{
			int numSegments = closed ? points.Length : points.Length - 1;
			LineSegment[] lineSegments = new LineSegment[numSegments];
			for (int i = 0; i < lineSegments.Length; i++)
			{
				int nextIndex = (i + 1) % points.Length;
				lineSegments[i] = new LineSegment() { pointA = points[i], pointB = points[nextIndex] };
			}
			Prepare(lineSegments);
		}
	}

	public void Draw(CommandBuffer cmd, Color colour, float thickness)
	{
		if (points != null && points.Length > 1)
		{
			lineMat.SetColor(colourID, colour);
			lineMat.SetFloat(thicknessID, thickness);
			//Graphics.DrawMeshInstancedIndirect(lineSegmentMesh, 0, lineMat, bounds, lineArgsBuffer);
			cmd.DrawMeshInstancedIndirect(lineSegmentMesh, 0, lineMat, 0, lineArgsBuffer);

			joinsMat.SetColor(colourID, colour);
			joinsMat.SetFloat(thicknessID, thickness);
			//Graphics.DrawMeshInstancedIndirect(circleJoinMesh, 0, joinsMat, bounds, joinsArgsBuffer);
			cmd.DrawMeshInstancedIndirect(circleJoinMesh, 0, joinsMat, 0, joinsArgsBuffer);
		}
	}


	void CreateLineMesh()
	{
		lineSegmentMesh = new Mesh();

		Vector3[] vertices = {
			new Vector3(0,-0.5f), // bottom left
			new Vector3(1,-0.5f), // bottom right
			new Vector3(1,0.5f), // top right
			new Vector3(0, 0.5f) // top left
		};

		int[] triangles = { 0, 2, 1, 0, 3, 2 };

		lineSegmentMesh.SetVertices(vertices);
		lineSegmentMesh.SetTriangles(triangles, 0, true);
	}

	void CreateCircleJoinMesh()
	{
		int numIncrements = (int)Mathf.Max(3, circleJoinResolution);

		float angleIncrement = (2 * Mathf.PI) / (numIncrements - 1f);
		var verts = new Vector3[numIncrements + 1];
		var tris = new int[(numIncrements - 1) * 3];
		verts[0] = Vector3.zero;

		for (int i = 0; i < numIncrements; i++)
		{
			float currAngle = angleIncrement * i;
			Vector3 pos = new Vector3(Mathf.Sin(currAngle), Mathf.Cos(currAngle), 0);
			verts[i + 1] = pos;

			if (i < numIncrements - 1)
			{
				tris[i * 3] = 0;
				tris[i * 3 + 1] = i + 1;
				tris[i * 3 + 2] = i + 2;
			}
		}
		circleJoinMesh = new Mesh();
		circleJoinMesh.SetVertices(verts);
		circleJoinMesh.SetTriangles(tris, 0, true);
	}

	void Prepare(LineSegment[] lineSegments)
	{
		if (lineSegmentsBuffer == null || lineSegments.Length != lineSegmentsBuffer.count)
		{
			Release();
			ComputeHelper.CreateStructuredBuffer<LineSegment>(ref lineSegmentsBuffer, lineSegments.Length);
			ComputeHelper.CreateStructuredBuffer<Vector3>(ref pointsBuffer, points.Length);
			lineArgsBuffer = ComputeHelper.CreateArgsBuffer(lineSegmentMesh, lineSegments.Length);
			joinsArgsBuffer = ComputeHelper.CreateArgsBuffer(circleJoinMesh, points.Length);
		}

		lineSegmentsBuffer.SetData(lineSegments);
		pointsBuffer.SetData(points);

		lineMat.SetBuffer("lineSegments", lineSegmentsBuffer);
		joinsMat.SetBuffer("points", pointsBuffer);

		// Calculate bounds
		bounds = new Bounds(lineSegments[0].pointA, Vector3.zero);
		for (int i = 1; i < lineSegments.Length; i++)
		{
			bounds.Encapsulate(lineSegments[i].pointB);
		}
	}

	public void Release()
	{
		ComputeHelper.Release(lineSegmentsBuffer, pointsBuffer, joinsArgsBuffer, lineArgsBuffer);
	}




	public struct LineSegment
	{
		public Vector3 pointA;
		public Vector3 pointB;
	}


}