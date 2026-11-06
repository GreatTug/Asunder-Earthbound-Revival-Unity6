using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public enum PlayerState
	{
		idle = 0,
		nonActive = 1,
		lookForInteractions = 2,
		decideInteraction = 3,
		readyObject = 4,
		interactWith = 5,
		talk = 6,
		limitInteractions = 7,
		wait = 8,
		smoking = 9
	}

	public enum InteractionType
	{
		TalkToWalter = 0,
		PeanutThrowing = 1,
		SeatKicking = 2,
		LightSwitching = 3,
		HandcuffHiding = 4,
		Pointing = 5,
		OpenHatch = 6
	}

	public GameObject _ActiveInteractionObject;

	private GameObject _ReadyInteractionObject;

	public GameObject _PlayerRoot;

	private GameObject _ActiveCamera;

	public PlayerState _ActivePlayerState;

	public HandController _HandController;

	public LegController _LegController;

	public AudioSource _PlayerVoiceSource;

	private PlayerState _lastPlayerState;

	private float _timer;

	public OverheadController _PlayerOverheadController;

	public InteractiveObject[] _InteractiveObjects;

	private float DEFAULT_LIMIT_DELAY = 3f;

	public OVRCameraController _OVRCameraController;

	private float _InitOculusFoV = 111f;

	public GameObject _EyeTelescope;

	private bool _hasStartedUsingTelescope;

	private StoryManager _storyManager;

	private bool _hasUsedTelescope;

	private float _fsmLoopFrequency = 0.01f;

	public void InitateCorrectCameraGameObject(GameObject aCamera)
	{
		_ActiveCamera = aCamera;
	}

	private void Start()
	{
		if ((bool)GameObject.Find("* Manager"))
		{
			_storyManager = GameObject.Find("* Manager").GetComponent<StoryManager>();
		}
		InteractiveObject[] interactiveObjects = _InteractiveObjects;
		foreach (InteractiveObject interactiveObject in interactiveObjects)
		{
			if (interactiveObject._enabled)
			{
				interactiveObject.SetInteractive(true);
			}
			else
			{
				interactiveObject.SetInteractive(false);
			}
		}
		setState(PlayerState.lookForInteractions);
		StartCoroutine(FSM());
	}

	private void Update()
	{
	}

	public void PlayVoice()
	{
		_PlayerVoiceSource.Play();
	}

	public bool IsInteractionActive(string anIO)
	{
		InteractiveObject[] interactiveObjects = _InteractiveObjects;
		foreach (InteractiveObject interactiveObject in interactiveObjects)
		{
			if (interactiveObject._InteractiveObject.name == anIO)
			{
				return interactiveObject._enabled;
			}
		}
		return false;
	}

	public void UnlockInteraction(string anIO)
	{
		InteractiveObject[] interactiveObjects = _InteractiveObjects;
		foreach (InteractiveObject interactiveObject in interactiveObjects)
		{
			if (interactiveObject._InteractiveObject.name == anIO)
			{
				interactiveObject.SetInteractive(true);
				break;
			}
		}
	}

	public void LockInteraction(string anIO)
	{
		InteractiveObject[] interactiveObjects = _InteractiveObjects;
		foreach (InteractiveObject interactiveObject in interactiveObjects)
		{
			if (interactiveObject._InteractiveObject.name == anIO)
			{
				interactiveObject.SetInteractive(false);
				break;
			}
		}
	}

	public bool isInteractionDone(string anIO)
	{
		bool result = false;
		InteractiveObject[] interactiveObjects = _InteractiveObjects;
		foreach (InteractiveObject interactiveObject in interactiveObjects)
		{
			if (interactiveObject._InteractiveObject.name == anIO && !interactiveObject._enabled)
			{
				result = true;
			}
		}
		return result;
	}

	public GameObject GetActiveInteractionObject()
	{
		return _ActiveInteractionObject;
	}

	public GameObject GetPlayerRoot()
	{
		return _PlayerRoot;
	}

	private Transform GetActiveCameraTransform()
	{
		if (_ActiveCamera != null)
		{
			return _ActiveCamera.transform;
		}
		if (Camera.main != null)
		{
			_ActiveCamera = Camera.main.gameObject;
			return Camera.main.transform;
		}
		if (Camera.allCamerasCount > 0 && Camera.allCameras[0] != null)
		{
			_ActiveCamera = Camera.allCameras[0].gameObject;
			return Camera.allCameras[0].transform;
		}
		return null;
	}

	private GameObject findSeenInteractiveGameobject()
	{
		Transform camTr = GetActiveCameraTransform();
		if (camTr == null)
		{
			return null;
		}
		RaycastHit[] array = AsunderInput.AimSphereCastAll(camTr, 0.1f);
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit raycastHit = array[i];
			if ((bool)raycastHit.transform.gameObject && raycastHit.transform.gameObject.CompareTag("Interactive"))
			{
				return raycastHit.transform.gameObject;
			}
		}
		return null;
	}

	private bool isInteractiveObjectAvailable()
	{
		Transform camTr = GetActiveCameraTransform();
		if (camTr == null)
		{
			return false;
		}
		RaycastHit[] array = AsunderInput.AimSphereCastAll(camTr, 0.1f);
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit raycastHit = array[i];
			if (raycastHit.transform.gameObject.CompareTag("Interactive"))
			{
				_ActiveInteractionObject = raycastHit.transform.gameObject;
				return true;
			}
		}
		return false;
	}

	public bool isInteractiveObjectReady(string aString)
	{
		if ((bool)_ActiveInteractionObject && aString == _ActiveInteractionObject.name)
		{
			Collider col = _ActiveInteractionObject.GetComponent<Collider>();
			return col != null && col.enabled;
		}
		return false;
	}

	public bool isInteractiveObjectSeen(string aString, float aRadius = 0.1f)
	{
		Transform camTr = GetActiveCameraTransform();
		if (camTr == null)
		{
			return false;
		}
		RaycastHit[] array = AsunderInput.AimSphereCastAll(camTr, aRadius);
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit raycastHit = array[i];
			if (raycastHit.transform.name == aString)
			{
				_ActiveInteractionObject = raycastHit.transform.gameObject;
				return true;
			}
		}
		return false;
	}

	private void DeactivateTelescope()
	{
		CameraFade.StartTeleAlphaFade(Color.black, true, 1f);
		if (_ActiveCamera != null && _ActiveCamera.name == "CameraRight")
		{
			if (_OVRCameraController != null)
			{
				_OVRCameraController.SetFOV_Right(_InitOculusFoV);
			}
			if (_EyeTelescope != null)
			{
				_EyeTelescope.SetActive(false);
			}
		}
		if (_HandController != null)
		{
			_HandController.setHandState(HandController.HandState.telescopeReturn);
			if (_HandController._Telescope != null)
			{
				_HandController._Telescope.SetActive(true);
			}
		}
		if (_ActiveCamera != null)
		{
			Camera cam = _ActiveCamera.GetComponent<Camera>();
			if (cam != null)
			{
				cam.fieldOfView = 60f;
			}
			CC_FastVignette vig = _ActiveCamera.GetComponent<CC_FastVignette>();
			if (vig != null)
			{
				vig.enabled = false;
			}
		}
	}

	private void ActivateTelescope()
	{
		if (_ActiveCamera != null && _ActiveCamera.name == "CameraRight")
		{
			if (_EyeTelescope != null)
			{
				_EyeTelescope.SetActive(true);
			}
			if (_OVRCameraController != null)
			{
				_InitOculusFoV = _OVRCameraController.GetFOV();
				_OVRCameraController.SetFOV_Right(60f);
			}
		}
		else if (_ActiveCamera != null)
		{
			Camera cam = _ActiveCamera.GetComponent<Camera>();
			if (cam != null)
			{
				cam.fieldOfView = 30f;
			}
		}
		CameraFade.StartTeleAlphaFade(Color.black, true, 1f);
		if (_ActiveCamera != null)
		{
			CC_FastVignette vig = _ActiveCamera.GetComponent<CC_FastVignette>();
			if (vig != null)
			{
				vig.enabled = true;
			}
		}
		if (_HandController != null && _HandController._Telescope != null)
		{
			_HandController._Telescope.SetActive(false);
		}
	}

	private void setState(PlayerState aState)
	{
		_lastPlayerState = _ActivePlayerState;
		_ActivePlayerState = aState;
	}

	private IEnumerator idle()
	{
		yield return null;
		while (_ActivePlayerState == PlayerState.idle)
		{
			if (isInteractiveObjectAvailable())
			{
				setState(PlayerState.decideInteraction);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator limitInteractions()
	{
		_HandController.setHandState(HandController.HandState.returnToChair);
		float timer = Time.time + DEFAULT_LIMIT_DELAY;
		yield return null;
		while (_ActivePlayerState == PlayerState.limitInteractions)
		{
			if (Time.time > timer)
			{
				setState(PlayerState.lookForInteractions);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator wait()
	{
		float timer = Time.time + DEFAULT_LIMIT_DELAY;
		yield return null;
		while (_ActivePlayerState == PlayerState.wait)
		{
			if (!_HandController._isHandMoving && Time.time > timer)
			{
				setState(PlayerState.lookForInteractions);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator lookForInteractions()
	{
		yield return null;
		while (_ActivePlayerState == PlayerState.lookForInteractions)
		{
			_ActiveInteractionObject = findSeenInteractiveGameobject();
			if ((bool)_ActiveInteractionObject)
			{
				setState(PlayerState.decideInteraction);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator decideInteraction()
	{
		yield return null;
		while (_ActivePlayerState == PlayerState.decideInteraction)
		{
			if ((bool)_HandController)
			{
				if (isInteractiveObjectReady("IO_Peanuts"))
				{
					_HandController.setHandState(HandController.HandState.grabPeanutsReady);
				}
				else if (isInteractiveObjectReady("IO_Handcuffs"))
				{
					_HandController.setHandState(HandController.HandState.hideHandcuffsReady);
				}
				else if (isInteractiveObjectReady("IO_WalterWindow"))
				{
					Invoke("pointScaredQuickly", 1.5f);
				}
				else if (!isInteractiveObjectReady("IO_PlayerWindow"))
				{
					if (isInteractiveObjectReady("IO_AuxDoor"))
					{
						_HandController.setHandState(HandController.HandState.auxReady);
					}
					else if (isInteractiveObjectReady("IO_Telescope"))
					{
						_HandController.setHandState(HandController.HandState.telescopeReady);
						if ((bool)_LegController && _LegController._ActiveLegState == LegController.LegState.ready)
						{
							_LegController.SetState(LegController.LegState.returnToFloor);
						}
					}
					else if (isInteractiveObjectReady("IO_FrontSeatBack"))
					{
						_HandController.setHandState(HandController.HandState.returnToChair);
						_LegController.SetState(LegController.LegState.ready);
					}
					else if (isInteractiveObjectReady("IO_BtnLight"))
					{
						_HandController.SetBtnType("Light");
						_HandController.setHandState(HandController.HandState.pushBtnReady);
					}
					else
					{
						if (!isInteractiveObjectReady("IO_BtnFan"))
						{
							_HandController.setHandState(HandController.HandState.returnToChair);
							if ((bool)_LegController)
							{
								_LegController.SetState(LegController.LegState.returnToFloor);
							}
							setState(PlayerState.lookForInteractions);
							break;
						}
						_HandController.SetBtnType("Fan");
						_HandController.setHandState(HandController.HandState.pushBtnReady);
					}
				}
				setState(PlayerState.readyObject);
				_ReadyInteractionObject = findSeenInteractiveGameobject();
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private void pointScaredQuickly()
	{
		_HandController.setHandState(HandController.HandState.pointScared);
	}

	private IEnumerator readyObject()
	{
		bool hasBeenPressed = false;
		float _confidenceStopWatch = 0f;
		yield return null;
		while (_ActivePlayerState == PlayerState.readyObject)
		{
			_ActiveInteractionObject = findSeenInteractiveGameobject();
			if (_ActiveInteractionObject == _ReadyInteractionObject)
			{
				if (AsunderInput.GetInteractUp())
				{
					hasBeenPressed = true;
				}
				if (!_HandController._isHandMoving && hasBeenPressed)
				{
					if (isInteractiveObjectReady("IO_Peanuts"))
					{
						_HandController.setHandState(HandController.HandState.grabPeanutsStart);
						AsunderInput.TriggerHaptic(AsunderInput.ActivePointerHand, 0.45f, 0.08f);
						setState(PlayerState.interactWith);
						UnlockInteraction("IO_Lydia");
						break;
					}
					if (isInteractiveObjectReady("IO_Telescope"))
					{
						_HandController.setHandState(HandController.HandState.telescopePickup);
						if (!_hasUsedTelescope && (bool)_storyManager && (bool)_storyManager.GetCurrentAppManager() && _storyManager.GetCurrentAppManager()._ApplicationType == AppManager.AppType.VR)
						{
							_storyManager.ShowSubtitleMessage("CLOSE YOUR LEFT EYE", 8f);
							_hasUsedTelescope = true;
						}
						if (!_hasStartedUsingTelescope)
						{
							Invoke("ActivateTelescope", 2f);
							CameraFade.StartAlphaFade(Color.black, false, 0.5f, 1.5f);
							_hasStartedUsingTelescope = true;
						}
						setState(PlayerState.interactWith);
					}
					else if (isInteractiveObjectReady("IO_Handcuffs"))
					{
						_HandController.setHandState(HandController.HandState.hideHandcuffsStart);
						LockInteraction("IO_Handcuffs");
						setState(PlayerState.lookForInteractions);
					}
					else if (isInteractiveObjectReady("IO_AuxDoor"))
					{
						_HandController.setHandState(HandController.HandState.auxOpen);
						setState(PlayerState.interactWith);
					}
					else if (!isInteractiveObjectReady("IO_WalterWindow"))
					{
						if (isInteractiveObjectReady("IO_FrontSeatBack"))
						{
							_LegController.SetState(LegController.LegState.kick);
							AsunderInput.TriggerHapticBoth(0.7f, 0.15f);
							LockInteraction("IO_FrontSeatBack");
							setState(PlayerState.lookForInteractions);
						}
						else if (isInteractiveObjectReady("IO_BtnLight"))
						{
							_HandController.setHandState(HandController.HandState.pushBtnStart);
							_PlayerOverheadController.PushButton();
							AsunderInput.TriggerHaptic(AsunderInput.ActivePointerHand, 0.5f, 0.05f);
							setState(PlayerState.lookForInteractions);
						}
						else if (isInteractiveObjectReady("IO_BtnFan"))
						{
							_HandController.setHandState(HandController.HandState.pushBtnStart);
							_PlayerOverheadController.PushForSound();
							setState(PlayerState.lookForInteractions);
						}
						else
						{
							_HandController.setHandState(HandController.HandState.returnToChair);
							if ((bool)_LegController)
							{
								_LegController.SetState(LegController.LegState.returnToFloor);
							}
							setState(PlayerState.lookForInteractions);
						}
					}
				}
			}
			else if (!_ActiveInteractionObject)
			{
				_confidenceStopWatch += Time.deltaTime;
				if (_confidenceStopWatch > 0.5f)
				{
					_HandController.setHandState(HandController.HandState.returnToChair);
					if ((bool)_LegController)
					{
						_LegController.SetState(LegController.LegState.returnToFloor);
					}
					setState(PlayerState.lookForInteractions);
				}
			}
			else
			{
				setState(PlayerState.lookForInteractions);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator interactWith()
	{
		float timer = Time.time + 1f;
		yield return null;
		while (_ActivePlayerState == PlayerState.interactWith)
		{
			if (AsunderInput.GetInteractUp())
			{
				if (_HandController.GetHandState() == HandController.HandState.throwReady)
				{
					_HandController.setHandState(HandController.HandState.throwRelease);
					AsunderInput.TriggerHaptic(AsunderInput.ActivePointerHand, 0.65f, 0.1f);
					Invoke("PeanutThowingEnd", 0.5f);
					setState(PlayerState.wait);
				}
				if (_HandController.GetHandState() == HandController.HandState.telescopeUse && _hasStartedUsingTelescope)
				{
					Invoke("DeactivateTelescope", 0.8f);
					CameraFade.StartAlphaFade(Color.black, false, 0.8f);
					setState(PlayerState.wait);
					_hasStartedUsingTelescope = false;
				}
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator smoking()
	{
		_HandController.setHandState(HandController.HandState.SmokeReady);
		yield return null;
		while (_ActivePlayerState == PlayerState.smoking)
		{
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator nonActive()
	{
		yield return null;
		while (_ActivePlayerState == PlayerState.nonActive)
		{
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private void PeanutThowingEnd()
	{
		if (isInteractiveObjectSeen("IO_Lydia"))
		{
			LockInteraction("IO_Lydia");
			LockInteraction("IO_Peanuts");
		}
	}

	private IEnumerator FSM()
	{
		while (true)
		{
			yield return StartCoroutine(_ActivePlayerState.ToString());
		}
	}
}
