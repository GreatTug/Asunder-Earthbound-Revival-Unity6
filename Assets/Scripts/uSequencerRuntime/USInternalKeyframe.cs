using System;
using UnityEngine;

[Serializable]
public class USInternalKeyframe : ScriptableObject
{
	[SerializeField]
	private float value;

	[SerializeField]
	private float time;

	[SerializeField]
	private float inTangent;

	[SerializeField]
	private float outTangent;

	[SerializeField]
	private bool brokenTangents;

	[SerializeField]
	public USInternalCurve curve;

	public float Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
			if ((bool)curve)
			{
				curve.BuildAnimationCurveFromInternalCurve();
			}
		}
	}

	public float Time
	{
		get
		{
			return time;
		}
		set
		{
			time = value;
			if ((bool)curve)
			{
				time = Mathf.Min(Mathf.Max(0f, time), curve.Duration);
				curve.BuildAnimationCurveFromInternalCurve();
			}
		}
	}

	public float InTangent
	{
		get
		{
			return inTangent;
		}
		set
		{
			inTangent = value;
			if ((bool)curve)
			{
				curve.BuildAnimationCurveFromInternalCurve();
			}
		}
	}

	public float OutTangent
	{
		get
		{
			return outTangent;
		}
		set
		{
			outTangent = value;
			if ((bool)curve)
			{
				curve.BuildAnimationCurveFromInternalCurve();
			}
		}
	}

	public bool BrokenTangents
	{
		get
		{
			return brokenTangents;
		}
		set
		{
			brokenTangents = value;
			if ((bool)curve)
			{
				curve.BuildAnimationCurveFromInternalCurve();
			}
		}
	}

	public void ConvertFrom(Keyframe keyframe)
	{
		value = keyframe.value;
		time = keyframe.time;
		inTangent = keyframe.inTangent;
		outTangent = keyframe.outTangent;
	}

	public void Smooth()
	{
		if (!curve)
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < curve.UnityAnimationCurve.keys.Length; i++)
		{
			Keyframe keyframe = curve.UnityAnimationCurve.keys[i];
			if (Mathf.Approximately(keyframe.time, time))
			{
				num = i;
			}
			if (num != -1)
			{
				break;
			}
		}
		if (num != -1)
		{
			curve.UnityAnimationCurve.SmoothTangents(num, 0f);
			curve.BuildInternalCurveFromAnimationCurve();
		}
	}

	public void Flatten()
	{
		if ((bool)curve)
		{
			inTangent = 0f;
			outTangent = 0f;
			curve.BuildAnimationCurveFromInternalCurve();
		}
	}

	public void RightTangentLinear()
	{
		if ((bool)curve)
		{
			USInternalKeyframe nextKeyframe = curve.GetNextKeyframe(this);
			if (!(nextKeyframe == null))
			{
				outTangent = (nextKeyframe.value - value) / (nextKeyframe.time - time);
				brokenTangents = true;
				curve.BuildAnimationCurveFromInternalCurve();
			}
		}
	}

	public void RightTangentConstant()
	{
		outTangent = float.PositiveInfinity;
		curve.BuildAnimationCurveFromInternalCurve();
	}

	public void LeftTangentLinear()
	{
		if ((bool)curve)
		{
			USInternalKeyframe prevKeyframe = curve.GetPrevKeyframe(this);
			if (!(prevKeyframe == null))
			{
				inTangent = (prevKeyframe.value - value) / (prevKeyframe.time - time);
				brokenTangents = true;
				curve.BuildAnimationCurveFromInternalCurve();
			}
		}
	}

	public void LeftTangentConstant()
	{
		inTangent = float.PositiveInfinity;
		curve.BuildAnimationCurveFromInternalCurve();
	}
}
