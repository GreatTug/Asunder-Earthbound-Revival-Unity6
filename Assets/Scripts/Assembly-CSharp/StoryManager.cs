using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
	public enum StoryState
	{
		scene0_fadeIn = 0,
		scene1_announcement = 1,
		scene1_lookAtWalter = 2,
		scene1_walterGreeting = 3,
		scene1_walterGreetRefuse = 4,
		scene1_walterGreetAccept = 5,
		scene1_walterHandcuff = 6,
		scene1_HandcuffsSeen = 7,
		scene1_HandcuffsHidden = 8,
		scene1_announcementSong = 9,
		scene2_edwardStartsHumming = 10,
		scene2_edwardKeepsHumming = 11,
		scene2_edwardStopsHumming = 12,
		scene2_edwardSeesYou = 13,
		scene2_edwardStopsBotheringYou = 14,
		scene2_edwardKillYou = 15,
		scene2_edwardHasKilledYou = 16,
		scene2_lydiaStartsLaughing = 17,
		scene2_lydiaHit = 18,
		scene2_lydiaAsks = 19,
		scene2_lydiaSeesYou = 20,
		scene2_lydiaIgnoresYou = 21,
		scene2_walterStoryTime = 22,
		scene3_announceStorm = 23,
		scene3_creatureFirstSighting = 24,
		scene3_creatureFirstSightingDone = 25,
		scene3_walterDidYouSeeSomething = 26,
		scene3_walterCreatureSeenAccept = 27,
		scene3_walterCreatureSeenRefuse = 28,
		scene3_creatureAudioWalking = 29,
		scene3_walterAsksYouToLook = 30,
		scene3_creatureSecondSighting = 31,
		scene3_creatureSecondPoint = 32,
		scene3_walterLooksOut = 33,
		scene3_creatureSecondightingDone = 34,
		scene3_creatureSecondWalterMumbles = 35,
		scene3_creatureThirdSighting = 36,
		scene3_creatureCrashesThePlane = 37,
		scene4_hatchOpened = 38,
		scene4_suckedOut = 39,
		theEnd = 40,
		gameOver_strangled = 41,
		idle = 42
	}

	public StoryState _ActiveStoryState;

	private StoryState _lastStoryState;

	private StoryState _lastCheckPoint;

	private GameObject _Subtitles;
	private TextMesh _cachedSubtitlesTextMesh;
	private Material _cachedHelpTextMaterial;
	private TextMesh _cachedHelpTextMesh;

	private float _timer;

	private float _currentDisplayTime;

	private int _currentCharacterSequenceID;

	private bool _isSequenceComplete;

	private bool _hasFadedIn;

	private Transform _mainCameraTransform;

	private string _currentHelpMessage = string.Empty;

	private LoadCharacterXML _LoadCharacterXML;

	private AppManager _referenceToAppManager;

	public EnvironmentManager _EnvironmentManager;

	public HeadGestureRecognizer _HeadGestureRecognizer;

	public GameObject[] _Characters;

	private GameObject _Player;

	private Character _character_Player;

	private GameObject _Walter;

	private Character _character_Walter;

	private WalterController _controller_Walter;

	private GameObject _Radio;

	private Character _character_Radio;

	private GameObject _Edward;

	private Character _character_Edward;

	private EdwardController _controller_Edward;

	private GameObject _Lydia;

	private Character _character_Lydia;

	private LydiaController _controller_Lydia;

	private GameObject _Biff;

	private Character _character_Biff;

	private BiffController _controller_Biff;

	private GameObject _Crowd;

	private Character _character_Crowd;

	private Character _character_Helper;

	public PlayerManager _PlayerManager;

	public GameObject _Cameras;

	private GameObject _OVRCameraLeft;

	public TextAsset _GameScript;

	public MouseLook _MouseLook;

	public GameObject _Hatch;

	public GameObject _AirMayhem;

	public GameObject _FallingPassanger;

	public GameObject _OutsideLight;

	public GameObject _CreatureFirstSighting;

	public GameObject _CreatureSecondSighting;

	public GameObject _CreatureThirdSighting;

	public GameObject _CreatureAudioWalker;

	public GameObject _WindowAudioSource;

	public GameObject _MusicAudioSource;

	public GameObject _DebrisSpawner;

	public GameObject _Paper;

	public GameObject _Effects;

	public GameObject _CamerasPivot;

	public GameObject _EffectsCrash;

	private GameObject _HelpText;

	public GameObject _CurtainPlayer;

	public GameObject _CurtainPhysical;

	public GameObject _WindowBroken;

	public Texture _WindowBrokenTexture1;

	public Texture _WindowBrokenTexture2;

	public Texture _WindowBrokenTexture3;

	public Texture _WindowBrokenTexture4;

	public Texture _WindowBrokenTexture5;

	public AudioSource _WindowBrakeSoundSource;

	public Transform _CreatureArm;

	public GameObject _MessageBoardText;

	public GameObject _HatchLight;

	private bool _hasThrownPeanutAtCrowd;

	private bool _helpFadeIn;

	public GameObject _SleepingPassenger;



	private float _fsmLoopFrequency = 0.01f;

	private int _currentHintDialogueID;

	private bool _hintsTempOn = true;

	private void Start()
	{
		_Player = _Characters[0];
		_Walter = _Characters[1];
		_character_Walter = _Walter.GetComponent<Character>();
		_controller_Walter = _Walter.GetComponent<WalterController>();
		_Radio = _Characters[2];
		_character_Radio = _Radio.GetComponent<Character>();
		_Edward = _Characters[3];
		_character_Edward = _Edward.GetComponent<Character>();
		_controller_Edward = _Edward.GetComponent<EdwardController>();
		_Lydia = _Characters[4];
		_character_Lydia = _Lydia.GetComponent<Character>();
		_controller_Lydia = _Lydia.GetComponent<LydiaController>();
		_Biff = _Characters[5];
		_character_Biff = _Biff.GetComponent<Character>();
		_controller_Biff = _Biff.GetComponent<BiffController>();
		_Crowd = _Characters[6];
		_character_Crowd = _Crowd.GetComponent<Character>();
		_character_Helper = _Characters[7].GetComponent<Character>();
		CameraFade.StartAlphaFade(Color.black, true, 10f);
		_LoadCharacterXML = new LoadCharacterXML(_GameScript);
		_LoadCharacterXML.processCharacter(_character_Radio);
		_LoadCharacterXML.processCharacter(_character_Walter);
		_LoadCharacterXML.processCharacter(_character_Biff);
		_LoadCharacterXML.processCharacter(_character_Lydia);
		_LoadCharacterXML.processCharacter(_character_Edward);
		_LoadCharacterXML.processCharacter(_character_Crowd);
		_LoadCharacterXML.processCharacter(_character_Helper);
		if ((bool)GameObject.Find("* App Manager"))
		{
			_referenceToAppManager = GameObject.Find("* App Manager").GetComponent<AppManager>();
			_referenceToAppManager.SetStoryManager(this);
			_referenceToAppManager.InitateCameras();
			GameObject mainCam = _referenceToAppManager.GetMainCamera();
			if (mainCam != null)
			{
				_mainCameraTransform = mainCam.transform;
			}
			correctSceneToCheckpoint(_referenceToAppManager.GetCheckpoint());
			if (_referenceToAppManager._ApplicationType == AppManager.AppType.PC)
			{
				if (_PlayerManager != null && _PlayerManager._PlayerOverheadController != null) _PlayerManager._PlayerOverheadController.TurnFlatOn();
			}
			else if (_referenceToAppManager._ApplicationType == AppManager.AppType.VR)
			{
				if (_PlayerManager != null && _PlayerManager._PlayerOverheadController != null) _PlayerManager._PlayerOverheadController.TurnOculusOn();
			}
		}
		if (_mainCameraTransform == null && Camera.allCamerasCount > 0 && Camera.allCameras[0] != null)
		{
			_mainCameraTransform = Camera.allCameras[0].transform;
		}
		if (_mainCameraTransform != null)
		{
			Transform helpTr = _mainCameraTransform.Find("+ Help");
			if (helpTr != null) _HelpText = helpTr.gameObject;
			Transform diaTr = _mainCameraTransform.Find("+ Dialogue");
			if (diaTr != null) _Subtitles = diaTr.gameObject;
			Transform handTr = _mainCameraTransform.Find("+ HandPosition");
			if (_PlayerManager != null && _PlayerManager._HandController != null && handTr != null)
			{
				_PlayerManager._HandController.InitateCameraHandPosition(handTr);
			}
			if (_PlayerManager != null)
			{
				_PlayerManager.InitateCorrectCameraGameObject(_mainCameraTransform.gameObject);
			}
			if (_Edward != null)
			{
				EdController ed = _Edward.GetComponent<EdController>();
				if (ed != null) ed.initateMainCameraTransform(_mainCameraTransform);
			}
			if (_HeadGestureRecognizer != null)
			{
				_HeadGestureRecognizer.InitCameraTransform(_mainCameraTransform);
			}
		}
		if (_Cameras != null)
		{
			Transform ovrLeftTr = _Cameras.transform.Find("* CameraOVR/CameraLeft");
			if (ovrLeftTr != null) _OVRCameraLeft = ovrLeftTr.gameObject;
		}
		StartCoroutine(FSM());
	}

	public void RestartStartCoroutine()
	{
		StartCoroutine(FSM());
	}

	public void PauseCoroutine()
	{
		StopCoroutine("FSM");
	}

	private void CreatureRetractArmCycle()
	{
		_CreatureArm.localRotation = Quaternion.Lerp(_CreatureArm.localRotation, Quaternion.Euler(295f, 252f, 227f), 0.05f);
	}

	private void CreatureAttackArmCycle()
	{
		_CreatureArm.localRotation = Quaternion.Lerp(_CreatureArm.localRotation, Quaternion.Euler(-56f, -7f, 434f), 0.25f);
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

	private void correctSceneToCheckpoint(int anID)
	{
		switch (anID)
		{
		case 0:
			_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.heavyClouds);
			setState(StoryState.scene0_fadeIn);
			break;
		case 1:
			_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.city);
			_PlayerManager._HandController.VisuallyHideHandcuffs();
			setState(StoryState.scene1_announcementSong);
			break;
		case 2:
			_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.heavyClouds);
			_PlayerManager._HandController.VisuallyHideHandcuffs();
			setState(StoryState.scene2_edwardStartsHumming);
			break;
		case 3:
			_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.stormEnterQuick);
			_PlayerManager._HandController.VisuallyHideHandcuffs();
			setState(StoryState.scene3_announceStorm);
			break;
		case 4:
			_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.stormEnterQuick);
			_EnvironmentManager.PlaySFX("disturbing-ambience-fadein", _MusicAudioSource.GetComponent<AudioSource>());
			_PlayerManager._HandController.VisuallyHideHandcuffs();
			setState(StoryState.scene3_creatureAudioWalking);
			break;
		}
		CameraFade.StartAlphaFade(Color.black, true, 8f);
		if ((bool)_referenceToAppManager && _referenceToAppManager.IsReplaying())
		{
			if (_OVRCameraLeft != null && _OVRCameraLeft.transform.parent != null)
			{
				OVRCameraController ovrCamCtrl = _OVRCameraLeft.transform.parent.GetComponent<OVRCameraController>();
				if (ovrCamCtrl != null && ovrCamCtrl.FollowOrientation != null)
				{
					ovrCamCtrl.FollowOrientation.rotation = _referenceToAppManager.GetOVROrientation();
				}
			}
			if (anID != 4)
			{
				showHelpMessage("You were discovered!", 0f);
			}
			_referenceToAppManager.SetIsReplaying(false);
		}
	}

	private void GiveHint(int hintID)
	{
		if (_PlayerManager != null && _PlayerManager._PlayerOverheadController != null)
		{
			if (!_PlayerManager._PlayerOverheadController._HintsActive && _hintsTempOn)
			{
				_PlayerManager._PlayerOverheadController.TurnHintsOn();
			}
		}
		if (_MessageBoardText != null && !_MessageBoardText.activeSelf &&
		    _PlayerManager != null && _PlayerManager._HandController != null &&
		    _PlayerManager._HandController._CurrentBtnName == "Fan" &&
		    _PlayerManager._HandController._activeHandState == HandController.HandState.pushBtnStart)
		{
			if (_PlayerManager._PlayerOverheadController != null)
			{
				_PlayerManager._PlayerOverheadController.TurnHintsOff();
			}
			_hintsTempOn = false;
			if (_EnvironmentManager != null && _Cameras != null)
			{
				_EnvironmentManager.PlaySFX("Bing", _Cameras.GetComponent<AudioSource>());
			}
			if (_character_Helper != null && _character_Helper.dialoguesequences != null && hintID >= 0 && hintID < _character_Helper.dialoguesequences.Count)
			{
				ArrayList arrayList = (ArrayList)_character_Helper.dialoguesequences[hintID];
				if (arrayList != null && _currentHintDialogueID >= 0 && _currentHintDialogueID < arrayList.Count)
				{
					string text = arrayList[_currentHintDialogueID].ToString();
					showHelpMessage(text.Replace("/n", "\n"), 0f);
					_currentHintDialogueID++;
					if (_currentHintDialogueID >= arrayList.Count)
					{
						_currentHintDialogueID = arrayList.Count - 1;
					}
				}
			}
		}
	}

	private void showHelpMessage(string aMessage, float aDelay = 0f)
	{
		_currentHelpMessage = aMessage;
		if (aDelay != 0f)
		{
			Invoke("enterHelpMessage", aDelay);
		}
		else
		{
			enterHelpMessage();
		}
	}

	private void showGameOverMessage(string aMessage)
	{
		if ((bool)_MessageBoardText && !_MessageBoardText.activeSelf)
		{
			_currentHelpMessage = aMessage;
			_EnvironmentManager.PlaySFX("Bing", _Cameras.GetComponent<AudioSource>());
			_Cameras.GetComponent<AudioSource>().loop = true;
			iTween.AudioTo(_Cameras, iTween.Hash("volume", 0, "pitch", -0.9f, "time", 8, "easetype", iTween.EaseType.easeInExpo));
			iTween.PunchPosition(_MessageBoardText.transform.Find("+ Help").gameObject, iTween.Hash("z", 0.05f, "time", 3, "islocal", true, "easetype", iTween.EaseType.punch));
			_MessageBoardText.SetActive(true);
			_MessageBoardText.transform.Find("+ Help").GetComponent<TextMesh>().color = Color.red;
			_MessageBoardText.transform.Find("+ Help").GetComponent<TextMesh>().fontStyle = FontStyle.Bold;
			_MessageBoardText.transform.Find("+ Help").GetComponent<TextMesh>().fontSize = 40;
			_MessageBoardText.transform.Find("+ Help").GetComponent<TextMesh>().GetComponent<Renderer>().material.color = new Color(0.6f, 0f, 0f, 1f);
			_MessageBoardText.transform.Find("+ Help").GetComponent<TextMesh>().text = _currentHelpMessage;
		}
	}

	public void ShowSubtitleMessage(string aMessage, float duration)
	{
		if ((bool)_HelpText)
		{
			_HelpText.SetActive(true);
			Color color = _HelpText.GetComponent<TextMesh>().GetComponent<Renderer>().material.color;
			color.a = 1f;
			_HelpText.GetComponent<TextMesh>().GetComponent<Renderer>().material.color = color;
			_HelpText.GetComponent<TextMesh>().text = aMessage;
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

	private void enterHelpMessage()
	{
		if ((bool)_MessageBoardText)
		{
			_MessageBoardText.SetActive(true);
			_MessageBoardText.transform.Find("+ Help").GetComponent<TextMesh>().text = _currentHelpMessage;
			Invoke("exitHelpMessage", 5f);
		}
	}

	private void exitHelpMessage()
	{
		if ((bool)_MessageBoardText)
		{
			_MessageBoardText.transform.Find("+ Help").GetComponent<TextMesh>().text = string.Empty;
			_MessageBoardText.SetActive(false);
			_hintsTempOn = true;
		}
	}

	private void saveCheckpoint(int aStoryStateID)
	{
		if ((bool)_referenceToAppManager)
		{
			_referenceToAppManager.SetCheckpoint(aStoryStateID);
		}
	}

	private void SetManagersActivity(bool aBool)
	{
		if (!aBool)
		{
			_EnvironmentManager.StopCoroutine();
			_PlayerManager.StopCoroutine("FSM");
		}
		_EnvironmentManager.enabled = aBool;
		_HeadGestureRecognizer.enabled = aBool;
		_PlayerManager.enabled = aBool;
	}

	private void replayGame()
	{
		if ((bool)_referenceToAppManager)
		{
			_referenceToAppManager.SetBeenKilled(true);
			_referenceToAppManager.SetIsReplaying(true);
			if (_referenceToAppManager._ApplicationType == AppManager.AppType.VR)
			{
				Quaternion identity = Quaternion.identity;
				identity = _mainCameraTransform.rotation;
				identity.x = 0f;
				identity.z = 0f;
				_referenceToAppManager.SaveOVROrientation(identity);
			}
		}
		StopCoroutine("FSM");
		SceneManager.LoadScene("MainScene");
	}

	private void loadEnd()
	{
		SceneManager.LoadScene("PostMainScene");
	}

	private void setState(StoryState aState)
	{
		_currentHintDialogueID = 0;
		_PlayerManager._PlayerOverheadController.TurnHintsOff();
		_lastStoryState = _ActiveStoryState;
		_ActiveStoryState = aState;
		_isSequenceComplete = false;
	}

	private void initDialogueCycle(Character aCharacter, int aSequenceID, float aDisplayTime)
	{
		aCharacter.switchSequence(aSequenceID);
		aCharacter.PlayCurrentAudioFile();
		string text = aCharacter.playNextDialogue();
		_currentDisplayTime = aDisplayTime;
		_timer = Time.time + _currentDisplayTime;
		_currentCharacterSequenceID = aSequenceID;
		if (text != "DIALOGUE_COMPLETE")
		{
			if (_cachedSubtitlesTextMesh == null && _Subtitles != null) _cachedSubtitlesTextMesh = _Subtitles.GetComponent<TextMesh>();
			if (_cachedSubtitlesTextMesh != null) _cachedSubtitlesTextMesh.text = text.Replace("/n", "\n");
		}
		else
		{
			_isSequenceComplete = true;
		}
	}

	private void nextDialogueSequence(Character aCharacter)
	{
		string text = aCharacter.playNextDialogue();
		_timer = Time.time + _currentDisplayTime;
		if (text != "DIALOGUE_COMPLETE")
		{
			if (_cachedSubtitlesTextMesh == null && _Subtitles != null) _cachedSubtitlesTextMesh = _Subtitles.GetComponent<TextMesh>();
			if (_cachedSubtitlesTextMesh != null) _cachedSubtitlesTextMesh.text = text.Replace("/n", "\n");
		}
		else
		{
			_isSequenceComplete = true;
		}
	}

	private void dialogueCycle(Character aCharacter, StoryState aNextState, bool autoFinish = true)
	{
		if (!(Time.time > _timer))
		{
			return;
		}
		nextDialogueSequence(aCharacter);
		if (_isSequenceComplete)
		{
			if (_cachedSubtitlesTextMesh == null && _Subtitles != null) _cachedSubtitlesTextMesh = _Subtitles.GetComponent<TextMesh>();
			if (_cachedSubtitlesTextMesh != null) _cachedSubtitlesTextMesh.text = string.Empty;
			if (autoFinish && _isSequenceComplete)
			{
				setState(aNextState);
			}
		}
	}

	private void dialogueDecisionCycle(Character aCharacter, StoryState aAcceptState, StoryState aRefuseState)
	{
		if (Time.time > _timer)
		{
			if (_isSequenceComplete)
			{
				_Subtitles.GetComponent<TextMesh>().text = string.Empty;
			}
			else
			{
				nextDialogueSequence(aCharacter);
			}
		}
		if (_isSequenceComplete && !_controller_Walter.GetComponent<AudioSource>().isPlaying && _HeadGestureRecognizer.GetCurrentHeadGesture() != HeadGestureRecognizer.HeadGesture.None)
		{
			if (_HeadGestureRecognizer.GetCurrentHeadGesture() == HeadGestureRecognizer.HeadGesture.AcceptNod)
			{
				setState(aAcceptState);
			}
			else if (_HeadGestureRecognizer.GetCurrentHeadGesture() == HeadGestureRecognizer.HeadGesture.RefuseShake)
			{
				setState(aRefuseState);
			}
		}
	}

	private bool waitForPlayerFocus(string anObject)
	{
		if ((bool)_PlayerManager.GetActiveInteractionObject())
		{
			if (_PlayerManager.GetActiveInteractionObject().name == anObject)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public AppManager GetCurrentAppManager()
	{
		return _referenceToAppManager;
	}

	private void creatureBreakWindow(ref GameObject windowGameObj, ref bool isBrokenWindowDone, ref float timerBrokenWindow, ref float timerAttack, ref float attackDelay, ref bool isAttacking, string sfxName, Texture aTexture)
	{
		if (isBrokenWindowDone || !(Time.time > timerBrokenWindow))
		{
			return;
		}
		if (!isAttacking)
		{
			timerAttack = Time.time + attackDelay;
			isAttacking = true;
		}
		if (Time.time > timerAttack)
		{
			if (!windowGameObj.activeSelf)
			{
				windowGameObj.SetActive(true);
			}
			_EnvironmentManager.PlaySFX(sfxName, _WindowBrakeSoundSource.GetComponent<AudioSource>());
			isAttacking = false;
			isBrokenWindowDone = true;
			windowGameObj.GetComponent<Renderer>().material.mainTexture = aTexture;
		}
	}

	private IEnumerator idle()
	{
		yield return null;
		while (_ActiveStoryState == StoryState.idle)
		{
			if (waitForPlayerFocus("IO_Walter"))
			{
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene0_fadeIn()
	{
		saveCheckpoint(0);
		_timer = Time.time + 10f;
		if ((bool)_referenceToAppManager && !_referenceToAppManager.HasBeenKilled())
		{
			ShowSubtitleMessage("TWO WEEKS LATER", 5f);
		}
		yield return null;
		while (_ActiveStoryState == StoryState.scene0_fadeIn)
		{
			GiveHint(0);
			if (Time.time > _timer)
			{
				setState(StoryState.scene1_announcement);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene1_announcement()
	{
		initDialogueCycle(_character_Radio, 0, 1.5f);
		float timer = Time.time + 15f;
		yield return null;
		while (_ActiveStoryState == StoryState.scene1_announcement)
		{
			GiveHint(0);
			dialogueCycle(_character_Radio, StoryState.scene1_lookAtWalter, false);
			if (Time.time > timer)
			{
				setState(StoryState.scene1_lookAtWalter);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene1_lookAtWalter()
	{
		_controller_Walter.SetState(WalterController.WalterState.reading);
		yield return null;
		while (_ActiveStoryState == StoryState.scene1_lookAtWalter)
		{
			if (waitForPlayerFocus("IO_Walter"))
			{
				setState(StoryState.scene1_walterGreeting);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene1_walterGreeting()
	{
		initDialogueCycle(_character_Walter, 0, 2.5f);
		float timerQuickLook = Time.time + 1f;
		bool hasLooked = false;
		_controller_Walter.SetState(WalterController.WalterState.lookAtPlayerPeeking);
		float timer = Time.time + 11f;
		bool hasAsked = false;
		yield return null;
		while (_ActiveStoryState == StoryState.scene1_walterGreeting)
		{
			GiveHint(1);
			if (!hasLooked && Time.time > timerQuickLook)
			{
				_controller_Walter.SetState(WalterController.WalterState.lookAtPlayer);
				hasLooked = true;
			}
			dialogueDecisionCycle(_character_Walter, StoryState.scene1_walterGreetAccept, StoryState.scene1_walterGreetRefuse);
			if (!hasAsked && Time.time > timer)
			{
				_controller_Walter.SetState(WalterController.WalterState.lookAtPlayerAsking);
				ShowSubtitleMessage("NOD OR SHAKE YOUR HEAD", 5f);
				hasAsked = true;
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene1_walterGreetAccept()
	{
		initDialogueCycle(_character_Walter, 1, 10.5f);
		_controller_Walter.SetState(WalterController.WalterState.lookAtPlayerHappy);
		yield return null;
		while (_ActiveStoryState == StoryState.scene1_walterGreetAccept)
		{
			dialogueCycle(_character_Walter, StoryState.scene1_walterHandcuff);
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene1_walterGreetRefuse()
	{
		initDialogueCycle(_character_Walter, 2, 6.5f);
		_controller_Walter.SetState(WalterController.WalterState.lookAtPlayerHappy);
		yield return null;
		while (_ActiveStoryState == StoryState.scene1_walterGreetRefuse)
		{
			dialogueCycle(_character_Walter, StoryState.scene1_walterHandcuff);
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene1_walterHandcuff()
	{
		initDialogueCycle(_character_Walter, 4, 6.5f);
		_controller_Walter.SetState(WalterController.WalterState.lookAtPlayerLeftHand);
		string interactionID = "IO_Handcuffs";
		_PlayerManager.UnlockInteraction(interactionID);
		float newTimer = Time.time + 18f;
		yield return null;
		while (_ActiveStoryState == StoryState.scene1_walterHandcuff)
		{
			GiveHint(2);
			dialogueCycle(_character_Walter, StoryState.scene1_walterHandcuff);
			if (Time.time > newTimer)
			{
				if (_PlayerManager.isInteractionDone(interactionID))
				{
					setState(StoryState.scene1_HandcuffsHidden);
				}
				else
				{
					setState(StoryState.scene1_HandcuffsSeen);
				}
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene1_HandcuffsSeen()
	{
		initDialogueCycle(_character_Walter, 5, 18f);
		float timer = Time.time + 6f;
		float timerWalterLooks = Time.time + 9f;
		bool peopleHaveScreamed = false;
		float timerWalterUpset = Time.time + 3f;
		bool isWalterUpset = false;
		float timerEnd = Time.time + 15.5f;
		yield return null;
		while (_ActiveStoryState == StoryState.scene1_HandcuffsSeen)
		{
			showGameOverMessage("You've been discovered!");
			if (!isWalterUpset && Time.time > timerWalterUpset)
			{
				_controller_Walter.SetState(WalterController.WalterState.upsetAtPlayer);
				isWalterUpset = true;
			}
			if (Time.time > timer && !peopleHaveScreamed)
			{
				_EnvironmentManager.PlaySFX("gunshot-screams", _MusicAudioSource.GetComponent<AudioSource>());
				peopleHaveScreamed = true;
			}
			if (Time.time > timerWalterLooks && _controller_Walter._WalterState != WalterController.WalterState.lookAtBiff)
			{
				_controller_Walter.SetState(WalterController.WalterState.lookAtBiff);
			}
			if (Time.time > timerEnd)
			{
				setState(StoryState.gameOver_strangled);
			}
			dialogueCycle(_character_Walter, StoryState.gameOver_strangled);
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene1_HandcuffsHidden()
	{
		initDialogueCycle(_character_Walter, 6, 11.5f);
		_controller_Walter.SetState(WalterController.WalterState.lookAtPlayer);
		float timer = Time.time + 10.8f;
		bool hasRead = false;
		yield return null;
		while (_ActiveStoryState == StoryState.scene1_HandcuffsHidden)
		{
			dialogueCycle(_character_Walter, StoryState.scene1_announcementSong);
			if (!hasRead && Time.time > timer)
			{
				setState(StoryState.scene1_announcementSong);
				hasRead = true;
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator gameOver_strangled()
	{
		initDialogueCycle(_character_Biff, 0, 3f);
		_controller_Biff.enabled = true;
		_controller_Biff.Strangle();
		_timer = Time.time + 0.5f;
		bool _hasBiffGrabbed = false;
		float timerSecond = 0f;
		NoiseSegment segment = null;
		bool hasFaded = false;
		if (_Cameras != null)
		{
			NoiseController ncInit = _Cameras.GetComponent<NoiseController>();
			if (ncInit != null && ncInit._Segments != null && ncInit._Segments.Length > 0)
			{
				segment = ncInit._Segments[0];
			}
		}
		yield return null;
		while (_ActiveStoryState == StoryState.gameOver_strangled)
		{
			if (Time.time > _timer)
			{
				if (!_hasBiffGrabbed)
				{
					if ((bool)_referenceToAppManager)
					{
						if (_referenceToAppManager._ApplicationType == AppManager.AppType.PC)
						{
							if (_MouseLook != null) _MouseLook.ConstrainNeck();
							if (_mainCameraTransform != null)
							{
								CC_DoubleVision dv = _mainCameraTransform.gameObject.GetComponent<CC_DoubleVision>();
								if (dv != null) dv.enabled = true;
							}
						}
						else
						{
							if (_mainCameraTransform != null)
							{
								MouseLook ml = _mainCameraTransform.gameObject.GetComponent<MouseLook>();
								if (ml != null) ml.enabled = true;
								OVRCamera oc = _mainCameraTransform.gameObject.GetComponent<OVRCamera>();
								if (oc != null) oc.enabled = false;
								CC_DoubleVision dv = _mainCameraTransform.gameObject.GetComponent<CC_DoubleVision>();
								if (dv != null) dv.enabled = true;
							}
							if (_OVRCameraLeft != null)
							{
								MouseLook mlLeft = _OVRCameraLeft.GetComponent<MouseLook>();
								if (mlLeft != null) mlLeft.enabled = true;
								OVRCamera ocLeft = _OVRCameraLeft.GetComponent<OVRCamera>();
								if (ocLeft != null) ocLeft.enabled = false;
								CC_DoubleVision dvLeft = _OVRCameraLeft.GetComponent<CC_DoubleVision>();
								if (dvLeft != null) dvLeft.enabled = true;
							}
						}
					}
					timerSecond = Time.time + 6.5f;
					if (_PlayerManager != null) _PlayerManager.PlayVoice();
				}
				_hasBiffGrabbed = true;
			}
			if (_hasBiffGrabbed)
			{
				float waveX = Mathf.Sin(Time.time * 6f);
				float waveY = Mathf.Sin(Time.time * 3f) / 2f;
				if ((bool)_referenceToAppManager)
				{
					if (_Cameras != null)
					{
						NoiseController nc = _Cameras.GetComponent<NoiseController>();
						if (nc != null && nc._Segments != null && nc._Segments.Length > 0 && nc._Segments[0] != null)
						{
							nc._Segments[0]._Enabled = true;
						}
					}
					if (_referenceToAppManager._ApplicationType == AppManager.AppType.PC)
					{
						if (_mainCameraTransform != null)
						{
							CC_DoubleVision dv = _mainCameraTransform.gameObject.GetComponent<CC_DoubleVision>();
							if (dv != null)
							{
								dv.amount = Mathf.Lerp(dv.amount, 1f, 0.04f);
								dv.displace = new Vector2(waveX * 2f, waveY);
							}
						}
					}
					else
					{
						if (_mainCameraTransform != null)
						{
							CC_DoubleVision dv = _mainCameraTransform.gameObject.GetComponent<CC_DoubleVision>();
							if (dv != null)
							{
								dv.amount = Mathf.Lerp(dv.amount, 1f, 0.04f);
								dv.displace = new Vector2(waveX * 2f, waveY);
							}
						}
						if (_OVRCameraLeft != null)
						{
							CC_DoubleVision dvLeft = _OVRCameraLeft.GetComponent<CC_DoubleVision>();
							if (dvLeft != null)
							{
								dvLeft.amount = Mathf.Lerp(dvLeft.amount, 1f, 0.04f);
								dvLeft.displace = new Vector2(waveX * 2f, waveY);
							}
						}
					}
				}
				if (!hasFaded && Time.time > timerSecond)
				{
					CameraFade.StartAlphaFade(Color.black, false, 5f);
					Invoke("replayGame", 5f);
					hasFaded = true;
				}
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene1_announcementSong()
	{
		saveCheckpoint(1);
		_controller_Walter.SetState(WalterController.WalterState.lookOverhead);
		initDialogueCycle(_character_Radio, 1, 2.5f);
		_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.city);
		float timerWalter = Time.time + 8f;
		bool hasWalterLooked = false;
		float timer = Time.time + 70f;
		float timerShipHorn = Time.time + 45f;
		bool shipHornHasSounded = false;
		if ((bool)_SleepingPassenger)
		{
			_SleepingPassenger.GetComponent<PassengerSleeperAI>()._DoFinger = true;
		}
		yield return null;
		while (_ActiveStoryState == StoryState.scene1_announcementSong)
		{
			if (!shipHornHasSounded && Time.time > timerShipHorn)
			{
				_EnvironmentManager.PlaySFX("inception-horn", _EnvironmentManager._ShipAudio);
				shipHornHasSounded = true;
			}
			if (Time.time > timerWalter && !hasWalterLooked)
			{
				_controller_Walter.SetState(WalterController.WalterState.reading);
				hasWalterLooked = true;
			}
			dialogueCycle(_character_Radio, StoryState.scene2_edwardStartsHumming, false);
			if (Time.time > timer)
			{
				setState(StoryState.scene2_edwardStartsHumming);
			}
			yield return AsunderCoroutines.FsmWait;
		}
		if ((bool)_SleepingPassenger)
		{
			_SleepingPassenger.GetComponent<PassengerSleeperAI>()._DoFinger = false;
		}
	}

	private IEnumerator scene2_edwardStartsHumming()
	{
		saveCheckpoint(2);
		_character_Edward.GetComponent<AudioSource>().loop = true;
		_controller_Walter.SetState(WalterController.WalterState.lookAtEd);
		_Edward.GetComponent<EdController>().SetState(EdController.EdState.moveBack);
		initDialogueCycle(_character_Edward, 0, 2.5f);
		_PlayerManager.UnlockInteraction("IO_FrontSeatBack");
		bool hasKicked = false;
		float timer = 0f;
		float timer2 = Time.time + 7f;
		bool walterHasSpoken = false;
		yield return null;
		while (_ActiveStoryState == StoryState.scene2_edwardStartsHumming)
		{
			GiveHint(3);
			if (_PlayerManager.isInteractionDone("IO_FrontSeatBack") && !hasKicked)
			{
				_character_Edward.GetComponent<AudioSource>().loop = false;
				initDialogueCycle(_character_Edward, 1, 2.5f);
				hasKicked = true;
				timer = Time.time + 4f;
			}
			if (Time.time > timer2 && !walterHasSpoken)
			{
				initDialogueCycle(_character_Walter, 7, 0f);
				walterHasSpoken = true;
			}
			if (hasKicked && _isSequenceComplete && Time.time > timer)
			{
				setState(StoryState.scene2_edwardKeepsHumming);
			}
			yield return AsunderCoroutines.FsmWait;
		}
		_character_Edward.GetComponent<AudioSource>().loop = false;
	}

	private IEnumerator scene2_edwardKeepsHumming()
	{
		_character_Edward.GetComponent<AudioSource>().loop = true;
		_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.cloudy);
		initDialogueCycle(_character_Edward, 2, 2.5f);
		_PlayerManager.UnlockInteraction("IO_FrontSeatBack");
		bool hasKicked = false;
		yield return null;
		while (_ActiveStoryState == StoryState.scene2_edwardKeepsHumming)
		{
			GiveHint(3);
			if (_PlayerManager.isInteractionDone("IO_FrontSeatBack") && !hasKicked)
			{
				_character_Edward.GetComponent<AudioSource>().loop = false;
				initDialogueCycle(_character_Edward, 3, 2.5f);
				hasKicked = true;
			}
			if (hasKicked && _isSequenceComplete)
			{
				setState(StoryState.scene2_edwardStopsHumming);
			}
			yield return AsunderCoroutines.FsmWait;
		}
		_character_Edward.GetComponent<AudioSource>().loop = false;
	}

	private IEnumerator scene2_edwardStopsHumming()
	{
		float timer = Time.time + 8f;
		_Edward.GetComponent<EdController>().SetState(EdController.EdState.moveForward);
		initDialogueCycle(_character_Edward, 3, 2.5f);
		float timeToUnlockAgain = Time.time + 3f;
		bool hasKicked = false;
		bool hasUnlocked = false;
		yield return null;
		while (_ActiveStoryState == StoryState.scene2_edwardStopsHumming)
		{
			GiveHint(3);
			if (Time.time > timeToUnlockAgain)
			{
				if (!hasUnlocked)
				{
					_PlayerManager.UnlockInteraction("IO_FrontSeatBack");
					hasUnlocked = true;
				}
				if (_PlayerManager.isInteractionDone("IO_FrontSeatBack") && !hasKicked)
				{
					initDialogueCycle(_character_Edward, 3, 2.5f);
					hasKicked = true;
				}
			}
			if (hasKicked && _isSequenceComplete)
			{
				setState(StoryState.scene2_edwardSeesYou);
			}
			if (Time.time > timer)
			{
				setState(StoryState.scene2_lydiaStartsLaughing);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene2_edwardSeesYou()
	{
		float timer = Time.time + 6f;
		_Edward.GetComponent<EdController>().SetState(EdController.EdState.peek);
		initDialogueCycle(_character_Edward, 4, 2.5f);
		float timer2 = Time.time + 21f;
		yield return null;
		while (_ActiveStoryState == StoryState.scene2_edwardSeesYou)
		{
			if (Time.time > timer)
			{
				setState(StoryState.scene2_edwardStopsBotheringYou);
			}
			if (Time.time > timer2)
			{
				setState(StoryState.scene2_lydiaStartsLaughing);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene2_edwardStopsBotheringYou()
	{
		initDialogueCycle(_character_Edward, 5, 2.5f);
		_PlayerManager.UnlockInteraction("IO_FrontSeatBack");
		bool hasKicked = false;
		float timer2 = Time.time + 21f;
		float timer3 = 0f;
		yield return null;
		while (_ActiveStoryState == StoryState.scene2_edwardStopsBotheringYou)
		{
			if (_PlayerManager.isInteractionDone("IO_FrontSeatBack") && !hasKicked)
			{
				timer3 = Time.time + 5f;
				hasKicked = true;
			}
			if (Time.time > timer2)
			{
				setState(StoryState.scene2_lydiaStartsLaughing);
			}
			if (hasKicked && Time.time > timer3)
			{
				setState(StoryState.scene2_edwardKillYou);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene2_edwardKillYou()
	{
		if (_PlayerManager != null) _PlayerManager.UnlockInteraction("IO_FrontSeatBack");
		if (_controller_Walter != null) _controller_Walter.SetState(WalterController.WalterState.upsetAtPlayer);
		if (_Edward != null)
		{
			EdController ed = _Edward.GetComponent<EdController>();
			if (ed != null) ed.SetState(EdController.EdState.raseGun);
		}
		if (_EnvironmentManager != null && _MusicAudioSource != null)
		{
			_EnvironmentManager.PlaySFX("gunshot-screams", _MusicAudioSource.GetComponent<AudioSource>());
		}
		if (_character_Walter != null) initDialogueCycle(_character_Walter, 23, 2.5f);
		yield return null;
		while (_ActiveStoryState == StoryState.scene2_edwardKillYou)
		{
			showGameOverMessage("You've been discovered!");
			if (_PlayerManager != null && _PlayerManager.isInteractionDone("IO_FrontSeatBack"))
			{
				_PlayerManager.UnlockInteraction("IO_FrontSeatBack");
			}
			EdController ed = _Edward != null ? _Edward.GetComponent<EdController>() : null;
			if (ed != null && ed._ActiveEdState == EdController.EdState.fireKill)
			{
				if ((bool)_referenceToAppManager)
				{
					if (_referenceToAppManager._ApplicationType == AppManager.AppType.VR)
					{
						if (_mainCameraTransform != null)
						{
							ScreenOverlay so = _mainCameraTransform.GetComponent<ScreenOverlay>();
							if (so != null) so.intensity = -30f;
						}
						if (_OVRCameraLeft != null)
						{
							CC_BrightnessContrastGamma bcg = _OVRCameraLeft.GetComponent<CC_BrightnessContrastGamma>();
							if (bcg != null) bcg.enabled = true;
						}
					}
					else
					{
						if (_mainCameraTransform != null)
						{
							CC_BrightnessContrastGamma bcg = _mainCameraTransform.GetComponent<CC_BrightnessContrastGamma>();
							if (bcg != null) bcg.enabled = true;
							CC_FastVignette fv = _mainCameraTransform.GetComponent<CC_FastVignette>();
							if (fv != null) fv.enabled = true;
						}
					}
				}
				else
				{
					if (_mainCameraTransform != null)
					{
						ScreenOverlay so = _mainCameraTransform.GetComponent<ScreenOverlay>();
						if (so != null) so.intensity = -30f;
					}
				}
				setState(StoryState.scene2_edwardHasKilledYou);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene2_edwardHasKilledYou()
	{
		CameraFade.StartAlphaFade(Color.black, false, 8f, 5f);
		Invoke("replayGame", 10f);
		yield return null;
		while (_ActiveStoryState == StoryState.scene2_edwardHasKilledYou)
		{
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene2_lydiaStartsLaughing()
	{
		initDialogueCycle(_character_Walter, 8, 2f);
		_controller_Walter.SetState(WalterController.WalterState.lookAtLydia);
		initDialogueCycle(_character_Lydia, 0, 2f);
		_PlayerManager.UnlockInteraction("IO_Peanuts");
		_PlayerManager.UnlockInteraction("IO_Walter");
		_PlayerManager.UnlockInteraction("IO_FrontSeatAbove");
		_PlayerManager.LockInteraction("IO_FrontSeatBack");
		_character_Lydia.GetComponent<AudioSource>().loop = true;
		float timerWalterWantsToThrow = Time.time + 20f;
		bool hasWantedToThrow = false;
		yield return null;
		while (_ActiveStoryState == StoryState.scene2_lydiaStartsLaughing)
		{
			GiveHint(4);
			if (Time.time > timerWalterWantsToThrow && !hasWantedToThrow)
			{
				initDialogueCycle(_character_Walter, 24, 2f);
				hasWantedToThrow = true;
			}
			if (_PlayerManager.isInteractiveObjectSeen("IO_FrontSeatAbove") && _PlayerManager._HandController.GetHandState() == HandController.HandState.throwComplete)
			{
				if (!_hasThrownPeanutAtCrowd)
				{
					Invoke("playAngryPeanutCrowd", 0.75f);
					_hasThrownPeanutAtCrowd = true;
				}
			}
			else
			{
				_hasThrownPeanutAtCrowd = false;
			}
			if (_PlayerManager.isInteractionDone("IO_Lydia") && _PlayerManager.isInteractionDone("IO_Peanuts"))
			{
				setState(StoryState.scene2_lydiaHit);
			}
			if (_PlayerManager.isInteractiveObjectSeen("IO_Walter") && _PlayerManager._HandController.GetHandState() == HandController.HandState.throwReady)
			{
				_PlayerManager._HandController.DeleteCurrentPeanut();
				switch (Random.Range(0, 4))
				{
				case 0:
					initDialogueCycle(_character_Walter, 9, 2f);
					break;
				case 1:
					initDialogueCycle(_character_Walter, 15, 2f);
					break;
				case 2:
					initDialogueCycle(_character_Walter, 16, 2f);
					break;
				default:
					initDialogueCycle(_character_Walter, 9, 2f);
					break;
				}
				_controller_Walter.SetState(WalterController.WalterState.lookAtPlayerWithAnger);
				_PlayerManager._ActivePlayerState = PlayerManager.PlayerState.lookForInteractions;
				_PlayerManager._HandController.setHandState(HandController.HandState.returnToChair);
			}
			yield return AsunderCoroutines.FsmWait;
		}
		_character_Lydia.GetComponent<AudioSource>().loop = false;
		_PlayerManager.LockInteraction("IO_FrontSeatAbove");
	}

	private void playAngryPeanutCrowd()
	{
		initDialogueCycle(_character_Crowd, Random.Range(0, 25), 2f);
	}

	private IEnumerator scene2_lydiaHit()
	{
		_character_Lydia.GetComponent<AudioSource>().loop = false;
		_Lydia.GetComponent<LydiaController>().SetState(LydiaController.LydiaState.lookAtYou);
		initDialogueCycle(_character_Lydia, 1, 2f);
		yield return null;
		while (_ActiveStoryState == StoryState.scene2_lydiaHit)
		{
			GiveHint(4);
			dialogueCycle(_character_Lydia, StoryState.scene2_lydiaAsks);
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene2_lydiaAsks()
	{
		initDialogueCycle(_character_Lydia, 2, 2.5f);
		float timer = Time.time + 10f;
		yield return null;
		while (_ActiveStoryState == StoryState.scene2_lydiaAsks)
		{
			GiveHint(4);
			dialogueCycle(_character_Lydia, StoryState.scene2_lydiaAsks, false);
			if (Time.time > timer)
			{
				if (_PlayerManager._PlayerOverheadController.IsLightOn())
				{
					setState(StoryState.scene2_lydiaSeesYou);
				}
				else
				{
					setState(StoryState.scene2_lydiaIgnoresYou);
				}
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene2_lydiaSeesYou()
	{
		showGameOverMessage("You've been discovered!");
		_Lydia.GetComponent<LydiaController>().SetState(LydiaController.LydiaState.returnToSeat);
		initDialogueCycle(_character_Lydia, 3, 2f);
		float timer = Time.time + 6f;
		yield return null;
		while (_ActiveStoryState == StoryState.scene2_lydiaSeesYou)
		{
			if (Time.time > timer)
			{
				_EnvironmentManager.PlaySFX("crowd-mute-newt-onboard", _Lydia.GetComponent<AudioSource>());
				setState(StoryState.scene2_edwardKillYou);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene2_lydiaIgnoresYou()
	{
		initDialogueCycle(_character_Lydia, 4, 2f);
		float timer = Time.time + 8f;
		_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.stormEnter);
		yield return null;
		while (_ActiveStoryState == StoryState.scene2_lydiaIgnoresYou)
		{
			if (Time.time > timer)
			{
				if (_PlayerManager._PlayerOverheadController.IsLightOn())
				{
					setState(StoryState.scene2_lydiaSeesYou);
				}
				else
				{
					setState(StoryState.scene2_walterStoryTime);
				}
			}
			yield return AsunderCoroutines.FsmWait;
		}
		initDialogueCycle(_character_Lydia, 5, 2f);
		_Lydia.GetComponent<LydiaController>().SetState(LydiaController.LydiaState.returnToSeat);
	}

	private IEnumerator scene2_walterStoryTime()
	{
		float timerWalterBegins = Time.time + 5f;
		bool walterBegan = false;
		float timer = Time.time + 60f;
		float timerII = Time.time + 40f;
		bool littleTurbulenceStarted = false;
		float stopWatchWalter = 0f;
		bool hasStoppedTalking = false;
		bool hasResponed = false;
		yield return null;
		while (_ActiveStoryState == StoryState.scene2_walterStoryTime)
		{
			if (!walterBegan && Time.time > timerWalterBegins)
			{
				initDialogueCycle(_character_Walter, 17, 2f);
				_controller_Walter.SetState(WalterController.WalterState.lookAtPlayer);
				walterBegan = true;
			}
			if (!hasStoppedTalking)
			{
				if (!_PlayerManager.isInteractiveObjectSeen("IO_Walter"))
				{
					stopWatchWalter += Time.deltaTime;
					if (stopWatchWalter > 10f)
					{
						initDialogueCycle(_character_Walter, 25, 2f);
						_character_Walter.GetComponent<AudioSource>().loop = true;
						hasStoppedTalking = true;
					}
				}
				else
				{
					stopWatchWalter = 0f;
				}
			}
			else if (!hasResponed && _PlayerManager.isInteractiveObjectSeen("IO_Walter"))
			{
				initDialogueCycle(_character_Walter, 26, 2f);
				_character_Walter.GetComponent<AudioSource>().loop = false;
				_controller_Walter.SetState(WalterController.WalterState.lookAtPlayerWithAnger);
				hasResponed = true;
			}
			if (hasResponed && !_character_Walter.GetComponent<AudioSource>().isPlaying)
			{
				_controller_Walter.SetState(WalterController.WalterState.reading);
				setState(StoryState.scene3_announceStorm);
			}
			if (Time.time > timer)
			{
				_controller_Walter.SetState(WalterController.WalterState.reading);
				setState(StoryState.scene3_announceStorm);
			}
			if (Time.time > timerII && !littleTurbulenceStarted)
			{
				_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.stormPhase1);
				littleTurbulenceStarted = true;
			}
			yield return AsunderCoroutines.FsmWait;
		}
		_character_Walter.GetComponent<AudioSource>().loop = false;
		_Lydia.GetComponent<LydiaController>().SetState(LydiaController.LydiaState.returnToSeat);
	}

	private IEnumerator scene3_announceStorm()
	{
		saveCheckpoint(3);
		initDialogueCycle(_character_Radio, 2, 10f);
		_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.stormPhase2);
		_EnvironmentManager.PlaySFX("disturbing-ambience-fadein", _MusicAudioSource.GetComponent<AudioSource>());
		float timer = Time.time + 60f;
		float timerDimensional = Time.time + 25f;
		bool hasRadioSounded = false;
		initDialogueCycle(_character_Walter, 27, 2f);
		float timerWindowShortTap = Time.time + 45f;
		bool hasTapped = false;
		yield return null;
		while (_ActiveStoryState == StoryState.scene3_announceStorm)
		{
			if (Time.time > timerDimensional && !hasRadioSounded)
			{
				initDialogueCycle(_character_Radio, 3, 10f);
				hasRadioSounded = true;
			}
			if (Time.time > timerWindowShortTap && !hasTapped)
			{
				_EnvironmentManager.PlaySFX("window-tap-short", _WindowAudioSource.GetComponent<AudioSource>());
				hasTapped = true;
			}
			if (Time.time > timer)
			{
				setState(StoryState.scene3_creatureFirstSighting);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene3_creatureFirstSighting()
	{
		_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.stormBlackout);
		float timer = Time.time + 2f;
		bool creatureIsShown = false;
		float timerCreature = 0f;
		bool hasPlayedSFX = false;
		_PlayerManager.UnlockInteraction("IO_PlayerWindow");
		yield return null;
		while (_ActiveStoryState == StoryState.scene3_creatureFirstSighting)
		{
			if (!creatureIsShown)
			{
				if (Time.time > timer && _PlayerManager.isInteractiveObjectSeen("IO_PlayerWindow"))
				{
					_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.stormPhase1);
					_CreatureFirstSighting.SetActive(true);
					creatureIsShown = true;
					timerCreature = Time.time + 2f;
					if (!hasPlayedSFX)
					{
						_EnvironmentManager.PlaySFX("first-sighting-eerie-sound", _WindowAudioSource.GetComponent<AudioSource>());
						hasPlayedSFX = true;
					}
				}
			}
			else if (Time.time > timerCreature)
			{
				setState(StoryState.scene3_creatureFirstSightingDone);
			}
			yield return AsunderCoroutines.FsmWait;
		}
		_PlayerManager.LockInteraction("IO_PlayerWindow");
	}

	private IEnumerator scene3_creatureFirstSightingDone()
	{
		_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.stormBlackout);
		float timer = Time.time + 2f;
		bool stormIsGone = false;
		while (_ActiveStoryState == StoryState.scene3_creatureFirstSightingDone)
		{
			if (!stormIsGone)
			{
				if (Time.time > timer)
				{
					_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.stormPhase2);
					_CreatureFirstSighting.SetActive(false);
					stormIsGone = true;
					_controller_Walter.SetState(WalterController.WalterState.lookAtPlayer);
				}
			}
			else if (waitForPlayerFocus("IO_Walter"))
			{
				setState(StoryState.scene3_walterDidYouSeeSomething);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene3_walterDidYouSeeSomething()
	{
		initDialogueCycle(_character_Walter, 10, 2.5f);
		_controller_Walter.SetState(WalterController.WalterState.lookAtPlayerWithAnger);
		float timer = Time.time + 2f;
		yield return null;
		while (_ActiveStoryState == StoryState.scene3_walterDidYouSeeSomething)
		{
			dialogueDecisionCycle(_character_Walter, StoryState.scene3_walterCreatureSeenAccept, StoryState.scene3_walterCreatureSeenRefuse);
			if (Time.time > timer)
			{
				_controller_Walter.SetState(WalterController.WalterState.lookAtPlayer);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene3_walterCreatureSeenAccept()
	{
		initDialogueCycle(_character_Walter, 11, 2.5f);
		_controller_Walter.SetState(WalterController.WalterState.reading);
		yield return null;
		while (_ActiveStoryState == StoryState.scene3_walterCreatureSeenAccept)
		{
			dialogueCycle(_character_Walter, StoryState.scene3_creatureAudioWalking);
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene3_walterCreatureSeenRefuse()
	{
		initDialogueCycle(_character_Walter, 12, 2.5f);
		_controller_Walter.SetState(WalterController.WalterState.lookAtPlayerWithAnger);
		yield return null;
		while (_ActiveStoryState == StoryState.scene3_walterCreatureSeenRefuse)
		{
			dialogueCycle(_character_Walter, StoryState.scene3_creatureAudioWalking);
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene3_creatureAudioWalking()
	{
		saveCheckpoint(4);
		_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.stormPhase2);
		bool hasStarted = false;
		float timer = Time.time + 25f;
		float timerII = timer + 25f;
		_controller_Walter.SetState(WalterController.WalterState.reading);
		yield return null;
		while (_ActiveStoryState == StoryState.scene3_creatureAudioWalking)
		{
			if (Time.time > timer && !hasStarted)
			{
				initDialogueCycle(_character_Walter, 18, 2.5f);
				_CreatureAudioWalker.SetActive(true);
				_controller_Walter.SetState(WalterController.WalterState.lookForSounds);
				hasStarted = true;
			}
			if (Time.time > timerII)
			{
				setState(StoryState.scene3_walterAsksYouToLook);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene3_walterAsksYouToLook()
	{
		_CreatureAudioWalker.SetActive(false);
		float timer = Time.time + 15f;
		initDialogueCycle(_character_Walter, 19, 2.5f);
		_PlayerManager.UnlockInteraction("IO_PlayerWindow");
		_controller_Walter.SetState(WalterController.WalterState.lookAtPlayerWithConcern);
		yield return null;
		while (_ActiveStoryState == StoryState.scene3_walterAsksYouToLook)
		{
			if (_PlayerManager.isInteractiveObjectSeen("IO_PlayerWindow"))
			{
				setState(StoryState.scene3_creatureSecondSighting);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene3_creatureSecondSighting()
	{
		initDialogueCycle(_character_Walter, 13, 2.5f);
		_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.stormBlackout);
		float timer = Time.time + 4f;
		yield return null;
		while (_ActiveStoryState == StoryState.scene3_creatureSecondSighting)
		{
			if (Time.time > timer)
			{
				_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.stormPhase1);
				_CreatureSecondSighting.SetActive(true);
				setState(StoryState.scene3_creatureSecondPoint);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene3_creatureSecondPoint()
	{
		_PlayerManager.UnlockInteraction("IO_WalterWindow");
		_PlayerManager.LockInteraction("IO_Walter");
		float timer = Time.time + 4f;
		float lookedAtTimer = 0f;
		bool hasPlayedSFX = false;
		yield return null;
		while (_ActiveStoryState == StoryState.scene3_creatureSecondPoint)
		{
			if (_PlayerManager.isInteractiveObjectReady("IO_WalterWindow"))
			{
				if (!hasPlayedSFX)
				{
					_EnvironmentManager.PlaySFX("percussion-scare-tone", _PlayerManager._PlayerVoiceSource.GetComponent<AudioSource>());
					hasPlayedSFX = true;
				}
				lookedAtTimer += Time.deltaTime;
				if (lookedAtTimer > 4f)
				{
					setState(StoryState.scene3_walterLooksOut);
				}
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene3_walterLooksOut()
	{
		initDialogueCycle(_character_Walter, 20, 2.5f);
		_controller_Walter.SetState(WalterController.WalterState.lookOutside);
		float timer = Time.time + 1f;
		yield return null;
		while (_ActiveStoryState == StoryState.scene3_walterLooksOut)
		{
			if (Time.time > timer)
			{
				_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.stormBlackout);
				setState(StoryState.scene3_creatureSecondightingDone);
			}
			yield return AsunderCoroutines.FsmWait;
		}
		_PlayerManager.LockInteraction("IO_WalterWindow");
	}

	private IEnumerator scene3_creatureSecondightingDone()
	{
		float timer = Time.time + 1f;
		yield return null;
		while (_ActiveStoryState == StoryState.scene3_creatureSecondightingDone)
		{
			if (Time.time > timer)
			{
				_CreatureSecondSighting.SetActive(false);
				setState(StoryState.scene3_creatureSecondWalterMumbles);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene3_creatureSecondWalterMumbles()
	{
		initDialogueCycle(_character_Walter, 21, 3f);
		float timer = Time.time + 35f;
		_PlayerManager.UnlockInteraction("IO_PlayerWindow");
		yield return null;
		while (_ActiveStoryState == StoryState.scene3_creatureSecondWalterMumbles)
		{
			if (Time.time > timer && !_PlayerManager.isInteractiveObjectSeen("IO_PlayerWindow", 0.7f))
			{
				_EnvironmentManager.PlaySFX("window-tap", _WindowAudioSource.GetComponent<AudioSource>());
				_WindowAudioSource.GetComponent<AudioSource>().loop = true;
				setState(StoryState.scene3_creatureThirdSighting);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene3_creatureThirdSighting()
	{
		_CreatureThirdSighting.SetActive(true);
		_PlayerManager.UnlockInteraction("IO_PlayerWindow");
		yield return null;
		while (_ActiveStoryState == StoryState.scene3_creatureThirdSighting)
		{
			if (_PlayerManager.isInteractiveObjectSeen("IO_PlayerWindow"))
			{
				setState(StoryState.scene3_creatureCrashesThePlane);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene3_creatureCrashesThePlane()
	{
		_WindowAudioSource.GetComponent<AudioSource>().loop = false;
		_EnvironmentManager.PlaySFX("crash-2-nosedive", _PlayerManager._PlayerVoiceSource.GetComponent<AudioSource>());
		_EnvironmentManager.PlaySFX("jumpscare-creature-inyourwindow-loud", _WindowAudioSource.GetComponent<AudioSource>());
		_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.crash);
		_PlayerManager.LockInteraction("IO_PlayerWindow");
		_PlayerManager.UnlockInteraction("IO_AuxDoor");
		_Effects.SetActive(true);
		initDialogueCycle(_character_Walter, 22, 4f);
		_controller_Walter.SetState(WalterController.WalterState.upsetAtPlayer);
		float timer2 = Time.time + 25f;
		float timer3 = timer2 + 3f;
		_Cameras.transform.parent = null;
		_CreatureAudioWalker.SetActive(true);
		float timerBrokenWindow1 = Time.time + 0.5f;
		bool isBrokenWindowDone1 = false;
		float timerBrokenWindow2 = Time.time + 5.1f;
		bool isBrokenWindowDone2 = false;
		float timerBrokenWindow3 = Time.time + 9.9f;
		bool isBrokenWindowDone3 = false;
		float timerBrokenWindow4 = Time.time + 10f;
		bool isBrokenWindowDone4 = false;
		float timerBrokenWindow5 = Time.time + 11.1f;
		bool isBrokenWindowDone5 = false;
		bool isAttacking = false;
		float attackDelay = 0.1f;
		float timerAttack = 0f;
		yield return null;
		while (_ActiveStoryState == StoryState.scene3_creatureCrashesThePlane)
		{
			GiveHint(5);
			_Cameras.transform.position = _CamerasPivot.transform.position;
			if (!isAttacking)
			{
				CreatureRetractArmCycle();
			}
			else
			{
				CreatureAttackArmCycle();
			}
			creatureBreakWindow(ref _WindowBroken, ref isBrokenWindowDone1, ref timerBrokenWindow1, ref timerAttack, ref attackDelay, ref isAttacking, "glass-break-1", _WindowBrokenTexture1);
			creatureBreakWindow(ref _WindowBroken, ref isBrokenWindowDone2, ref timerBrokenWindow2, ref timerAttack, ref attackDelay, ref isAttacking, "glass-break-2", _WindowBrokenTexture2);
			creatureBreakWindow(ref _WindowBroken, ref isBrokenWindowDone3, ref timerBrokenWindow3, ref timerAttack, ref attackDelay, ref isAttacking, "glass-break-3", _WindowBrokenTexture3);
			creatureBreakWindow(ref _WindowBroken, ref isBrokenWindowDone4, ref timerBrokenWindow4, ref timerAttack, ref attackDelay, ref isAttacking, "glass-break-4", _WindowBrokenTexture4);
			creatureBreakWindow(ref _WindowBroken, ref isBrokenWindowDone5, ref timerBrokenWindow5, ref timerAttack, ref attackDelay, ref isAttacking, "glass-break-5", _WindowBrokenTexture5);
			if (Time.time > timer2)
			{
				showGameOverMessage(" | | | | | ");
				if (!_EffectsCrash.activeSelf)
				{
					_PlayerManager.LockInteraction("IO_AuxDoor");
					_EffectsCrash.SetActive(true);
					_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.stormBlackout);
				}
			}
			if (Time.time > timer3)
			{
				if (_CreatureAudioWalker.activeSelf)
				{
					_CreatureAudioWalker.SetActive(false);
				}
				CameraFade.StartAlphaFade(Color.black, false, 0f);
				Invoke("replayGame", 5f);
				StopCoroutine("FSM");
			}
			if (_PlayerManager._HandController._AuxOpened)
			{
				setState(StoryState.scene4_hatchOpened);
				_PlayerManager._HandController._AuxOpened = false;
				if (_CreatureAudioWalker.activeSelf)
				{
					_CreatureAudioWalker.SetActive(false);
				}
			}
			yield return AsunderCoroutines.FsmWait;
		}
		_Cameras.transform.parent = _CamerasPivot.transform.parent;
	}

	private IEnumerator scene4_hatchOpened()
	{
		_HatchLight.SetActive(true);
		_CurtainPlayer.SetActive(false);
		_CurtainPhysical.SetActive(true);
		_Cameras.GetComponent<NoiseController>().RefreshInitRotations();
		_character_Walter.GetComponent<NoiseController>().enabled = true;
		_MusicAudioSource.GetComponent<AudioSource>().Stop();
		_Paper.SetActive(false);
		_DebrisSpawner.SetActive(true);
		_Cameras.GetComponent<NoiseController>()._Segments[1]._Enabled = true;
		_PlayerManager._PlayerVoiceSource.volume = 0.65f;
		_EnvironmentManager.PlaySFX("open-hatch-4", _PlayerManager._PlayerVoiceSource);
		AsunderInput.TriggerHapticBoth(1.0f, 0.6f);
		_character_Walter.GetComponent<AudioSource>().Stop();
		_controller_Walter.SetState(WalterController.WalterState.reachForPlayer);
		_Hatch.transform.parent = null;
		Rigidbody hatchRb = AppManager.PrepareAndAttachDynamicRigidbody(_Hatch, 0.75f);
		hatchRb.AddForce(new Vector3(-600f, 300f, 0f));
		hatchRb.AddTorque(new Vector3(100f, 40f, 10f));
		_Cameras.transform.parent = _PlayerManager.GetPlayerRoot().transform;
		float timer = Time.time + 6.5f;
		float timer2 = Time.time + 5f;
		yield return null;
		while (_ActiveStoryState == StoryState.scene4_hatchOpened)
		{
			if (Time.time > timer)
			{
				setState(StoryState.scene4_suckedOut);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator scene4_suckedOut()
	{
		_Cameras.GetComponent<NoiseController>()._Segments[1]._Enabled = false;
		_PlayerManager.GetPlayerRoot().transform.parent = null;
		_EnvironmentManager.setState(EnvironmentManager.EnvironmentState.disableCloseWeather);
		_timer = Time.time + 0.2f;
		float timerII = _timer + 5f;
		float timerIII = Time.time + 0.1f;
		GameObject playerRoot = _PlayerManager.GetPlayerRoot();
		Rigidbody playerRb = AppManager.PrepareAndAttachDynamicRigidbody(playerRoot, 0.75f);
		ConstantForce playerCf = playerRoot.GetComponent<ConstantForce>();
		if (playerCf == null)
		{
			playerCf = playerRoot.AddComponent<ConstantForce>();
		}
		playerRb.AddForce(new Vector3(-400f, 0f, 0f));
		playerCf.relativeTorque = new Vector3(-2.1f, 0.3f, 1.2f);
		_OutsideLight.SetActive(true);
		bool isGonaSplash = true;
		yield return null;
		while (_ActiveStoryState == StoryState.scene4_suckedOut)
		{
			_EnvironmentManager._Aeroplane.transform.Translate(new Vector3(0f, 0f, Time.deltaTime * 15f), Space.Self);
			if (Time.time > _timer)
			{
				_PlayerManager.GetPlayerRoot().GetComponent<Rigidbody>().AddForce(new Vector3(0f, 0f, -400f));
				AsunderInput.TriggerHapticBoth(0.9f, 0.4f);
				_PlayerManager.GetPlayerRoot().GetComponent<ConstantForce>().relativeTorque = new Vector3(0f, 0f, 0.1f);
				_timer = Time.time + 10000000f;
			}
			if (Time.time > timerIII && !_FallingPassanger.activeSelf)
			{
				_FallingPassanger.transform.parent = null;
				_FallingPassanger.SetActive(true);
				_DebrisSpawner.SetActive(false);
			}
			_EnvironmentManager._Aeroplane.transform.localRotation = Quaternion.Lerp(_EnvironmentManager._Aeroplane.transform.localRotation, Quaternion.Euler(-30f, 25f, 48f), 0.01f);
			if (_mainCameraTransform.position.y <= 60f && isGonaSplash)
			{
				_EnvironmentManager.PlaySFX("water-splash", _PlayerManager._PlayerVoiceSource.GetComponent<AudioSource>());
				isGonaSplash = false;
			}
			if (_mainCameraTransform.position.y <= 0f)
			{
				setState(StoryState.theEnd);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator theEnd()
	{
		_DebrisSpawner.SetActive(false);
		CameraFade.StartAlphaFade(Color.black, false, 0f);
		Invoke("loadEnd", 10f);
		yield return null;
		while (_ActiveStoryState == StoryState.theEnd)
		{
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
