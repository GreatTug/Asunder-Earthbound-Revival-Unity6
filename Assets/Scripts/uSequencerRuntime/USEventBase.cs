using UnityEngine;

[ExecuteInEditMode]
public abstract class USEventBase : MonoBehaviour
{
	public enum UILayer
	{
		Front = 0,
		Middle = 1,
		Back = 2
	}

	[SerializeField]
	private bool fireOnSkip;

	[SerializeField]
	private float firetime = -1f;

	[SerializeField]
	private float duration = -1f;

	public float Firetime
	{
		get
		{
			return firetime;
		}
		set
		{
			firetime = value;
			if (firetime < 0f)
			{
				firetime = 0f;
			}
			if (firetime > Sequence.Duration)
			{
				firetime = Sequence.Duration;
			}
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
		}
	}

	public USSequencer Sequence
	{
		get
		{
			return (!Timeline) ? null : Timeline.Sequence;
		}
	}

	public USTimelineBase Timeline
	{
		get
		{
			if (!base.transform.parent)
			{
				return null;
			}
			USTimelineBase component = base.transform.parent.GetComponent<USTimelineBase>();
			if (!component)
			{
				Debug.LogWarning(base.name + " does not have a parent with an attached timeline, this is a problem. " + base.name + "'s parent is : " + base.transform.parent.name);
			}
			return component;
		}
	}

	public USTimelineContainer TimelineContainer
	{
		get
		{
			return (!Timeline) ? null : Timeline.TimelineContainer;
		}
	}

	public GameObject AffectedObject
	{
		get
		{
			return (!TimelineContainer.AffectedObject) ? null : TimelineContainer.AffectedObject.gameObject;
		}
	}

	public bool IsFireAndForgetEvent
	{
		get
		{
			return Duration < 0f;
		}
	}

	public bool FireOnSkip
	{
		get
		{
			return fireOnSkip;
		}
	}

	public abstract void FireEvent();

	public abstract void ProcessEvent(float runningTime);

	public virtual void PauseEvent()
	{
	}

	public virtual void ResumeEvent()
	{
	}

	public virtual void StopEvent()
	{
	}

	public virtual void EndEvent()
	{
	}

	public virtual void UndoEvent()
	{
	}

	public virtual void ManuallySetTime(float deltaTime)
	{
	}
}
