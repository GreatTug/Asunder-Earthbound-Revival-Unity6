using UnityEngine;

public class NoiseController : MonoBehaviour
{
	public NoiseSegment[] _Segments;

	private float _noise;

	private void Start()
	{
		RefreshInitRotations();
	}

	public void RefreshInitRotations()
	{
		NoiseSegment[] segments = _Segments;
		foreach (NoiseSegment noiseSegment in segments)
		{
			_noise = Mathf.PerlinNoise(Time.time * noiseSegment._Speed, Time.time * noiseSegment._Speed);
			noiseSegment.SetOriginalNoise(new Vector3(_noise * noiseSegment._Scale.x, _noise * noiseSegment._Scale.y, _noise * noiseSegment._Scale.z));
			noiseSegment.SetOriginalAngels(new Vector3(noiseSegment._Segment.localEulerAngles.x, noiseSegment._Segment.localEulerAngles.y, noiseSegment._Segment.localEulerAngles.z));
			noiseSegment._RandomTiming = Random.Range(0.001f, 1.2342f);
		}
	}

	private void LateUpdate()
	{
		NoiseSegment[] segments = _Segments;
		foreach (NoiseSegment noiseSegment in segments)
		{
			if (noiseSegment._Enabled)
			{
				_noise = Mathf.PerlinNoise((Time.time + noiseSegment._RandomTiming) * noiseSegment._Speed, (Time.time + noiseSegment._RandomTiming) * noiseSegment._Speed);
				noiseSegment.SetCurrentAngels(new Vector3(_noise * noiseSegment._Scale.x, _noise * noiseSegment._Scale.y, _noise * noiseSegment._Scale.z));
				float num = noiseSegment.GetCurrentAngels().x - noiseSegment.GetOriginialNoise().x;
				float num2 = noiseSegment.GetCurrentAngels().y - noiseSegment.GetOriginialNoise().y;
				float num3 = noiseSegment.GetCurrentAngels().z - noiseSegment.GetOriginialNoise().z;
				noiseSegment._Segment.localEulerAngles = new Vector3(noiseSegment.GetOriginialAngels().x + (num - num / 2f), noiseSegment.GetOriginialAngels().y + (num2 - num2 / 2f), noiseSegment.GetOriginialAngels().z + (num3 - num3 / 2f));
			}
		}
	}
}
