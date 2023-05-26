using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VidTools.Helpers
{
	public static class VectorHelper
	{
		/// <summary> Converts array of 2D vectors to array of 3D vectors with given z value (0 by default). </summary>
		public static Vector3[] To3DArray(Vector2[] array2D, float z = 0)
		{
			Vector3[] array3D = new Vector3[array2D.Length];

			for (int i = 0; i < array3D.Length; i++)
			{
				array3D[i] = new Vector3(array2D[i].x, array2D[i].y, z);
			}

			return array3D;
		}

		/// <summary> Converts array of 3D vectors to array of 2D vectors </summary>
		public static Vector2[] To2DArray(Vector3[] array3D)
		{
			Vector2[] array2D = new Vector2[array3D.Length];

			for (int i = 0; i < array3D.Length; i++)
			{
				array2D[i] = array3D[i];
			}

			return array2D;
		}



		public static Vector3 WithX(Vector3 vec, float x)
		{
			return new Vector3(x, vec.y, vec.z);
		}

		public static Vector3 WithY(Vector3 vec, float y)
		{
			return new Vector3(vec.x, y, vec.z);
		}

		public static Vector3 WithZ(Vector3 vec, float z)
		{
			return new Vector3(vec.x, vec.y, z);
		}


	}
}