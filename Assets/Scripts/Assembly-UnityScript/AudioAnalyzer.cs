using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class AudioAnalyzer : MonoBehaviour
{
	public Material RadioMaterial;

	public Color RadioMColor;

	public int qSamples;

	public float refValue;

	public float threshold;

	public float rmsValue;

	public float dbValue;

	public float pitchValue;

	private float[] samples;

	private float[] spectrum;

	public Text display;

	public AudioAnalyzer()
	{
		qSamples = 1024;
		refValue = 0.1f;
		threshold = 0.02f;
	}

	public virtual void Start()
	{
		samples = new float[qSamples];
		spectrum = new float[qSamples];
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

	public virtual void Update()
	{
		AnalyzeSound();
		float value = 1f / (dbValue - 40f) * -100f - 1.7f;
		float num = Mathf.Clamp(value, 0.25f, 0.8f);
		RadioMColor.r = num;
		RadioMColor.g = num;
		RadioMColor.b = num;
		RadioMaterial.color = RadioMColor;
	}

	public virtual void Main()
	{
	}
}
