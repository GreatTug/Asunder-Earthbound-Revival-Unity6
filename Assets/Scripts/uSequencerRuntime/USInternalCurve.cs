using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class USInternalCurve : ScriptableObject
{
	private bool hasBeenModified;

	[SerializeField]
	private float duration = 10f;

	[SerializeField]
	private AnimationCurve animationCurve;

	[SerializeField]
	private List<USInternalKeyframe> internalKeyframes = new List<USInternalKeyframe>();

	public bool HasBeenModified
	{
		get
		{
			return hasBeenModified;
		}
		set
		{
			hasBeenModified = value;
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

	public float FirstKeyframeTime
	{
		get
		{
			if (internalKeyframes.Count == 0)
			{
				return 0f;
			}
			return internalKeyframes[0].Time;
		}
	}

	public float LastKeyframeTime
	{
		get
		{
			if (internalKeyframes.Count == 0)
			{
				return Duration;
			}
			return internalKeyframes[internalKeyframes.Count - 1].Time;
		}
	}

	public AnimationCurve UnityAnimationCurve
	{
		get
		{
			return animationCurve;
		}
		set
		{
			animationCurve = new AnimationCurve();
			Keyframe[] keys = value.keys;
			foreach (Keyframe key in keys)
			{
				animationCurve.AddKey(key);
			}
			BuildInternalCurveFromAnimationCurve();
		}
	}

	public USInternalKeyframe[] Keys
	{
		get
		{
			return internalKeyframes.ToArray();
		}
	}

	private int KeyframeComparer(USInternalKeyframe a, USInternalKeyframe b)
	{
		return a.Time.CompareTo(b.Time);
	}

	public float Evaluate(float time)
	{
		return animationCurve.Evaluate(time);
	}

	public void BuildInternalCurveFromAnimationCurve()
	{
		Keyframe[] keys = animationCurve.keys;
		for (int i = 0; i < keys.Length; i++)
		{
			Keyframe keyframe = keys[i];
			USInternalKeyframe uSInternalKeyframe = null;
			foreach (USInternalKeyframe internalKeyframe in internalKeyframes)
			{
				if (Mathf.Approximately(keyframe.time, internalKeyframe.Time))
				{
					uSInternalKeyframe = internalKeyframe;
					break;
				}
			}
			if ((bool)uSInternalKeyframe)
			{
				uSInternalKeyframe.ConvertFrom(keyframe);
				continue;
			}
			USInternalKeyframe uSInternalKeyframe2 = ScriptableObject.CreateInstance<USInternalKeyframe>();
			uSInternalKeyframe2.ConvertFrom(keyframe);
			uSInternalKeyframe2.curve = this;
			internalKeyframes.Add(uSInternalKeyframe2);
			internalKeyframes.Sort(KeyframeComparer);
		}
		HasBeenModified = true;
	}

	public void BuildAnimationCurveFromInternalCurve()
	{
		while (animationCurve.keys.Length > 0)
		{
			animationCurve.RemoveKey(0);
		}
		Keyframe key = default(Keyframe);
		USInternalKeyframe[] keys = Keys;
		foreach (USInternalKeyframe uSInternalKeyframe in keys)
		{
			key.value = uSInternalKeyframe.Value;
			key.time = uSInternalKeyframe.Time;
			key.inTangent = uSInternalKeyframe.InTangent;
			key.outTangent = uSInternalKeyframe.OutTangent;
			animationCurve.AddKey(key);
		}
		internalKeyframes.Sort(KeyframeComparer);
		HasBeenModified = true;
	}

	public void ValidateKeyframeTimes()
	{
		for (int num = Keys.Length - 1; num >= 0; num--)
		{
			if (num != Keys.Length - 1 && Mathf.Approximately(Keys[num].Time, Keys[num + 1].Time))
			{
				internalKeyframes.Remove(Keys[num]);
			}
		}
	}

	public USInternalKeyframe AddKeyframe(float time, float value)
	{
		USInternalKeyframe uSInternalKeyframe = null;
		foreach (USInternalKeyframe internalKeyframe in internalKeyframes)
		{
			if (Mathf.Approximately(internalKeyframe.Time, time))
			{
				uSInternalKeyframe = internalKeyframe;
			}
			if (uSInternalKeyframe != null)
			{
				break;
			}
		}
		if (!uSInternalKeyframe)
		{
			uSInternalKeyframe = ScriptableObject.CreateInstance<USInternalKeyframe>();
			internalKeyframes.Add(uSInternalKeyframe);
		}
		uSInternalKeyframe.Time = time;
		uSInternalKeyframe.Value = value;
		uSInternalKeyframe.InTangent = 0f;
		uSInternalKeyframe.OutTangent = 0f;
		uSInternalKeyframe.curve = this;
		internalKeyframes.Sort(KeyframeComparer);
		BuildAnimationCurveFromInternalCurve();
		return uSInternalKeyframe;
	}

	public void RemoveKeyframe(USInternalKeyframe internalKeyframe)
	{
		for (int num = internalKeyframes.Count - 1; num >= 0; num--)
		{
			if (internalKeyframes[num] == internalKeyframe)
			{
				internalKeyframes.RemoveAt(num);
			}
		}
		internalKeyframes.Sort(KeyframeComparer);
		BuildAnimationCurveFromInternalCurve();
	}

	public USInternalKeyframe GetNextKeyframe(USInternalKeyframe keyframe)
	{
		if (internalKeyframes.Count == 0)
		{
			return null;
		}
		if (internalKeyframes[internalKeyframes.Count - 1] == keyframe)
		{
			return null;
		}
		int num = -1;
		for (int i = 0; i < internalKeyframes.Count; i++)
		{
			if (internalKeyframes[i] == keyframe)
			{
				num = i;
			}
			if (num != -1)
			{
				break;
			}
		}
		return internalKeyframes[num + 1];
	}

	public USInternalKeyframe GetPrevKeyframe(USInternalKeyframe keyframe)
	{
		if (internalKeyframes.Count == 0)
		{
			return null;
		}
		if (internalKeyframes[0] == keyframe)
		{
			return null;
		}
		int num = -1;
		for (int i = 0; i < internalKeyframes.Count; i++)
		{
			if (internalKeyframes[i] == keyframe)
			{
				num = i;
			}
			if (num != -1)
			{
				break;
			}
		}
		return internalKeyframes[num - 1];
	}

	public bool Contains(USInternalKeyframe keyframe)
	{
		for (int num = Keys.Length - 1; num >= 0; num--)
		{
			if (Keys[num] == keyframe)
			{
				return true;
			}
		}
		return false;
	}

	public float FindNextKeyframeTime(float time)
	{
		float time2 = duration;
		for (int num = Keys.Length - 1; num >= 0; num--)
		{
			if (Keys[num].Time < time2 && Keys[num].Time > time)
			{
				time2 = Keys[num].Time;
			}
		}
		return time2;
	}

	public float FindPrevKeyframeTime(float time)
	{
		float num = 0f;
		for (int i = 0; i < Keys.Length; i++)
		{
			if (Keys[i].Time > num && Keys[i].Time < time)
			{
				num = Keys[i].Time;
			}
		}
		return num;
	}
}
