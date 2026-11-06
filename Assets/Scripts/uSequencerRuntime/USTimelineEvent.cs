using System;
using System.Linq;
using UnityEngine;

public class USTimelineEvent : USTimelineBase
{
	public int EventCount
	{
		get
		{
			return base.transform.childCount;
		}
	}

	public USEventBase[] Events
	{
		get
		{
			return GetComponentsInChildren<USEventBase>();
		}
	}

	public USEventBase Event(int index)
	{
		if (index >= EventCount)
		{
			Debug.LogError("Trying to access an element in mEventList that does not exist from USTimelineEvent::Event");
			return null;
		}
		USEventBase component = base.transform.GetChild(index).GetComponent<USEventBase>();
		if (!component)
		{
			Debug.LogWarning("We've requested an event that doesn't have a USEventBase component : " + component.name + " from timeline : " + base.name);
		}
		return component;
	}

	public override void StopTimeline()
	{
		float prevElapsedTime = elapsedTime;
		elapsedTime = 0f;
		USEventBase[] array = Events.Where((USEventBase e) => e.Firetime < prevElapsedTime).ToArray();
		USEventBase[] array2 = array;
		foreach (USEventBase uSEventBase in array2)
		{
			uSEventBase.StopEvent();
		}
	}

	public override void PauseTimeline()
	{
		USEventBase[] events = Events;
		foreach (USEventBase uSEventBase in events)
		{
			uSEventBase.PauseEvent();
		}
	}

	public override void ResumeTimeline()
	{
		USEventBase[] events = Events;
		foreach (USEventBase uSEventBase in events)
		{
			if (!uSEventBase.IsFireAndForgetEvent && base.Sequence.RunningTime > uSEventBase.Firetime && base.Sequence.RunningTime < uSEventBase.Firetime + uSEventBase.Duration)
			{
				uSEventBase.ResumeEvent();
			}
		}
	}

	public override void SkipTimelineTo(float time)
	{
		float num = elapsedTime;
		elapsedTime = time;
		foreach (Transform item in base.transform)
		{
			USEventBase component = item.GetComponent<USEventBase>();
			if ((bool)component && component.IsFireAndForgetEvent && component.FireOnSkip && (num < component.Firetime || num <= 0f) && base.Sequence.IsPlaying && (bool)component.AffectedObject)
			{
				component.FireEvent();
			}
		}
	}

	public override void Process(float sequencerTime, float playbackRate)
	{
		float num = elapsedTime;
		elapsedTime = sequencerTime;
		USEventBase[] events = Events;
		if (num < elapsedTime)
		{
			Array.Sort(events, (USEventBase a, USEventBase b) => a.Firetime.CompareTo(b.Firetime));
		}
		else
		{
			Array.Sort(events, (USEventBase a, USEventBase b) => b.Firetime.CompareTo(a.Firetime));
		}
		USEventBase[] array = events;
		foreach (USEventBase baseEvent in array)
		{
			if (playbackRate >= 0f)
			{
				FireEvent(baseEvent, num, elapsedTime);
			}
			else
			{
				FireEventReverse(baseEvent, num, elapsedTime);
			}
			FireEventCommon(baseEvent, sequencerTime, num, elapsedTime);
		}
	}

	private void FireEvent(USEventBase baseEvent, float prevElapsedTime, float elapsedTime)
	{
		if ((prevElapsedTime < baseEvent.Firetime || prevElapsedTime <= 0f) && elapsedTime >= baseEvent.Firetime && (bool)baseEvent.AffectedObject)
		{
			baseEvent.FireEvent();
		}
	}

	private void FireEventReverse(USEventBase baseEvent, float prevElapsedTime, float elapsedTime)
	{
		if ((prevElapsedTime > baseEvent.Firetime || prevElapsedTime >= base.Sequence.Duration) && elapsedTime <= baseEvent.Firetime && (bool)baseEvent.AffectedObject)
		{
			baseEvent.FireEvent();
		}
	}

	private void FireEventCommon(USEventBase baseEvent, float sequencerTime, float prevElapsedTime, float elapsedTime)
	{
		if (elapsedTime > baseEvent.Firetime && elapsedTime <= baseEvent.Firetime + baseEvent.Duration)
		{
			float runningTime = sequencerTime - baseEvent.Firetime;
			if ((bool)baseEvent.AffectedObject)
			{
				baseEvent.ProcessEvent(runningTime);
			}
		}
		if (prevElapsedTime <= baseEvent.Firetime + baseEvent.Duration && elapsedTime > baseEvent.Firetime + baseEvent.Duration && (bool)baseEvent.AffectedObject)
		{
			baseEvent.EndEvent();
		}
		if (prevElapsedTime >= baseEvent.Firetime && elapsedTime < baseEvent.Firetime && (bool)baseEvent.AffectedObject)
		{
			baseEvent.UndoEvent();
		}
	}

	public override void ManuallySetTime(float sequencerTime)
	{
		foreach (Transform item in base.transform)
		{
			USEventBase component = item.GetComponent<USEventBase>();
			if ((bool)component)
			{
				float deltaTime = sequencerTime - component.Firetime;
				if ((bool)component.AffectedObject)
				{
					component.ManuallySetTime(deltaTime);
				}
			}
		}
	}

	public void AddNewEvent(USEventBase sequencerEvent)
	{
		sequencerEvent.transform.parent = base.transform;
		SortEvents();
	}

	public void RemoveAndDestroyEvent(Transform sequencerEvent)
	{
		if (!sequencerEvent.IsChildOf(base.transform))
		{
			Debug.LogError("We are trying to delete an Event that doesn't belong to this Timeline, from USTimelineEvent::RemoveAndDestroyEvent");
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(sequencerEvent.gameObject);
		}
	}

	public void SortEvents()
	{
		Debug.LogWarning("Implement a sorting algorithm here!");
	}
}
