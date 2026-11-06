using UnityEngine;

public class OverheadController : MonoBehaviour
{
	public GameObject _LightBtn;

	public Material _LightOnMat;

	public Material _LightOffMat;

	public Material _LightFrontOnMat;

	public Material _LightFrontOffMat;

	public GameObject _FlatBtn;

	public Material _FlatOn;

	public Material _FlatOff;

	public GameObject _OculusBtn;

	public Material _OculusOn;

	public Material _OculusOff;

	public GameObject _HintsBtn;

	public Material _HintsOn;

	public Material _HintsOff;

	public bool _HintsActive;

	public GameObject _LightBeam;

	public Light _Light;

	private bool _On = true;

	private void SetButtonMaterials(GameObject btn, Material baseMat, Material secondMat)
	{
		if (btn == null)
		{
			return;
		}
		Renderer rend = btn.GetComponent<Renderer>();
		if (rend == null)
		{
			return;
		}
		if (baseMat != null)
		{
			rend.material = baseMat;
		}
		if (secondMat != null)
		{
			Material[] materials = rend.materials;
			if (materials != null && materials.Length > 1)
			{
				materials[1] = secondMat;
				rend.materials = materials;
			}
		}
	}

	public void PushButton()
	{
		if (_On)
		{
			TurnLightsOff();
		}
		else
		{
			TurnLightsOn();
		}
		AudioSource audio = GetComponent<AudioSource>();
		if (audio != null)
		{
			audio.Play();
		}
	}

	public void PushForSound()
	{
		AudioSource audio = GetComponent<AudioSource>();
		if (audio != null)
		{
			audio.Play();
		}
	}

	public void TurnHintsOn()
	{
		_HintsActive = true;
		SetButtonMaterials(_HintsBtn, _LightOnMat, _HintsOn);
	}

	public void TurnHintsOff()
	{
		_HintsActive = false;
		SetButtonMaterials(_HintsBtn, _LightOffMat, _HintsOff);
	}

	public void TurnOculusOn()
	{
		SetButtonMaterials(_OculusBtn, _LightOnMat, _OculusOn);
	}

	public void TurnOculusOff()
	{
		SetButtonMaterials(_OculusBtn, _LightOffMat, _OculusOff);
	}

	public void TurnFlatOn()
	{
		SetButtonMaterials(_FlatBtn, _LightOnMat, _FlatOn);
	}

	public void TurnFlatOff()
	{
		SetButtonMaterials(_FlatBtn, _LightOffMat, _FlatOff);
	}

	public void TurnLightsOn()
	{
		if (_Light != null)
		{
			_Light.enabled = true;
		}
		_On = true;
		SetButtonMaterials(_LightBtn, _LightOnMat, _LightFrontOnMat);
		if (_LightBeam != null)
		{
			_LightBeam.SetActive(true);
		}
	}

	public void TurnLightsOff()
	{
		if (_Light != null)
		{
			_Light.enabled = false;
		}
		_On = false;
		SetButtonMaterials(_LightBtn, _LightOffMat, _LightFrontOffMat);
		if (_LightBeam != null)
		{
			_LightBeam.SetActive(false);
		}
	}

	public bool IsLightOn()
	{
		return _On;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
