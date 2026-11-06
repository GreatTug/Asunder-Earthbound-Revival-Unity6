using UnityEngine;

public class WavePulse : MonoBehaviour
{
	public Transform _Target;

	public Vector3 _LocalVector = Vector3.zero;

	public float _Amount = 1f;

	public float _Frequency = 1f;

	private Vector3 _currentLocalPos = Vector3.zero;

	private Vector3 _moveVector = Vector3.zero;

	private float _timerDelta;

	public float _StartTimer;

	private void Start()
	{
		if (!_Target)
		{
			_currentLocalPos = base.transform.localPosition;
		}
		else
		{
			_currentLocalPos = _Target.localPosition;
		}
	}

	private void Update()
	{
		_timerDelta += Time.deltaTime;
		_moveVector.x = Mathf.Sin(_timerDelta * _Frequency + _StartTimer) * _Amount * _LocalVector.x;
		_moveVector.y = Mathf.Sin(_timerDelta * _Frequency + _StartTimer) * _Amount * _LocalVector.y;
		_moveVector.z = Mathf.Sin(_timerDelta * _Frequency + _StartTimer) * _Amount * _LocalVector.z;
		if (!_Target)
		{
			base.transform.localPosition = _currentLocalPos + _moveVector;
		}
		else
		{
			_Target.localPosition = _currentLocalPos + _moveVector;
		}
	}
}
