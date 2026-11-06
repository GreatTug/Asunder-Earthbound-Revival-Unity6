using UnityEngine;

public class Breather : MonoBehaviour
{
	public Transform _spine;

	private Quaternion _orignalRotation;

	private float _timer;

	public float _speed = 1f;

	public float _amplify = 2f;

	private Vector3 currentEulers;

	private float _randomNumber;

	private void Start()
	{
		_orignalRotation = _spine.localRotation;
		currentEulers = _orignalRotation.eulerAngles;
		_randomNumber = Random.Range(0.0001f, 123.5432f);
	}

	private void LateUpdate()
	{
		_timer += Time.deltaTime * _speed;
		float num = Mathf.Sin(_timer + _randomNumber);
		currentEulers.y += num * _amplify / 100f;
		_spine.localRotation = Quaternion.Euler(currentEulers);
	}
}
