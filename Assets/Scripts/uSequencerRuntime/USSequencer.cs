using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class USSequencer : MonoBehaviour
{
	public delegate void PlaybackDelegate();

	[SerializeField]
	private List<Transform> observedObjects = new List<Transform>();

	[SerializeField]
	private float runningTime;

	[SerializeField]
	private float playbackRate = 1f;

	[SerializeField]
	private int version = 2;

	[SerializeField]
	private float duration = 10f;

	[SerializeField]
	private bool isLoopingSequence;

	[SerializeField]
	private bool isPingPongingSequence;

	private bool playing;

	private bool isFreshPlayback = true;

	private float previousTime;

	private float minPlaybackRate = -4f;

	private float maxPlaybackRate = 4f;

	private float setSkipTime = -1f;

	public PlaybackDelegate PlaybackStarted = () =>
	{
	};

	public PlaybackDelegate PlaybackStopped = () =>
	{
	};

	public PlaybackDelegate PlaybackPaused = () =>
	{
	};

	public PlaybackDelegate PlaybackFinished = () =>
	{
	};

	public int Version
	{
		get
		{
			return version;
		}
		set
		{
			version = value;
		}
	}

	public List<Transform> ObservedObjects
	{
		get
		{
			return observedObjects;
		}
	}

	public float Duration
	{
		get
		{
			return duration;
		}
		set
		{
			duration = value;
			if (duration <= 0f)
			{
				duration = 0.1f;
			}
		}
	}

	public bool IsPlaying
	{
		get
		{
			return playing;
		}
	}

	public bool IsLopping
	{
		get
		{
			return isLoopingSequence;
		}
		set
		{
			isLoopingSequence = value;
		}
	}

	public bool IsPingPonging
	{
		get
		{
			return isPingPongingSequence;
		}
		set
		{
			isPingPongingSequence = value;
		}
	}

	public float RunningTime
	{
		get
		{
			return runningTime;
		}
		set
		{
			runningTime = value;
			if (runningTime <= 0f)
			{
				runningTime = 0f;
			}
			if (runningTime > duration)
			{
				runningTime = duration;
			}
			if (isFreshPlayback)
			{
				USTimelineContainer[] timelineContainers = TimelineContainers;
				foreach (USTimelineContainer uSTimelineContainer in timelineContainers)
				{
					USTimelineBase[] timelines = uSTimelineContainer.Timelines;
					foreach (USTimelineBase uSTimelineBase in timelines)
					{
						uSTimelineBase.StartTimeline();
					}
				}
				isFreshPlayback = false;
			}
			USTimelineContainer[] timelineContainers2 = TimelineContainers;
			foreach (USTimelineContainer uSTimelineContainer2 in timelineContainers2)
			{
				uSTimelineContainer2.ManuallySetTime(RunningTime);
				uSTimelineContainer2.ProcessTimelines(RunningTime, PlaybackRate);
			}
		}
	}

	public float PlaybackRate
	{
		get
		{
			return playbackRate;
		}
		set
		{
			playbackRate = Mathf.Clamp(value, MinPlaybackRate, MaxPlaybackRate);
		}
	}

	public float MinPlaybackRate
	{
		get
		{
			return minPlaybackRate;
		}
	}

	public float MaxPlaybackRate
	{
		get
		{
			return maxPlaybackRate;
		}
	}

	public USTimelineContainer[] TimelineContainers
	{
		get
		{
			return GetComponentsInChildren<USTimelineContainer>();
		}
	}

	public USTimelineContainer[] SortedTimelineContainers
	{
		get
		{
			USTimelineContainer[] componentsInChildren = GetComponentsInChildren<USTimelineContainer>();
			Array.Sort(componentsInChildren, USTimelineContainer.Comparer);
			return componentsInChildren;
		}
	}

	public int TimelineContainerCount
	{
		get
		{
			return TimelineContainers.Length;
		}
	}

	public int ObservedObjectCount
	{
		get
		{
			return ObservedObjects.Count;
		}
	}

	private void Start()
	{
		USTimelineContainer[] timelineContainers = TimelineContainers;
		foreach (USTimelineContainer uSTimelineContainer in timelineContainers)
		{
			if ((bool)uSTimelineContainer && !uSTimelineContainer.AffectedObject && uSTimelineContainer.affectedObjectPath.Length != 0)
			{
				GameObject gameObject = GameObject.Find(uSTimelineContainer.affectedObjectPath);
				if ((bool)gameObject)
				{
					uSTimelineContainer.AffectedObject = gameObject.transform;
				}
			}
		}
	}

	public void TogglePlayback()
	{
		if (playing)
		{
			Pause();
		}
		else
		{
			Play();
		}
	}

	public void Play()
	{
		if (PlaybackStarted != null)
		{
			PlaybackStarted();
		}
		StartCoroutine("UpdateSequencerCoroutine");
		if (isFreshPlayback)
		{
			USTimelineContainer[] timelineContainers = TimelineContainers;
			foreach (USTimelineContainer uSTimelineContainer in timelineContainers)
			{
				USTimelineBase[] timelines = uSTimelineContainer.Timelines;
				foreach (USTimelineBase uSTimelineBase in timelines)
				{
					uSTimelineBase.StartTimeline();
				}
			}
			isFreshPlayback = false;
		}
		else
		{
			USTimelineContainer[] timelineContainers2 = TimelineContainers;
			foreach (USTimelineContainer uSTimelineContainer2 in timelineContainers2)
			{
				USTimelineBase[] timelines2 = uSTimelineContainer2.Timelines;
				foreach (USTimelineBase uSTimelineBase2 in timelines2)
				{
					uSTimelineBase2.ResumeTimeline();
				}
			}
		}
		playing = true;
		previousTime = Time.time;
	}

	public void Pause()
	{
		if (PlaybackPaused != null)
		{
			PlaybackPaused();
		}
		playing = false;
		USTimelineContainer[] timelineContainers = TimelineContainers;
		foreach (USTimelineContainer uSTimelineContainer in timelineContainers)
		{
			USTimelineBase[] timelines = uSTimelineContainer.Timelines;
			foreach (USTimelineBase uSTimelineBase in timelines)
			{
				uSTimelineBase.PauseTimeline();
			}
		}
	}

	public void Stop()
	{
		if (PlaybackStopped != null)
		{
			PlaybackStopped();
		}
		StopCoroutine("UpdateSequencerCoroutine");
		USTimelineContainer[] timelineContainers = TimelineContainers;
		foreach (USTimelineContainer uSTimelineContainer in timelineContainers)
		{
			USTimelineBase[] timelines = uSTimelineContainer.Timelines;
			foreach (USTimelineBase uSTimelineBase in timelines)
			{
				uSTimelineBase.StopTimeline();
			}
		}
		isFreshPlayback = true;
		playing = false;
		runningTime = 0f;
	}

	public USTimelineContainer CreateNewTimelineContainer(Transform affectedObject)
	{
		string path = "EmptyPrefab";
		GameObject gameObject = Resources.Load(path, typeof(GameObject)) as GameObject;
		if (!gameObject)
		{
			Debug.Log("Couldn't find prefab for USSequencer::CreateAndAddNewTimeline, it is possible it has been moved from the resources folder");
			return null;
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject) as GameObject;
		gameObject2.transform.parent = base.transform;
		gameObject2.name = "Timelines for " + affectedObject.name;
		USTimelineContainer uSTimelineContainer = gameObject2.AddComponent<USTimelineContainer>();
		uSTimelineContainer.AffectedObject = affectedObject;
		int num = 0;
		USTimelineContainer[] timelineContainers = TimelineContainers;
		foreach (USTimelineContainer uSTimelineContainer2 in timelineContainers)
		{
			if (uSTimelineContainer2.Index > num)
			{
				num = uSTimelineContainer2.Index;
			}
		}
		uSTimelineContainer.Index = num + 1;
		return uSTimelineContainer;
	}

	public void DeleteTimelineContainer(USTimelineContainer timelineContainer)
	{
		UnityEngine.Object.DestroyImmediate(timelineContainer.gameObject);
	}

	public void RemoveObservedObject(Transform observedObject)
	{
		if (observedObjects.Contains(observedObject))
		{
			observedObjects.Remove(observedObject);
		}
	}

	public void SkipTimelineTo(float time)
	{
		setSkipTime = time;
	}

	public void SetPlaybackRate(float rate)
	{
		PlaybackRate = rate;
	}

	public void SetPlaybackTime(float time)
	{
		RunningTime = time;
	}

	public void UpdateSequencer(float deltaTime)
	{
		deltaTime *= playbackRate;
		bool flag = false;
		if (playbackRate > 0f && RunningTime >= duration)
		{
			flag = true;
		}
		if (playbackRate < 0f && RunningTime <= 0f)
		{
			flag = true;
		}
		if (playing && !flag)
		{
			runningTime += deltaTime;
			if (runningTime <= 0f)
			{
				runningTime = 0f;
			}
			if (runningTime > Duration)
			{
				runningTime = Duration;
			}
			USTimelineContainer[] timelineContainers = TimelineContainers;
			foreach (USTimelineContainer uSTimelineContainer in timelineContainers)
			{
				uSTimelineContainer.ProcessTimelines(runningTime, PlaybackRate);
			}
		}
		else if (playing)
		{
			if (isLoopingSequence)
			{
				Stop();
				RunningTime = 0f;
				previousTime = -1f;
				Play();
				return;
			}
			if (isPingPongingSequence)
			{
				playbackRate *= -1f;
				return;
			}
			if (PlaybackFinished != null)
			{
				PlaybackFinished();
			}
			playing = false;
		}
		if (setSkipTime > 0f)
		{
			USTimelineContainer[] timelineContainers2 = TimelineContainers;
			foreach (USTimelineContainer uSTimelineContainer2 in timelineContainers2)
			{
				uSTimelineContainer2.SkipTimelineTo(setSkipTime);
			}
			runningTime = setSkipTime;
			previousTime = runningTime;
			setSkipTime = -1f;
		}
	}

	private IEnumerator UpdateSequencerCoroutine()
	{
		while (true)
		{
			float currentTime = Time.time;
			UpdateSequencer(currentTime - previousTime);
			previousTime = currentTime;
			yield return new WaitForSeconds(0.01f * Time.timeScale);
		}
	}
}
