using UnityEngine;

public class PassengerSleeperAI : MonoBehaviour
{
	public Transform _FingerIndex;

	private float _rythmIndex;

	private Vector3 _initRotationIndex;

	public Transform _FingerMiddle;

	private float _rythmMiddle = 0.25f;

	private Vector3 _initRotationMiddle;

	public Transform _FingerRing;

	private float _rythmRing = 0.5f;

	private Vector3 _initRotationRing;

	public Transform _FingerPinky;

	private float _rythmPinky = 0.75f;

	private Vector3 _initRotationPinky;

	private float Speed = 5f;

	private float AngleLength = 15f;


	public bool _DoFinger;

	public Transform _Spine;

	public float _BreathingSpeed = 2f;

	public float _BreathingAmount = 2f;

	private Vector3 _SpineScale = Vector3.one;

	private void Start()
	{
		_initRotationIndex = _FingerIndex.localEulerAngles;
		_initRotationMiddle = _FingerMiddle.localEulerAngles;
		_initRotationRing = _FingerRing.localEulerAngles;
		_initRotationPinky = _FingerPinky.localEulerAngles;
	}

	private void Update()
	{
		if (_DoFinger)
		{
			_FingerIndex.localRotation = Quaternion.Euler(_initRotationIndex.x, _initRotationIndex.y, Mathf.Sin((Time.time + _rythmIndex) * Speed) * AngleLength + _initRotationIndex.z / 2f);
			_FingerMiddle.localRotation = Quaternion.Euler(_initRotationMiddle.x, _initRotationMiddle.y, Mathf.Sin((Time.time + _rythmMiddle) * Speed) * AngleLength + _initRotationMiddle.z / 2f);
			_FingerRing.localRotation = Quaternion.Euler(_initRotationRing.x, _initRotationRing.y, Mathf.Sin((Time.time + _rythmRing) * Speed) * AngleLength + _initRotationRing.z / 2f);
			_FingerPinky.localRotation = Quaternion.Euler(_initRotationPinky.x, _initRotationPinky.y, Mathf.Sin((Time.time + _rythmPinky) * Speed) * AngleLength + _initRotationPinky.z / 2f);
		}
		_SpineScale.z = Mathf.Sin(Time.time * _BreathingSpeed) / _BreathingAmount + 1f;
		_Spine.localScale = _SpineScale;
	}
}
