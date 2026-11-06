using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreStoryManager : MonoBehaviour
{
	public enum PreStoryState
	{
		idle = 0,
		flashback1 = 1,
		flashback2 = 2,
		flashback3 = 3
	}

	public PreStoryState _ActiveStoryState;

	private AppManager _referenceToAppManager;

	public GameObject _A1_Scene;

	public GameObject _A2_Scene;

	public GameObject _A3_Scene;

	public GameObject _HeavyRain;

	private float _FLASHBACK1_DURATION = 20f;

	private float _FLASHBACK2_DURATION = 20f;

	private float _FLASHBACK3_DURATION = 11f;

	private float _FADEINOUT_DURATION = 8f;

	public GameObject _PoliceLights;

	private GameObject _HelpText;
	private Material _cachedHelpTextMaterial;
	private TextMesh _cachedHelpTextMesh;

	private bool _helpFadeIn;

	private Transform _mainCameraTransform;

	private float _fsmLoopFrequency = 0.01f;

	private void Start()
	{
		if ((bool)GameObject.Find("* App Manager"))
		{
			_referenceToAppManager = GameObject.Find("* App Manager").GetComponent<AppManager>();
			_referenceToAppManager.InitateCameras();
			GameObject mainCam = _referenceToAppManager.GetMainCamera();
			if (mainCam != null)
			{
				_mainCameraTransform = mainCam.transform;
			}
			AudioSource appAudio = _referenceToAppManager.GetComponent<AudioSource>();
			if (appAudio != null)
			{
				appAudio.spatialBlend = 0f;
				appAudio.Play();
			}
		}
		if (_mainCameraTransform == null && Camera.allCamerasCount > 0 && Camera.allCameras[0] != null)
		{
			_mainCameraTransform = Camera.allCameras[0].transform;
		}
		if (_mainCameraTransform != null)
		{
			Transform helpTr = _mainCameraTransform.Find("+ Help");
			if (helpTr != null)
			{
				_HelpText = helpTr.gameObject;
			}
		}
		showSubtitleMessage("1936", 5f);
		StartCoroutine(FSM());
	}

	private void Update()
	{
		if (_helpFadeIn && _HelpText != null)
		{
			if (_cachedHelpTextMesh == null) _cachedHelpTextMesh = _HelpText.GetComponent<TextMesh>();
			if (_cachedHelpTextMaterial == null && _cachedHelpTextMesh != null)
			{
				Renderer r = _cachedHelpTextMesh.GetComponent<Renderer>();
				if (r != null) _cachedHelpTextMaterial = r.material;
			}
			if (_cachedHelpTextMaterial != null)
			{
				Color color = _cachedHelpTextMaterial.color;
				color.a -= Time.deltaTime * 2f;
				if (color.a < 0f)
				{
					_HelpText.SetActive(false);
					_helpFadeIn = false;
				}
				else
				{
					_cachedHelpTextMaterial.color = color;
				}
			}
			else
			{
				_HelpText.SetActive(false);
				_helpFadeIn = false;
			}
		}
	}

	public void SetState(PreStoryState aState)
	{
		_ActiveStoryState = aState;
	}

	private void showSubtitleMessage(string aMessage, float duration)
	{
		if ((bool)_HelpText)
		{
			_HelpText.SetActive(true);
			TextMesh tm = _HelpText.GetComponent<TextMesh>();
			if (tm != null)
			{
				tm.text = aMessage;
			}
			Invoke("exitSubtitleMessage", duration);
		}
	}

	private void exitSubtitleMessage()
	{
		if ((bool)_HelpText)
		{
			_helpFadeIn = true;
		}
	}

	private IEnumerator idle()
	{
		yield return null;
		while (_ActiveStoryState == PreStoryState.idle)
		{
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator flashback1()
	{
		CameraFade.StartAlphaFade(Color.black, true, _FADEINOUT_DURATION);
		float timer = Time.time + _FLASHBACK1_DURATION;
		bool isDone = false;
		yield return null;
		while (_ActiveStoryState == PreStoryState.flashback1)
		{
			if (Time.time > timer && !isDone)
			{
				Invoke("nextstoryscenestate", _FADEINOUT_DURATION);
				CameraFade.StartAlphaFade(Color.black, false, _FADEINOUT_DURATION);
				isDone = true;
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private void nextstoryscenestate()
	{
		SetState(PreStoryState.flashback2);
	}

	private IEnumerator flashback2()
	{
		_A1_Scene.SetActive(false);
		_A2_Scene.SetActive(true);
		if ((bool)_referenceToAppManager)
		{
			_referenceToAppManager.InitateCameras();
		}
		CameraFade.StartAlphaFade(Color.black, true, _FADEINOUT_DURATION);
		float timer = Time.time + _FLASHBACK2_DURATION;
		bool isDone = false;
		yield return null;
		while (_ActiveStoryState == PreStoryState.flashback2)
		{
			if (_PoliceLights != null)
			{
				_PoliceLights.transform.position = Vector3.Lerp(_PoliceLights.transform.position, new Vector3(-23.839f, 36.02656f, -85.2817f), 0.005f);
			}
			if (Time.time > timer && !isDone)
			{
				Invoke("GoToScene3", 10f);
				if (_HeavyRain != null)
				{
					iTween.AudioTo(_HeavyRain, 0f, 1f, 10f);
				}
				CameraFade.StartAlphaFade(Color.black, false, 5f, 0f, () =>
				{
					Camera[] allCameras = Camera.allCameras;
					foreach (Camera camera in allCameras)
					{
						if ((bool)camera.gameObject.GetComponent<ScreenOverlay>())
						{
							camera.gameObject.GetComponent<ScreenOverlay>().enabled = true;
							camera.gameObject.GetComponent<ScreenOverlay>().intensity = 100f;
						}
					}
				});
				isDone = true;
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private void GoToScene3()
	{
		if ((bool)_referenceToAppManager)
		{
			_referenceToAppManager.LoadMainScene();
		}
		else
		{
			SceneManager.LoadScene("MainScene");
		}
	}

	private IEnumerator flashback3()
	{
		_A2_Scene.SetActive(false);
		_A3_Scene.SetActive(true);
		if ((bool)_referenceToAppManager)
		{
			_referenceToAppManager.InitateCameras();
		}
		Camera[] allCameras = Camera.allCameras;
		foreach (Camera cam in allCameras)
		{
			if ((bool)cam.gameObject.GetComponent<ScreenOverlay>())
			{
				cam.gameObject.GetComponent<ScreenOverlay>().enabled = true;
				cam.gameObject.GetComponent<ScreenOverlay>().intensity = 100f;
			}
		}
		float timer = Time.time + _FLASHBACK3_DURATION;
		bool isDone = false;
		yield return null;
		while (_ActiveStoryState == PreStoryState.flashback3)
		{
			if (Time.time > timer && !isDone)
			{
				if ((bool)_referenceToAppManager)
				{
					_referenceToAppManager.LoadMainScene();
				}
				else
				{
					SceneManager.LoadScene("MainScene");
				}
				isDone = true;
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator FSM()
	{
		while (true)
		{
			yield return StartCoroutine(_ActiveStoryState.ToString());
		}
	}
}
