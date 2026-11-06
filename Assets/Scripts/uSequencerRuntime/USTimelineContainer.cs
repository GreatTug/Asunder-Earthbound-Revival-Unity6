using UnityEngine;

public class USTimelineContainer : MonoBehaviour
{
	[SerializeField]
	private Transform affectedObject;

	public string affectedObjectPath = string.Empty;

	[SerializeField]
	private int index = -1;

	public Transform AffectedObject
	{
		get
		{
			return affectedObject;
		}
		set
		{
			affectedObject = value;
			RenameTimelineContainer();
		}
	}

	public USSequencer Sequence
	{
		get
		{
			USSequencer component = base.transform.parent.GetComponent<USSequencer>();
			if (!component)
			{
				Debug.LogWarning(base.name + " does not have a parent with an attached sequence, this is a problem. " + base.name + "'s parent is : " + base.transform.parent.name);
			}
			return component;
		}
	}

	public USTimelineBase[] Timelines
	{
		get
		{
			return GetComponentsInChildren<USTimelineBase>();
		}
	}

	public int Index
	{
		get
		{
			return index;
		}
		set
		{
			index = value;
		}
	}

	public static int Comparer(USTimelineContainer a, USTimelineContainer b)
	{
		return a.Index.CompareTo(b.Index);
	}

	public void AddNewTimeline(USTimelineBase timeline)
	{
		base.transform.parent = base.transform;
	}

	public void ProcessTimelines(float sequencerTime, float playbackRate)
	{
		USTimelineBase[] timelines = Timelines;
		foreach (USTimelineBase uSTimelineBase in timelines)
		{
			uSTimelineBase.Process(sequencerTime, playbackRate);
		}
	}

	public void SkipTimelineTo(float sequencerTime)
	{
		USTimelineBase[] timelines = Timelines;
		foreach (USTimelineBase uSTimelineBase in timelines)
		{
			uSTimelineBase.SkipTimelineTo(sequencerTime);
		}
	}

	public void ManuallySetTime(float sequencerTime)
	{
		USTimelineBase[] timelines = Timelines;
		foreach (USTimelineBase uSTimelineBase in timelines)
		{
			uSTimelineBase.ManuallySetTime(sequencerTime);
		}
	}

	public void RenameTimelineContainer()
	{
		if ((bool)affectedObject)
		{
			base.name = "Timelines for " + affectedObject.name;
		}
	}
}
