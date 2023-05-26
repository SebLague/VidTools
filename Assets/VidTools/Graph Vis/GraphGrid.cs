using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VidTools.GraphVis
{

	[ExecuteAlways]
	public class GraphGrid : MonoBehaviour
	{
		public const float thicknessMultiplier = 0.001f;

		public float axesThickness = 3;
		public float majorThickness = 2;
		public float minorThickness = 1;
		public float majorStepSize = 1;
		public float minorStepSize = 1;
		public GraphColours theme;

		Material axesMat;
		Material minorMat;
		Material majorMat;
		Mesh axesMesh;
		Mesh minorGridlineMesh;
		Mesh majorGridlineMesh;
		Camera cam;

		void Init()
		{
			cam = Camera.main;
		}

		void Update()
		{
			Init();
			SetTheme();

			if (minorThickness > 0)
			{
				CreateMesh(ref minorGridlineMesh, minorThickness);
				DrawGridLines(minorGridlineMesh, minorStepSize, minorMat, true, majorStepSize, 0.002f);
			}

			if (majorThickness > 0)
			{
				CreateMesh(ref majorGridlineMesh, majorThickness);
				DrawGridLines(majorGridlineMesh, majorStepSize, majorMat, false, 0, 0.001f);
			}

			if (axesThickness > 0)
			{
				CreateMesh(ref axesMesh, axesThickness);
				Vector3 xAxisPos = new Vector3(cam.transform.position.x, 0, 0);
				Vector3 yAxisPos = new Vector3(0, cam.transform.position.y, 0);
				Graphics.DrawMesh(axesMesh, xAxisPos, Quaternion.identity, axesMat, gameObject.layer);
				Graphics.DrawMesh(axesMesh, yAxisPos, Quaternion.Euler(0, 0, 90), axesMat, gameObject.layer);
			}
		}

		void DrawGridLines(Mesh mesh, float stepSize, Material material, bool useSkipStep, float skipStep, float z)
		{
			float screenHeight = cam.orthographicSize * 2;
			float screenWidth = screenHeight * cam.aspect;
			int numStepsX = Mathf.CeilToInt(screenWidth / stepSize) + 1;
			int numStepsY = Mathf.CeilToInt(screenHeight / stepSize) + 1;
			float depth = z;

			// vertical gridlines
			float startX = FloorToMultiple(cam.transform.position.x - screenWidth / 2, stepSize);
			for (int i = 0; i < numStepsX; i++)
			{
				float x = startX + i * stepSize;
				bool skip = Approximately(x, 0) || (useSkipStep && Approximately(Mathf.RoundToInt(x / skipStep), x / skipStep));
				if (!skip)
				{
					Vector3 pos = new Vector3(x, cam.transform.position.y, depth);
					Graphics.DrawMesh(mesh, pos, Quaternion.Euler(0, 0, 90), material, gameObject.layer);
				}
			}

			// horizontal gridlines
			float startY = FloorToMultiple(cam.transform.position.y - screenHeight / 2, stepSize);
			for (int i = 0; i < numStepsY; i++)
			{
				float y = startY + i * stepSize;
				bool skip = Approximately(y, 0) || (useSkipStep && Approximately(Mathf.RoundToInt(y / skipStep), y / skipStep));
				if (!skip)
				{
					Vector3 pos = new Vector3(cam.transform.position.x, y, depth);
					Graphics.DrawMesh(mesh, pos, Quaternion.identity, material, gameObject.layer);
				}
			}
		}

		float FloorToMultiple(float value, float multiple)
		{
			return Mathf.Floor(value / multiple) * multiple;
		}

		void CreateMesh(ref Mesh mesh, float thickness)
		{
			var cam = Camera.main;
			float screenHeight = cam.orthographicSize * 2;
			float screenWidth = screenHeight * cam.aspect;
			float lineLength = Mathf.Max(screenWidth, screenHeight);
			float finalThickness = screenHeight * thickness * thicknessMultiplier;

			Vector3[] vertices = {
			Vector3.left * lineLength / 2 + Vector3.down * finalThickness / 2,
			Vector3.left * lineLength / 2 + Vector3.up * finalThickness / 2,
			Vector3.right * lineLength / 2 + Vector3.down * finalThickness / 2,
			Vector3.right * lineLength / 2 + Vector3.up * finalThickness / 2
		};

			int[] triangles = { 0, 1, 2, 1, 3, 2 };

			if (mesh == null)
			{
				mesh = new Mesh();
			}
			else
			{
				mesh.Clear();
			}

			mesh.vertices = vertices;
			mesh.SetTriangles(triangles, 0, true);
		}

		void SetTheme()
		{
			if (theme != null)
			{
				SetMaterial(ref axesMat, theme.axesCol);
				SetMaterial(ref minorMat, theme.minorGridlineCol);
				SetMaterial(ref majorMat, theme.majorGridlineCol);
				Camera.main.backgroundColor = theme.backgroundCol;
			}
		}

		void SetMaterial(ref Material material, Color color)
		{
			if (material == null)
			{
				material = new Material(Shader.Find("Unlit/Color"));
			}
			material.color = color;
		}

		void OnValidate()
		{
			axesThickness = Mathf.Max(0, axesThickness);
			minorThickness = Mathf.Max(0, minorThickness);
			majorThickness = Mathf.Max(0, majorThickness);
		}

		bool Approximately(float a, float b)
		{
			const float epsilon = 0.0001f;
			return Mathf.Abs(a - b) < epsilon;
		}
	}
}