using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VidTools;
using VidTools.Vis;
using UnityEngine.InputSystem;

namespace VidTools.Examples
{
	public class SpiralVis : ImmediateAnimation
	{

		[Header("Display Settings")]
		public int numPoints;
		public float spiralRadius;
		public float spiralTurnsMin;
		public float spiralTurnsMax;

		public float radius;
		public float startScale = 1;
		public Color pointColStart = Color.white;
		public Color pointColEnd = Color.white;
		public Color pointColMouseOver = Color.white;
		public Color pointColSelected = Color.white;

		public Vector2 outlineSize;
		public float outlineThickness;
		public Color outlineCol = Color.white;

		HashSet<int> selectedIndices = new();

		protected override void AnimationStart()
		{
			Debug.Log("Left click points to highlight. Press spacebar to trigger spin animation.");
		}

		protected override void UpdateAnimation()
		{
			float boxAnimT = CalculateClipTime(2, Ease.Cubic.InOut);
			Draw.BoxOutline(Vector2.zero, outlineSize, outlineThickness, outlineCol, boxAnimT);
			WaitUntilFinished();

			float spinAnimT = InputActivatedTime(5, Key.Space, Ease.Cubic.InOut);
			float currTurnCount = Mathf.Lerp(spiralTurnsMin, spiralTurnsMax, spinAnimT);

			for (int i = 0; i < numPoints; i++)
			{
				float animT = CalculateClipTime(1, Ease.Cubic.InOut);
				Wait(0.02f);

				float spiralT = i / (numPoints - 1f);
				float angle = spiralT * Mathf.PI * 2 * currTurnCount;
				Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
				Vector2 pos = dir * spiralRadius * spiralT;
				float scale = Mathf.Lerp(startScale, 1, spiralT);
				Color pointCol = Color.Lerp(pointColStart, pointColEnd, animT);

				HandlePointInteraction(pos, i, ref scale, ref pointCol);

				Draw.Point(pos, radius * scale * animT, pointCol);
			}
		}

		void HandlePointInteraction(Vector2 pos, int index, ref float scale, ref Color col)
		{
			// Don't allow interaction outside of play-mode
			if (!Application.isPlaying) { return; }

			// Test if point is already selected
			if (selectedIndices.Contains(index))
			{
				col = pointColSelected;
			}
			// Otherwise test if mouse over point
			else if (Vector2.Distance(GetMousePos(), pos) < radius * scale)
			{
				scale *= 1.2f;
				col = pointColMouseOver;
				if (Mouse.current.leftButton.wasPressedThisFrame)
				{
					selectedIndices.Add(index);
				}
			}
		}
	}
}