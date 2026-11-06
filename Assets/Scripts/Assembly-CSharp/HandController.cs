using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
	private enum FingersState
	{
		Relax = 0,
		Flat = 1,
		Grab = 2,
		HalfGrab = 3,
		Point = 4,
		ThrowReady = 5,
		ThrowRelease = 6
	}

	public enum HandState
	{
		idle = 0,
		grabPeanutsReady = 1,
		grabPeanutsStart = 2,
		grabPeanutsComplete = 3,
		throwReady = 4,
		throwStart = 5,
		throwRelease = 6,
		throwComplete = 7,
		pushBtnReady = 8,
		pushBtnStart = 9,
		pushBtnComplete = 10,
		hideHandcuffsReady = 11,
		hideHandcuffsStart = 12,
		hideHandcuffsComplete = 13,
		auxReady = 14,
		auxOpen = 15,
		auxComplete = 16,
		telescopeReady = 17,
		telescopePickup = 18,
		telescopeUse = 19,
		telescopeReturn = 20,
		SmokeReady = 21,
		SmokeThat = 22,
		pointScared = 23,
		returnToChair = 24
	}

	public HandState _activeHandState = HandState.returnToChair;

	public Transform _HandTarget;

	private Transform _CameraHandPosition;

	public Transform _HandPositionsRoot;

	private List<Transform> _HandPositions = new List<Transform>();

	private int _ActiveHandPositionID;

	private float _HandSmoothing = 2f;

	private float _FingerSmooth = 0.5f;

	public Transform _FingerIndex;

	private List<FingerSegment> _FingerIndexSegments = new List<FingerSegment>();

	public Transform _FingerMiddle;

	private List<FingerSegment> _FingerMiddleSegments = new List<FingerSegment>();

	public Transform _FingerRing;

	private List<FingerSegment> _FingerRingSegments = new List<FingerSegment>();

	public Transform _FingerPinky;

	private List<FingerSegment> _FingerPinkySegments = new List<FingerSegment>();

	public Transform _FingerThumb;

	private List<FingerSegment> _FingerThumbSegments = new List<FingerSegment>();

	private FingersState _ActiveFingersState = FingersState.HalfGrab;

	private bool _activeItween;

	private float _timer;

	private float _timerII;

	public GameObject _PeanutForThrowingPrefab;

	public Transform _HandRightTransform;

	private GameObject _currentPeanut;

	public GameObject _PeanutBag;

	public GameObject _ChairPosition;

	public GameObject _PeanutReadyPosition;

	public GameObject _PeanutGrabPosition;

	public GameObject _LightBtnPosition;

	public GameObject _OculusBtnPosition;

	public GameObject _FlatBtnPosition;

	public GameObject _FanBtnPosition;

	private GameObject _SelectedBtnObject;

	public GameObject _PoinToWalterWindowPosition;

	public GameObject _HandcuffsReady;

	public GameObject _HandcuffsHide;

	public GameObject _Handcuffs;

	public GameObject _HandcuffsHidden;

	public GameObject _AuxReadyPosition;

	public GameObject _AuxGrabPosition;

	public GameObject _AuxTurnPosition;

	public bool _AuxOpened;

	public GameObject _TelescopeReadyPosition;

	public GameObject _TelescopeGrabPosition;

	public GameObject _Telescope;

	private Transform _TelescopeParent;

	public bool _isHandMoving;

	private float _currentStateTimer;

	private Vector3 _currentStartingHandPostion = Vector3.zero;

	public Transform _SmokingHole;

	public string _CurrentBtnName;

	private float _fsmLoopFrequency = 0.01f;

	public void InitateCameraHandPosition(Transform aHandTransform)
	{
		_CameraHandPosition = aHandTransform;
	}

	private void Start()
	{
		applyFingerSegment(_FingerIndex, _FingerIndexSegments, new Vector3(0f, 25f, 0f));
		applyFingerSegment(_FingerMiddle, _FingerMiddleSegments, new Vector3(0f, 0f, 100f));
		applyFingerSegment(_FingerRing, _FingerRingSegments, new Vector3(0f, 0f, 100f));
		applyFingerSegment(_FingerPinky, _FingerPinkySegments, new Vector3(0f, -25f, 0f));
		applyFingerSegment(_FingerThumb, _FingerThumbSegments, new Vector3(0f, 0f, 0f));
		if (_HandPositionsRoot != null)
		{
			for (int i = 0; i < _HandPositionsRoot.childCount; i++)
			{
				_HandPositions.Add(_HandPositionsRoot.GetChild(i));
			}
		}
		if ((bool)_Telescope)
		{
			_TelescopeParent = _Telescope.transform.parent;
		}
		_activeHandState = HandState.returnToChair;
		StartCoroutine(FSM());
	}

	private void Update()
	{
		loopFingerMovements();
		_currentStateTimer += Time.deltaTime;
	}

	public HandState GetHandState()
	{
		return _activeHandState;
	}

	private void loopFingerMovements()
	{
		if (_ActiveFingersState == FingersState.Relax)
		{
			poseFinger(45f, 80f, 80f, 80f, new Vector2(0f, 30f));
			moveFingerRandomly(_FingerIndexSegments);
			moveFingerRandomly(_FingerMiddleSegments);
			moveFingerRandomly(_FingerRingSegments);
			moveFingerRandomly(_FingerPinkySegments);
		}
		else if (_ActiveFingersState == FingersState.HalfGrab)
		{
			poseFinger(35f, 35f, 35f, 35f, new Vector2(35f, 35f));
		}
		else if (_ActiveFingersState == FingersState.Grab)
		{
			poseFinger(80f, 80f, 80f, 80f, new Vector2(20f, 20f));
		}
		else if (_ActiveFingersState == FingersState.Point)
		{
			poseFinger(0f, 80f, 80f, 80f, new Vector2(0f, 45f));
		}
		else if (_ActiveFingersState == FingersState.Flat)
		{
			poseFinger(0f, 0f, 0f, 0f, new Vector2(0f, 0f));
		}
		else if (_ActiveFingersState == FingersState.ThrowReady)
		{
			poseFinger(45f, 80f, 80f, 80f, new Vector2(0f, 30f));
			moveFingerRandomly(_FingerMiddleSegments);
			moveFingerRandomly(_FingerRingSegments);
			moveFingerRandomly(_FingerPinkySegments);
		}
		else if (_ActiveFingersState == FingersState.ThrowRelease)
		{
			poseFinger(-7f, 30f, 20f, 10f, new Vector2(25f, 65f));
		}
	}

	private void createNewPeanut()
	{
		GameObject gameObject = (GameObject)Object.Instantiate(_PeanutForThrowingPrefab);
		gameObject.transform.parent = _HandRightTransform;
		gameObject.transform.localPosition = new Vector3(-9.334869f, -6.415769f, -4.893144f);
		gameObject.transform.localRotation = Quaternion.Euler(9.334869f, 178.141f, 2.958359f);
		gameObject.transform.localScale = new Vector3(5.140929f, 5.140929f, 5.140929f);
		_currentPeanut = gameObject;
	}

	private void setFingerState(FingersState aHandState, float aSpeed)
	{
		_FingerSmooth = aSpeed;
		_ActiveFingersState = aHandState;
	}

	private void poseFinger(float aIndexGrab, float aMiddleGrab, float aRingGrab, float aPinkyGrab, Vector2 aThumbGrab)
	{
		moveFingerSmoothPrecisely(_FingerIndexSegments, aIndexGrab);
		moveFingerSmoothPrecisely(_FingerMiddleSegments, aMiddleGrab);
		moveFingerSmoothPrecisely(_FingerRingSegments, aRingGrab);
		moveFingerSmoothPrecisely(_FingerPinkySegments, aPinkyGrab);
		moveFingerSmoothPrecisely2D(_FingerThumbSegments, aThumbGrab);
	}

	private void moveFingerRandomly(List<FingerSegment> aSegmentList)
	{
		if (aSegmentList == null)
		{
			return;
		}
		foreach (FingerSegment aSegment in aSegmentList)
		{
			if (aSegment != null && aSegment._Transform != null)
			{
				aSegment._GrabAmount = (Mathf.Sin((Time.time + aSegment._RandomNumber) * 1.5f) / 2f + 1f) * 20f;
				Quaternion to = Quaternion.Euler(new Vector3(aSegment._rotationOffset.x, aSegment._rotationOffset.y, aSegment._rotationOffset.z + aSegment._GrabAmount));
				aSegment._Transform.localRotation = Quaternion.Lerp(aSegment._Transform.localRotation, to, _FingerSmooth);
			}
		}
	}

	private void moveFingerSmoothPrecisely(List<FingerSegment> aSegmentList, float aGrabAmount)
	{
		if (aSegmentList == null)
		{
			return;
		}
		foreach (FingerSegment aSegment in aSegmentList)
		{
			if (aSegment != null && aSegment._Transform != null)
			{
				aSegment._GrabAmount = aGrabAmount;
				Quaternion to = Quaternion.Euler(new Vector3(aSegment._rotationOffset.x, aSegment._rotationOffset.y, aSegment._rotationOffset.z + aSegment._GrabAmount));
				aSegment._Transform.localRotation = Quaternion.Lerp(aSegment._Transform.localRotation, to, _FingerSmooth);
			}
		}
	}

	private void moveFingerSmoothPrecisely2D(List<FingerSegment> aSegmentList, Vector2 aGrabAmount)
	{
		if (aSegmentList == null)
		{
			return;
		}
		foreach (FingerSegment aSegment in aSegmentList)
		{
			if (aSegment != null && aSegment._Transform != null)
			{
				Quaternion to = Quaternion.Euler(new Vector3(aSegment._rotationOffset.x, aSegment._rotationOffset.y + aGrabAmount.y, aSegment._rotationOffset.z + aGrabAmount.x));
				aSegment._Transform.localRotation = Quaternion.Lerp(aSegment._Transform.localRotation, to, _FingerSmooth);
			}
		}
	}

	private void rotateHandTo(Quaternion aGaol, float aSmooth = 3.1f)
	{
		if (_HandTarget != null)
		{
			_HandTarget.rotation = Quaternion.Slerp(_HandTarget.rotation, aGaol, aSmooth * Time.deltaTime);
		}
	}

	private void moveHandTo(Transform aTGoal, bool precise = false, float smoothing = 3.1f)
	{
		if (_HandTarget == null || aTGoal == null)
		{
			return;
		}
		Vector3 position = aTGoal.position;
		float t = smoothing * Time.deltaTime;
		_HandTarget.position = Vector3.Lerp(_HandTarget.position, position, t);
		_isHandMoving = true;
		int digits = 3;
		if (RoundTo(_HandTarget.position.x, digits) == RoundTo(position.x, digits) && RoundTo(_HandTarget.position.y, digits) == RoundTo(position.y, digits) && RoundTo(_HandTarget.position.z, digits) == RoundTo(position.z, digits))
		{
			_HandTarget.position = position;
			_isHandMoving = false;
		}
		if (!precise)
		{
			digits = 2;
			if (RoundTo(_HandTarget.position.x, digits) == RoundTo(position.x, digits) && RoundTo(_HandTarget.position.y, digits) == RoundTo(position.y, digits) && RoundTo(_HandTarget.position.z, digits) == RoundTo(position.z, digits))
			{
				_isHandMoving = false;
			}
		}
	}

	private static float RoundTo(float value, int digits)
	{
		float num = Mathf.Pow(10f, digits);
		return Mathf.Round(value * num) / num;
	}

	private void applyFingerSegment(Transform aTransform, List<FingerSegment> aSegmentList, Vector3 aOffset)
	{
		if (aTransform == null || aSegmentList == null)
		{
			return;
		}
		FingerSegment fingerSegment = new FingerSegment();
		fingerSegment._Transform = aTransform;
		fingerSegment._rotationOffset = aOffset;
		fingerSegment._RandomNumber = Random.Range(0, 10);
		aSegmentList.Add(fingerSegment);
		if (aTransform.childCount > 0)
		{
			Transform child0 = aTransform.GetChild(0);
			FingerSegment fingerSegment2 = new FingerSegment();
			fingerSegment2._Transform = child0;
			aSegmentList.Add(fingerSegment2);
			if (aTransform.name != "RightHandThumb2" && child0.childCount > 0)
			{
				FingerSegment fingerSegment3 = new FingerSegment();
				fingerSegment3._Transform = child0.GetChild(0);
				aSegmentList.Add(fingerSegment3);
			}
		}
	}

	public void setHandState(HandState aHandState)
	{
		_activeHandState = aHandState;
		if (_HandTarget != null)
		{
			_currentStartingHandPostion = _HandTarget.position;
		}
		_currentStateTimer = 0f;
	}

	public void SetBtnType(string aBtnType)
	{
		_CurrentBtnName = aBtnType;
		switch (aBtnType)
		{
		case "Oculus":
			_SelectedBtnObject = _OculusBtnPosition;
			break;
		case "Light":
			_SelectedBtnObject = _LightBtnPosition;
			break;
		case "Flat":
			_SelectedBtnObject = _FlatBtnPosition;
			break;
		case "Fan":
			_SelectedBtnObject = _FanBtnPosition;
			break;
		}
	}

	private IEnumerator idle()
	{
		yield return null;
		while (_activeHandState == HandState.idle)
		{
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator auxReady()
	{
		setFingerState(FingersState.Relax, 0.1f);
		yield return null;
		while (_activeHandState == HandState.auxReady)
		{
			if (_AuxReadyPosition != null)
			{
				moveHandTo(_AuxReadyPosition.transform);
				rotateHandTo(_AuxReadyPosition.transform.rotation);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator telescopeReady()
	{
		setFingerState(FingersState.Relax, 0.1f);
		yield return null;
		while (_activeHandState == HandState.telescopeReady)
		{
			if (_TelescopeReadyPosition != null)
			{
				moveHandTo(_TelescopeReadyPosition.transform);
				rotateHandTo(_TelescopeReadyPosition.transform.rotation);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator telescopePickup()
	{
		setFingerState(FingersState.HalfGrab, 0.1f);
		float timer = Time.time + 1.5f;
		yield return null;
		while (_activeHandState == HandState.telescopePickup)
		{
			if (_TelescopeGrabPosition != null)
			{
				moveHandTo(_TelescopeGrabPosition.transform, true);
				rotateHandTo(_TelescopeGrabPosition.transform.rotation);
			}
			if (!_isHandMoving)
			{
				setHandState(HandState.telescopeUse);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator telescopeUse()
	{
		setFingerState(FingersState.HalfGrab, 0.1f);
		Vector3 initLocalPos = Vector3.zero;
		if (_CameraHandPosition != null)
		{
			initLocalPos = _CameraHandPosition.localPosition;
			Vector3 newLocal = _CameraHandPosition.localPosition;
			newLocal.z = 0f;
			newLocal.x = 0.1f;
			newLocal.y = 0.1f;
			_CameraHandPosition.localPosition = newLocal;
		}
		if (_Telescope != null && _HandRightTransform != null)
		{
			_Telescope.transform.parent = _HandRightTransform;
		}
		yield return null;
		while (_activeHandState == HandState.telescopeUse)
		{
			if (_Telescope != null && _Telescope.activeSelf)
			{
				if (_HandTarget != null)
				{
					_HandTarget.localRotation = Quaternion.Lerp(_HandTarget.localRotation, Quaternion.Euler(0f, -60f, 0f), _HandSmoothing * Time.deltaTime);
					if (_CameraHandPosition != null)
					{
						_HandTarget.position = Vector3.Lerp(_HandTarget.position, _CameraHandPosition.position, _HandSmoothing * Time.deltaTime);
					}
				}
			}
			else if (_HandTarget != null)
			{
				_HandTarget.localPosition = new Vector3(0f, 0f, -100f);
			}
			yield return AsunderCoroutines.FsmWait;
		}
		if (_CameraHandPosition != null)
		{
			_CameraHandPosition.localPosition = initLocalPos;
		}
	}

	private IEnumerator telescopeReturn()
	{
		setFingerState(FingersState.HalfGrab, 0.1f);
		float timer = Time.time + 1000.5f;
		if (_HandTarget != null)
		{
			_HandTarget.localRotation = Quaternion.Euler(0f, 0f, 0f);
			if (_CameraHandPosition != null)
			{
				_HandTarget.position = _CameraHandPosition.position;
			}
		}
		timer = Time.time + 2.5f;
		yield return null;
		while (_activeHandState == HandState.telescopeReturn)
		{
			if (_TelescopeGrabPosition != null)
			{
				moveHandTo(_TelescopeGrabPosition.transform, true);
				rotateHandTo(_TelescopeGrabPosition.transform.rotation);
			}
			if (!_isHandMoving)
			{
				if (_Telescope != null)
				{
					_Telescope.transform.parent = _TelescopeParent;
				}
				setHandState(HandState.returnToChair);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator auxOpen()
	{
		setFingerState(FingersState.HalfGrab, 0.1f);
		_timer = Time.time + 1.2f;
		yield return null;
		while (_activeHandState == HandState.auxOpen)
		{
			if (_AuxGrabPosition != null)
			{
				moveHandTo(_AuxGrabPosition.transform);
				if (_HandTarget != null)
				{
					_HandTarget.rotation = Quaternion.Lerp(_HandTarget.rotation, _AuxGrabPosition.transform.rotation, _HandSmoothing * Time.deltaTime);
				}
			}
			if (Time.time > _timer)
			{
				setHandState(HandState.auxComplete);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator auxComplete()
	{
		setFingerState(FingersState.Grab, 0.1f);
		float duration = 0.95f;
		_timer = Time.time + duration;
		yield return null;
		while (_activeHandState == HandState.auxComplete)
		{
			if (_HandTarget != null && _AuxTurnPosition != null)
			{
				_HandTarget.rotation = Quaternion.Lerp(_HandTarget.rotation, _AuxTurnPosition.transform.rotation, _HandSmoothing * Time.deltaTime);
			}
			if (Time.time > _timer)
			{
				_AuxOpened = true;
				setHandState(HandState.returnToChair);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator hideHandcuffsReady()
	{
		setFingerState(FingersState.Relax, 0.1f);
		yield return null;
		while (_activeHandState == HandState.hideHandcuffsReady)
		{
			if (_HandcuffsReady != null)
			{
				moveHandTo(_HandcuffsReady.transform);
				if (_HandTarget != null)
				{
					_HandTarget.rotation = Quaternion.Lerp(_HandTarget.rotation, _HandcuffsReady.transform.rotation, _HandSmoothing * Time.deltaTime);
				}
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator hideHandcuffsStart()
	{
		setFingerState(FingersState.HalfGrab, 0.1f);
		_timer = Time.time + 1.2f;
		yield return null;
		while (_activeHandState == HandState.hideHandcuffsStart)
		{
			if (_HandcuffsHide != null)
			{
				moveHandTo(_HandcuffsHide.transform);
				if (_HandTarget != null)
				{
					_HandTarget.rotation = Quaternion.Lerp(_HandTarget.rotation, _HandcuffsHide.transform.rotation, _HandSmoothing * Time.deltaTime);
				}
			}
			if (Time.time > _timer)
			{
				setHandState(HandState.hideHandcuffsComplete);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator hideHandcuffsComplete()
	{
		float duration = 0.65f;
		if (_HandTarget != null)
		{
			iTween.ShakeRotation(_HandTarget.gameObject, iTween.Hash("y", 2, "time", duration));
		}
		VisuallyHideHandcuffs();
		if (_HandcuffsHidden != null)
		{
			AudioSource audio = _HandcuffsHidden.GetComponent<AudioSource>();
			if (audio != null)
			{
				audio.Play();
			}
		}
		_timer = Time.time + duration;
		yield return null;
		while (_activeHandState == HandState.hideHandcuffsComplete)
		{
			if (Time.time > _timer)
			{
				setHandState(HandState.returnToChair);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	public void VisuallyHideHandcuffs()
	{
		if (_Handcuffs != null)
		{
			_Handcuffs.SetActive(false);
		}
		if (_HandcuffsHidden != null)
		{
			_HandcuffsHidden.SetActive(true);
		}
	}

	private IEnumerator pushBtnReady()
	{
		setFingerState(FingersState.Point, 0.1f);
		yield return null;
		while (_activeHandState == HandState.pushBtnReady)
		{
			if (_SelectedBtnObject != null)
			{
				moveHandTo(_SelectedBtnObject.transform);
				if (_HandTarget != null)
				{
					_HandTarget.rotation = Quaternion.Lerp(_HandTarget.rotation, _SelectedBtnObject.transform.rotation, 5f * Time.deltaTime);
				}
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator pushBtnStart()
	{
		_timer = Time.time + 1f;
		yield return null;
		while (_activeHandState == HandState.pushBtnStart)
		{
			if (_SelectedBtnObject != null && _HandTarget != null)
			{
				Vector3 newPushPosition = _SelectedBtnObject.transform.position;
				newPushPosition.y += 0.01f;
				_HandTarget.position = Vector3.Lerp(_HandTarget.position, newPushPosition, 3f * Time.deltaTime);
				_HandTarget.rotation = Quaternion.Lerp(_HandTarget.rotation, _SelectedBtnObject.transform.rotation, 3f * Time.deltaTime);
			}
			if (Time.time > _timer)
			{
				setHandState(HandState.pushBtnReady);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator grabPeanutsReady()
	{
		setFingerState(FingersState.Relax, 0.1f);
		yield return null;
		while (_activeHandState == HandState.grabPeanutsReady)
		{
			if (_PeanutReadyPosition != null)
			{
				moveHandTo(_PeanutReadyPosition.transform);
			}
			if (_HandTarget != null && _CameraHandPosition != null)
			{
				_HandTarget.rotation = Quaternion.Lerp(_HandTarget.rotation, _CameraHandPosition.rotation, _HandSmoothing * Time.deltaTime);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator grabPeanutsStart()
	{
		float duration = 1f;
		setFingerState(FingersState.Grab, 0.05f);
		_timer = Time.time + duration;
		yield return null;
		while (_activeHandState == HandState.grabPeanutsStart)
		{
			if (_PeanutGrabPosition != null)
			{
				moveHandTo(_PeanutGrabPosition.transform);
				if (_HandTarget != null)
				{
					_HandTarget.rotation = Quaternion.Lerp(_HandTarget.rotation, _PeanutGrabPosition.transform.rotation, _HandSmoothing * Time.deltaTime);
				}
			}
			if (Time.time > _timer)
			{
				setHandState(HandState.grabPeanutsComplete);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator grabPeanutsComplete()
	{
		if (_currentPeanut == null)
		{
			createNewPeanut();
		}
		float duration = 0.65f;
		if (_PeanutBag != null)
		{
			iTween.ShakeRotation(_PeanutBag.gameObject, iTween.Hash("y", 5, "time", duration));
		}
		if (_HandTarget != null)
		{
			iTween.ShakeRotation(_HandTarget.gameObject, iTween.Hash("y", 2, "time", duration));
		}
		setFingerState(FingersState.ThrowReady, 0.1f);
		_timer = Time.time + duration;
		yield return null;
		while (_activeHandState == HandState.grabPeanutsComplete)
		{
			if (Time.time > _timer)
			{
				setHandState(HandState.throwReady);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator throwReady()
	{
		yield return null;
		while (_activeHandState == HandState.throwReady)
		{
			if (_HandTarget != null && _CameraHandPosition != null)
			{
				_HandTarget.rotation = Quaternion.Lerp(_HandTarget.rotation, _CameraHandPosition.rotation, _HandSmoothing * Time.deltaTime);
				_HandTarget.position = Vector3.Lerp(_HandTarget.position, _CameraHandPosition.position, _HandSmoothing * Time.deltaTime);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator SmokeReady()
	{
		float speedSmooth = 0.15f;
		setFingerState(FingersState.ThrowReady, 0.1f);
		GameObject _refCamHandPos = null;
		Vector3 position = Vector3.zero;
		if (!_CameraHandPosition)
		{
			_refCamHandPos = GameObject.Find("+ HandPosition");
		}
		yield return null;
		while (_activeHandState == HandState.SmokeReady)
		{
			Transform camHand = _refCamHandPos != null ? _refCamHandPos.transform : _CameraHandPosition;
			if (camHand != null && _HandTarget != null)
			{
				position = camHand.position;
				_HandTarget.rotation = Quaternion.Lerp(_HandTarget.rotation, camHand.rotation, speedSmooth);
				position.y -= 0.1f;
				_HandTarget.position = Vector3.Lerp(_HandTarget.position, position, speedSmooth);
				Vector3 newLocalPosition = _HandTarget.localPosition;
				newLocalPosition.z -= 0.1f;
				_HandTarget.localPosition = Vector3.Lerp(_HandTarget.localPosition, newLocalPosition, speedSmooth);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator SmokeThat()
	{
		float speedSmooth = 0.075f;
		setFingerState(FingersState.ThrowReady, 0.1f);
		GameObject _refCamHandPos = null;
		Vector3 position = Vector3.zero;
		if (!_CameraHandPosition)
		{
			_refCamHandPos = GameObject.Find("+ HandPosition");
		}
		yield return null;
		while (_activeHandState == HandState.SmokeThat)
		{
			if (_HandTarget != null)
			{
				if ((bool)_refCamHandPos)
				{
					position = _refCamHandPos.transform.position;
					_HandTarget.localRotation = Quaternion.Lerp(_HandTarget.localRotation, Quaternion.Euler(80f, 65f, 0f), speedSmooth);
				}
				else if (_CameraHandPosition != null)
				{
					position = _CameraHandPosition.position;
					_HandTarget.rotation = Quaternion.Lerp(_HandTarget.rotation, _CameraHandPosition.rotation, speedSmooth);
				}
				if ((bool)_SmokingHole)
				{
					_HandTarget.position = Vector3.Lerp(_HandTarget.position, _SmokingHole.position, speedSmooth);
				}
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator throwStart()
	{
		float duration = 0.3f;
		_timer = Time.time + duration;
		if (_HandTarget != null)
		{
			iTween.RotateTo(_HandTarget.gameObject, iTween.Hash("z", -70, "time", duration, "islocal", true, "easetype", iTween.EaseType.easeOutQuad));
			iTween.MoveBy(_HandTarget.gameObject, iTween.Hash("y", -0.025, "time", duration, "islocal", true, "easetype", iTween.EaseType.easeOutQuad));
		}
		yield return null;
		while (_activeHandState == HandState.throwStart)
		{
			if (Time.time > _timer)
			{
				setHandState(HandState.throwRelease);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	public void DeleteCurrentPeanut()
	{
		if (_currentPeanut != null)
		{
			Object.Destroy(_currentPeanut);
			_currentPeanut = null;
		}
	}

	private IEnumerator throwRelease()
	{
		float duration = 0.3f;
		_timer = Time.time + duration;
		if (_HandTarget != null)
		{
			iTween.RotateTo(_HandTarget.gameObject, iTween.Hash("z", -10, "time", duration, "islocal", true, "easetype", iTween.EaseType.easeInExpo));
		}
		_timerII = Time.time + 0.2f;
		float tinyTimer = Time.time + 0.25f;
		yield return null;
		while (_activeHandState == HandState.throwRelease)
		{
			if (Time.time > _timerII)
			{
				setFingerState(FingersState.ThrowRelease, 0.5f);
			}
			if (Time.time > tinyTimer)
			{
				Object.Destroy(_currentPeanut);
				_currentPeanut = null;
			}
			if (Time.time > _timer)
			{
				setHandState(HandState.throwComplete);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator throwComplete()
	{
		_timer = Time.time + 1f;
		setFingerState(FingersState.Relax, 0.1f);
		yield return null;
		while (_activeHandState == HandState.throwComplete)
		{
			if (Time.time > _timer)
			{
				setHandState(HandState.returnToChair);
			}
			if (_HandTarget != null && _CameraHandPosition != null)
			{
				_HandTarget.position = Vector3.Lerp(_HandTarget.position, _CameraHandPosition.position, 0.01f);
				_HandTarget.rotation = Quaternion.Lerp(_HandTarget.rotation, _CameraHandPosition.rotation, _HandSmoothing * Time.deltaTime);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator pointScared()
	{
		setFingerState(FingersState.Point, 0.1f);
		yield return null;
		while (_activeHandState == HandState.pointScared)
		{
			if (_HandTarget != null && _PoinToWalterWindowPosition != null)
			{
				_HandTarget.rotation = Quaternion.Lerp(_HandTarget.rotation, _PoinToWalterWindowPosition.transform.rotation, _HandSmoothing * Time.deltaTime);
				_HandTarget.position = Vector3.Lerp(_HandTarget.position, _PoinToWalterWindowPosition.transform.position, 1f * Time.deltaTime);
				Vector3 newLocal = _HandTarget.localRotation.eulerAngles;
				newLocal.x += Mathf.Sin(Time.time * 35f) * 1.5f;
				newLocal.z += Mathf.Sin(Time.time * 31f) * 1.5f;
				_HandTarget.localRotation = Quaternion.Euler(newLocal);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator returnToChair()
	{
		float duration = 5f;
		setFingerState(FingersState.HalfGrab, 0.05f);
		_timer = Time.time + duration;
		yield return null;
		while (_activeHandState == HandState.returnToChair)
		{
			if (_ChairPosition != null)
			{
				moveHandTo(_ChairPosition.transform);
				rotateHandTo(_ChairPosition.transform.rotation);
			}
			if (Time.time > _timer)
			{
				setHandState(HandState.idle);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator FSM()
	{
		while (true)
		{
			yield return StartCoroutine(_activeHandState.ToString());
		}
	}
}
