using UnityEngine;

public class Lightning : MonoBehaviour
{
	public Light _LightLightning;

	private float _timer;

	private void Start()
	{
		_timer = 5 + Random.Range(0, 10);
	}

	private void Update()
	{
		if (Time.time > _timer)
		{
			_LightLightning.intensity = 1.5f;
			_timer = Time.time + 5f + (float)Random.Range(0, 10);
		}
		if (_LightLightning.intensity > 0f)
		{
			_LightLightning.intensity -= 0.075f;
		}
	}
}
