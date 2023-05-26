using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VidTools.Vis;
namespace VidTools.Examples
{
	[ExecuteAlways]
	public class EaseGraphPlotter : MonoBehaviour
	{

		public Ease.EaseType easeFunction;
		public int resolution = 100;
		public float thickness;
		public Color col;

		void Update()
		{

			float startX = 0;
			float endX = 1;

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
			return Ease.GetEasing(x, easeFunction);
		}
	}

}