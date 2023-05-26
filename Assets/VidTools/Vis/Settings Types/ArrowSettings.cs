using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VidTools.Vis
{
	[System.Serializable]
	public struct ArrowSettings
	{
		public float thickness;
		public float headLength;
		public float headAngleDegrees;
		public bool fillHead;
		public Color col;

		public ArrowSettings(float thickness, float headLength, float headAngleDegrees, bool fillHead, Color col)
		{
			this.thickness = thickness;
			this.headLength = headLength;
			this.headAngleDegrees = headAngleDegrees;
			this.fillHead = fillHead;
			this.col = col;
		}
	}
}