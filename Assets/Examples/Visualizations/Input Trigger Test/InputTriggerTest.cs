using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VidTools;
using VidTools.Vis;
using UnityEngine.InputSystem;

namespace VidTools.Examples
{
	public class InputTriggerTest : ImmediateAnimation
	{

		protected override void UpdateAnimation()
		{
			// First action (activated on first spacebar press)
			float firstActionTime1 = InputActivatedTime(1, Key.Space, Ease.Cubic.InOut, waitable: true);
			Draw.Point(Vector3.left * 4, 1 * firstActionTime1, Color.red);

			WaitUntilFinished();
			float firstActionTime2 = CalculateClipTime(1, Ease.Cubic.InOut);
			Draw.Point(Vector3.zero, 1 * firstActionTime2, Color.yellow);

			// Second action (activated on second spacebar press)
			float secondActionTime1 = InputActivatedTime(1, Key.Space, Ease.Cubic.InOut, waitable: true);
			Draw.Point(Vector3.left * 4 + Vector3.up * -3, 1 * secondActionTime1, Color.green);

			WaitUntilFinished();
			float secondActionTime2 = CalculateClipTime(1, Ease.Cubic.InOut);
			Draw.Point(Vector3.zero + Vector3.up * -3, 1 * secondActionTime2, Color.cyan);
		}
	}
}