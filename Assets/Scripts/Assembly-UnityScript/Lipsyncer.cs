using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Lipsyncer : MonoBehaviour
{
	public float SmoothJaw;

	public float SmoothHead;

	public Transform Jaw;

	public Transform HeadBone;

	public Transform Lips_R;

	private Vector3 Lips_R_initPos;

	public Transform Lips_L;

	private Vector3 Lips_L_initPos;

	public float LipConstant;

	public Vector3 LipMagnifier;

	public float Offset;

	public float Offset2;

	public float Multiplier;

	public float Multiplier2;

	public int qSamples;

	public float refValue;

	public float threshold;

	public float rmsValue;

	public float dbValue;

	public float pitchValue;

	private float[] samples;

	private float[] spectrum;

	public Text display;

	public Lipsyncer()
	{
		SmoothJaw = 0.1f;
		SmoothHead = 0.5f;
		LipConstant = 0.01f;
		Multiplier = 0.5f;
		Multiplier2 = 0.5f;
		qSamples = 1024;
		refValue = 0.1f;
		threshold = 0.02f;
	}

	public virtual void Start()
	{
		samples = new float[qSamples];
		spectrum = new float[qSamples];
		Lips_R_initPos = Lips_R.localPosition;
		Lips_L_initPos = Lips_L.localPosition;
	}

	public virtual void AnalyzeSound()
	{
		GetComponent<AudioSource>().GetOutputData(samples, 0);
		int num = default(int);
		float num2 = 0f;
		for (num = 0; num < qSamples; num++)
		{
			num2 += samples[num] * samples[num];
		}
		rmsValue = Mathf.Sqrt(num2 / (float)qSamples);
		dbValue = 20f * Mathf.Log10(rmsValue / refValue);
		if (!(dbValue >= -160f))
		{
			dbValue = -160f;
		}
		GetComponent<AudioSource>().GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
		float num3 = 0f;
		int num4 = 0;
		for (num = 0; num < qSamples; num++)
		{
			if (!(spectrum[num] <= num3) && !(spectrum[num] <= threshold))
			{
				num3 = spectrum[num];
				num4 = num;
			}
		}
		float num5 = num4;
		if (num4 > 0 && num4 < qSamples - 1)
		{
			float num6 = spectrum[num4 - 1] / spectrum[num4];
			float num7 = spectrum[num4 + 1] / spectrum[num4];
			num5 += 0.5f * (num7 * num7 - num6 * num6);
		}
		pitchValue = num5 * 24000f / (float)qSamples;
	}

	public virtual void LateUpdate()
	{
		if (GetComponent<AudioSource>().isPlaying)
		{
			AnalyzeSound();
			float y = 1f / (dbValue - 40f) * Multiplier * -1f + Offset;
			float y2 = 1f / (dbValue - 40f) * Multiplier2 * -1f;
			float num = 1f / (dbValue - 40f) * -200f;
			if (!(num >= 3f))
			{
				num = 0f;
				y = Offset;
				y2 = 0f;
			}
			if (HeadBone != null)
			{
				HeadBone.localRotation = Quaternion.Lerp(HeadBone.localRotation, Quaternion.Euler(0f, y2, 0f), SmoothHead * Time.deltaTime);
			}
			Jaw.localRotation = Quaternion.Lerp(Jaw.localRotation, Quaternion.Euler(0f, y, 0f), SmoothJaw * Time.deltaTime);
			Vector3 vector = new Vector3
			{
				x = LipMagnifier.x * num * LipConstant,
				y = LipMagnifier.y * num * LipConstant,
				z = LipMagnifier.z * num * LipConstant
			};
			Lips_R.localPosition = Vector3.Lerp(Lips_R.localPosition, vector + Lips_R_initPos, SmoothJaw * Time.deltaTime);
			Lips_L.localPosition = Vector3.Lerp(Lips_L.localPosition, vector + Lips_L_initPos, SmoothJaw * Time.deltaTime);
		}
	}

	public virtual void Main()
	{
	}
}
