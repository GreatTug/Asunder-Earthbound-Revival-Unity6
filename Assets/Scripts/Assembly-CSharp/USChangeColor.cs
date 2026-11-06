using UnityEngine;

[USequencerFriendlyName("Change Color")]
[USequencerEvent("Render/Change Objects Color")]
public class USChangeColor : USEventBase
{
	public Color newColor;

	private Color previousColor;

	public override void FireEvent()
	{
		if ((bool)base.AffectedObject)
		{
			if (!Application.isPlaying && Application.isEditor)
			{
				previousColor = base.AffectedObject.GetComponent<Renderer>().sharedMaterial.color;
				base.AffectedObject.GetComponent<Renderer>().sharedMaterial.color = newColor;
			}
			else
			{
				previousColor = base.AffectedObject.GetComponent<Renderer>().material.color;
				base.AffectedObject.GetComponent<Renderer>().material.color = newColor;
			}
		}
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
			if (!Application.isPlaying && Application.isEditor)
			{
				base.AffectedObject.GetComponent<Renderer>().sharedMaterial.color = previousColor;
			}
			else
			{
				base.AffectedObject.GetComponent<Renderer>().material.color = previousColor;
			}
		}
	}
}
