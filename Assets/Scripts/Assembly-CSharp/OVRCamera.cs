using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OVRCamera : OVRComponent
{
	private RenderTexture CameraTexture;

	private Material ColorOnlyMaterial;

	private OVRCameraController CameraController;

	private Camera _cam;

	[HideInInspector]
	public bool IsLeftEye;

	[HideInInspector]
	public Vector3 NeckPosition = new Vector3(0f, 0f, 0f);

	[HideInInspector]
	public Vector3 EyePosition = new Vector3(0f, 0.09f, 0.16f);

	private new void Awake()
	{
		base.Awake();
		_cam = GetComponent<Camera>();
		IsLeftEye = base.gameObject.name == "CameraLeft";
		if (ColorOnlyMaterial == null)
		{
			Shader s = Shader.Find("Hidden/Internal-Colored");
			if (s != null)
			{
				ColorOnlyMaterial = new Material(s);
				ColorOnlyMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
		}
	}

	private new void Start()
	{
		base.Start();
		if (base.transform.parent != null)
		{
			CameraController = base.transform.parent.GetComponent<OVRCameraController>();
		}
		if (CameraController == null)
		{
			Debug.LogWarning("WARNING: OVRCameraController not found!");
		}
	}

	private new void Update()
	{
		base.Update();
	}

	private new void LateUpdate()
	{
		if (CameraController != null && !CameraController.CallInPreRender)
		{
			SetCameraOrientation();
		}
	}

	private void OnPreCull()
	{
		if (CameraController != null && !CameraController.CallInPreRender)
		{
			SetCameraOrientation();
		}
	}

	private void OnPreRender()
	{
		if (CameraController != null && CameraController.CallInPreRender)
		{
			SetCameraOrientation();
		}
		if (CameraController != null && CameraController.WireMode)
		{
			GL.wireframe = true;
		}
		if (CameraTexture != null && _cam != null)
		{
			Graphics.SetRenderTarget(CameraTexture);
			GL.Clear(true, true, _cam.backgroundColor);
		}
	}

	private void OnPostRender()
	{
		if (CameraController != null && CameraController.WireMode)
		{
			GL.wireframe = false;
		}
	}

	// Modern Unity 6: OpenXR compositor handles eye distortion natively.
	// Redundant pass-through OnRenderImage blit removed to prevent intermediate render texture overhead.

	public bool TryGetControllerWorldPose(UnityEngine.XR.XRNode hand, out Vector3 pos, out Quaternion rot)
	{
		pos = Vector3.zero;
		rot = Quaternion.identity;
		if (CameraController == null || base.transform.parent == null)
		{
			return false;
		}
		if (!AsunderInput.GetControllerLocalPose(hand, out Quaternion localRot, out Vector3 localPos))
		{
			return false;
		}
		float yRotation = 0f;
		CameraController.GetYRotation(ref yRotation);
		Quaternion orientationOffset = Quaternion.identity;
		CameraController.GetOrientationOffset(ref orientationOffset);
		Quaternion rigBaseRot = orientationOffset * Quaternion.Euler(0f, yRotation, 0f);
		Vector3 neckCenter = base.transform.parent.position + rigBaseRot * (NeckPosition + new Vector3(0f, EyePosition.y, EyePosition.z));
		pos = neckCenter + rigBaseRot * localPos;
		rot = rigBaseRot * localRot;
		return true;
	}

	private void SetCameraOrientation()
	{
		if (CameraController == null || _cam == null || base.transform.parent == null)
		{
			return;
		}

		float yRotation = 0f;
		CameraController.GetYRotation(ref yRotation);

		Quaternion orientationOffset = Quaternion.identity;
		CameraController.GetOrientationOffset(ref orientationOffset);

		Quaternion rigBaseRot = orientationOffset * Quaternion.Euler(0f, yRotation, 0f);

		float ipd = 0.064f;
		CameraController.GetIPD(ref ipd);

		Quaternion localEyeRot = Quaternion.identity;
		Vector3 localEyePos = Vector3.zero;
		if (CameraController.EnableOrientation)
		{
			AsunderInput.GetEyePose(IsLeftEye, ipd, out localEyeRot, out localEyePos);
		}

		Quaternion worldRot = rigBaseRot * localEyeRot;
		Vector3 baseNeckCenter = base.transform.parent.position + rigBaseRot * (NeckPosition + new Vector3(0f, EyePosition.y, EyePosition.z));
		Vector3 worldPos = baseNeckCenter + rigBaseRot * localEyePos;

		base.transform.rotation = worldRot;
		base.transform.position = worldPos;

		if (CameraController.TrackerRotatesY && !IsLeftEye)
		{
			Vector3 eulerAngles = worldRot.eulerAngles;
			eulerAngles.x = 0f;
			eulerAngles.z = 0f;
			base.transform.parent.eulerAngles = eulerAngles;
		}

		if (AsunderInput.IsOpenXRActive)
		{
			Camera.StereoscopicEye stereoEye = IsLeftEye ? Camera.StereoscopicEye.Left : Camera.StereoscopicEye.Right;
			_cam.SetStereoViewMatrix(stereoEye, _cam.worldToCameraMatrix);
		}
	}

	public void SetPerspectiveOffset(ref Vector3 offset)
	{
		if (_cam != null && !AsunderInput.IsOpenXRActive)
		{
			_cam.ResetProjectionMatrix();
		}
	}
}
