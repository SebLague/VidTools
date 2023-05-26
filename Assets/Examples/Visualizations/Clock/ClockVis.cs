using UnityEngine;
using VidTools.Vis;

namespace VidTools.Examples
{
	public class ClockVis : ImmediateAnimation
	{

		[Header("Display Settings")]
		public float radius;
		public float centreRad;

		public Vector2 outlineSize;
		[Range(0, 1)] public float innerT;
		[Range(0, 1)] public float outerT;
		public float lineThickness;
		public float handThickness;
		[Range(0, 1)] public float lineThicknessT;
		[Range(0, 1)] public float secLenT;
		[Range(0, 1)] public float handT;
		[Range(0, 1)] public float hourHandT;
		[Range(0, 1)] public float minuteHandThickT;
		[Range(0, 1)] public float extendT;
		public Color pinCol = Color.white;

		protected override void UpdateAnimation()
		{
			Draw.Point(Vector2.zero, radius, Color.white);
			Draw.Point(Vector2.zero, centreRad, Color.black);

			// Hands
			float minuteT = CalculateClipTime(5);
			float minuteAngle = minuteT * Mathf.PI * 2;

			Vector2 minuteDir = new Vector2(Mathf.Sin(minuteAngle), Mathf.Cos(minuteAngle));
			Draw.Line(-minuteDir * radius * extendT, minuteDir * radius * handT, handThickness * minuteHandThickT, Color.black);

			float hourAngle = minuteT * Mathf.PI * 2 / 12;
			Vector2 hourDir = new Vector2(Mathf.Sin(hourAngle), Mathf.Cos(hourAngle));
			Draw.Line(-hourDir * radius * extendT, hourDir * radius * hourHandT, handThickness, Color.black);
			Draw.Point(Vector2.zero, centreRad * 0.7f, pinCol);

			// Ticks
			for (int i = 0; i < 60; i++)
			{
				float t = i / 60f;
				float angle = t * Mathf.PI * 2;
				Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

				Vector2 start = dir * radius * Mathf.Lerp(innerT, outerT, secLenT);
				float thick = lineThickness * lineThicknessT;
				if (i % 5 == 0)
				{
					start = dir * radius * innerT;
					thick = lineThickness;
				}
				Draw.Line(start, dir * radius * outerT, thick, Color.black, false);
			}
		}

	}
}