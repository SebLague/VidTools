using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VidTools.Vis.Internal
{

	public static class DrawManager
	{

		public static MaterialPropertyBlock materialProperties;
		public static CommandBuffer cmd;
		static int lastFrameWithDrawCommands;
		public static Pool<Mesh> meshPool;
		public static PathDrawPool pathDrawPool;
		const string buffName = "Vis Draw Commands";

		// Called every frame by every draw function
		public static void EnsureFrameInitialized()
		{
			// Only need to init if this is the first draw request of current frame
			if (lastFrameWithDrawCommands != Time.frameCount)
			{
				lastFrameWithDrawCommands = Time.frameCount;
				if (cmd == null)
				{
					cmd = new CommandBuffer();
					cmd.name = buffName;
				}
				cmd.Clear();

				DrawMaterials.Init();
				if (materialProperties == null)
				{
					materialProperties = new MaterialPropertyBlock();
				}

				meshPool ??= new Pool<Mesh>();
				pathDrawPool ??= new PathDrawPool();
				meshPool.FinishedUsingAllitems();
				pathDrawPool.ReturnAll();
			}

		}

		static void OnPreRender(Camera cam)
		{
			CameraEvent drawCameraEvent = CameraEvent.BeforeImageEffects;

			var allBuffers = cam.GetCommandBuffers(drawCameraEvent);

			// Remove buffer by name.
			// This is done because there are situations in which the buffer can be
			// null (but still attached to camera), and I don't want to think about it.
			foreach (var b in allBuffers)
			{
				if (string.Equals(b.name, buffName, System.StringComparison.Ordinal))
				{
					cam.RemoveCommandBuffer(drawCameraEvent, b);
				}
			}

			if (lastFrameWithDrawCommands == Time.frameCount && cmd != null)
			{
				cam.AddCommandBuffer(drawCameraEvent, cmd);
			}

		}

		static void CleanupBeforeAssemblyReload()
		{
			pathDrawPool?.ReturnAndReleaseAll();
		}

		// Called on enter playmode (before awake), and on script recompile in editor
		static void Init()
		{
			lastFrameWithDrawCommands = -1;
			Camera.onPreRender -= OnPreRender;
			Camera.onPreRender += OnPreRender;
#if UNITY_EDITOR
			UnityEditor.AssemblyReloadEvents.beforeAssemblyReload -= CleanupBeforeAssemblyReload;
			UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += CleanupBeforeAssemblyReload;
#endif

			meshPool?.FinishedUsingAllitems();
			pathDrawPool?.ReturnAndReleaseAll();
		}

#if UNITY_EDITOR
		[UnityEditor.Callbacks.DidReloadScripts]
#endif
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void InitializeOnLoad()
		{
			Init();
		}

	}

}