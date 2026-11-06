using System.Collections;
using UnityEngine;

public class EdController : MonoBehaviour
{
	public enum EdState
	{
		idle = 0,
		moveBack = 1,
		moveForward = 2,
		peek = 3,
		hide = 4,
		raseGun = 5,
		aim = 6,
		fireMiss = 7,
		fireSecondMiss = 8,
		fireKill = 9
	}

	public EdState _ActiveEdState;

	private EdState _lastActiveState;

	public GameObject _EdSeat;

	public Transform _EdSeatPivot;

	public Transform _EdSpine;

	public GameObject _Revolver;

	private Vector3 _initRevolverPosition;

	public GameObject _BloodSplatters;

	public GameObject _GunParticles1;

	public GameObject _GunParticles2;

	public GameObject _GunParticles3;

	public GameObject _BulletHoleOne;

	public GameObject _BulletHoleTwo;

	public GameObject _ReverbZone;

	public GameObject _ReverbZone2;

	public GameObject _ReverbZone3;

	public GameObject _GunSmokePSys;

	private HeadLookController _edsHeadLookController;

	private FacialController _edsFacialController;

	private NoiseController _edsNoiseController;

	private Vector3 _initEdSpineLocalPosition;

	private Quaternion _initEdSeatLocalRotation;

	private float _seatAngle;

	private Vector3 initPosition = Vector3.zero;

	private Transform mainCameraTransform;

	private float _seatAngleConst = 0.38f;

	private float _fsmLoopFrequency = 0.01f;

	public void initateMainCameraTransform(Transform aTransform)
	{
		mainCameraTransform = aTransform;
	}

	private void SafeSetActive(GameObject go, bool active)
	{
		if (go != null)
		{
			go.SetActive(active);
		}
	}

	private void SafePlayAudio(GameObject go)
	{
		if (go != null)
		{
			AudioSource audio = go.GetComponent<AudioSource>();
			if (audio != null)
			{
				audio.Play();
			}
		}
	}

	private void SafeSetNoiseSegment(int segmentIndex, bool enabled, float scaleX = -1f)
	{
		if (_edsNoiseController != null && _edsNoiseController._Segments != null && segmentIndex >= 0 && segmentIndex < _edsNoiseController._Segments.Length && _edsNoiseController._Segments[segmentIndex] != null)
		{
			_edsNoiseController._Segments[segmentIndex]._Enabled = enabled;
			if (scaleX >= 0f)
			{
				_edsNoiseController._Segments[segmentIndex]._Scale.x = Mathf.Lerp(_edsNoiseController._Segments[segmentIndex]._Scale.x, scaleX, 0.1f);
			}
		}
	}

	private void Start()
	{
		if (_Revolver != null)
		{
			_initRevolverPosition = _Revolver.transform.localPosition;
		}
		_edsFacialController = base.gameObject.GetComponent<FacialController>();
		_edsHeadLookController = base.gameObject.GetComponent<HeadLookController>();
		_edsNoiseController = base.gameObject.GetComponent<NoiseController>();
		if (_EdSpine != null)
		{
			_initEdSpineLocalPosition = _EdSpine.localPosition;
		}
		if (_EdSeat != null)
		{
			_initEdSeatLocalRotation = _EdSeat.transform.localRotation;
		}
		StartCoroutine(FSM());
	}

	private void Update()
	{
	}

	public void SetState(EdState aState)
	{
		_lastActiveState = _ActiveEdState;
		_ActiveEdState = aState;
	}

	private void tweenOnUpdateCallBack(float aFloat)
	{
		_seatAngle = aFloat;
	}

	private IEnumerator idle()
	{
		yield return null;
		while (_ActiveEdState == EdState.idle)
		{
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator moveBack()
	{
		SafeSetNoiseSegment(1, true);
		_seatAngle = _seatAngleConst;
		float duration = 3f;
		float timer = Time.time + duration;
		if (_EdSeat != null)
		{
			initPosition = _EdSeat.transform.localPosition;
		}
		yield return null;
		while (_ActiveEdState == EdState.moveBack)
		{
			if (_EdSeat != null)
			{
				_EdSeat.transform.localRotation = Quaternion.Lerp(_EdSeat.transform.localRotation, Quaternion.Euler(-18f, 0f, 0f), 0.1f);
				_EdSeat.transform.localPosition = Vector3.Lerp(_EdSeat.transform.localPosition, new Vector3(initPosition.x, initPosition.y - 0.1f, initPosition.z - 0.1f), 0.1f);
			}
			if (Time.time > timer)
			{
				SetState(EdState.idle);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator moveForward()
	{
		SafeSetNoiseSegment(1, false);
		_seatAngle = _seatAngleConst;
		float duration = 3f;
		float timer = Time.time + duration;
		yield return null;
		while (_ActiveEdState == EdState.moveForward)
		{
			if (_EdSeat != null)
			{
				_EdSeat.transform.localRotation = Quaternion.Lerp(_EdSeat.transform.localRotation, Quaternion.Euler(0f, 0f, 0f), 0.1f);
				_EdSeat.transform.localPosition = Vector3.Lerp(_EdSeat.transform.localPosition, initPosition, 0.1f);
			}
			if (Time.time > timer)
			{
				SetState(EdState.idle);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator peek()
	{
		SafeSetNoiseSegment(1, false);
		float timer = Time.time + 3.15f;
		float timerII = timer + 1.75f;
		Vector3 spinePeekPosition = _initEdSpineLocalPosition;
		spinePeekPosition.x -= 0.1f;
		HeadLookController hlc = GetComponent<HeadLookController>();
		if (hlc != null)
		{
			hlc.enabled = true;
		}
		yield return null;
		while (_ActiveEdState == EdState.peek)
		{
			if (_EdSpine != null)
			{
				_EdSpine.localPosition = Vector3.Lerp(_EdSpine.localPosition, spinePeekPosition, 0.035f);
			}
			if (_edsHeadLookController != null && _edsHeadLookController.segments != null && _edsHeadLookController.segments.Length > 0 && _edsHeadLookController.segments[0].target != null && _BulletHoleTwo != null)
			{
				_edsHeadLookController.segments[0].target.position = _BulletHoleTwo.transform.position;
			}
			if (Time.time > timer)
			{
				SafeSetNoiseSegment(0, true, 6f);
				if (_edsFacialController != null)
				{
					_edsFacialController.SetExpression(1f, 1f, 1f, 0.1f, 0f);
				}
			}
			if (Time.time > timerII)
			{
				SetState(EdState.hide);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator hide()
	{
		Vector3 spinePeekPosition = _initEdSpineLocalPosition;
		spinePeekPosition.x += 0.25f;
		float timer = Time.time + 2f;
		yield return null;
		while (_ActiveEdState == EdState.hide)
		{
			if (_EdSpine != null)
			{
				_EdSpine.localPosition = Vector3.Lerp(_EdSpine.localPosition, spinePeekPosition, 0.11f);
			}
			if (Time.time > timer)
			{
				SetState(EdState.idle);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator raseGun()
	{
		SafeSetActive(_Revolver, true);
		if (_Revolver != null)
		{
			_Revolver.transform.localPosition = new Vector3(_Revolver.transform.localPosition.x, _Revolver.transform.localPosition.y - 0.15f, _Revolver.transform.localPosition.z);
		}
		float timer = Time.time + 3f;
		Vector3 spinePeekPosition = _initEdSpineLocalPosition;
		spinePeekPosition.x += 0.25f;
		yield return null;
		while (_ActiveEdState == EdState.raseGun)
		{
			if (_EdSpine != null)
			{
				_EdSpine.localPosition = Vector3.Lerp(_EdSpine.localPosition, spinePeekPosition, 0.11f);
			}
			if (_Revolver != null)
			{
				_Revolver.transform.localPosition = Vector3.Lerp(_Revolver.transform.localPosition, _initRevolverPosition, 0.075f);
			}
			if (Time.time > timer)
			{
				SetState(EdState.aim);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator aim()
	{
		float timer = Time.time + 1.1f;
		yield return null;
		while (_ActiveEdState == EdState.aim)
		{
			if (mainCameraTransform == null)
			{
				Camera mainCam = Camera.main != null ? Camera.main : (Camera.allCamerasCount > 0 ? Camera.allCameras[0] : null);
				if (mainCam != null)
				{
					mainCameraTransform = mainCam.transform;
				}
			}
			if (_Revolver != null && mainCameraTransform != null)
			{
				HeadLookController revolverHlc = _Revolver.GetComponent<HeadLookController>();
				if (revolverHlc != null && revolverHlc.segments != null && revolverHlc.segments.Length > 0 && revolverHlc.segments[0].target != null)
				{
					revolverHlc.segments[0].target.position = new Vector3(mainCameraTransform.position.x, mainCameraTransform.position.y - 0.09f, mainCameraTransform.position.z);
				}
			}
			if (Time.time > timer)
			{
				if (_lastActiveState == EdState.raseGun)
				{
					SetState(EdState.fireMiss);
				}
				else if (_lastActiveState == EdState.fireMiss)
				{
					SetState(EdState.fireSecondMiss);
				}
				else if (_lastActiveState == EdState.fireSecondMiss)
				{
					SetState(EdState.fireKill);
				}
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator fireMiss()
	{
		SafeSetActive(_ReverbZone, false);
		SafeSetActive(_ReverbZone2, true);
		SafePlayAudio(_Revolver);
		AsunderInput.TriggerHapticBoth(0.85f, 0.15f);
		SafeSetActive(_GunParticles1, true);
		SafeSetActive(_BulletHoleOne, true);
		float timer = Time.time + 0.8f;
		yield return null;
		while (_ActiveEdState == EdState.fireMiss)
		{
			if (Time.time > timer)
			{
				SetState(EdState.aim);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator fireSecondMiss()
	{
		SafePlayAudio(_Revolver);
		AsunderInput.TriggerHapticBoth(0.9f, 0.18f);
		SafeSetActive(_GunParticles2, true);
		SafeSetActive(_BulletHoleOne, true);
		float timer = Time.time + 0.5f;
		yield return null;
		while (_ActiveEdState == EdState.fireSecondMiss)
		{
			if (Time.time > timer)
			{
				SetState(EdState.aim);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator fireKill()
	{
		SafeSetActive(_ReverbZone2, false);
		SafeSetActive(_ReverbZone3, true);
		SafePlayAudio(_Revolver);
		AsunderInput.TriggerHapticBoth(1.0f, 0.25f);
		SafeSetActive(_GunParticles3, true);
		SafeSetActive(_BloodSplatters, true);
		yield return null;
		while (_ActiveEdState == EdState.fireKill)
		{
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator FSM()
	{
		while (true)
		{
			yield return StartCoroutine(_ActiveEdState.ToString());
		}
	}
}
