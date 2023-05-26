using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace VidTools.Vis
{
	[ExecuteAlways]
	public abstract class ImmediateAnimation : MonoBehaviour
	{

		[Header("Animator")]
		[Range(0, 1)] public float animationProgress;
		public float playbackSpeed = 1;

		float lastClipDuration;
		float currentTime;
		float duration;

		public delegate float EaseFunction(float t);

		protected float globalStartTime { get; private set; }
		protected float globalEndTime { get; private set; }

		protected bool isPaused { get; private set; }

		float clipStartTime;
		// List of time values at which a particular key has been pressed since the animation began.
		Dictionary<Key, List<float>> pressedKeyLookup;
		// Number of times an input-activated time has been requested this frame (by key).
		// For example, say an animation uses the spacebar to consecutively trigger 3 different sections of the animation.
		// That means that each frame there will be 3 requests to the 'InputActivatedTime(key: space)' function.
		// With each request, the corresponding 'spacebar pressed time' needs to be looked up (time of press 1, 2, or 3).
		// So, this dictionary keeps track of the number of requests for a particular key so far in the frame.
		Dictionary<Key, int> keyTriggerCountThisFrame;
		HashSet<Key> keysToListenFor;
		Camera cam;

		protected virtual void OnEnable()
		{
			if (Application.isPlaying)
			{
				AnimationStart();
			}
			pressedKeyLookup = new();
			keysToListenFor = new HashSet<Key>();
			keyTriggerCountThisFrame = new();
		}

		protected virtual void AnimationStart()
		{
		}

		protected virtual void Update()
		{
			Init();
			UpdateInput();
			UpdateAnimation();
			UpdateTime();
		}


		protected abstract void UpdateAnimation();

		void Init()
		{
			lastClipDuration = 0;
			globalStartTime = 0;
			globalEndTime = 0;
			clipStartTime = 0;
			keyTriggerCountThisFrame.Clear();
		}

		void UpdateInput()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			Keyboard keyboard = Keyboard.current;
			foreach (Key key in keysToListenFor)
			{
				if (keyboard[key].wasPressedThisFrame)
				{
					if (pressedKeyLookup.ContainsKey(key))
					{
						pressedKeyLookup[key].Add(currentTime);
					}
					else
					{
						List<float> pressedTimes = new();
						pressedTimes.Add(currentTime);
						pressedKeyLookup.Add(key, pressedTimes);
					}
				}
			}
		}

		public void SetPlaybackTime(float time)
		{
			currentTime = time;
		}

		void UpdateTime()
		{
			if (Application.isPlaying)
			{
				if (!isPaused)
				{
					currentTime += Time.deltaTime * playbackSpeed;
				}
			}
			else
			{
				// Allow scrubbing while outside of playmode
				currentTime = Mathf.Lerp(globalStartTime, globalEndTime, animationProgress);
			}
			currentTime = Mathf.Max(currentTime, globalStartTime);
			animationProgress = Mathf.InverseLerp(globalStartTime, globalEndTime, currentTime);

			duration = globalEndTime - globalStartTime;
		}


		public float CalculateClipTime(float clipDuration, EaseFunction ease = null)
		{
			// Calculate 'time' value for current clip: 0 = start of clip, 1 = end of clip.
			float timePassedSinceClipStart = currentTime - clipStartTime;
			float linearT = Mathf.Clamp01(timePassedSinceClipStart / clipDuration);
			float animT = Ease(linearT, ease);

			globalStartTime = Mathf.Min(globalStartTime, clipStartTime);
			globalEndTime = Mathf.Max(globalEndTime, clipStartTime + clipDuration);

			lastClipDuration = clipDuration;
			return animT;
		}

		public void WaitUntilFinished()
		{
			clipStartTime += lastClipDuration;
		}

		public void WaitUntilFinishedPlusDelay(float waitTime)
		{
			clipStartTime += lastClipDuration + waitTime;
		}

		public void WaitUntilFractionFinished(float t)
		{
			clipStartTime += lastClipDuration * t;
		}

		public void Wait(float waitTime)
		{
			clipStartTime += waitTime;
		}

		float Ease(float t, EaseFunction ease)
		{
			return (ease == null) ? Mathf.Clamp01(t) : ease.Invoke(t);
		}

		public float InputActivatedTime(float duration, Key key, EaseFunction ease = null, bool waitable = true)
		{
			if (Application.isPlaying)
			{
				keysToListenFor.Add(key);
				// Record number of times this key has been checked so far this frame
				if (!keyTriggerCountThisFrame.TryAdd(key, 1))
				{
					keyTriggerCountThisFrame[key] += 1;
				}

				if (pressedKeyLookup.TryGetValue(key, out List<float> pressedTimes))
				{
					float pressedTime = 0;
					int n = keyTriggerCountThisFrame[key] - 1;
					if (n < pressedTimes.Count)
					{
						pressedTime = pressedTimes[n];

						if (waitable)
						{
							lastClipDuration = pressedTime - clipStartTime + duration;
						}
						float t = Ease((currentTime - pressedTime) / duration, ease);
						return t;
					}
				}

				if (waitable)
				{
					lastClipDuration = currentTime - clipStartTime;
				}
			}
			return 0;
		}

		public void PausePlayback() => isPaused = true;
		public void UnPausePlayback() => isPaused = false;
		public void TogglePlaybackPaused() => isPaused = !isPaused;

		// Divides a [0,1] time value 'parent time' into multiple faster [0,1] times values 'child times'
		public float SubdivideTime(float parentTime, int numChildren, int childIndex, EaseFunction ease = null)
		{
			float durationFraction = 1f / numChildren;
			return SubdivideTime(parentTime, numChildren, childIndex, durationFraction, ease);
		}

		public float SubdivideTime(float parentTime, int numChildren, int childIndex, float durationFraction, EaseFunction ease = null)
		{
			float finalChildStartTime = 1 - durationFraction;
			float indexT = childIndex / (numChildren - 1f);
			float startTime = finalChildStartTime * indexT;
			float t = Mathf.InverseLerp(startTime, startTime + durationFraction, parentTime);
			float easeT = (ease == null) ? Mathf.Clamp01(t) : ease.Invoke(t);
			return easeT;
		}

		public Vector2 GetMousePos()
		{
			if (cam == null)
			{
				cam = Camera.main;
			}
			return cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
		}
	}
}