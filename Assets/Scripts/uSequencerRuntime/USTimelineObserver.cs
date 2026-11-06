using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class USTimelineObserver : USTimelineBase
{
	[Serializable]
	[Obsolete("USTimelineObserverKeyframe is Obsolete, please us USObserverKeyframe instead")]
	public class USTimelineObserverKeyframe
	{
		public USTimelineObserver observer;

		public bool fired;

		public bool prevActiveState;

		public Camera camera;

		[SerializeField]
		private float fireTime;

		public float FireTime
		{
			get
			{
				return fireTime;
			}
			set
			{
				fireTime = Mathf.Min(Mathf.Max(value, 0f), observer.Sequence.Duration);
			}
		}
	}

	[SerializeField]
	public List<USObserverKeyframe> observerKeyframes = new List<USObserverKeyframe>();

	public List<USObserverKeyframe> baseStates = new List<USObserverKeyframe>();

	private USObserverKeyframe currentlyActiveKeyframe;

	public LayerMask layersToIgnore = 0;

	[SerializeField]
	public List<float> tempKeyframeData = new List<float>();

	private static Type objectUtilsType;

	private static MethodInfo setObjectActiveMethodInfo;

	private static MethodInfo isObjectActiveMethodInfo;

	[Obsolete("keyframes Obseolete, please us observerKeyframes")]
	[SerializeField]
	public List<USTimelineObserverKeyframe> keyframes = new List<USTimelineObserverKeyframe>();

	public USObserverKeyframe CurrentlyActiveKeyframe
	{
		get
		{
			return currentlyActiveKeyframe;
		}
	}

	private Type ObjectUtilsType
	{
		get
		{
			if (objectUtilsType == null)
			{
				objectUtilsType = USSequencerUtility.GetType("USGameObjectUtils");
			}
			return objectUtilsType;
		}
	}

	private MethodInfo SetObjectActive
	{
		get
		{
			if (setObjectActiveMethodInfo == null)
			{
				setObjectActiveMethodInfo = ObjectUtilsType.GetMethod("SetObjectActive", BindingFlags.Static | BindingFlags.Public);
			}
			return setObjectActiveMethodInfo;
		}
	}

	private MethodInfo IsObjectActive
	{
		get
		{
			if (isObjectActiveMethodInfo == null)
			{
				isObjectActiveMethodInfo = ObjectUtilsType.GetMethod("IsObjectActive", BindingFlags.Static | BindingFlags.Public);
			}
			return isObjectActiveMethodInfo;
		}
	}

	private int KeyframeComparer(USObserverKeyframe a, USObserverKeyframe b)
	{
		if (a == null && b == null)
		{
			return 0;
		}
		if (a == null)
		{
			return -1;
		}
		if (b == null)
		{
			return 1;
		}
		return a.FireTime.CompareTo(b.FireTime);
	}

	private void OnEnable()
	{
		baseStates.Clear();
	}

	public override void StopTimeline()
	{
		currentlyActiveKeyframe = null;
		for (int num = observerKeyframes.Count - 1; num >= 0; num--)
		{
			RevertKeyframe(observerKeyframes[num]);
		}
		RevertToBaseState();
		baseStates.Clear();
	}

	public override void PauseTimeline()
	{
	}

	public override void ResumeTimeline()
	{
	}

	public override void StartTimeline()
	{
		baseStates.Clear();
		AddDefaultCamerasToBaseState();
	}

	public override void ManuallySetTime(float sequencerTime)
	{
		if (!base.Sequence.IsPlaying && !(currentlyActiveKeyframe != null))
		{
			baseStates.Clear();
			AddDefaultCamerasToBaseState();
		}
	}

	public override void SkipTimelineTo(float time)
	{
		Process(time, 1f);
	}

	public override void Process(float sequencerTime, float playbackRate)
	{
		foreach (USObserverKeyframe observerKeyframe in observerKeyframes)
		{
			if ((bool)observerKeyframe && observerKeyframe.FireTime < sequencerTime && !observerKeyframe.fired)
			{
				FireKeyframe(observerKeyframe);
			}
		}
		if (!(currentlyActiveKeyframe != null) || !(currentlyActiveKeyframe.FireTime > sequencerTime) || !currentlyActiveKeyframe.fired)
		{
			return;
		}
		foreach (USObserverKeyframe observerKeyframe2 in observerKeyframes)
		{
			if ((bool)observerKeyframe2 && observerKeyframe2.FireTime <= currentlyActiveKeyframe.FireTime)
			{
				observerKeyframe2.fired = false;
			}
		}
		RevertKeyframe(currentlyActiveKeyframe);
		currentlyActiveKeyframe = null;
		RevertToBaseState();
		Process(sequencerTime, playbackRate);
	}

	public void RevertToBaseState()
	{
		foreach (USObserverKeyframe baseState in baseStates)
		{
			if ((bool)baseState && (bool)baseState.KeyframeCamera)
			{
				baseState.fired = false;
				SetObjectActive.Invoke(null, new object[2]
				{
					baseState.KeyframeCamera.gameObject,
					baseState.prevActiveState
				});
			}
		}
		baseStates.Clear();
		AddDefaultCamerasToBaseState();
	}

	private void AddDefaultCamerasToBaseState()
	{
		Camera[] array = Resources.FindObjectsOfTypeAll(typeof(Camera)) as Camera[];
		Camera[] array2 = array;
		foreach (Camera camera in array2)
		{
			USObserverKeyframe uSObserverKeyframe = ScriptableObject.CreateInstance<USObserverKeyframe>();
			uSObserverKeyframe.observer = this;
			uSObserverKeyframe.KeyframeCamera = camera;
			uSObserverKeyframe.prevActiveState = (bool)IsObjectActive.Invoke(null, new object[1] { camera.gameObject });
			AddToBaseStates(uSObserverKeyframe);
		}
	}

	public void FireKeyframe(USObserverKeyframe keyframe)
	{
		if (keyframe == null || keyframe.fired)
		{
			return;
		}
		if ((bool)currentlyActiveKeyframe)
		{
			SetObjectActive.Invoke(null, new object[2]
			{
				currentlyActiveKeyframe.KeyframeCamera.gameObject,
				currentlyActiveKeyframe.prevActiveState
			});
		}
		currentlyActiveKeyframe = keyframe;
		currentlyActiveKeyframe.fired = true;
		Camera keyframeCamera = currentlyActiveKeyframe.KeyframeCamera;
		if (!keyframeCamera)
		{
			return;
		}
		currentlyActiveKeyframe.prevActiveState = (bool)IsObjectActive.Invoke(null, new object[1] { keyframeCamera.gameObject });
		SetObjectActive.Invoke(null, new object[2] { keyframeCamera.gameObject, true });
		Camera[] allCameras = Camera.allCameras;
		foreach (Camera camera in allCameras)
		{
			if ((bool)camera && !(camera == keyframeCamera) && ((1 << camera.gameObject.layer) & (int)layersToIgnore) <= 0)
			{
				SetObjectActive.Invoke(null, new object[2] { camera.gameObject, false });
			}
		}
	}

	public void RevertKeyframe(USObserverKeyframe keyframe)
	{
		if (!(keyframe == null) && keyframe.fired)
		{
			keyframe.fired = false;
			if ((bool)keyframe.KeyframeCamera)
			{
				SetObjectActive.Invoke(null, new object[2]
				{
					keyframe.KeyframeCamera.gameObject,
					keyframe.prevActiveState
				});
			}
		}
	}

	private void AddToBaseStates(USObserverKeyframe keyframe)
	{
		if (!keyframe)
		{
			return;
		}
		bool flag = false;
		foreach (USObserverKeyframe baseState in baseStates)
		{
			if (baseState.KeyframeCamera == keyframe.KeyframeCamera)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			baseStates.Add(keyframe);
		}
	}

	public USObserverKeyframe AddKeyframe(float fireTime, Camera camera)
	{
		USObserverKeyframe uSObserverKeyframe = ScriptableObject.CreateInstance<USObserverKeyframe>();
		uSObserverKeyframe.observer = this;
		uSObserverKeyframe.FireTime = fireTime;
		uSObserverKeyframe.KeyframeCamera = camera;
		observerKeyframes.Add(uSObserverKeyframe);
		sortKeyframes();
		return uSObserverKeyframe;
	}

	public void sortKeyframes()
	{
		observerKeyframes.Sort(KeyframeComparer);
	}

	public void SetKeyframeCamera(USObserverKeyframe keyframe, Camera camera)
	{
		keyframe.KeyframeCamera = camera;
	}

	public void RemoveKeyframe(USObserverKeyframe keyframe)
	{
		if (!(keyframe == null) && observerKeyframes.Contains(keyframe))
		{
			if (currentlyActiveKeyframe == keyframe)
			{
				RevertKeyframe(currentlyActiveKeyframe);
				currentlyActiveKeyframe = null;
			}
			observerKeyframes.Remove(keyframe);
		}
	}
}
