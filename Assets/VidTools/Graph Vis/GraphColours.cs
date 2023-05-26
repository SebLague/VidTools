using UnityEngine;

namespace VidTools.GraphVis
{
	[CreateAssetMenu()]
	public class GraphColours : ScriptableObject
	{
		public Color axesCol;
		public Color majorGridlineCol;
		public Color minorGridlineCol;
		public Color backgroundCol;
	}
}