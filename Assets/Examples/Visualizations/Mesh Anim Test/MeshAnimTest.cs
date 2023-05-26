using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VidTools;
using VidTools.Vis;
using UnityEngine.InputSystem;
using VidTools.Helpers;

namespace VidTools.Examples
{
	[ExecuteAlways]
	public class MeshAnimTest : ImmediateAnimation
	{

		[Header("Display Settings")]
		public Vector2Int resolution;
		public Material mat;
		Mesh mesh;


		protected override void UpdateAnimation()
		{
			if (mesh == null)
			{
				mesh = new Mesh();
			}
			else
			{
				mesh.Clear();
			}

			float animT = CalculateClipTime(5, Ease.Cubic.InOut);

			resolution = new Vector2Int(Mathf.Max(2, resolution.x), Mathf.Max(2, resolution.y));
			Vector3[] verts = new Vector3[resolution.x * resolution.y];
			Vector2[] uvs = new Vector2[verts.Length];
			int numTris = (resolution.x - 1) * (resolution.y - 1) * 2;
			int[] indices = new int[numTris * 3];
			int triIndex = 0;

			for (int y = 0; y < resolution.y; y++)
			{
				for (int x = 0; x < resolution.x; x++)
				{
					Vector2 t = new Vector2(x / (resolution.x - 1f), y / (resolution.y - 1f));
					float latitude = (t.y - 0.5f) * Mathf.PI;
					float longitude = (t.x - 0.5f) * Mathf.PI * 2;

					Vector2 posPlane = new Vector2(longitude, latitude);
					float sphereRadius = Mathf.Sqrt(Mathf.PI / 2f);
					Vector3 posSphere = Maths.CoordinateToSpherePoint(latitude, longitude, sphereRadius);
					Vector3 vertexPos = Vector3.Lerp(posPlane, posSphere, animT);

					int vertIndex = y * resolution.x + x;
					verts[vertIndex] = vertexPos;
					uvs[vertIndex] = t;

					if (y < resolution.y - 1 && x < resolution.x - 1)
					{
						indices[triIndex + 0] = vertIndex;
						indices[triIndex + 1] = vertIndex + resolution.x;
						indices[triIndex + 2] = vertIndex + 1;
						indices[triIndex + 3] = vertIndex + 1;
						indices[triIndex + 4] = vertIndex + resolution.x;
						indices[triIndex + 5] = vertIndex + resolution.x + 1;

						triIndex += 6;
					}
				}
			}

			mesh.SetVertices(verts);
			mesh.SetTriangles(indices, 0, true);
			mesh.SetUVs(0, uvs);
			mesh.RecalculateNormals();

			Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, mat, 0);
		}

	}
}