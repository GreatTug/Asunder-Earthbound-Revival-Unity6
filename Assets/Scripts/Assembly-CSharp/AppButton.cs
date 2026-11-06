using UnityEngine;
using UnityEngine.SceneManagement;

public class AppButton : MonoBehaviour
{
	public OverheadController _OverheadController;

	public AppManager.AppType _currentType = AppManager.AppType.PC;

	private AppManager _referenceToAppManager;

	private bool _hasSelected;

	private bool _isVR = true;

	private bool _isTransitioning;

	private void Start()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		CameraFade.StartAlphaFade(Color.black, true, 0.5f);
		GameObject appMgrGo = GameObject.Find("* App Manager");
		if (appMgrGo != null)
		{
			_referenceToAppManager = appMgrGo.GetComponent<AppManager>();
		}
	}

	private void Update()
	{
		if (_isTransitioning)
		{
			return;
		}

		if (AsunderInput.TryGetAimRay(out Ray ray))
		{
			if (Physics.Raycast(ray, out RaycastHit hit, 50f) && hit.collider != null && hit.collider.gameObject == base.gameObject)
			{
				if (!_hasSelected)
				{
					OnMouseEnter();
				}
			}
			else if (_hasSelected && !AsunderInput.GetMouseButton(0))
			{
				OnMouseExit();
			}
		}

		bool shortcutPc = _currentType == AppManager.AppType.PC && (AsunderInput.GetKeyUp(KeyCode.Alpha1) || AsunderInput.GetKeyUp(KeyCode.P));
		bool shortcutVr = _currentType == AppManager.AppType.VR && (AsunderInput.GetKeyUp(KeyCode.Alpha2) || AsunderInput.GetKeyUp(KeyCode.V) || (!AsunderInput.PointerActive && AsunderInput.GetXrControllerInteractUp()));

		if ((_hasSelected && AsunderInput.GetInteractUp()) || shortcutPc || shortcutVr)
		{
			_isTransitioning = true;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			AsunderInput.TriggerHapticBoth(0.6f, 0.08f);
			bool selectVr = shortcutVr || (!shortcutPc && _isVR);
			if (_referenceToAppManager != null)
			{
				_referenceToAppManager._ApplicationType = selectVr ? AppManager.AppType.VR : AppManager.AppType.PC;
			}
			if (selectVr)
			{
				AsunderInput.StartVR();
			}
			else
			{
				AsunderInput.StopVR();
			}
			Invoke("loadNextLevel", 0.5f);
			CameraFade.StartAlphaFade(Color.black, false, 0.5f);
		}
	}

	private void loadNextLevel()
	{
		SceneManager.LoadScene("IntroScene");
	}

	private void OnMouseEnter()
	{
		_hasSelected = true;
		AsunderInput.TriggerHapticBoth(0.2f, 0.03f);
		if (_currentType == AppManager.AppType.PC)
		{
			if (_OverheadController != null)
			{
				_OverheadController.TurnFlatOn();
			}
			_isVR = false;
		}
		else
		{
			_isVR = true;
			if (_OverheadController != null)
			{
				_OverheadController.TurnOculusOn();
			}
		}
	}

	private void OnMouseExit()
	{
		_hasSelected = false;
		if (_currentType == AppManager.AppType.PC)
		{
			if (_OverheadController != null)
			{
				_OverheadController.TurnFlatOff();
			}
		}
		else
		{
			if (_OverheadController != null)
			{
				_OverheadController.TurnOculusOff();
			}
		}
	}
}
