using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VidTools.Vis;

namespace VidTools.Editors
{
	[CustomEditor(typeof(ImmediateAnimation), true)]
	public class ImmediateAnimationEditor : Editor
	{

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorApplication.QueuePlayerLoopUpdate();
		}
	}
}