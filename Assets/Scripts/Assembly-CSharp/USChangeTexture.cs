using UnityEngine;

[USequencerFriendlyName("Change Texture")]
[USequencerEvent("Render/Change Objects Texture")]
public class USChangeTexture : USEventBase
{
	public Texture newTexture;

	private Texture previousTexture;

	public override void FireEvent()
	{
		if ((bool)base.AffectedObject)
		{
			if (!newTexture)
			{
				Debug.LogWarning("you've not given a texture to the USChangeTexture Event", this);
			}
			else if (!Application.isPlaying && Application.isEditor)
			{
				previousTexture = base.AffectedObject.GetComponent<Renderer>().sharedMaterial.mainTexture;
				base.AffectedObject.GetComponent<Renderer>().sharedMaterial.mainTexture = newTexture;
			}
			else
			{
				previousTexture = base.AffectedObject.GetComponent<Renderer>().material.mainTexture;
				base.AffectedObject.GetComponent<Renderer>().material.mainTexture = newTexture;
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
		if ((bool)base.AffectedObject && (bool)previousTexture)
		{
			if (!Application.isPlaying && Application.isEditor)
			{
				base.AffectedObject.GetComponent<Renderer>().sharedMaterial.mainTexture = previousTexture;
			}
			else
			{
				base.AffectedObject.GetComponent<Renderer>().material.mainTexture = previousTexture;
			}
			previousTexture = null;
		}
	}
}
