using UnityEngine;
using VidTools.Vis.Internal;
using VidTools.Vis.Internal.MeshGeneration;
using static VidTools.Vis.Internal.DrawManager;

namespace VidTools.Vis
{
	// TODO: most draw functions don't support instancing. Should probably fix that at some point...
	public static class Draw
	{
		
		// NOTE: My polygon mesh generator is a bit buggy. Should be replaced with better implementation.
		public static void Polygon(Vector2[] points, Color col)
		{
			if (points.Length <= 2) { return; }

			EnsureFrameInitialized();
			materialProperties.SetColor(DrawMaterials.colorID, col);
			Mesh mesh = meshPool.GetItem();
			PolygonMeshGenerator.GeneratePolygonMesh(mesh, points);
			cmd.DrawMesh(mesh, Matrix4x4.identity, DrawMaterials.unlitMat, 0, 0, materialProperties);
		}


		public static void Quad(Vector2 centre, Vector2 size, Color col)
		{
			if (size.x == 0 && size.y == 0) { return; }
			EnsureFrameInitialized();
			materialProperties.SetColor(DrawMaterials.colorID, col);
			Matrix4x4 matrix = Matrix4x4.TRS(centre, Quaternion.identity, new Vector3(size.x, size.y, 1));
			cmd.DrawMesh(QuadMeshGenerator.GetQuadMesh(), matrix, DrawMaterials.unlitMat, 0, 0, materialProperties);
		}

		public static void Quad(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color col)
		{
			EnsureFrameInitialized();
			Mesh mesh = QuadMeshGenerator.GetQuadMesh();
			materialProperties.SetColor(DrawMaterials.colorID, col);
			materialProperties.SetVector(DrawMaterials.quadPointA, a);
			materialProperties.SetVector(DrawMaterials.quadPointB, b);
			materialProperties.SetVector(DrawMaterials.quadPointC, c);
			materialProperties.SetVector(DrawMaterials.quadPointD, d);
			cmd.DrawMesh(mesh, Matrix4x4.identity, DrawMaterials.quadMat, 0, 0, materialProperties);
		}

		public static void Mesh(Mesh mesh, Color col)
		{
			Mesh(mesh, Vector3.zero, Quaternion.identity, Vector3.one, col);
		}

		public static void Mesh(Mesh mesh, Vector3 pos, Quaternion rot, Vector3 scale, Color col)
		{
			EnsureFrameInitialized();
			materialProperties.SetColor(DrawMaterials.colorID, col);
			Matrix4x4 matrix = Matrix4x4.TRS(pos, rot, scale);
			cmd.DrawMesh(mesh, matrix, DrawMaterials.unlitMat, 0, 0, materialProperties);
		}

		public static void Arrow(Vector2 start, Vector2 end, ArrowSettings settings, float t = 1)
		{
			end = Vector3.Lerp(start, end, t); // animate end point
			if (settings.thickness <= 0 || (start - end).sqrMagnitude == 0) { return; }
			EnsureFrameInitialized();

			Vector2 dir = (end - start).normalized;
			Vector2 perp = new Vector2(-dir.y, dir.x);
			float angle = Mathf.Atan2(dir.y, dir.x);


			float headAngleA = angle + Mathf.PI + settings.headAngleDegrees * Mathf.Deg2Rad * (settings.fillHead ? 1 : t);
			float headAngleB = angle + Mathf.PI - settings.headAngleDegrees * Mathf.Deg2Rad * (settings.fillHead ? 1 : t);
			Vector2 headDirA = new Vector2(Mathf.Cos(headAngleA), Mathf.Sin(headAngleA));
			Vector2 headDirB = new Vector2(Mathf.Cos(headAngleB), Mathf.Sin(headAngleB));

			float maxHeadLength = VisMaths.RayLineIntersectionDistance(end, headDirA, start - perp, start + perp);
			Vector2 headEndA = end + headDirA * Mathf.Min(maxHeadLength, settings.headLength);
			Vector2 headEndB = end + headDirB * Mathf.Min(maxHeadLength, settings.headLength);

			if (settings.fillHead)
			{
				float v = Mathf.Cos(settings.headAngleDegrees * Mathf.Deg2Rad) * settings.headLength;
				Line(start, end - dir * Mathf.Min((end - start).magnitude, v), settings.thickness, settings.col, false);
				Polygon(new Vector2[] { end, headEndA, headEndB }, settings.col);
			}
			else
			{
				Line(start, end, settings.thickness, settings.col, true);
				Line(end, headEndA, settings.thickness, settings.col, true, t);
				Line(end, headEndB, settings.thickness, settings.col, true, t);
			}
		}

		public static void Line(Vector3 start, Vector3 end, float thickness, Color col, float t = 1)
		{
			Line(start, end, thickness, col, false, t);
		}

		public static void Line(Vector3 start, Vector3 end, float thickness, Color col, bool roundedEdges, float t = 1)
		{
			if (roundedEdges == false)
			{
				DrawLineSharpEdges(start, end, thickness, col, t);
				return;
			}
			end = Vector3.Lerp(start, end, t); // animate end point
			if (thickness <= 0 || (start - end).sqrMagnitude == 0) { return; }

			EnsureFrameInitialized();
			float length = (start - end).magnitude;
			// Squish the rounding effect to 0 as line length goes from thickness -> 0
			float thicknessScaleT = Mathf.Min(1, length / thickness);
			Vector3 scale = new Vector3(length + thickness * 1, thickness, 1);

			materialProperties.SetColor(DrawMaterials.colorID, col);
			materialProperties.SetVector(DrawMaterials.sizeID, new Vector3(length + thickness, thickness, 1));

			Vector3 centre = (start + end) / 2;
			Quaternion rot = Quaternion.FromToRotation(Vector3.left, start - end);

			Matrix4x4 matrix = Matrix4x4.TRS(centre, rot, scale);
			cmd.DrawMesh(QuadMeshGenerator.GetQuadMesh(), matrix, DrawMaterials.lineMatRoundedEdge, 0, 0, materialProperties);
		}

		static void DrawLineSharpEdges(Vector3 start, Vector3 end, float thickness, Color col, float t = 1)
		{
			end = Vector3.Lerp(start, end, t); // animate end point
			if (thickness <= 0 || (start - end).sqrMagnitude == 0) { return; }

			EnsureFrameInitialized();
			float length = (start - end).magnitude;
			Vector3 scale = new Vector3(length, thickness, 1);

			materialProperties.SetColor(DrawMaterials.colorID, col);
			materialProperties.SetVector(DrawMaterials.sizeID, new Vector3(length, thickness, 1));

			Vector3 centre = (start + end) / 2;
			Quaternion rot = Quaternion.FromToRotation(Vector3.left, start - end);

			Matrix4x4 matrix = Matrix4x4.TRS(centre, rot, scale);
			cmd.DrawMesh(QuadMeshGenerator.GetQuadMesh(), matrix, DrawMaterials.lineMat, 0, 0, materialProperties);
		}


		public static void Ray(Vector3 start, Vector3 offset, float thickness, Color col)
		{
			Line(start, start + offset, thickness, col);
		}
		public static void PathInstancedTest(Vector3[] points, float thickness, Color col, bool closed = false)
		{
			EnsureFrameInitialized();

			var p = pathDrawPool.Get();
			p.Create(points, closed);
			p.Draw(cmd, col, thickness);
		}

		public static void Path(Vector2[] points, float thickness, bool closed, Color col)
		{
			Vector3[] points3D = new Vector3[points.Length];
			for (int i = 0; i < points.Length; i++)
			{
				points3D[i] = new Vector3(points[i].x, points[i].y, 0);
			}
			Path(points3D, thickness, closed, col);
		}

		public static void Path(Vector3[] points, float thickness, bool closed, Color col, float t = 1)
		{
			if (t == 0) { return; }

			float totalLength = 0;
			for (int i = 0; i < points.Length - 1; i++)
			{
				totalLength += Vector3.Distance(points[i], points[i + 1]);
			}
			if (closed)
			{
				totalLength += (Vector3.Distance(points[0], points[^1]));
			}

			float drawLength = totalLength * t;
			float lengthDrawn = 0;

			int lim = closed ? points.Length : points.Length - 1;
			for (int i = 0; i < lim; i++)
			{
				bool exit = false;
				int nextIndex = (i + 1) % points.Length;
				float segLength = Vector3.Distance(points[i], points[nextIndex]);
				if (lengthDrawn + segLength > drawLength)
				{
					segLength = drawLength - lengthDrawn;
					exit = true;
				}
				Vector3 a = points[i];
				Vector3 b = points[nextIndex];
				b = a + (b - a).normalized * segLength;
				Draw.Line(a, b, thickness, col, true);
				lengthDrawn += segLength;
				if (exit)
				{
					break;
				}
			}
		}

		public static void BoxOutline(Vector2 centre, Vector2 size, float thickness, Color col, float t = 1)
		{
			Vector3[] path =
			{
				centre + new Vector2(-size.x,size.y) * 0.5f,
				centre + new Vector2(size.x,size.y) * 0.5f,
				centre + new Vector2(size.x,-size.y) * 0.5f,
				centre + new Vector2(-size.x,-size.y) * 0.5f
			};
			Draw.Path(path, thickness, true, col, t);
		}

		// Draw a 2D point
		public static void Point(Vector3 centre, float radius, Color col)
		{
			// Skip if radius or alpha is zero
			if (radius == 0 || col.a == 0) { return; }

			// Initialize frame (ensures draw commands from prev frame are cleared, etc.)
			EnsureFrameInitialized();

			// Create matrix to control position/rotation/scale of mesh
			Vector3 scale = new Vector3(radius * 2, radius * 2, 1);
			Matrix4x4 matrix = Matrix4x4.TRS(centre, Quaternion.identity, scale);

			// Draw quad mesh with point shader
			Mesh quadMesh = QuadMeshGenerator.GetQuadMesh();
			materialProperties.SetColor(DrawMaterials.colorID, col);
			cmd.DrawMesh(quadMesh, matrix, DrawMaterials.pointMat, 0, 0, materialProperties);
		}


		public static void Sphere(Vector3 centre, float radius, Color col, bool unlit = false)
		{
			EnsureFrameInitialized();
			materialProperties.SetColor(DrawMaterials.colorID, col);
			Matrix4x4 matrix = Matrix4x4.TRS(centre, Quaternion.identity, Vector3.one * radius);
			cmd.DrawMesh(SphereMeshGenerator.GetMesh(), matrix, unlit ? DrawMaterials.unlitMat : DrawMaterials.shadedMat, 0, 0, materialProperties);
		}

		public static void Cube(Vector3 centre, Quaternion rotation, Vector3 scale, Color col)
		{
			EnsureFrameInitialized();
			materialProperties.SetColor(DrawMaterials.colorID, col);
			Matrix4x4 matrix = Matrix4x4.TRS(centre, rotation, scale);
			cmd.DrawMesh(CubeMeshGenerator.GetMesh(), matrix, DrawMaterials.unlitMat, 0, 0, materialProperties);
		}

	}
}