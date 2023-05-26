using UnityEditor;
using UnityEngine;
using VidTools.GraphVis;

namespace VidTools.Editors
{
	[CustomEditor(typeof(GraphGrid))]
	public class GridEditor : Editor
	{

		GraphGrid grid;
		Editor themeEditor;
		bool themeFoldout;

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			DrawSettingsEditor(grid.theme, ref themeFoldout, ref themeEditor);
		}

		void DrawSettingsEditor(Object settings, ref bool foldout, ref Editor editor)
		{
			if (settings != null)
			{
				foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
				if (foldout)
				{
					CreateCachedEditor(settings, null, ref editor);
					editor.OnInspectorGUI();
				}
			}
		}

		private void OnEnable()
		{
			themeFoldout = EditorPrefs.GetBool(nameof(themeFoldout), false);
			grid = target as GraphGrid;
		}

		void OnDisable()
		{
			EditorPrefs.SetBool(nameof(themeFoldout), themeFoldout);
		}
	}
}