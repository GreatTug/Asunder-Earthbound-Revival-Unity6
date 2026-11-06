using UnityEngine;

public class BreatherII : MonoBehaviour
{
	public AudioClip _BreathIn;

	public AudioClip _BreathOut;

	public Transform _spine;

	private Quaternion _orignalRotation;

	private float _timer;

	public float _speed = 1f;

	public float _amplify = 2f;

	private Vector3 currentEulers;

	public ParticleSystem visualBreather;

	private void Start()
	{
		_orignalRotation = _spine.localRotation;
		currentEulers = _orignalRotation.eulerAngles;
	}

	private void LateUpdate()
	{
		_timer += Time.deltaTime * _speed;
		float num = Mathf.Sin(_timer);
		currentEulers.x += num * _amplify / 100f;
		_spine.localRotation = Quaternion.Euler(currentEulers);
		ParticleSystem.EmissionModule emission = visualBreather.emission;
		if (num > 0f)
		{
			if (emission.enabled)
			{
				emission.enabled = false;
			}
		}
		else if (!emission.enabled)
		{
			emission.enabled = true;
		}
	}
}
