using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtrasManager : MonoBehaviour
{
	public bool _Noclip;

	public bool _WireFrame;

	private AppManager _referenceToAppManager;

	private bool _hasQuit;

	private Transform _mainCameraTransform;

	private Vector3 _currentLocalPos = Vector3.zero;

	private void Start()
	{
		if ((bool)GameObject.Find("* App Manager"))
		{
			_referenceToAppManager = GameObject.Find("* App Manager").GetComponent<AppManager>();
			_referenceToAppManager.InitateCameras();
			GameObject mainCam = _referenceToAppManager.GetMainCamera();
			_mainCameraTransform = (mainCam != null) ? mainCam.transform : (Camera.allCamerasCount > 0 && Camera.allCameras[0] != null ? Camera.allCameras[0].transform : null);
		}
		else
		{
			GameObject camObj = GameObject.FindGameObjectWithTag("CameraNormal") ?? GameObject.FindGameObjectWithTag("CameraOVR");
			_mainCameraTransform = (camObj != null) ? camObj.transform : (Camera.allCamerasCount > 0 && Camera.allCameras[0] != null ? Camera.allCameras[0].transform : null);
		}
		CameraFade.StartAlphaFade(Color.black, true, 8f);
	}

	private void Update()
	{
		if (_Noclip && _mainCameraTransform != null)
		{
			if (AsunderInput.GetInteractUp())
			{
				_currentLocalPos.z++;
				_mainCameraTransform.Translate(0f, 0f, 1f, Space.Self);
			}
		}
		else if (AsunderInput.GetInteractUp() && !_hasQuit)
		{
			_hasQuit = true;
			Invoke("goBackToMenu", 4f);
			CameraFade.StartAlphaFade(Color.black, false, 4f);
		}
	}

	private void goBackToMenu()
	{
		SceneManager.LoadScene("ExtrasMenuScene");
	}
}
