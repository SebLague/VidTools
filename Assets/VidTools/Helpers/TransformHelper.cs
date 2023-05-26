using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VidTools.Helpers
{

	public static class TransformHelper
	{

		/// <summary>Returns an array containing the positions of all children of the given parent transform. </summary>
		public static Vector3[] GetAllChildPositions(Transform parent)
		{
			Vector3[] positions = new Vector3[parent.childCount];
			for (int i = 0; i < positions.Length; i++)
			{
				positions[i] = parent.GetChild(i).position;
			}
			return positions;
		}

		/// <summary> Returns an array containing the 2D positions of all children of the given parent transform. </summary>
		public static Vector2[] GetAllChildPositions2D(Transform parent, float z = 0)
		{
			Vector2[] positions = new Vector2[parent.childCount];
			for (int i = 0; i < positions.Length; i++)
			{
				positions[i] = parent.GetChild(i).position;
			}
			return positions;
		}


		/// <summary> Returns an array containing the transforms of all children of the given parent transform. </summary>
		public static Transform[] GetAllChildTransforms(Transform parent)
		{
			Transform[] children = new Transform[parent.childCount];

			for (int i = 0; i < children.Length; i++)
			{
				children[i] = parent.GetChild(i);
			}
			return children;
		}

		public static Vector3[] ToPositions3D(Transform[] transforms)
		{
			Vector3[] positions = new Vector3[transforms.Length];

			for (int i = 0; i < positions.Length; i++)
			{
				positions[i] = transforms[i].position;
			}
			return positions;
		}

		public static Vector2[] ToPositions2D(Transform[] transforms)
		{
			Vector2[] positions = new Vector2[transforms.Length];

			for (int i = 0; i < positions.Length; i++)
			{
				positions[i] = transforms[i].position;
			}
			return positions;
		}

	}
}
