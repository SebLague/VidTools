using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VidTools.Vis.Internal;

namespace VidTools.Vis
{
	[ExecuteAlways]
	public class ArrowDrawer : DrawerBase
	{
		[Header("Draw Settings")]
		public Vector3 offset = Vector3.up;
		public ArrowSettings arrowSettings;

		[Header("Animate Settings")]
		[Range(0, 1)] public float t = 1;
		public Ease.EaseType easeFunction;
		[SerializeField, HideInInspector]
		bool hasInit;

		void Update()
		{
			Vector3 startPoint = transform.position;
			Vector3 endPoint = startPoint + offset;
			float easedT = Ease.GetEasing(t, easeFunction);
			Draw.Arrow(startPoint, endPoint, arrowSettings, easedT);
		}

		public override void SetTime(float t) => this.t = t;

		void OnValidate()
		{
			if (!hasInit)
			{
				hasInit = true;
				arrowSettings = new ArrowSettings(0.1f, 0.35f, 33, true, Color.white);
			}
		}

	}
}