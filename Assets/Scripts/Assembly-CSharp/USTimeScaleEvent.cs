using UnityEngine;

[USequencerEvent("Time/Time Scale")]
[USequencerFriendlyName("Time Scale")]
public class USTimeScaleEvent : USEventBase
{
	public AnimationCurve scaleCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f), new Keyframe(3f, 1f), new Keyframe(4f, 0f));

	private float currentCurveSampleTime;

	private float prevTimeScale = 1f;

	public override void FireEvent()
	{
		prevTimeScale = Time.timeScale;
	}

	public override void ProcessEvent(float deltaTime)
	{
		currentCurveSampleTime = deltaTime;
		Time.timeScale = Mathf.Max(0f, scaleCurve.Evaluate(currentCurveSampleTime));
	}

	public override void EndEvent()
	{
		float time = scaleCurve.keys[scaleCurve.length - 1].time;
		Time.timeScale = Mathf.Max(0f, scaleCurve.Evaluate(time));
	}

	public override void StopEvent()
	{
		UndoEvent();
	}

	public override void UndoEvent()
	{
		currentCurveSampleTime = 0f;
		Time.timeScale = prevTimeScale;
	}
}
