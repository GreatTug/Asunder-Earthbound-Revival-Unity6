using UnityEngine;

public class OVRMagCalibration
{
	public enum MagCalibrationState
	{
		MagUncalibrated = 0,
		MagDisabled = 1,
		MagReady = 2
	}

	private MagCalibrationState MagCalState;

	private Vector3 CurEulerRef = Vector3.zero;

	private bool MagShowGeometry;

	public OVRCameraController CameraController;

	public GameObject GeometryReference;

	public GameObject GeometryCompass;

	public Material GeometryReferenceMarkMat;

	public void SetInitialCalibarationState()
	{
		if (OVRDevice.IsMagCalibrated(0) && OVRDevice.IsYawCorrectionEnabled(0))
		{
			MagCalState = MagCalibrationState.MagReady;
		}
		else
		{
			MagCalState = MagCalibrationState.MagUncalibrated;
		}
	}

	public void SetOVRCameraController(ref OVRCameraController cameraController)
	{
		CameraController = cameraController;
	}

	public void ShowGeometry(bool show)
	{
		if (GeometryReference == null)
		{
			GeometryReference = Object.Instantiate(Resources.Load("OVRMagReference")) as GameObject;
			GeometryReferenceMarkMat = GeometryReference.transform.Find("Mark").GetComponent<Renderer>().material;
		}
		if (GeometryReference != null)
		{
			GeometryReference.SetActive(show);
			AttachGeometryToCamera(show, ref GeometryReference);
		}
		if (GeometryCompass == null)
		{
			GeometryCompass = Object.Instantiate(Resources.Load("OVRMagCompass")) as GameObject;
		}
		if (GeometryCompass != null)
		{
			GeometryCompass.SetActive(show);
			AttachGeometryToCamera(show, ref GeometryCompass);
		}
	}

	public void AttachGeometryToCamera(bool attach, ref GameObject go)
	{
		if (CameraController != null && attach)
		{
			CameraController.AttachGameObjectToCamera(ref go);
			OVRUtils.SetLocalTransformIdentity(ref go);
			Vector3 localPosition = go.transform.localPosition;
			float ipd = 0f;
			CameraController.GetIPD(ref ipd);
			localPosition.x -= ipd * 0.5f;
			go.transform.localPosition = localPosition;
		}
	}

	public void UpdateGeometry()
	{
		if (MagShowGeometry && !(CameraController == null) && !(GeometryReference == null) && !(GeometryCompass == null))
		{
			Quaternion q = Quaternion.identity;
			if (CameraController != null && CameraController.PredictionOn)
			{
				OVRDevice.GetPredictedOrientation(0, ref q);
			}
			else
			{
				OVRDevice.GetOrientation(0, ref q);
			}
			Vector3 localEulerAngles = GeometryCompass.transform.localEulerAngles;
			localEulerAngles.y = 0f - q.eulerAngles.y + CurEulerRef.y;
			GeometryCompass.transform.localEulerAngles = localEulerAngles;
			if (GeometryReferenceMarkMat != null)
			{
				Color red = Color.red;
				GeometryReferenceMarkMat.SetColor("_Color", red);
			}
		}
	}

	public void UpdateMagYawDriftCorrection()
	{
		if (MagCalState == MagCalibrationState.MagUncalibrated)
		{
			return;
		}
		if (MagCalState == MagCalibrationState.MagReady)
		{
			if (AsunderInput.GetKeyDown(KeyCode.X))
			{
				MagCalState = MagCalibrationState.MagDisabled;
				OVRDevice.EnableMagYawCorrection(0, false);
				MagShowGeometry = false;
				ShowGeometry(MagShowGeometry);
			}
			else if (AsunderInput.GetKeyDown(KeyCode.F6))
			{
				if (!MagShowGeometry)
				{
					MagShowGeometry = true;
					ShowGeometry(MagShowGeometry);
				}
				else
				{
					MagShowGeometry = false;
					ShowGeometry(MagShowGeometry);
				}
			}
			UpdateGeometry();
		}
		else if (MagCalState == MagCalibrationState.MagDisabled && AsunderInput.GetKeyDown(KeyCode.X))
		{
			MagCalState = MagCalibrationState.MagReady;
			EnableYawCorrection(0);
		}
	}

	public void GUIMagYawDriftCorrection(int xLoc, int yLoc, int xWidth, int yWidth, ref OVRGUI guiHelper)
	{
		string text = string.Empty;
		Color red = Color.red;
		switch (MagCalState)
		{
		case MagCalibrationState.MagUncalibrated:
			text = "Mag Uncalibrated";
			break;
		case MagCalibrationState.MagDisabled:
			text = "Mag Calibration OFF";
			break;
		case MagCalibrationState.MagReady:
			text = "Mag Correction ON";
			red = Color.red;
			break;
		}
		guiHelper.StereoBox(xLoc, yLoc, xWidth, yWidth, ref text, red);
	}

	private void EnableYawCorrection(int sensor)
	{
		OVRDevice.EnableMagYawCorrection(sensor, true);
		Quaternion q = Quaternion.identity;
		if (CameraController != null && CameraController.PredictionOn)
		{
			OVRDevice.GetPredictedOrientation(sensor, ref q);
		}
		else
		{
			OVRDevice.GetOrientation(sensor, ref q);
		}
		CurEulerRef = q.eulerAngles;
	}
}
