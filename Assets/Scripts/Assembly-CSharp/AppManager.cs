using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
	public enum AppType
	{
		VR = 0,
		PC = 1
	}

	private static readonly HashSet<string> _unity4TwoDClips = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
	{
		"barking-dogs-siren",
		"beach-30sec",
		"beach-60sec",
		"biff-got-himself-criminal",
		"crash-2-nosedive",
		"crowd-mute-newt-onboard",
		"first-sighting-eerie-sound",
		"first-sighting-long-ambience",
		"gunshot-screams",
		"percussion-scare-tone",
		"ramp-airport",
		"scary-ambience-window-2",
		"title-audio-fx",
		"water-splash"
	};

	private static readonly Dictionary<EntityId, float> _initialSpatialBlends = new Dictionary<EntityId, float>();

	public static AppManager Instance { get; private set; }

	public AppType _ApplicationType = AppType.PC;

	public bool _LockHideCursor = true;

	private int _CheckpointID = -1;

	private GameObject _mainCamera;

	private bool _paused;

	private StoryManager _refStoryManager;

	private List<AudioSource> _playingAudioSources = new List<AudioSource>();

	public bool _isReplaying;

	public bool _skipLoadingMenu;

	private bool _hasBeenKilled;

	private Quaternion _keptOrientation = Quaternion.identity;

	private bool _EscDone = true;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void EnsureAppManagerExists()
	{
		if (GameObject.Find("* App Manager") != null)
		{
			return;
		}
		string activeScene = SceneManager.GetActiveScene().name;
		if (activeScene == "Loading")
		{
			return;
		}

		GameObject go = new GameObject("* App Manager");
		AudioSource audioSrc = go.AddComponent<AudioSource>();
		audioSrc.playOnAwake = false;
		audioSrc.spatialBlend = 0f;
		audioSrc.clip = Resources.Load<AudioClip>("Audio/SFX/intro-disturbing-ambience");

		AppManager mgr = go.AddComponent<AppManager>();
		if (!mgr._skipLoadingMenu)
		{
			mgr._ApplicationType = (AsunderInput.IsOpenXRActive || Application.platform == RuntimePlatform.Android) ? AppType.VR : AppType.PC;
		}
		mgr._CheckpointID = (activeScene == "IntroScene") ? (-1) : 0;
		if (mgr._LockHideCursor)
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
		UnityEngine.Object.DontDestroyOnLoad(go);
	}

	public static void ApplyUnity4AudioSpatialBlend(AudioSource source, AudioClip clip)
	{
		if (source == null)
		{
			return;
		}
		EntityId id = source.GetEntityId();
		if (!_initialSpatialBlends.TryGetValue(id, out float origBlend))
		{
			origBlend = source.spatialBlend;
			_initialSpatialBlends[id] = origBlend;
		}
		source.spatialBlend = (clip != null && _unity4TwoDClips.Contains(clip.name)) ? 0f : origBlend;
	}

	public static Rigidbody PrepareAndAttachDynamicRigidbody(GameObject target, float damping)
	{
		if (target == null)
		{
			return null;
		}
		MeshCollider[] meshColliders = target.GetComponentsInChildren<MeshCollider>(true);
		for (int i = 0; i < meshColliders.Length; i++)
		{
			MeshCollider mc = meshColliders[i];
			if (mc == null)
			{
				continue;
			}
			string goName = mc.gameObject.name;
			if (goName == "FakeAO" || goName == "BackQuad")
			{
				mc.enabled = false;
			}
			else if (!mc.convex)
			{
				mc.convex = true;
			}
		}
		Rigidbody rb = target.GetComponent<Rigidbody>();
		if (rb == null)
		{
			rb = target.AddComponent<Rigidbody>();
		}
		rb.linearDamping = damping;
		return rb;
	}

	private void Awake()
	{
		Instance = this;
		if (Application.platform == RuntimePlatform.Android)
		{
			// Quest standalone: no desktop mode and no mouse menu -> go straight to VR and the intro scene.
			_ApplicationType = AppType.VR;
			_skipLoadingMenu = true;
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}
		else
		{
			string[] args = System.Environment.GetCommandLineArgs();
			string exeName = string.Empty;
			try
			{
				exeName = System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLowerInvariant();
			}
			catch { }

			bool cliPc = false;
			bool cliVr = false;

			for (int i = 0; i < args.Length; i++)
			{
				string arg = args[i].ToLowerInvariant();
				if (arg == "-pc" || arg == "--pc" || arg == "-desktop" || arg == "-flat")
				{
					cliPc = true;
				}
				else if (arg == "-vr" || arg == "--vr" || arg == "-openxr" || arg == "-vrmode")
				{
					cliVr = true;
				}
			}

			bool forcePc = cliPc;
			bool forceVr = cliVr;

			// CLI arguments take precedence over process executable name
			if (!cliPc && !cliVr)
			{
				// Check VR indicators first (including PCVR which contains "pc")
				if (exeName.Contains("pcvr") || exeName.Contains("vr") || exeName.Contains("quest") || exeName.Contains("hmd") || exeName.Contains("openxr"))
				{
					forceVr = true;
				}
				else if (exeName.Contains("pc") || exeName.Contains("desktop") || exeName.Contains("flat"))
				{
					forcePc = true;
				}
			}

			if (forcePc)
			{
				_ApplicationType = AppType.PC;
				_skipLoadingMenu = true;
			}
			else if (forceVr)
			{
				_ApplicationType = AppType.VR;
				_skipLoadingMenu = true;
			}
		}
		// Enforce modern next-gen graphics & frame pacing
		QualitySettings.softParticles = true;
		QualitySettings.softVegetation = true;
		QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
		QualitySettings.antiAliasing = 8;
		QualitySettings.realtimeReflectionProbes = true;
		QualitySettings.billboardsFaceCameraPosition = true;
		QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
		QualitySettings.shadowCascades = 4;
		QualitySettings.shadowDistance = 150f;

		if (_ApplicationType == AppType.VR)
		{
			Application.targetFrameRate = -1; // Uncapped for OpenXR native refresh rate (72/90/120Hz)
			QualitySettings.vSyncCount = 0;   // Handled by OpenXR compositor
		}
		else
		{
			Application.targetFrameRate = -1; // Match high-refresh rate desktop monitor
			QualitySettings.vSyncCount = 1;   // Standard desktop VSync
		}

		AudioSource ownAudio = GetComponent<AudioSource>();
		if (ownAudio != null)
		{
			ownAudio.spatialBlend = 0f;
		}
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		AudioSource ownAudio = GetComponent<AudioSource>();
		if (ownAudio != null)
		{
			ownAudio.spatialBlend = 0f;
		}
		if (_skipLoadingMenu)
		{
			if (_LockHideCursor)
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
			if (_ApplicationType == AppType.VR)
			{
				AsunderInput.StartVR();
			}
			else
			{
				AsunderInput.StopVR();
			}
			SceneManager.LoadScene("IntroScene");
		}
	}

	private void Update()
	{
		if (!AsunderInput.GetMenuExitUp() || !_EscDone)
		{
			return;
		}
		_EscDone = false;
		string activeScene = SceneManager.GetActiveScene().name;
		if (activeScene != "IntroScene")
		{
			if (iTween.tweens.Count > 0)
			{
				iTween.Stop();
			}
			if (activeScene == "PreMainScene" || activeScene == "MainScene" || activeScene == "PostMainScene" || activeScene == "EndScene" || activeScene == "ExtrasMenuScene")
			{
				Invoke("loadIntroScene", 0.75f);
			}
			else if (activeScene == "Loading")
			{
				Application.Quit();
			}
			else
			{
				Invoke("loadExtrasMenuScene", 0.75f);
			}
			CameraFade.StartAlphaFade(Color.black, false, 0.75f);
		}
		else
		{
			Application.Quit();
		}
	}

	private void loadIntroScene()
	{
		SceneManager.LoadScene("IntroScene");
		_EscDone = true;
	}

	private void loadExtrasMenuScene()
	{
		SceneManager.LoadScene("ExtrasMenuScene");
		_EscDone = true;
	}

	private static GameObject FindSceneTaggedObject(string tag)
	{
		GameObject found = GameObject.FindGameObjectWithTag(tag);
		if (found != null)
		{
			return found;
		}
		Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
		for (int i = 0; i < allTransforms.Length; i++)
		{
			GameObject go = allTransforms[i].gameObject;
			if (go.scene.isLoaded && go.CompareTag(tag))
			{
				return go;
			}
		}
		return null;
	}

	public void InitateCameras()
	{
		GameObject camNormal = FindSceneTaggedObject("CameraNormal");
		GameObject camOvr = FindSceneTaggedObject("CameraOVR");
		AudioListener targetListener = null;

		if (_ApplicationType == AppType.PC)
		{
			AsunderInput.StopVR();
			if (camNormal != null)
			{
				camNormal.SetActive(true);
				Camera c = camNormal.GetComponent<Camera>();
				if (c != null && c.fieldOfView < 70f)
				{
					c.fieldOfView = 75f;
				}
				targetListener = camNormal.GetComponent<AudioListener>();
				if (targetListener == null)
				{
					targetListener = camNormal.AddComponent<AudioListener>();
				}
				targetListener.enabled = true;
				_mainCamera = camNormal;
			}
			else if (camOvr != null)
			{
				camOvr.SetActive(true);
				Transform camRight = camOvr.transform.Find("CameraRight");
				GameObject camTarget = (camRight != null) ? camRight.gameObject : camOvr;
				Camera c = camTarget.GetComponent<Camera>();
				if (c != null && c.fieldOfView < 70f)
				{
					c.fieldOfView = 75f;
				}
				targetListener = camTarget.GetComponent<AudioListener>();
				if (targetListener == null)
				{
					targetListener = camTarget.AddComponent<AudioListener>();
				}
				targetListener.enabled = true;
				_mainCamera = camTarget;
			}
			else if (Camera.main != null)
			{
				Camera c = Camera.main;
				if (c != null && c.fieldOfView < 70f)
				{
					c.fieldOfView = 75f;
				}
				targetListener = Camera.main.GetComponent<AudioListener>();
				if (targetListener == null)
				{
					targetListener = Camera.main.gameObject.AddComponent<AudioListener>();
				}
				targetListener.enabled = true;
				_mainCamera = Camera.main.gameObject;
			}
			if (camOvr != null && camNormal != null)
			{
				camOvr.SetActive(false);
			}
		}
		else
		{
			AsunderInput.StartVR();
			if (camNormal != null && camOvr != null)
			{
				camNormal.SetActive(false);
			}
			if (camOvr != null)
			{
				camOvr.SetActive(true);
				Transform camRight = camOvr.transform.Find("CameraRight");
				if (camRight != null)
				{
					targetListener = camRight.GetComponent<AudioListener>();
					if (targetListener == null)
					{
						targetListener = camRight.gameObject.AddComponent<AudioListener>();
					}
					targetListener.enabled = true;
					_mainCamera = camRight.gameObject;
				}
				else
				{
					targetListener = camOvr.GetComponent<AudioListener>();
					if (targetListener == null)
					{
						targetListener = camOvr.AddComponent<AudioListener>();
					}
					targetListener.enabled = true;
					_mainCamera = camOvr;
				}
			}
			else if (camNormal != null)
			{
				camNormal.SetActive(true);
				targetListener = camNormal.GetComponent<AudioListener>();
				if (targetListener == null)
				{
					targetListener = camNormal.AddComponent<AudioListener>();
				}
				targetListener.enabled = true;
				_mainCamera = camNormal;
			}
			else if (Camera.main != null)
			{
				targetListener = Camera.main.GetComponent<AudioListener>();
				if (targetListener == null)
				{
					targetListener = Camera.main.gameObject.AddComponent<AudioListener>();
				}
				targetListener.enabled = true;
				_mainCamera = Camera.main.gameObject;
			}
		}

		if (targetListener != null)
		{
			AudioListener[] allListeners = UnityEngine.Object.FindObjectsByType<AudioListener>();
			for (int i = 0; i < allListeners.Length; i++)
			{
				if (allListeners[i] != null && allListeners[i] != targetListener)
				{
					allListeners[i].enabled = false;
				}
			}
		}
	}

	public void LoadMainScene()
	{
		SetCheckpoint(0);
		Invoke("loadMScene", 5f);
		CameraFade.StartAlphaFade(Color.black, false, 5f);
	}

	private void loadMScene()
	{
		SceneManager.LoadScene("MainScene");
	}

	public void SetStoryManager(StoryManager aStoryManager)
	{
		_refStoryManager = aStoryManager;
	}

	private void pauseAllAudioSources()
	{
		AudioSource[] array = UnityEngine.Object.FindObjectsByType<AudioSource>();
		foreach (AudioSource audioSource in array)
		{
			if (audioSource.isPlaying)
			{
				audioSource.Pause();
				_playingAudioSources.Add(audioSource);
			}
		}
	}

	private void playActiveAudioSources()
	{
		foreach (AudioSource playingAudioSource in _playingAudioSources)
		{
			if (playingAudioSource != null)
			{
				playingAudioSource.Play();
			}
		}
		_playingAudioSources.Clear();
	}

	public int GetCheckpoint()
	{
		return _CheckpointID;
	}

	public void SetCheckpoint(int anID)
	{
		_CheckpointID = anID;
	}

	public GameObject GetMainCamera()
	{
		if (_mainCamera != null)
		{
			return _mainCamera;
		}
		if (Camera.main != null)
		{
			_mainCamera = Camera.main.gameObject;
			return _mainCamera;
		}
		if (Camera.allCamerasCount > 0 && Camera.allCameras[0] != null)
		{
			_mainCamera = Camera.allCameras[0].gameObject;
			return _mainCamera;
		}
		return null;
	}

	public void SetIsReplaying(bool aBool)
	{
		_isReplaying = aBool;
	}

	public bool IsReplaying()
	{
		return _isReplaying;
	}

	public void SetBeenKilled(bool aBool)
	{
		_hasBeenKilled = aBool;
	}

	public bool HasBeenKilled()
	{
		return _hasBeenKilled;
	}

	public void SaveOVROrientation(Quaternion aQuat)
	{
		_keptOrientation = aQuat;
	}

	public Quaternion GetOVROrientation()
	{
		return _keptOrientation;
	}

	private void OnApplicationQuit()
	{
		AsunderInput.StopVR();
	}
}
