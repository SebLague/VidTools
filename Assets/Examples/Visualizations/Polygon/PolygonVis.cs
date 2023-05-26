using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VidTools.Vis;
using VidTools.Helpers;

namespace VidTools.Examples
{
	[ExecuteAlways]
	public class PolygonVis : MonoBehaviour
	{
		public Color polygonCol;
		public float radius;
		public float thickness;
		public Vector2Int numTestGridPoints;
		public Color insideTestCol;
		public Color outsideTestCol;

		void Update()
		{
			Vector3[] points = TransformHelper.GetAllChildPositions(transform);
			Vector2[] points2D = VectorHelper.To2DArray(points);

			// Draw polygon with outline
			Draw.Polygon(points2D, polygonCol);
			Draw.PathInstancedTest(points, thickness, Color.white, closed: true);

			for (int i = 0; i < points.Length; i++)
			{
				Draw.Point(points[i], radius, Color.white);
			}

			// Draw grid of points to test polygon contains point function
			numTestGridPoints = new Vector2Int(Mathf.Max(2, numTestGridPoints.x), Mathf.Max(2, numTestGridPoints.y));
			Camera cam = Camera.main;
			float screenH = cam.orthographicSize * 2;
			float screenW = screenH * cam.aspect;
			Vector2 camPos = (Vector2)cam.transform.position;

			for (int y = 0; y < numTestGridPoints.y; y++)
			{
				for (int x = 0; x < numTestGridPoints.x; x++)
				{
					Vector2 t = new Vector2(x / (numTestGridPoints.x - 1f), y / (numTestGridPoints.y - 1f));
					Vector2 pos = camPos + new Vector2((t.x - 0.5f) * screenW, (t.y - 0.5f) * screenH);
					bool insidePolygon = Maths.PolygonContainsPoint(pos, points2D);
					Draw.Point(pos, radius * 0.5f, insidePolygon ? insideTestCol : outsideTestCol);
				}
			}
		}
	}

}