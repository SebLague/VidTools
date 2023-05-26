using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VidTools.Vis.Internal
{
	public static class VisMaths
	{
		const float Epsilon = 1.175494351E-38f;

		public static float RayLineIntersectionDistance(Vector2 rayOrigin, Vector2 rayDir, Vector2 lineA, Vector2 lineB)
		{
			var res = LineIntersectsLine(lineA, lineB, rayOrigin, rayOrigin + rayDir);
			if (res.intersects)
			{
				return Vector2.Distance(rayOrigin, res.point);
			}
			return Mathf.Infinity;
		}

		/// <summary> Test if two infinite 2D lines intersect (don't if parallel), and get point of intersection </summary>
		static (bool intersects, Vector2 point) LineIntersectsLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
		{
			float d = (a1.x - a2.x) * (b1.y - b2.y) - (a1.y - a2.y) * (b1.x - b2.x);
			// Check if parallel
			if (ApproximatelyEqual(d, 0))
			{
				return (false, Vector2.zero);
			}

			float n = (a1.x - b1.x) * (b1.y - b2.y) - (a1.y - b1.y) * (b1.x - b2.x);
			float t = n / d;
			Vector2 intersectionPoint = a1 + (a2 - a1) * t;
			return (true, intersectionPoint);
		}

		static bool ApproximatelyEqual(float a, float b) => System.Math.Abs(a - b) < Epsilon;
	}
}