using UnityEngine;

public class DC3_AI : MonoBehaviour
{
	public float _TurbulenceShakeAmount = 0.5f;

	public float _Duration = 1f;

	public GameObject _InteriorPassengersLightsRoot;

	public GameObject _InteriorCabinLightsRoot;

	public Material _OverheadLightMaterial;

	private Color _OverheadLightsInitColor;

	private Quaternion _initialRotaiton;

	private void OnEnable()
	{
	}

	private void Start()
	{
		_initialRotaiton = base.transform.localRotation;
		_OverheadLightsInitColor = _OverheadLightMaterial.color;
	}

	private void Update()
	{
		if (AsunderInput.GetKeyDown(KeyCode.F1))
		{
			CabinLightsOff();
		}
		if (AsunderInput.GetKeyDown(KeyCode.F2))
		{
			CabinLightsOn();
		}
		if (AsunderInput.GetKeyDown(KeyCode.F3))
		{
			PassengerLightsOff();
		}
		if (AsunderInput.GetKeyDown(KeyCode.F4))
		{
			PassengerLightsOn();
		}
		if (AsunderInput.GetKeyDown(KeyCode.F5))
		{
			ShakePlane();
		}
	}

	private void ResetPos()
	{
		base.transform.localRotation = _initialRotaiton;
	}

	private void ShakePlane()
	{
		iTween.ShakeRotation(base.gameObject, iTween.Hash("amount", new Vector3(_TurbulenceShakeAmount, _TurbulenceShakeAmount, _TurbulenceShakeAmount), "space", Space.Self, "time", _Duration, "oncomplete", "ResetPos"));
	}

	private void CabinLightsOff()
	{
		_InteriorCabinLightsRoot.SetActive(false);
	}

	private void CabinLightsOn()
	{
		_InteriorCabinLightsRoot.SetActive(true);
	}

	private void PassengerLightsOff()
	{
		_InteriorPassengersLightsRoot.SetActive(false);
		_OverheadLightMaterial.color = Color.black;
	}

	private void PassengerLightsOn()
	{
		_InteriorPassengersLightsRoot.SetActive(true);
		_OverheadLightMaterial.color = _OverheadLightsInitColor;
	}
}
