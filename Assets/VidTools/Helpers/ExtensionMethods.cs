using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VidTools.Extensions
{
	public static class ExtensionMethods
	{

		public static Vector3 WithZ(this Vector3 vec, float z)
		{
			return new Vector3(vec.x, vec.y, z);
		}

		public static Vector3 WithZ(this Vector2 vec, float z)
		{
			return new Vector3(vec.x, vec.y, z);
		}

		public static Color WithAlpha(this Color col, float alpha)
		{
			return new Color(col.r, col.g, col.b, alpha);
		}
	}
}