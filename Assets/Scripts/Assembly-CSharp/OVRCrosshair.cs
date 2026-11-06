using UnityEngine;

public class OVRCrosshair
{
	public Texture ImageCrosshair;

	public OVRCameraController CameraController;

	public OVRPlayerController PlayerController;

	public float FadeTime = 0.3f;

	public float FadeScale = 0.6f;

	public float CrosshairDistance = 1f;

	private float DeadZoneX = 400f;

	private float DeadZoneY = 75f;

	private float ScaleSpeedX = 7f;

	private float ScaleSpeedY = 7f;

	private bool DisplayCrosshair;

	private bool CollisionWithGeometry;

	private float FadeVal;

	private Camera MainCam;

	private float XL;

	private float YL;

	private float ScreenWidth = 1280f;

	private float ScreenHeight = 800f;

	public void SetCrosshairTexture(ref Texture image)
	{
		ImageCrosshair = image;
	}

	public void SetOVRCameraController(ref OVRCameraController cameraController)
	{
		CameraController = cameraController;
		CameraController.GetCamera(ref MainCam);
		if (CameraController.PortraitMode)
		{
			float deadZoneX = DeadZoneX;
			DeadZoneX = DeadZoneY;
			DeadZoneY = deadZoneX;
		}
	}

	public void SetOVRPlayerController(ref OVRPlayerController playerController)
	{
		PlayerController = playerController;
	}

	public bool IsCrosshairVisible()
	{
		if (FadeVal > 0f)
		{
			return true;
		}
		return false;
	}

	public void Init()
	{
		DisplayCrosshair = false;
		CollisionWithGeometry = false;
		FadeVal = 0f;
		ScreenWidth = Screen.width;
		ScreenHeight = Screen.height;
		XL = ScreenWidth * 0.5f;
		YL = ScreenHeight * 0.5f;
	}

	public void UpdateCrosshair()
	{
		ShouldDisplayCrosshair();
		CollisionWithGeometryCheck();
	}

	public void OnGUICrosshair()
	{
		if (DisplayCrosshair && !CollisionWithGeometry)
		{
			FadeVal += Time.deltaTime / FadeTime;
		}
		else
		{
			FadeVal -= Time.deltaTime / FadeTime;
		}
		FadeVal = Mathf.Clamp(FadeVal, 0f, 1f);
		if (PlayerController != null)
		{
			PlayerController.SetAllowMouseRotation(false);
		}
		if (!(ImageCrosshair != null) || FadeVal == 0f)
		{
			return;
		}
		if (PlayerController != null)
		{
			PlayerController.SetAllowMouseRotation(true);
		}
		GUI.color = new Color(1f, 1f, 1f, FadeVal * FadeScale);
		XL += AsunderInput.GetAxis("Mouse X") * ScaleSpeedX;
		if (XL < DeadZoneX)
		{
			if (PlayerController != null)
			{
				PlayerController.SetAllowMouseRotation(false);
			}
			XL = DeadZoneX - 0.001f;
		}
		else if (XL > (float)Screen.width - DeadZoneX)
		{
			if (PlayerController != null)
			{
				PlayerController.SetAllowMouseRotation(false);
			}
			XL = ScreenWidth - DeadZoneX + 0.001f;
		}
		YL -= AsunderInput.GetAxis("Mouse Y") * ScaleSpeedY;
		if (YL < DeadZoneY)
		{
			if (YL < 0f)
			{
				YL = 0f;
			}
		}
		else if (YL > ScreenHeight - DeadZoneY && YL > ScreenHeight)
		{
			YL = ScreenHeight;
		}
		bool allowMouseRotation = true;
		if (PlayerController != null)
		{
			PlayerController.GetAllowMouseRotation(ref allowMouseRotation);
		}
		if (allowMouseRotation)
		{
			GUI.DrawTexture(new Rect(XL - (float)ImageCrosshair.width * 0.5f, YL - (float)ImageCrosshair.height * 0.5f, ImageCrosshair.width, ImageCrosshair.height), ImageCrosshair);
		}
		GUI.color = Color.white;
	}

	private bool ShouldDisplayCrosshair()
	{
		if (AsunderInput.GetKeyDown(KeyCode.C))
		{
			if (!DisplayCrosshair)
			{
				DisplayCrosshair = true;
				XL = ScreenWidth * 0.5f;
				YL = ScreenHeight * 0.5f;
			}
			else
			{
				DisplayCrosshair = false;
			}
		}
		return DisplayCrosshair;
	}

	private bool CollisionWithGeometryCheck()
	{
		CollisionWithGeometry = false;
		Vector3 position = MainCam.transform.position;
		Vector3 forward = Vector3.forward;
		forward = MainCam.transform.rotation * forward;
		forward *= CrosshairDistance;
		Vector3 end = position + forward;
		RaycastHit hitInfo;
		if (Physics.Linecast(position, end, out hitInfo) && !hitInfo.collider.isTrigger)
		{
			CollisionWithGeometry = true;
		}
		return CollisionWithGeometry;
	}
}
