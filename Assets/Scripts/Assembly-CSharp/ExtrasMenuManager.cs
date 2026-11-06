using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtrasMenuManager : MonoBehaviour
{
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
		if (AsunderInput.GetInteractUp() && iTween.tweens.Count == 1)
		{
			if (_AudioSource != null)
			{
				iTween.AudioTo(_AudioSource, iTween.Hash("volume", 0, "time", 0.5f));
			}
		}
		GameObject gameObject = findSeenInteractiveGameobject();
		if ((bool)gameObject)
		{
			setSeenChapter(gameObject);
			if (!_hasSelectedScene && AsunderInput.GetInteractUp())
			{
				_hasSelectedScene = true;
				AsunderInput.TriggerHapticBoth(0.6f, 0.1f);
				switch (gameObject.name)
				{
				case "Extra1":
					Invoke("loadExtra1", 4f);
					break;
				case "Extra2":
					Invoke("loadExtra2", 4f);
					break;
				case "Extra3":
					Invoke("loadExtra3", 4f);
					break;
				case "Extra4":
					Invoke("loadExtra4", 4f);
					break;
				case "Extra5":
					Invoke("loadExtra5", 4f);
					break;
				case "Extra6":
					Invoke("loadExtra6", 4f);
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
	}

	private void loadExtra1()
	{
		SceneManager.LoadScene("Extra1_WalterNightmare");
	}

	private void loadExtra2()
	{
		SceneManager.LoadScene("Extra2_Fall");
	}

	private void loadExtra3()
	{
		SceneManager.LoadScene("Extra3_Flyby");
	}

	private void loadExtra4()
	{
		SceneManager.LoadScene("Extra4_Blood");
	}

	private void loadExtra5()
	{
		SceneManager.LoadScene("Extra5_Beach");
	}

	private void loadExtra6()
	{
		SceneManager.LoadScene("Extra6_City");
	}

	private void loadBack()
	{
		SceneManager.LoadScene("IntroScene");
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
