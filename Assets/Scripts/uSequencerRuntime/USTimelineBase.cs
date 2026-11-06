using UnityEngine;

public class USTimelineBase : MonoBehaviour
{
	public float elapsedTime;

	public Transform AffectedObject
	{
		get
		{
			USTimelineContainer component = base.transform.parent.GetComponent<USTimelineContainer>();
			if (!component)
			{
				Debug.LogWarning(base.name + " does not have a parent with an attached container, this is a problem. " + base.name + "'s parent is : " + base.transform.parent.name);
			}
			return component.AffectedObject;
		}
	}

	public USTimelineContainer TimelineContainer
	{
		get
		{
			USTimelineContainer component = base.transform.parent.GetComponent<USTimelineContainer>();
			if (!component)
			{
				Debug.LogWarning(base.name + " does not have a parent with an attached timeline Container, this is a problem. " + base.name + "'s parent is : " + base.transform.parent.name);
			}
			return component;
		}
	}

	public USSequencer Sequence
	{
		get
		{
			return TimelineContainer.Sequence;
		}
	}

	public virtual void Reset()
	{
	}

	public virtual void StopTimeline()
	{
	}

	public virtual void StartTimeline()
	{
	}

	public virtual void PauseTimeline()
	{
	}

	public virtual void ResumeTimeline()
	{
	}

	public virtual void SkipTimelineTo(float time)
	{
	}

	public virtual void Process(float sequencerTime, float playbackRate)
	{
	}

	public virtual void ManuallySetTime(float sequencerTime)
	{
	}
}
