using UnityEngine;

[USequencerEvent("Attach/Attach To Parent")]
[USequencerFriendlyName("Attach Object To Parent")]
public class USAttachToParentEvent : USEventBase
{
	public Transform parentObject;

	private Transform originalParent;

	public override void FireEvent()
	{
		if (!parentObject)
		{
			Debug.Log("USAttachEvent has been asked to attach an object, but it hasn't been given a parent from USAttachEvent::FireEvent");
			return;
		}
		originalParent = base.AffectedObject.transform.parent;
		base.AffectedObject.transform.parent = parentObject;
	}

	public override void ProcessEvent(float deltaTime)
	{
	}

	public override void StopEvent()
	{
		UndoEvent();
	}

	public override void UndoEvent()
	{
		if ((bool)base.AffectedObject)
		{
			base.AffectedObject.transform.parent = originalParent;
		}
	}
}
