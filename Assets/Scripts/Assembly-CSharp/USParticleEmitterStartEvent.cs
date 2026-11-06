using UnityEngine;

[USequencerFriendlyName("Start Emitter (Legacy)")]
[USequencerEvent("Particle System/Start Emitter")]
public class USParticleEmitterStartEvent : USEventBase
{
	public void Update()
	{
		if ((bool)base.AffectedObject)
		{
			ParticleSystem component = base.AffectedObject.GetComponent<ParticleSystem>();
			if ((bool)component)
			{
				base.Duration = component.main.duration + component.main.startLifetime.constantMax;
			}
		}
	}

	public override void FireEvent()
	{
		if ((bool)base.AffectedObject)
		{
			ParticleSystem component = base.AffectedObject.GetComponent<ParticleSystem>();
			if (!component)
			{
				Debug.Log("Attempting to emit particles, but the object has no particleSystem USParticleEmitterStartEvent::FireEvent");
			}
			else if (Application.isPlaying)
			{
				component.Play();
			}
		}
	}

	public override void ProcessEvent(float deltaTime)
	{
		if (!Application.isPlaying)
		{
			ParticleSystem component = base.AffectedObject.GetComponent<ParticleSystem>();
			component.Simulate(deltaTime);
		}
	}

	public override void StopEvent()
	{
		UndoEvent();
	}

	public override void UndoEvent()
	{
		if ((bool)base.AffectedObject)
		{
			ParticleSystem component = base.AffectedObject.GetComponent<ParticleSystem>();
			if ((bool)component)
			{
				component.Stop();
			}
		}
	}
}
