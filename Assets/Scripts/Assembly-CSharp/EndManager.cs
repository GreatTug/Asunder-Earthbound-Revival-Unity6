using UnityEngine;

public class EndManager : MonoBehaviour
{
	private AppManager _referenceToAppManager;

	public GameObject _OVRCameraLeft;

	private float _timer;

	private void Start()
	{
		if ((bool)GameObject.Find("* App Manager"))
		{
			_referenceToAppManager = GameObject.Find("* App Manager").GetComponent<AppManager>();
			_referenceToAppManager.InitateCameras();
		}
		CameraFade.StartAlphaFade(Color.black, true, 8f);
	}

	private void Update()
	{
	}
}
