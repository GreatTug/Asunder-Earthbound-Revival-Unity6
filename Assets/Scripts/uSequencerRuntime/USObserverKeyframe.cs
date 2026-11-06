using System;
using UnityEngine;

[Serializable]
public class USObserverKeyframe : ScriptableObject
{
	public USTimelineObserver observer;

	public bool fired;

	public bool prevActiveState;

	[SerializeField]
	private Camera camera;

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
			if ((bool)observer)
			{
				fireTime = Mathf.Min(Mathf.Max(value, 0f), observer.Sequence.Duration);
			}
			else
			{
				fireTime = value;
			}
		}
	}

	public Camera KeyframeCamera
	{
		get
		{
			return camera;
		}
		set
		{
			camera = value;
		}
	}
}
