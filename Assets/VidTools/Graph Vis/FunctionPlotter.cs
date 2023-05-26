using System.Collections.Generic;
using UnityEngine;

namespace VidTools.GraphVis
{
	public class FunctionPlotter : MonoBehaviour
	{
		public int resolution = 1000;
		public float thickness = 10f;

		[SerializeField, HideInInspector]
		List<Line> lines;
		int lastSetupFrame;

		public Plot CreatePlot(System.Func<double, double> function)
		{
			return CreatePlot(function, Color.white);
		}

		public Plot CreatePlot(System.Func<double, double> function, Color color, float thicknessMultiplier = 1, bool useCustomRange = false, Vector2 customRange = default)
		{
			SetUp();
			Material lineMaterial = new Material(Shader.Find("Unlit/Line"));
			lineMaterial.color = color;

			Line line = GetOrCreateLine();

			UpdateLineSettings(line.renderer, color, thicknessMultiplier, lineMaterial);

			// Calculate screen x-axis bounds
			Camera cam = Camera.main;
			float screenHeightMetres = cam.orthographicSize * 2;
			float screenWidthMetres = screenHeightMetres * cam.aspect;

			float startX = cam.transform.position.x - screenWidthMetres / 2;
			float endX = cam.transform.position.x + screenWidthMetres / 2;
			if (useCustomRange)
			{
				startX = customRange.x;
				endX = customRange.y;
			}

			// Sample points
			Vector3[] points = new Vector3[resolution];
			for (int i = 0; i < resolution; i++)
			{
				float t = i / (resolution - 1f);
				float x = Mathf.Lerp(startX, endX, t);
				double y = function(x);
				points[i] = new Vector3(x, (float)y, 0);
			}
			line.renderer.positionCount = resolution;
			line.renderer.SetPositions(points);
			line.renderer.textureMode = LineTextureMode.DistributePerSegment;

			Plot plot = new Plot() { material = lineMaterial };
			return plot;
		}

		void SetUp()
		{
			if (Time.frameCount != lastSetupFrame)
			{
				lastSetupFrame = Time.frameCount;

				if (lines == null)
				{
					lines = new List<Line>();
				}
				if (lines.Count == 0 && transform.GetComponentInChildren<LineRenderer>())
				{
					var lineRenderers = transform.GetComponentsInChildren<LineRenderer>();
					foreach (var l in lineRenderers)
					{
						lines.Add(new Line() { renderer = l });
					}
				}

				// Cleanup null entries and hide all renderers
				for (int i = lines.Count - 1; i >= 0; i--)
				{
					if (lines[i] == null || lines[i].renderer == null)
					{
						lines.RemoveAt(i);
					}
					else
					{
						lines[i].Hide();
					}
				}
			}
		}

		void UpdateLineSettings(LineRenderer line, Color color, float thicknessMultiplier, Material material)
		{
			Camera cam = Camera.main;
			float screenHeightMetres = cam.orthographicSize / 2;
			float adjustedLineWidth = screenHeightMetres * thickness * thicknessMultiplier * GraphGrid.thicknessMultiplier;
			line.startWidth = adjustedLineWidth;
			line.endWidth = adjustedLineWidth;
			line.startColor = color;
			line.endColor = color;
			line.sharedMaterial = material;
		}

		Line GetOrCreateLine()
		{
			// Find unused line
			int frame = Time.frameCount;
			for (int i = 0; i < lines.Count; i++)
			{
				if (lines[i].lastDrawFrame != frame)
				{
					lines[i].lastDrawFrame = frame;
					lines[i].Show();
					return lines[i];
				}
			}

			// Create new line if none available
			GameObject lineHolder = new GameObject("Line");
			lineHolder.layer = gameObject.layer;
			lineHolder.transform.parent = transform;
			var renderer = lineHolder.AddComponent<LineRenderer>();
			var line = new Line() { renderer = renderer, lastDrawFrame = frame };
			lines.Add(line);
			return line;
		}

		[System.Serializable]
		public class Line
		{
			public LineRenderer renderer;
			public int lastDrawFrame;

			public void Show()
			{
				renderer.gameObject.SetActive(true);
			}
			public void Hide()
			{

				renderer.gameObject.SetActive(false);
			}
		}

		void OnValidate()
		{
			resolution = Mathf.Max(resolution, 2);
		}

		public class Plot
		{
			public Material material;

			public void UpdateColour(Color col)
			{
				material.color = col;
			}
		}
	}
}