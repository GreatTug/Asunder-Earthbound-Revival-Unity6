using UnityEngine;

[USequencerFriendlyName("stop uSequence")]
[USequencerEvent("Sequence/Stop uSequence")]
public class USStopSequenceEvent : USEventBase
{
	public USSequencer sequence;

	public override void FireEvent()
	{
		if (!sequence)
		{
			Debug.LogWarning("No sequence for USstopSequenceEvent : " + base.name, this);
		}
		if ((bool)sequence)
		{
			sequence.Stop();
		}
	}

	public override void ProcessEvent(float deltaTime)
	{
	}
}
