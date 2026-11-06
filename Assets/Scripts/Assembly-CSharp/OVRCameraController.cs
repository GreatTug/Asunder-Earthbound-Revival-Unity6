using UnityEngine;

public class OVRCameraController : OVRComponent
{
	private bool UpdateCamerasDirtyFlag;

	private Camera CameraLeft;

	private Camera CameraRight;

	private float LensOffsetLeft;

	private float LensOffsetRight;

	private float AspectRatio = 1f;

	private float DistK0;

	private float DistK1;

	private float DistK2;

	private float DistK3;

	private Quaternion OrientationOffset = Quaternion.identity;

	private float YRotation;

	[SerializeField]
	private float ipd = 0.064f;

	[SerializeField]
	private float verticalFOV = 110f;

	private float FOV_Right = 110f;

	private float FOV_Left = 110f;

	public Vector3 CameraRootPosition = new Vector3(0f, 1f, 0f);

	public Vector3 NeckPosition = new Vector3(0f, 0.7f, 0f);

	public Vector3 EyeCenterPosition = new Vector3(0f, 0.15f, 0.09f);

	public bool UsePlayerEyeHeight;

	private bool PrevUsePlayerEyeHeight;

	public Transform FollowOrientation;

	public bool TrackerRotatesY;

	public bool PortraitMode;


	public bool EnableOrientation = true;

	public bool PredictionOn = true;

	public bool CallInPreRender;

	public bool WireMode;

	public bool LensCorrection = true;

	public bool Chromatic = true;

	[SerializeField]
	private Color backgroundColor = new Color(0f, 0f, 0f, 1f);

	[SerializeField]
	private float nearClipPlane = 0.04f;

	[SerializeField]
	private float farClipPlane = 2000f;

	private bool _lastOpenXRActive;

	private int _origLeftCullingMask;

	private int _origRightCullingMask;

	private bool _cachedOrigMasks;

	public float IPD
	{
		get
		{
			return ipd;
		}
		set
		{
			ipd = value;
			UpdateCamerasDirtyFlag = true;
		}
	}

	private float VerticalFOV
	{
		get
		{
			return verticalFOV;
		}
		set
		{
			verticalFOV = value;
			UpdateCamerasDirtyFlag = true;
		}
	}

	private Color BackgroundColor
	{
		get
		{
			return backgroundColor;
		}
		set
		{
			backgroundColor = value;
			UpdateCamerasDirtyFlag = true;
		}
	}

	private float NearClipPlane
	{
		get
		{
			return nearClipPlane;
		}
		set
		{
			nearClipPlane = value;
			UpdateCamerasDirtyFlag = true;
		}
	}

	private float FarClipPlane
	{
		get
		{
			return farClipPlane;
		}
		set
		{
			farClipPlane = value;
			UpdateCamerasDirtyFlag = true;
		}
	}

	private new void Awake()
	{
		base.Awake();
		Camera[] componentsInChildren = base.gameObject.GetComponentsInChildren<Camera>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].name == "CameraLeft")
			{
				CameraLeft = componentsInChildren[i];
			}
			if (componentsInChildren[i].name == "CameraRight")
			{
				CameraRight = componentsInChildren[i];
			}
		}
		if (CameraLeft != null && CameraRight != null)
		{
			if (!_cachedOrigMasks)
			{
				_origLeftCullingMask = CameraLeft.cullingMask;
				_origRightCullingMask = CameraRight.cullingMask;
				_cachedOrigMasks = true;
			}
			AudioListener leftListener = CameraLeft.GetComponent<AudioListener>();
			AudioListener rightListener = CameraRight.GetComponent<AudioListener>();
			if (leftListener != null && rightListener != null)
			{
				leftListener.enabled = false;
			}
			AudioListener[] allListeners = Object.FindObjectsByType<AudioListener>(FindObjectsInactive.Include);
			AudioListener activeVRListener = rightListener != null ? rightListener : leftListener;
			if (activeVRListener != null)
			{
				activeVRListener.enabled = true;
				for (int alIdx = 0; alIdx < allListeners.Length; alIdx++)
				{
					if (allListeners[alIdx] != activeVRListener)
					{
						allListeners[alIdx].enabled = false;
					}
				}
			}
		}
		else
		{
			Debug.LogWarning("WARNING: Unity Cameras in OVRCameraController not found!");
		}
	}

	private new void Start()
	{
		base.Start();
		BackgroundColor = backgroundColor;
		NearClipPlane = nearClipPlane;
		FarClipPlane = farClipPlane;
		InitCameraControllerVariables();
		_lastOpenXRActive = AsunderInput.IsOpenXRActive;
		UpdateCamerasDirtyFlag = true;
		UpdateCameras();
		SetMaximumVisualQuality();
		if (GetComponent<AsunderQuestControllerRay>() == null)
		{
			base.gameObject.AddComponent<AsunderQuestControllerRay>();
		}
	}

	private new void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.C))
		{
			AsunderInput.IsCardboardMode = !AsunderInput.IsCardboardMode;
			UpdateCamerasDirtyFlag = true;
			Debug.Log("[AsunderCardboard] Cardboard stereo split-screen toggled: " + AsunderInput.IsCardboardMode);
		}
		if (_lastOpenXRActive != AsunderInput.IsOpenXRActive)
		{
			_lastOpenXRActive = AsunderInput.IsOpenXRActive;
			UpdateCamerasDirtyFlag = true;
			SetMaximumVisualQuality();
		}
		UpdateCameras();
		if (AsunderInput.GetResetOrientationUp())
		{
			OVRDevice.ResetOrientation(0);
		}
	}

	public void InitCameraControllerVariables()
	{
		OVRDevice.GetIPD(ref ipd);
		OVRDevice.CalculatePhysicalLensOffsets(ref LensOffsetLeft, ref LensOffsetRight);
		VerticalFOV = OVRDevice.VerticalFOV();
		AspectRatio = OVRDevice.CalculateAspectRatio();
		OVRDevice.GetDistortionCorrectionCoefficients(ref DistK0, ref DistK1, ref DistK2, ref DistK3);
		PortraitMode = false;
		if (FollowOrientation != null)
		{
			OrientationOffset = FollowOrientation.rotation;
		}
		else
		{
			OrientationOffset = base.transform.rotation;
		}
	}

	private void UpdateCameras()
	{
		if (FollowOrientation != null)
		{
			OrientationOffset = FollowOrientation.rotation;
		}
		else
		{
			OrientationOffset = base.transform.rotation;
		}
		UpdatePlayerEyeHeight();
		if (UpdateCamerasDirtyFlag)
		{
			bool xrActive = AsunderInput.IsOpenXRActive;
			if (xrActive)
			{
				ConfigureOpenXREyeCamera(CameraLeft, true, FOV_Left);
				ConfigureOpenXREyeCamera(CameraRight, false, FOV_Right);
			}
			else
			{
				bool isCardboard = AsunderInput.IsCardboardMode || Application.isMobilePlatform;
				if (isCardboard && CameraLeft != null && CameraRight != null)
				{
					ConfigureCardboardEyeCamera(CameraLeft, true, FOV_Left);
					ConfigureCardboardEyeCamera(CameraRight, false, FOV_Right);
				}
				else
				{
					if (CameraLeft != null)
					{
						CameraLeft.stereoTargetEye = StereoTargetEyeMask.None;
						CameraLeft.enabled = false;
					}
					ConfigureDesktopFallbackCamera(CameraRight, FOV_Right);
				}
			}
			UpdateCamerasDirtyFlag = false;
		}
	}

	private void ConfigureCardboardEyeCamera(Camera cam, bool isLeft, float afov)
	{
		if (cam == null) return;
		cam.enabled = true;
		cam.rect = isLeft ? new Rect(0f, 0f, 0.5f, 1f) : new Rect(0.5f, 0f, 0.5f, 1f);
		cam.stereoTargetEye = StereoTargetEyeMask.None;
		if (_cachedOrigMasks)
		{
			cam.cullingMask = isLeft ? _origLeftCullingMask : _origRightCullingMask;
		}
		cam.ResetStereoViewMatrices();
		cam.ResetProjectionMatrix();
		cam.ResetAspect();
		cam.backgroundColor = Color.black;
		cam.nearClipPlane = Mathf.Max(0.035f, nearClipPlane);
		cam.farClipPlane = farClipPlane > 0f ? farClipPlane : 2750f;
		cam.fieldOfView = Mathf.Clamp(afov > 0f ? afov : 80f, 45f, 95f);

		OVRLensCorrection lens = cam.GetComponent<OVRLensCorrection>();
		if (lens != null)
		{
			lens.enabled = false;
		}

		OVRCamera ovrCam = cam.GetComponent<OVRCamera>();
		if (ovrCam != null)
		{
			ovrCam.IsLeftEye = isLeft;
			ovrCam.NeckPosition = NeckPosition;
			ovrCam.EyePosition = EyeCenterPosition;
		}
	}

	private void ConfigureOpenXREyeCamera(Camera cam, bool isLeft, float afov)
	{
		if (cam == null)
		{
			return;
		}
		cam.enabled = true;
		cam.rect = new Rect(0f, 0f, 1f, 1f);
		cam.stereoTargetEye = isLeft ? StereoTargetEyeMask.Left : StereoTargetEyeMask.Right;
		if (_cachedOrigMasks)
		{
			cam.cullingMask = isLeft ? _origLeftCullingMask : _origRightCullingMask;
		}
		cam.backgroundColor = Color.black;
		cam.nearClipPlane = Mathf.Max(0.035f, nearClipPlane);
		cam.farClipPlane = farClipPlane > 0f ? farClipPlane : 2750f;
		cam.ResetAspect();
		cam.ResetProjectionMatrix();

		OVRLensCorrection lens = cam.GetComponent<OVRLensCorrection>();
		if (lens != null)
		{
			lens.enabled = false;
		}

		OVRCamera ovrCam = cam.GetComponent<OVRCamera>();
		if (ovrCam != null)
		{
			ovrCam.IsLeftEye = isLeft;
			ovrCam.NeckPosition = NeckPosition;
			ovrCam.EyePosition = EyeCenterPosition;
		}
	}

	private void ConfigureDesktopFallbackCamera(Camera cam, float afov)
	{
		if (cam == null)
		{
			return;
		}
		cam.enabled = true;
		cam.rect = new Rect(0f, 0f, 1f, 1f);
		cam.stereoTargetEye = StereoTargetEyeMask.None;
		if (_cachedOrigMasks)
		{
			cam.cullingMask = _origRightCullingMask | _origLeftCullingMask;
		}
		cam.ResetStereoViewMatrices();
		cam.ResetProjectionMatrix();
		cam.ResetAspect();
		cam.backgroundColor = Color.black;
		cam.nearClipPlane = Mathf.Max(0.035f, nearClipPlane);
		cam.farClipPlane = farClipPlane > 0f ? farClipPlane : 2750f;
		cam.fieldOfView = Mathf.Clamp(afov, 30f, 75f);

		OVRLensCorrection lens = cam.GetComponent<OVRLensCorrection>();
		if (lens != null)
		{
			lens.enabled = false;
		}

		OVRCamera ovrCam = cam.GetComponent<OVRCamera>();
		if (ovrCam != null)
		{
			ovrCam.IsLeftEye = false;
			ovrCam.NeckPosition = NeckPosition;
			ovrCam.EyePosition = EyeCenterPosition;
		}
	}

	private void UpdatePlayerEyeHeight()
	{
		if (UsePlayerEyeHeight && !PrevUsePlayerEyeHeight)
		{
			float eyeHeight = 0f;
			if (OVRDevice.GetPlayerEyeHeight(ref eyeHeight))
			{
				NeckPosition.y = eyeHeight - CameraRootPosition.y - EyeCenterPosition.y;
			}
		}
		PrevUsePlayerEyeHeight = UsePlayerEyeHeight;
	}

	public void SetCameras(ref Camera cameraLeft, ref Camera cameraRight)
	{
		CameraLeft = cameraLeft;
		CameraRight = cameraRight;
		UpdateCamerasDirtyFlag = true;
	}

	public void GetIPD(ref float ipd)
	{
		ipd = IPD;
	}

	public void SetIPD(float ipd)
	{
		IPD = ipd;
		UpdateCamerasDirtyFlag = true;
	}

	public void GetVerticalFOV(ref float verticalFOV)
	{
		verticalFOV = VerticalFOV;
	}

	public void SetVerticalFOV(float verticalFOV)
	{
		VerticalFOV = verticalFOV;
		UpdateCamerasDirtyFlag = true;
	}

	public void GetAspectRatio(ref float aspecRatio)
	{
		aspecRatio = AspectRatio;
	}

	public void SetAspectRatio(float aspectRatio)
	{
		AspectRatio = aspectRatio;
		UpdateCamerasDirtyFlag = true;
	}

	public void GetDistortionCoefs(ref float distK0, ref float distK1, ref float distK2, ref float distK3)
	{
		distK0 = DistK0;
		distK1 = DistK1;
		distK2 = DistK2;
		distK3 = DistK3;
	}

	public void SetDistortionCoefs(float distK0, float distK1, float distK2, float distK3)
	{
		DistK0 = distK0;
		DistK1 = distK1;
		DistK2 = distK2;
		DistK3 = distK3;
		UpdateCamerasDirtyFlag = true;
	}

	public void GetCameraRootPosition(ref Vector3 cameraRootPosition)
	{
		cameraRootPosition = CameraRootPosition;
	}

	public void SetCameraRootPosition(ref Vector3 cameraRootPosition)
	{
		CameraRootPosition = cameraRootPosition;
		UpdateCamerasDirtyFlag = true;
	}

	public void GetNeckPosition(ref Vector3 neckPosition)
	{
		neckPosition = NeckPosition;
	}

	public void SetNeckPosition(Vector3 neckPosition)
	{
		if (!UsePlayerEyeHeight)
		{
			NeckPosition = neckPosition;
			UpdateCamerasDirtyFlag = true;
		}
	}

	public void GetEyeCenterPosition(ref Vector3 eyeCenterPosition)
	{
		eyeCenterPosition = EyeCenterPosition;
	}

	public void SetEyeCenterPosition(Vector3 eyeCenterPosition)
	{
		EyeCenterPosition = eyeCenterPosition;
		UpdateCamerasDirtyFlag = true;
	}

	public void GetOrientationOffset(ref Quaternion orientationOffset)
	{
		orientationOffset = OrientationOffset;
	}

	public void SetOrientationOffset(Quaternion orientationOffset)
	{
		OrientationOffset = orientationOffset;
	}

	public void GetYRotation(ref float yRotation)
	{
		yRotation = YRotation;
	}

	public void SetYRotation(float yRotation)
	{
		YRotation = yRotation;
	}

	public void GetTrackerRotatesY(ref bool trackerRotatesY)
	{
		trackerRotatesY = TrackerRotatesY;
	}

	public void SetTrackerRotatesY(bool trackerRotatesY)
	{
		TrackerRotatesY = trackerRotatesY;
	}

	public bool GetCameraOrientationEulerAngles(ref Vector3 angles)
	{
		if (CameraRight == null)
		{
			return false;
		}
		angles = CameraRight.transform.rotation.eulerAngles;
		return true;
	}

	public bool GetCameraOrientation(ref Quaternion quaternion)
	{
		if (CameraRight == null)
		{
			return false;
		}
		quaternion = CameraRight.transform.rotation;
		return true;
	}

	public bool GetCameraPosition(ref Vector3 position)
	{
		if (CameraRight == null)
		{
			return false;
		}
		position = CameraRight.transform.position;
		return true;
	}

	public void GetCamera(ref Camera camera)
	{
		camera = CameraRight;
	}

	public bool AttachGameObjectToCamera(ref GameObject gameObject)
	{
		if (CameraRight == null)
		{
			return false;
		}
		gameObject.transform.parent = CameraRight.transform;
		return true;
	}

	public bool DetachGameObjectFromCamera(ref GameObject gameObject)
	{
		if (CameraRight != null && CameraRight.transform == gameObject.transform.parent)
		{
			gameObject.transform.parent = null;
			return true;
		}
		return false;
	}

	public bool GetPlayerEyeHeight(ref float eyeHeight)
	{
		eyeHeight = CameraRootPosition.y + NeckPosition.y + EyeCenterPosition.y;
		return true;
	}

	public void SetMaximumVisualQuality()
	{
		if (AsunderQuestRuntime.IsQuestBuild)
		{
			AsunderQuestRuntime.ApplyQuestQuality();
			return;
		}
		QualitySettings.softParticles = true;
		QualitySettings.softVegetation = true;
		QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
		QualitySettings.antiAliasing = 8;
		QualitySettings.realtimeReflectionProbes = true;
		QualitySettings.billboardsFaceCameraPosition = true;
		QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
		QualitySettings.shadowCascades = 4;
		QualitySettings.shadowDistance = 150f;
		QualitySettings.vSyncCount = AsunderInput.IsOpenXRActive ? 0 : 1;
		if (UnityEngine.XR.XRSettings.isDeviceActive)
		{
			UnityEngine.XR.XRSettings.eyeTextureResolutionScale = 1.35f;
		}
	}

	public void SetFOV_Right(float aFOV)
	{
		FOV_Right = aFOV;
		UpdateCamerasDirtyFlag = true;
	}

	public float GetFOV()
	{
		return verticalFOV;
	}
}
