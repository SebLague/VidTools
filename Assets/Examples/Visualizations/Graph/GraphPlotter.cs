using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VidTools.Vis;
namespace VidTools.Examples
{
	[ExecuteAlways]
	public class GraphPlotter : MonoBehaviour
	{
		public enum ExampleFunction { Linear, Quadratic, Cubic }
		public ExampleFunction exampleFunction;
		public int resolution = 100;
		public float thickness;
		public Color col;

		void Update()
		{
			Camera cam = Camera.main;
			float screenHeightMetres = cam.orthographicSize * 2;
			float screenWidthMetres = screenHeightMetres * cam.aspect;

			float startX = cam.transform.position.x - screenWidthMetres / 2;
			float endX = cam.transform.position.x + screenWidthMetres / 2;

			// Sample points
			Vector3[] points = new Vector3[resolution];
			for (int i = 0; i < resolution; i++)
			{
				float t = i / (resolution - 1f);
				float x = Mathf.Lerp(startX, endX, t);
				double y = GetY(x);
				points[i] = new Vector3(x, (float)y, 0);
			}

			Draw.PathInstancedTest(points, thickness, col);
		}

		float GetY(float x)
		{
			switch (exampleFunction)
			{
				case ExampleFunction.Linear:
					return x;
				case ExampleFunction.Quadratic:
					return x * x;
				case ExampleFunction.Cubic:
					return x * x * x;
			}

			return 0;
		}
	}

}