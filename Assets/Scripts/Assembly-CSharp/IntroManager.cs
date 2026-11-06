using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
	public GameObject _AsunderText;

	public float _initalAsunderScale = 25f;

	public float _goalAsunderScale = 0.15f;

	public float _ScaleDuration = 20f;

	private AppManager _referenceToAppManager;

	public GameObject _OVRCameraLeft;

	private bool _hasFadedIn;

	private bool _hasStarted;

	private GameObject _chapterObjectSeen;

	private GameObject _lastChapterObjectSeen;

	public GameObject _AudioSource;

	private bool _hasSelectedScene;

	private void Start()
	{
		if ((bool)GameObject.Find("* App Manager"))
		{
			_referenceToAppManager = GameObject.Find("* App Manager").GetComponent<AppManager>();
			_referenceToAppManager.InitateCameras();
		}
		if (_AsunderText != null)
		{
			_AsunderText.gameObject.transform.localScale = new Vector3(_AsunderText.gameObject.transform.localScale.x, _AsunderText.gameObject.transform.localScale.y, _initalAsunderScale);
			if ((bool)_referenceToAppManager)
			{
				if (_referenceToAppManager.GetCheckpoint() == -1)
				{
					iTween.ScaleTo(_AsunderText, iTween.Hash("z", _goalAsunderScale, "time", _ScaleDuration, "easetype", iTween.EaseType.easeInOutCubic));
				}
				else
				{
					if (_AudioSource != null) iTween.AudioTo(_AudioSource, iTween.Hash("volume", 0, "time", 0.5f));
					iTween.ScaleTo(_AsunderText, iTween.Hash("z", _goalAsunderScale, "time", 0.5f, "easetype", iTween.EaseType.easeInOutCubic));
				}
			}
			else
			{
				iTween.ScaleTo(_AsunderText, iTween.Hash("z", _goalAsunderScale, "time", _ScaleDuration, "easetype", iTween.EaseType.easeInOutCubic));
			}
		}
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
		CameraFade.StartAlphaFade(Color.black, true, 8f);
	}

	private void Update()
	{
		if (AsunderInput.GetInteractDown() && iTween.tweens.Count == 1)
		{
			if (_AsunderText != null)
			{
				iTween.Stop(_AsunderText);
				if (_AudioSource != null) iTween.AudioTo(_AudioSource, iTween.Hash("volume", 0, "time", 0.5f));
				iTween.ScaleTo(_AsunderText, iTween.Hash("z", _goalAsunderScale, "time", 0.5f, "easetype", iTween.EaseType.easeInOutCubic));
			}
		}
		GameObject gameObject = findSeenInteractiveGameobject();
		if ((bool)gameObject && _AsunderText != null && _AsunderText.transform.localScale.z <= _goalAsunderScale)
		{
			setSeenChapter(gameObject);
			if (!_hasSelectedScene && AsunderInput.GetInteractUp())
			{
				_hasSelectedScene = true;
				AsunderInput.TriggerHapticBoth(0.6f, 0.1f);
				switch (gameObject.name)
				{
				case "Chapter0":
					if ((bool)_referenceToAppManager) _referenceToAppManager.SetCheckpoint(0);
					Invoke("loadPreMainScene", 4f);
					break;
				case "ChapterI":
					if ((bool)_referenceToAppManager) _referenceToAppManager.SetCheckpoint(0);
					Invoke("loadPreMainScene", 4f);
					break;
				case "ChapterII":
					if ((bool)_referenceToAppManager) _referenceToAppManager.SetCheckpoint(1);
					Invoke("loadMainScene", 4f);
					break;
				case "ChapterIII":
					if ((bool)_referenceToAppManager) _referenceToAppManager.SetCheckpoint(2);
					Invoke("loadMainScene", 4f);
					break;
				case "ChapterIV":
					if ((bool)_referenceToAppManager) _referenceToAppManager.SetCheckpoint(3);
					Invoke("loadMainScene", 4f);
					break;
				case "ChapterV":
					if ((bool)_referenceToAppManager) _referenceToAppManager.SetCheckpoint(4);
					Invoke("loadMainScene", 4f);
					break;
				case "Credits":
					if ((bool)_referenceToAppManager) _referenceToAppManager.SetCheckpoint(0);
					Invoke("loadEndScene", 4f);
					break;
				case "Extras":
					if ((bool)_referenceToAppManager) _referenceToAppManager.SetCheckpoint(0);
					Invoke("loadExtrasMenuScene", 4f);
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
	}

	private void loadExtrasMenuScene()
	{
		SceneManager.LoadScene("ExtrasMenuScene");
	}

	private void loadMainScene()
	{
		SceneManager.LoadScene("MainScene");
	}

	private void loadPreMainScene()
	{
		SceneManager.LoadScene("PreMainScene");
	}

	private void loadEndScene()
	{
		SceneManager.LoadScene("EndScene");
	}

	private GameObject findSeenInteractiveGameobject()
	{
		GameObject result = null;
		Transform camTr = ((bool)_referenceToAppManager && _referenceToAppManager.GetMainCamera() != null)
			? _referenceToAppManager.GetMainCamera().transform
			: (Camera.allCamerasCount > 0 ? Camera.allCameras[0].transform : null);
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
		if (anObj != null && anObj != _lastChapterObjectSeen)
		{
			AsunderInput.TriggerHaptic(AsunderInput.ActivePointerHand, 0.2f, 0.03f);
		}
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
