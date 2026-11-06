using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OVRMainMenu : MonoBehaviour
{
	public enum Device
	{
		HMDSensor = 0,
		HMD = 1,
		LatencyTester = 2
	}

	private delegate void updateFunctions();

	private OVRPresetManager PresetManager = new OVRPresetManager();

	public float FadeInTime = 2f;

	public Texture FadeInTexture;

	public Font FontReplace;

	public string[] SceneNames;

	public string[] Scenes;

	private bool ScenesVisible;

	private int StartX = 490;

	private int StartY = 300;

	private int WidthX = 300;

	private int WidthY = 23;

	private int VRVarsSX = 553;

	private int VRVarsSY = 350;

	private int VRVarsWidthX = 175;

	private int VRVarsWidthY = 23;

	private int StepY = 25;

	private OVRCameraController CameraController;

	private OVRPlayerController PlayerController;

	private bool PrevStartDown;

	private bool PrevHatDown;

	private bool PrevHatUp;

	private bool ShowVRVars;

	private bool OldSpaceHit;

	private float UpdateInterval = 0.5f;

	private float Accum;

	private int Frames;

	private float TimeLeft;

	private string strFPS = "FPS: 0";

	public float IPDIncrement = 0.0025f;

	private string strIPD = "IPD: 0.000";

	public float PredictionIncrement = 0.001f;

	private string strPrediction = "Pred: OFF";

	public float FOVIncrement = 0.2f;

	private string strFOV = "FOV: 0.0f";

	public float DistKIncrement = 0.001f;

	public float HeightIncrement = 0.01f;

	private string strHeight = "Height: 0.0f";

	public float SpeedRotationIncrement = 0.05f;

	private string strSpeedRotationMultipler = "Spd. X: 0.0f Rot. X: 0.0f";

	private bool LoadingLevel;

	private float AlphaFadeValue = 1f;

	private int CurrentLevel;

	private bool HMDPresent;

	private bool SensorPresent;

	private float RiftPresentTimeout;

	private string strRiftPresent = string.Empty;

	private float DeviceDetectionTimeout;

	private string strDeviceDetection = string.Empty;

	private OVRMagCalibration MagCal = new OVRMagCalibration();

	private OVRGUI GuiHelper = new OVRGUI();

	private GameObject GUIRenderObject;

	private RenderTexture GUIRenderTexture;

	public Texture CrosshairImage;

	private OVRCrosshair Crosshair = new OVRCrosshair();

	private updateFunctions UpdateFunctions;

	private void Awake()
	{
		OVRCameraController[] componentsInChildren = base.gameObject.GetComponentsInChildren<OVRCameraController>();
		if (componentsInChildren.Length == 0)
		{
			Debug.LogWarning("OVRMainMenu: No OVRCameraController attached.");
		}
		else if (componentsInChildren.Length > 1)
		{
			Debug.LogWarning("OVRMainMenu: More then 1 OVRCameraController attached.");
		}
		else
		{
			CameraController = componentsInChildren[0];
		}
		OVRPlayerController[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<OVRPlayerController>();
		if (componentsInChildren2.Length == 0)
		{
			Debug.LogWarning("OVRMainMenu: No OVRPlayerController attached.");
		}
		else if (componentsInChildren2.Length > 1)
		{
			Debug.LogWarning("OVRMainMenu: More then 1 OVRPlayerController attached.");
		}
		else
		{
			PlayerController = componentsInChildren2[0];
		}
	}

	private void Start()
	{
		AlphaFadeValue = 1f;
		CurrentLevel = 0;
		PrevStartDown = false;
		PrevHatDown = false;
		PrevHatUp = false;
		ShowVRVars = false;
		OldSpaceHit = false;
		strFPS = "FPS: 0";
		LoadingLevel = false;
		ScenesVisible = false;
		if (CameraController != null)
		{
			CameraController.InitCameraControllerVariables();
			GuiHelper.SetCameraController(ref CameraController);
		}
		GUIRenderObject = UnityEngine.Object.Instantiate(Resources.Load("OVRGUIObjectMain")) as GameObject;
		if (GUIRenderObject != null && GUIRenderTexture == null)
		{
			int num = Screen.width;
			int num2 = Screen.height;
			if (CameraController.PortraitMode)
			{
				int num3 = num2;
				num2 = num;
				num = num3;
			}
			GUIRenderTexture = new RenderTexture(num, num2, 0);
			GuiHelper.SetPixelResolution(num, num2);
			GuiHelper.SetDisplayResolution(OVRDevice.HResolution, OVRDevice.VResolution);
		}
		if (GUIRenderTexture != null && GUIRenderObject != null)
		{
			GUIRenderObject.GetComponent<Renderer>().material.mainTexture = GUIRenderTexture;
			if (CameraController != null)
			{
				Transform xfrm = GUIRenderObject.transform;
				CameraController.AttachGameObjectToCamera(ref GUIRenderObject);
				OVRUtils.SetLocalTransform(ref GUIRenderObject, ref xfrm);
				Vector3 localPosition = GUIRenderObject.transform.localPosition;
				float ipd = 0f;
				CameraController.GetIPD(ref ipd);
				localPosition.x -= ipd * 0.5f;
				GUIRenderObject.transform.localPosition = localPosition;
				GUIRenderObject.SetActive(false);
			}
		}
		StoreSnapshot("DEFAULT");
		if (!Application.isEditor)
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
		UpdateFunctions = (updateFunctions)Delegate.Combine(UpdateFunctions, new updateFunctions(UpdateFPS));
		if (CameraController != null)
		{
			UpdateFunctions = (updateFunctions)Delegate.Combine(UpdateFunctions, new updateFunctions(UpdateIPD));
			UpdateFunctions = (updateFunctions)Delegate.Combine(UpdateFunctions, new updateFunctions(UpdatePrediction));
			UpdateFunctions = (updateFunctions)Delegate.Combine(UpdateFunctions, new updateFunctions(UpdateFOV));
			UpdateFunctions = (updateFunctions)Delegate.Combine(UpdateFunctions, new updateFunctions(UpdateDistortionCoefs));
			UpdateFunctions = (updateFunctions)Delegate.Combine(UpdateFunctions, new updateFunctions(UpdateEyeHeightOffset));
		}
		if (PlayerController != null)
		{
			UpdateFunctions = (updateFunctions)Delegate.Combine(UpdateFunctions, new updateFunctions(UpdateSpeedAndRotationScaleMultiplier));
			UpdateFunctions = (updateFunctions)Delegate.Combine(UpdateFunctions, new updateFunctions(UpdatePlayerControllerMovement));
		}
		UpdateFunctions = (updateFunctions)Delegate.Combine(UpdateFunctions, new updateFunctions(UpdateSelectCurrentLevel));
		UpdateFunctions = (updateFunctions)Delegate.Combine(UpdateFunctions, new updateFunctions(UpdateHandleSnapshots));
		UpdateFunctions = (updateFunctions)Delegate.Combine(UpdateFunctions, new updateFunctions(UpdateDeviceDetection));
		UpdateFunctions = (updateFunctions)Delegate.Combine(UpdateFunctions, new updateFunctions(UpdateResetOrientation));
		OVRMessenger.AddListener<Device, bool>("Sensor_Attached", UpdateDeviceDetectionMsgCallback);
		MagCal.SetInitialCalibarationState();
		UpdateFunctions = (updateFunctions)Delegate.Combine(UpdateFunctions, new updateFunctions(MagCal.UpdateMagYawDriftCorrection));
		MagCal.SetOVRCameraController(ref CameraController);
		Crosshair.Init();
		Crosshair.SetCrosshairTexture(ref CrosshairImage);
		Crosshair.SetOVRCameraController(ref CameraController);
		Crosshair.SetOVRPlayerController(ref PlayerController);
		UpdateFunctions = (updateFunctions)Delegate.Combine(UpdateFunctions, new updateFunctions(Crosshair.UpdateCrosshair));
		CheckIfRiftPresent();
		ScenesVisible = false;
	}

	private void Update()
	{
		if (!LoadingLevel)
		{
			UpdateFunctions();
			if (AsunderInput.GetKeyDown(KeyCode.F11))
			{
				Screen.fullScreen = !Screen.fullScreen;
			}
			if (AsunderInput.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			}
		}
	}

	private void UpdateFPS()
	{
		TimeLeft -= Time.deltaTime;
		Accum += Time.timeScale / Time.deltaTime;
		Frames++;
		if ((double)TimeLeft <= 0.0)
		{
			float num = Accum / (float)Frames;
			if (ShowVRVars)
			{
				strFPS = string.Format("FPS: {0:F2}", num);
			}
			TimeLeft += UpdateInterval;
			Accum = 0f;
			Frames = 0;
		}
	}

	private void UpdateIPD()
	{
		if (AsunderInput.GetKeyDown(KeyCode.Equals))
		{
			float ipd = 0f;
			CameraController.GetIPD(ref ipd);
			ipd += IPDIncrement;
			CameraController.SetIPD(ipd);
		}
		else if (AsunderInput.GetKeyDown(KeyCode.Minus))
		{
			float ipd2 = 0f;
			CameraController.GetIPD(ref ipd2);
			ipd2 -= IPDIncrement;
			CameraController.SetIPD(ipd2);
		}
		if (ShowVRVars)
		{
			float ipd3 = 0f;
			CameraController.GetIPD(ref ipd3);
			strIPD = string.Format("IPD (mm): {0:F4}", ipd3 * 1000f);
		}
	}

	private void UpdatePrediction()
	{
		if (AsunderInput.GetKeyDown(KeyCode.P))
		{
			if (!CameraController.PredictionOn)
			{
				CameraController.PredictionOn = true;
			}
			else
			{
				CameraController.PredictionOn = false;
			}
		}
		if (CameraController.PredictionOn)
		{
			float num = OVRDevice.GetPredictionTime(0);
			if (AsunderInput.GetKeyDown(KeyCode.Comma))
			{
				num -= PredictionIncrement;
			}
			else if (AsunderInput.GetKeyDown(KeyCode.Period))
			{
				num += PredictionIncrement;
			}
			OVRDevice.SetPredictionTime(0, num);
			num = OVRDevice.GetPredictionTime(0) * 1000f;
			if (ShowVRVars)
			{
				strPrediction = string.Format("Pred (ms): {0:F3}", num);
			}
		}
		else
		{
			strPrediction = "Pred: OFF";
		}
	}

	private void UpdateFOV()
	{
		if (AsunderInput.GetKeyDown(KeyCode.LeftBracket))
		{
			float verticalFOV = 0f;
			CameraController.GetVerticalFOV(ref verticalFOV);
			verticalFOV -= FOVIncrement;
			CameraController.SetVerticalFOV(verticalFOV);
		}
		else if (AsunderInput.GetKeyDown(KeyCode.RightBracket))
		{
			float verticalFOV2 = 0f;
			CameraController.GetVerticalFOV(ref verticalFOV2);
			verticalFOV2 += FOVIncrement;
			CameraController.SetVerticalFOV(verticalFOV2);
		}
		if (ShowVRVars)
		{
			float verticalFOV3 = 0f;
			CameraController.GetVerticalFOV(ref verticalFOV3);
			strFOV = string.Format("FOV (deg): {0:F3}", verticalFOV3);
		}
	}

	private void UpdateDistortionCoefs()
	{
		float distK = 0f;
		float distK2 = 0f;
		float distK3 = 0f;
		float distK4 = 0f;
		CameraController.GetDistortionCoefs(ref distK, ref distK2, ref distK3, ref distK4);
		if (AsunderInput.GetKeyDown(KeyCode.Alpha1))
		{
			distK2 -= DistKIncrement;
		}
		else if (AsunderInput.GetKeyDown(KeyCode.Alpha2))
		{
			distK2 += DistKIncrement;
		}
		if (AsunderInput.GetKeyDown(KeyCode.Alpha3))
		{
			distK3 -= DistKIncrement;
		}
		else if (AsunderInput.GetKeyDown(KeyCode.Alpha4))
		{
			distK3 += DistKIncrement;
		}
		CameraController.SetDistortionCoefs(distK, distK2, distK3, distK4);
	}

	private void UpdateEyeHeightOffset()
	{
		if (AsunderInput.GetKeyDown(KeyCode.Alpha5))
		{
			Vector3 neckPosition = Vector3.zero;
			CameraController.GetNeckPosition(ref neckPosition);
			neckPosition.y -= HeightIncrement;
			CameraController.SetNeckPosition(neckPosition);
		}
		else if (AsunderInput.GetKeyDown(KeyCode.Alpha6))
		{
			Vector3 neckPosition2 = Vector3.zero;
			CameraController.GetNeckPosition(ref neckPosition2);
			neckPosition2.y += HeightIncrement;
			CameraController.SetNeckPosition(neckPosition2);
		}
		if (ShowVRVars)
		{
			float eyeHeight = 0f;
			CameraController.GetPlayerEyeHeight(ref eyeHeight);
			strHeight = string.Format("Eye Height (m): {0:F3}", eyeHeight);
		}
	}

	private void UpdateSpeedAndRotationScaleMultiplier()
	{
		float moveScaleMultiplier = 0f;
		PlayerController.GetMoveScaleMultiplier(ref moveScaleMultiplier);
		if (AsunderInput.GetKeyDown(KeyCode.Alpha7))
		{
			moveScaleMultiplier -= SpeedRotationIncrement;
		}
		else if (AsunderInput.GetKeyDown(KeyCode.Alpha8))
		{
			moveScaleMultiplier += SpeedRotationIncrement;
		}
		PlayerController.SetMoveScaleMultiplier(moveScaleMultiplier);
		float rotationScaleMultiplier = 0f;
		PlayerController.GetRotationScaleMultiplier(ref rotationScaleMultiplier);
		if (AsunderInput.GetKeyDown(KeyCode.Alpha9))
		{
			rotationScaleMultiplier -= SpeedRotationIncrement;
		}
		else if (AsunderInput.GetKeyDown(KeyCode.Alpha0))
		{
			rotationScaleMultiplier += SpeedRotationIncrement;
		}
		PlayerController.SetRotationScaleMultiplier(rotationScaleMultiplier);
		if (ShowVRVars)
		{
			strSpeedRotationMultipler = string.Format("Spd.X: {0:F2} Rot.X: {1:F2}", moveScaleMultiplier, rotationScaleMultiplier);
		}
	}

	private void UpdatePlayerControllerMovement()
	{
		if (PlayerController != null)
		{
			PlayerController.SetHaltUpdateMovement(ScenesVisible);
		}
	}

	private void UpdateSelectCurrentLevel()
	{
		ShowLevels();
		if (ScenesVisible)
		{
			CurrentLevel = GetCurrentLevel();
			if (Scenes.Length != 0 && (OVRGamepadController.GPC_GetButton(0) || AsunderInput.GetKeyDown(KeyCode.Return)))
			{
				LoadingLevel = true;
				SceneManager.LoadSceneAsync(Scenes[CurrentLevel]);
			}
		}
	}

	private bool ShowLevels()
	{
		if (Scenes.Length == 0)
		{
			ScenesVisible = false;
			return ScenesVisible;
		}
		bool flag = false;
		if (OVRGamepadController.GPC_GetButton(8))
		{
			flag = true;
		}
		if ((!PrevStartDown && flag) || AsunderInput.GetKeyDown(KeyCode.RightShift))
		{
			if (ScenesVisible)
			{
				ScenesVisible = false;
			}
			else
			{
				ScenesVisible = true;
			}
		}
		PrevStartDown = flag;
		return ScenesVisible;
	}

	private int GetCurrentLevel()
	{
		bool flag = false;
		if (OVRGamepadController.GPC_GetButton(5))
		{
			flag = true;
		}
		bool flag2 = false;
		if (OVRGamepadController.GPC_GetButton(5))
		{
			flag2 = true;
		}
		if ((!PrevHatDown && flag) || AsunderInput.GetKeyDown(KeyCode.DownArrow))
		{
			CurrentLevel = (CurrentLevel + 1) % SceneNames.Length;
		}
		else if ((!PrevHatUp && flag2) || AsunderInput.GetKeyDown(KeyCode.UpArrow))
		{
			CurrentLevel--;
			if (CurrentLevel < 0)
			{
				CurrentLevel = SceneNames.Length - 1;
			}
		}
		PrevHatDown = flag;
		PrevHatUp = flag2;
		return CurrentLevel;
	}

	private void OnGUI()
	{
		if (Event.current.type != EventType.Repaint)
		{
			return;
		}
		if (AlphaFadeValue > 0f)
		{
			AlphaFadeValue -= Mathf.Clamp01(Time.deltaTime / FadeInTime);
			if (!(AlphaFadeValue < 0f))
			{
				GUI.color = new Color(0f, 0f, 0f, AlphaFadeValue);
				GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), FadeInTexture);
				return;
			}
			AlphaFadeValue = 0f;
		}
		if (GUIRenderObject != null)
		{
			if (ScenesVisible || ShowVRVars || Crosshair.IsCrosshairVisible() || RiftPresentTimeout > 0f || DeviceDetectionTimeout > 0f)
			{
				GUIRenderObject.SetActive(true);
			}
			else
			{
				GUIRenderObject.SetActive(false);
			}
		}
		Vector3 one = Vector3.one;
		if (CameraController.PortraitMode)
		{
			float num = OVRDevice.HResolution;
			float num2 = OVRDevice.VResolution;
			one.x = num2 / num;
			one.y = num / num2;
		}
		Matrix4x4 matrix = GUI.matrix;
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, one);
		RenderTexture renderTexture = RenderTexture.active;
		if (GUIRenderTexture != null)
		{
			RenderTexture.active = GUIRenderTexture;
			GL.Clear(false, true, new Color(0f, 0f, 0f, 0f));
		}
		GuiHelper.SetFontReplace(FontReplace);
		if (!GUIShowRiftDetected())
		{
			GUIShowLevels();
			GUIShowVRVariables();
		}
		Crosshair.OnGUICrosshair();
		RenderTexture.active = renderTexture;
		GUI.matrix = matrix;
	}

	private void GUIShowLevels()
	{
		if (!ScenesVisible)
		{
			return;
		}
		GUI.color = new Color(0f, 0f, 0f, 0.5f);
		GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), FadeInTexture);
		GUI.color = Color.white;
		if (LoadingLevel)
		{
			string text = "LOADING...";
			GuiHelper.StereoBox(StartX, StartY, WidthX, WidthY, ref text, Color.yellow);
			return;
		}
		for (int i = 0; i < SceneNames.Length; i++)
		{
			Color color = ((i != CurrentLevel) ? Color.black : Color.yellow);
			int y = StartY + i * StepY;
			GuiHelper.StereoBox(StartX, y, WidthX, WidthY, ref SceneNames[i], color);
		}
	}

	private void GUIShowVRVariables()
	{
		bool key = AsunderInput.GetKey("space");
		if (!OldSpaceHit && key)
		{
			if (ShowVRVars)
			{
				ShowVRVars = false;
			}
			else
			{
				ShowVRVars = true;
			}
		}
		OldSpaceHit = key;
		int vRVarsSY = VRVarsSY;
		MagCal.GUIMagYawDriftCorrection(VRVarsSX, vRVarsSY, VRVarsWidthX, VRVarsWidthY, ref GuiHelper);
		GuiHelper.StereoBox(VRVarsSX, vRVarsSY += StepY, VRVarsWidthX, VRVarsWidthY, ref strFPS, Color.green);
		if (CameraController != null)
		{
			GuiHelper.StereoBox(VRVarsSX, vRVarsSY += StepY, VRVarsWidthX, VRVarsWidthY, ref strPrediction, Color.white);
			GuiHelper.StereoBox(VRVarsSX, vRVarsSY += StepY, VRVarsWidthX, VRVarsWidthY, ref strIPD, Color.yellow);
			GuiHelper.StereoBox(VRVarsSX, vRVarsSY += StepY, VRVarsWidthX, VRVarsWidthY, ref strFOV, Color.white);
		}
		if (PlayerController != null)
		{
			GuiHelper.StereoBox(VRVarsSX, vRVarsSY += StepY, VRVarsWidthX, VRVarsWidthY, ref strHeight, Color.yellow);
			GuiHelper.StereoBox(VRVarsSX, vRVarsSY += StepY, VRVarsWidthX, VRVarsWidthY, ref strSpeedRotationMultipler, Color.white);
		}
	}

	private void UpdateHandleSnapshots()
	{
		if (AsunderInput.GetKeyDown(KeyCode.F2))
		{
			LoadSnapshot("DEFAULT");
		}
		if (AsunderInput.GetKeyDown(KeyCode.F3))
		{
			if (AsunderInput.GetKey(KeyCode.Tab))
			{
				StoreSnapshot("SNAPSHOT1");
			}
			else
			{
				LoadSnapshot("SNAPSHOT1");
			}
		}
		if (AsunderInput.GetKeyDown(KeyCode.F4))
		{
			if (AsunderInput.GetKey(KeyCode.Tab))
			{
				StoreSnapshot("SNAPSHOT2");
			}
			else
			{
				LoadSnapshot("SNAPSHOT2");
			}
		}
		if (AsunderInput.GetKeyDown(KeyCode.F5))
		{
			if (AsunderInput.GetKey(KeyCode.Tab))
			{
				StoreSnapshot("SNAPSHOT3");
			}
			else
			{
				LoadSnapshot("SNAPSHOT3");
			}
		}
	}

	private bool StoreSnapshot(string snapshotName)
	{
		float ipd = 0f;
		PresetManager.SetCurrentPreset(snapshotName);
		if (CameraController != null)
		{
			CameraController.GetIPD(ref ipd);
			PresetManager.SetPropertyFloat("IPD", ref ipd);
			ipd = OVRDevice.GetPredictionTime(0);
			PresetManager.SetPropertyFloat("PREDICTION", ref ipd);
			CameraController.GetVerticalFOV(ref ipd);
			PresetManager.SetPropertyFloat("FOV", ref ipd);
			Vector3 neckPosition = Vector3.zero;
			CameraController.GetNeckPosition(ref neckPosition);
			PresetManager.SetPropertyFloat("HEIGHT", ref neckPosition.y);
			float distK = 0f;
			float distK2 = 0f;
			float distK3 = 0f;
			float distK4 = 0f;
			CameraController.GetDistortionCoefs(ref distK, ref distK2, ref distK3, ref distK4);
			PresetManager.SetPropertyFloat("DISTORTIONK0", ref distK);
			PresetManager.SetPropertyFloat("DISTORTIONK1", ref distK2);
			PresetManager.SetPropertyFloat("DISTORTIONK2", ref distK3);
			PresetManager.SetPropertyFloat("DISTORTIONK3", ref distK4);
		}
		if (PlayerController != null)
		{
			PlayerController.GetMoveScaleMultiplier(ref ipd);
			PresetManager.SetPropertyFloat("SPEEDMULT", ref ipd);
			PlayerController.GetRotationScaleMultiplier(ref ipd);
			PresetManager.SetPropertyFloat("ROTMULT", ref ipd);
		}
		return true;
	}

	private bool LoadSnapshot(string snapshotName)
	{
		float v = 0f;
		PresetManager.SetCurrentPreset(snapshotName);
		if (CameraController != null)
		{
			if (PresetManager.GetPropertyFloat("IPD", ref v))
			{
				CameraController.SetIPD(v);
			}
			if (PresetManager.GetPropertyFloat("PREDICTION", ref v))
			{
				OVRDevice.SetPredictionTime(0, v);
			}
			if (PresetManager.GetPropertyFloat("FOV", ref v))
			{
				CameraController.SetVerticalFOV(v);
			}
			if (PresetManager.GetPropertyFloat("HEIGHT", ref v))
			{
				Vector3 neckPosition = Vector3.zero;
				CameraController.GetNeckPosition(ref neckPosition);
				neckPosition.y = v;
				CameraController.SetNeckPosition(neckPosition);
			}
			float distK = 0f;
			float distK2 = 0f;
			float distK3 = 0f;
			float distK4 = 0f;
			CameraController.GetDistortionCoefs(ref distK, ref distK2, ref distK3, ref distK4);
			if (PresetManager.GetPropertyFloat("DISTORTIONK0", ref v))
			{
				distK = v;
			}
			if (PresetManager.GetPropertyFloat("DISTORTIONK1", ref v))
			{
				distK2 = v;
			}
			if (PresetManager.GetPropertyFloat("DISTORTIONK2", ref v))
			{
				distK3 = v;
			}
			if (PresetManager.GetPropertyFloat("DISTORTIONK3", ref v))
			{
				distK4 = v;
			}
			CameraController.SetDistortionCoefs(distK, distK2, distK3, distK4);
		}
		if (PlayerController != null)
		{
			if (PresetManager.GetPropertyFloat("SPEEDMULT", ref v))
			{
				PlayerController.SetMoveScaleMultiplier(v);
			}
			if (PresetManager.GetPropertyFloat("ROTMULT", ref v))
			{
				PlayerController.SetRotationScaleMultiplier(v);
			}
		}
		return true;
	}

	private void CheckIfRiftPresent()
	{
		HMDPresent = OVRDevice.IsHMDPresent();
		SensorPresent = OVRDevice.IsSensorPresent(0);
		if (!HMDPresent || !SensorPresent)
		{
			RiftPresentTimeout = 5f;
			if (!HMDPresent && !SensorPresent)
			{
				strRiftPresent = "NO HMD AND SENSOR DETECTED";
			}
			else if (!HMDPresent)
			{
				strRiftPresent = "NO HMD DETECTED";
			}
			else if (!SensorPresent)
			{
				strRiftPresent = "NO SENSOR DETECTED";
			}
		}
	}

	private bool GUIShowRiftDetected()
	{
		if (RiftPresentTimeout > 0f)
		{
			GuiHelper.StereoBox(StartX, StartY, WidthX, WidthY, ref strRiftPresent, Color.white);
			return true;
		}
		if (DeviceDetectionTimeout > 0f)
		{
			GuiHelper.StereoBox(StartX, StartY, WidthX, WidthY, ref strDeviceDetection, Color.white);
			return true;
		}
		return false;
	}

	private void UpdateDeviceDetection()
	{
		if (RiftPresentTimeout > 0f)
		{
			RiftPresentTimeout -= Time.deltaTime;
		}
		if (DeviceDetectionTimeout > 0f)
		{
			DeviceDetectionTimeout -= Time.deltaTime;
		}
	}

	private void UpdateDeviceDetectionMsgCallback(Device device, bool attached)
	{
		if (attached)
		{
			switch (device)
			{
			case Device.HMDSensor:
				strDeviceDetection = "HMD SENSOR ATTACHED";
				break;
			case Device.HMD:
				strDeviceDetection = "HMD ATTACHED";
				break;
			case Device.LatencyTester:
				strDeviceDetection = "LATENCY SENSOR ATTACHED";
				break;
			}
		}
		else
		{
			switch (device)
			{
			case Device.HMDSensor:
				strDeviceDetection = "HMD SENSOR DETACHED";
				break;
			case Device.HMD:
				strDeviceDetection = "HMD DETACHED";
				break;
			case Device.LatencyTester:
				strDeviceDetection = "LATENCY SENSOR DETACHED";
				break;
			}
		}
		if (AlphaFadeValue == 0f)
		{
			DeviceDetectionTimeout = 3f;
		}
	}

	private void UpdateResetOrientation()
	{
		if ((!ScenesVisible && OVRGamepadController.GPC_GetButton(5)) || AsunderInput.GetKeyDown(KeyCode.B))
		{
			OVRDevice.ResetOrientation(0);
		}
	}
}
