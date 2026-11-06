using UnityEngine;

public class HeadGestureRecognizer : MonoBehaviour
{
	public enum HeadGesture
	{
		None = 0,
		AcceptNod = 1,
		RefuseShake = 2,
		PointRight = 3,
		PointLeft = 4
	}

	private Transform _EyeRightTransform;

	private GameObject _CurrentObjectFocus;

	private float _lastAngleX;

	private float _lastAngleY;

	private float _lastAngleZ;

	private float _timer;

	private float _timerQuick;

	private float _sampleAccumulator;

	private const float _SAMPLE_INTERVAL = 1f / 60f;

	private HeadGesture _currentHeadGesture;

	private int _gestureMoveCount;

	private bool _gesturePositive;

	private float _DELTA_LIMIT_ACCEPTNOD = 2f;

	private float _DELTA_LIMIT_REFUSESHAKE = 2.5f;

	private float _DELTA_LIMIT_HEADPOINTING = 3f;

	private float _DETECTION_DURATION = 1f;

	private float _GESTURE_RECOGNITION_LOSS = 1f;

	private int _GESTURE_MOVEMENT_REQUIRED = 8;

	private int _GESTURE_POINT_REQUIRED = 8;

	public void InitCameraTransform(Transform aTransform)
	{
		_EyeRightTransform = aTransform;
		if (_EyeRightTransform != null)
		{
			Vector3 euler = _EyeRightTransform.localEulerAngles;
			_lastAngleX = euler.x;
			_lastAngleY = euler.y;
			_lastAngleZ = euler.z;
		}
	}

	private void Start()
	{
		if (_EyeRightTransform != null)
		{
			Vector3 euler = _EyeRightTransform.localEulerAngles;
			_lastAngleX = euler.x;
			_lastAngleY = euler.y;
			_lastAngleZ = euler.z;
		}
	}

	private void Update()
	{
		if (_EyeRightTransform != null)
		{
			detectGazeDirection();
			detectHeadGestures();
		}
	}

	public void resetRecognizer()
	{
		_timer = 0f;
		_timerQuick = 0f;
		_sampleAccumulator = 0f;
		_gestureMoveCount = 0;
		_currentHeadGesture = HeadGesture.None;
	}

	private void detectHeadGestures()
	{
		if (AsunderInput.GetKeyDown(KeyCode.Y) || AsunderInput.GetThumbstickNodDown())
		{
			_timer = Time.time + _DETECTION_DURATION;
			setDetectedGesture(HeadGesture.AcceptNod);
			return;
		}
		if (AsunderInput.GetKeyDown(KeyCode.N) || AsunderInput.GetThumbstickShakeDown())
		{
			_timer = Time.time + _DETECTION_DURATION;
			setDetectedGesture(HeadGesture.RefuseShake);
			return;
		}

		_sampleAccumulator += Time.deltaTime;
		if (_sampleAccumulator >= _SAMPLE_INTERVAL)
		{
			_sampleAccumulator = Mathf.Min(_sampleAccumulator - _SAMPLE_INTERVAL, _SAMPLE_INTERVAL);
			if (_currentHeadGesture == HeadGesture.None)
			{
				bool nodded = detectAcceptNod();
				bool shook = detectRefuseShake();
				if (nodded)
				{
					setDetectedGesture(HeadGesture.AcceptNod);
				}
				else if (shook)
				{
					setDetectedGesture(HeadGesture.RefuseShake);
				}
			}
			else
			{
				Vector3 euler = _EyeRightTransform.localEulerAngles;
				_lastAngleX = euler.x;
				_lastAngleY = euler.y;
				_lastAngleZ = euler.z;
			}
		}

		if (Time.time > _timer)
		{
			_currentHeadGesture = HeadGesture.None;
		}
		if (Time.time > _timerQuick)
		{
			_gestureMoveCount = 0;
		}
	}

	private void detectGazeDirection()
	{
	}

	private void setDetectedGesture(HeadGesture aHeadGesture)
	{
		_currentHeadGesture = aHeadGesture;
	}

	private bool isAnswerGestureWithinLimits(float delta, float limit)
	{
		bool result = false;
		if (delta > limit || delta < 0f - limit)
		{
			_gestureMoveCount++;
			_timerQuick = Time.time + _GESTURE_RECOGNITION_LOSS;
			if (_gestureMoveCount > _GESTURE_MOVEMENT_REQUIRED)
			{
				_timer = Time.time + _DETECTION_DURATION;
				result = true;
			}
		}
		return result;
	}

	private bool isPointingGestureWithinLimits(float delta, float limit)
	{
		bool result = false;
		if (delta > limit || delta < 0f - limit)
		{
			_gesturePositive = delta > limit;
			_gestureMoveCount++;
			_timerQuick = Time.time + _GESTURE_RECOGNITION_LOSS;
			if (_gestureMoveCount > _GESTURE_POINT_REQUIRED)
			{
				_timer = Time.time + _DETECTION_DURATION;
				result = true;
			}
		}
		return result;
	}

	private bool detectAcceptNod()
	{
		float currentX = _EyeRightTransform.localEulerAngles.x;
		float delta = Mathf.DeltaAngle(_lastAngleX, currentX);
		_lastAngleX = currentX;
		return isAnswerGestureWithinLimits(delta, _DELTA_LIMIT_ACCEPTNOD);
	}

	private bool detectRefuseShake()
	{
		float currentY = _EyeRightTransform.localEulerAngles.y;
		float delta = Mathf.DeltaAngle(_lastAngleY, currentY);
		_lastAngleY = currentY;
		return isAnswerGestureWithinLimits(delta, _DELTA_LIMIT_REFUSESHAKE);
	}

	private bool detectSideToSide()
	{
		float currentZ = _EyeRightTransform.localEulerAngles.z;
		float delta = Mathf.DeltaAngle(_lastAngleZ, currentZ);
		_lastAngleZ = currentZ;
		return isPointingGestureWithinLimits(delta, _DELTA_LIMIT_HEADPOINTING);
	}

	public HeadGesture GetCurrentHeadGesture()
	{
		return _currentHeadGesture;
	}
}
