using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PostStoryManager : MonoBehaviour
{
	public GameObject[] _Characters;

	private List<HeadLookController> _headLookControllers = new List<HeadLookController>();

	public HandController _HandController;

	public GameObject _CrowdGazeTarget;

	public GameObject _DoorLight;

	public GameObject _DoorLeft;

	public GameObject _DoorRight;

	public AudioSource _AudioPiano;

	public AudioSource _AudioCrowd;

	public AudioSource _AudioStorm;

	public GameObject _Villan;

	public GameObject _Credits;

	private Transform _mainCameraTransform;

	public PlayerManager _PlayerManager;

	private GameObject _SmokePSys;

	private int _puffs;

	private bool _isInhaling;

	private float _FADE_IN_TIME = 12.5f;

	private GameObject _chapterObjectSeen;

	private GameObject _lastChapterObjectSeen;

	private bool _hasSelectedScene;

	private AppManager _referenceToAppManager;

	private void Awake()
	{
		GameObject[] characters = _Characters;
		foreach (GameObject obj in characters)
		{
			assignAttributesToHeadCtrls(obj);
		}
	}

	private void Start()
	{
		if (_AudioPiano != null) _AudioPiano.volume = 0f;
		if (_AudioCrowd != null) _AudioCrowd.volume = 0f;
		if (_AudioStorm != null) _AudioStorm.volume = 0f;
		if (_AudioPiano != null) iTween.AudioTo(_AudioPiano.gameObject, iTween.Hash("volume", 1, "time", _FADE_IN_TIME));
		if (_AudioCrowd != null) iTween.AudioTo(_AudioCrowd.gameObject, iTween.Hash("volume", 1, "time", _FADE_IN_TIME));
		if (_AudioStorm != null) iTween.AudioTo(_AudioStorm.gameObject, iTween.Hash("volume", 1, "time", _FADE_IN_TIME));
		if ((bool)GameObject.Find("* App Manager"))
		{
			_referenceToAppManager = GameObject.Find("* App Manager").GetComponent<AppManager>();
			_referenceToAppManager.InitateCameras();
			GameObject mainCam = _referenceToAppManager.GetMainCamera();
			if (mainCam != null)
			{
				_mainCameraTransform = mainCam.transform;
			}
		}
		if (_mainCameraTransform == null && Camera.allCamerasCount > 0 && Camera.allCameras[0] != null)
		{
			_mainCameraTransform = Camera.allCameras[0].transform;
		}
		if (_mainCameraTransform != null)
		{
			Transform smokeTr = _mainCameraTransform.Find("+ Smoke Particle System");
			if (smokeTr != null) _SmokePSys = smokeTr.gameObject;
			if (_PlayerManager != null) _PlayerManager.InitateCorrectCameraGameObject(_mainCameraTransform.gameObject);
		}
		Invoke("showCredits", _FADE_IN_TIME);
		CameraFade.StartAlphaFade(Color.black, true, _FADE_IN_TIME);
		GameObject[] array = GameObject.FindGameObjectsWithTag("Interactive");
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				Renderer r = gameObject.GetComponent<Renderer>();
				if (r != null && r.material != null)
				{
					r.material.color = Color.gray;
				}
			}
		}
	}

	private void loadExtras()
	{
		SceneManager.LoadScene("ExtrasMenuScene");
	}

	private void loadBack()
	{
		SceneManager.LoadScene("IntroScene");
	}

	private void Update()
	{
		GameObject gameObject = findSeenInteractiveGameobject();
		if ((bool)gameObject)
		{
			setSeenChapter(gameObject);
			if (!_hasSelectedScene && AsunderInput.GetInteractUp())
			{
				_hasSelectedScene = true;
				switch (gameObject.name)
				{
				case "Extras":
					Invoke("loadExtras", 4f);
					break;
				case "Back":
					Invoke("loadBack", 4f);
					break;
				}
				CameraFade.StartAlphaFade(Color.black, false, 4f);
			}
		}
		if ((bool)_lastChapterObjectSeen && _lastChapterObjectSeen != gameObject)
		{
			Renderer r = _lastChapterObjectSeen.GetComponent<Renderer>();
			if (r != null && r.material != null)
			{
				r.material.color = Color.gray;
			}
			_lastChapterObjectSeen = null;
		}
		if (!_isInhaling && _PlayerManager != null && _PlayerManager.isInteractiveObjectSeen("IO_Smoke"))
		{
			if (_PlayerManager._HandController != null) _PlayerManager._HandController.setHandState(HandController.HandState.SmokeReady);
			_PlayerManager.LockInteraction("IO_Smoke");
		}
		if (AsunderInput.GetInteractUp())
		{
			if (!_isInhaling)
			{
				if (_PlayerManager != null && _PlayerManager._HandController != null && _PlayerManager._HandController._activeHandState == HandController.HandState.SmokeReady)
				{
					_PlayerManager._HandController.setHandState(HandController.HandState.SmokeThat);
					_isInhaling = true;
				}
			}
			else if (_puffs < 4)
			{
				if (_SmokePSys != null)
				{
					ParticleSystem ps = _SmokePSys.GetComponent<ParticleSystem>();
					if (ps != null) ps.Emit(25);
				}
				if (_PlayerManager != null && _PlayerManager._HandController != null) _PlayerManager._HandController.setHandState(HandController.HandState.returnToChair);
				_puffs++;
			}
			else
			{
				_puffs = 0;
				if (_PlayerManager != null) _PlayerManager.UnlockInteraction("IO_Smoke");
				_isInhaling = false;
			}
		}
		if (_mainCameraTransform != null)
		{
			CC_Grayscale gray = _mainCameraTransform.GetComponent<CC_Grayscale>();
			if (gray != null && gray.amount > 0f)
			{
				gray.amount -= 0.00032f;
				if (_mainCameraTransform.name == "CameraRight" && _mainCameraTransform.parent != null)
				{
					Transform camLeft = _mainCameraTransform.parent.Find("CameraLeft");
					if (camLeft != null)
					{
						CC_Grayscale grayLeft = camLeft.GetComponent<CC_Grayscale>();
						if (grayLeft != null) grayLeft.amount -= 0.00032f;
					}
				}
			}
		}
	}

	private void showCredits()
	{
		_Credits.SetActive(true);
	}

	private void assignAttributesToHeadCtrls(GameObject obj)
	{
		Transform[] componentsInChildren = obj.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (transform.name == "Head")
			{
				HeadLookController component = obj.GetComponent<HeadLookController>();
				_headLookControllers.Add(component);
				component.enabled = false;
				component.overrideAnimation = true;
				BendingSegment bendingSegment = component.segments[0];
				bendingSegment.responsiveness = 0.75f;
				bendingSegment.target = _CrowdGazeTarget.transform;
				bendingSegment.firstTransform = transform;
				bendingSegment.lastTransform = transform;
			}
		}
	}

	private void allLookAtGazeTarget()
	{
		foreach (HeadLookController headLookController in _headLookControllers)
		{
			headLookController.enabled = true;
		}
	}

	private void openTheDoor()
	{
		if (_DoorLight != null) _DoorLight.SetActive(true);
		if (_AudioPiano != null) _AudioPiano.Stop();
		if (_AudioCrowd != null) _AudioCrowd.Stop();
	}

	private void openDoors()
	{
		if (_DoorLeft != null) _DoorLeft.transform.localRotation = Quaternion.Lerp(_DoorLeft.transform.localRotation, Quaternion.Euler(0f, -40f, 0f), 0.4f);
		if (_DoorRight != null) _DoorRight.transform.localRotation = Quaternion.Lerp(_DoorRight.transform.localRotation, Quaternion.Euler(0f, 40f, 0f), 0.4f);
	}

	private GameObject findSeenInteractiveGameobject()
	{
		GameObject result = null;
		Transform camTr = ((bool)_referenceToAppManager && _referenceToAppManager.GetMainCamera() != null)
			? _referenceToAppManager.GetMainCamera().transform
			: (Camera.allCamerasCount > 0 && Camera.allCameras[0] != null ? Camera.allCameras[0].transform : null);
		if (camTr == null)
		{
			return null;
		}
		RaycastHit[] array = AsunderInput.AimRaycastAll(camTr);
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit raycastHit = array[i];
			if ((bool)raycastHit.transform.gameObject && raycastHit.transform.gameObject.CompareTag("Interactive"))
			{
				return raycastHit.transform.gameObject;
			}
		}
		return result;
	}

	private void setSeenChapter(GameObject anObj)
	{
		_chapterObjectSeen = anObj;
		if (_chapterObjectSeen != null)
		{
			Renderer r = _chapterObjectSeen.GetComponent<Renderer>();
			if (r != null && r.material != null)
			{
				r.material.color = Color.white;
			}
		}
		_lastChapterObjectSeen = _chapterObjectSeen;
	}
}
