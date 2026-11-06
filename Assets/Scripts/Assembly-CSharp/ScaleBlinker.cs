using UnityEngine;

public class ScaleBlinker : MonoBehaviour
{
	private float _Frequency = 0.2f;

	private float _Amount = 2.1f;

	private Vector3 _currentScale = Vector3.one;

	private void Start()
	{
		_currentScale = base.transform.localScale;
	}

	private void Update()
	{
		Vector3 currentScale = _currentScale;
		currentScale.z = _currentScale.z * (Mathf.PerlinNoise(Time.time * _Frequency, 0f) + 0.1f) * _Amount;
		base.transform.localScale = currentScale;
	}
}
