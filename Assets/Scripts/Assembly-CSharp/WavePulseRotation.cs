using UnityEngine;

public class WavePulseRotation : MonoBehaviour
{
	public Transform _Target;

	public Vector3 _LocalVector = Vector3.zero;

	public float _Amount = 1f;

	public float _Frequency = 1f;

	public float _TimerOffset;

	public bool _DoOnce;

	private Vector3 _currentLocalRotation;

	private Vector3 _moveVector = Vector3.zero;

	private bool _stopped;

	private float _timerDelta;

	private void Start()
	{
		if (!_Target)
		{
			_currentLocalRotation = base.transform.localRotation.eulerAngles;
		}
		else
		{
			_currentLocalRotation = _Target.localRotation.eulerAngles;
		}
	}

	private void Update()
	{
		_timerDelta += Time.deltaTime;
		if (!_DoOnce)
		{
			calculate();
		}
		else if (!_stopped)
		{
			if (Mathf.Sin((_timerDelta + _TimerOffset) * _Frequency) < 0.95f)
			{
				calculate();
			}
			else
			{
				_stopped = true;
			}
		}
	}

	private void calculate()
	{
		_moveVector.x = Mathf.Sin((_timerDelta + _TimerOffset) * _Frequency) * _Amount * _LocalVector.x;
		_moveVector.y = Mathf.Sin((_timerDelta + _TimerOffset) * _Frequency) * _Amount * _LocalVector.y;
		_moveVector.z = Mathf.Sin((_timerDelta + _TimerOffset) * _Frequency) * _Amount * _LocalVector.z;
		if (!_Target)
		{
			base.transform.localRotation = Quaternion.Euler(_moveVector + _currentLocalRotation);
		}
		else
		{
			_Target.localRotation = Quaternion.Euler(_moveVector + _currentLocalRotation);
		}
	}
}
