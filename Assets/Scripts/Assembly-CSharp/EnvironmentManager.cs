using System.Collections;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
	public enum EnvironmentState
	{
		idle = 0,
		intro = 1,
		calm = 2,
		city = 3,
		stormEnter = 4,
		stormEnterQuick = 5,
		stormPhase1 = 6,
		stormPhase2 = 7,
		stormBlackout = 8,
		crash = 9,
		heavyClouds = 10,
		cloudy = 11,
		disableCloseWeather = 12
	}

	public EnvironmentState _ActiveEnvironmentState = EnvironmentState.calm;

	public GameObject _Aeroplane;

	public GameObject _OutsideLight;

	public GameObject _City;

	public GameObject _CloudsPlane;

	public GameObject _CloudsDomeStorm;

	public GameObject _CloudsDomeStormClose;

	public GameObject _CloudsDomeCloudy;

	public GameObject _CloudsDomeCloudyClose;

	public GameObject _CloudsPSys;

	public GameObject _CloudsWingPSys;

	public GameObject _Lightning;

	public GameObject _CabinLights;

	public GameObject _PassengerLights;

	public Material _OverheadLightMaterial;

	public GameObject _CamerasParent;

	public GameObject _TurbulenceAudioSource;

	public Light _LightLightning;

	public GameObject _WingRightAudiosource;

	public GameObject _WingLeftAudiosource;

	private Color _initCityLightsColor;

	public AudioSource _ShipAudio;

	private Vector3 _StartingCameraParentPosition;

	private Color _OutsideLightColor;

	private float _sideTiltSmooth = 0.06f;

	private float _verticalTiltSmooth = 0.01f;

	private float _DURATION_TRANSITION_STORM = 45f;

	private float _DURATION_TRANSITION_STORM_CLOSE = 2f;

	private float _DURATION_TRANSITION_CLOUDY = 10f;

	private float _DURATION_TRANSITION_HEAVYCLOUDS = 30f;

	private Color _OVERHEADLIGHT_COLOR = new Color(1f, 0.85f, 0.45f);

	private float _fsmLoopFrequency = 0.01f;

	private void Start()
	{
		if (_OverheadLightMaterial != null)
		{
			_OverheadLightMaterial.color = _OVERHEADLIGHT_COLOR;
		}
		if (_OutsideLight != null)
		{
			Light l = _OutsideLight.GetComponent<Light>();
			if (l != null)
			{
				_OutsideLightColor = l.color;
			}
		}
		if (_CamerasParent != null)
		{
			_StartingCameraParentPosition = _CamerasParent.transform.localPosition;
		}
		StartCoroutine(FSM());
	}

	private void Update()
	{
	}

	public void StopCoroutine()
	{
		StopCoroutine("FSM");
	}

	private void rotateAeroplane(float aVerticalTilt, float aSideTilt, float aFrequency)
	{
		if (_Aeroplane == null)
		{
			return;
		}
		float x = Mathf.Lerp(_Aeroplane.transform.localEulerAngles.x, aVerticalTilt, _verticalTiltSmooth);
		Quaternion to = Quaternion.Euler(x, 0f, (Mathf.PerlinNoise(Time.time * aFrequency, 0f) - 0.5f) * aSideTilt);
		_Aeroplane.transform.localRotation = Quaternion.Lerp(_Aeroplane.transform.localRotation, to, _sideTiltSmooth);
	}

	public void PlaySFX(string aFile, AudioSource aLocation)
	{
		if (aLocation == null)
		{
			return;
		}
		AudioSource src = aLocation.GetComponent<AudioSource>();
		if (src == null)
		{
			return;
		}
		AudioClip clip = Resources.Load("Audio/SFX/" + aFile) as AudioClip;
		if (clip != null)
		{
			AppManager.ApplyUnity4AudioSpatialBlend(src, clip);
			src.clip = clip;
			src.Play();
		}
	}

	public void SwitchPassangerLights()
	{
		if (_PassengerLights != null)
		{
			if (_PassengerLights.activeSelf)
			{
				_PassengerLights.SetActive(false);
				if (_OverheadLightMaterial != null)
				{
					_OverheadLightMaterial.color = new Color(0f, 0f, 0f);
				}
			}
			else
			{
				_PassengerLights.SetActive(true);
				if (_OverheadLightMaterial != null)
				{
					_OverheadLightMaterial.color = _OVERHEADLIGHT_COLOR;
				}
			}
		}
	}

	public void SwitchCabinLights()
	{
		if (_CabinLights != null)
		{
			_CabinLights.SetActive(!_CabinLights.activeSelf);
		}
	}

	public void EnableTubulenceCycle(Vector3 aScale, float aSpeed)
	{
		if (_CamerasParent != null)
		{
			Quaternion to = Quaternion.Euler((Mathf.PerlinNoise(Time.time * aSpeed, 0f) - 0.5f) * aScale.x, (Mathf.PerlinNoise(Time.time * aSpeed, 0f) - 0.5f) * aScale.y, (Mathf.PerlinNoise(Time.time * aSpeed, 0f) - 0.5f) * aScale.z);
			rotateAeroplane(0f, 35f, 3.15f);
			_CamerasParent.transform.localRotation = Quaternion.Lerp(_CamerasParent.transform.localRotation, to, 0.3f);
			Vector3 startingCameraParentPosition = _StartingCameraParentPosition;
			startingCameraParentPosition.y += Mathf.Sin(Time.time * 4f) * 0.025f;
			_CamerasParent.transform.localPosition = Vector3.Lerp(_CamerasParent.transform.localPosition, startingCameraParentPosition, Time.deltaTime * 2f);
		}
	}

	private void BalanceTurbulence()
	{
		if (_CamerasParent != null)
		{
			_CamerasParent.transform.localPosition = Vector3.Lerp(_CamerasParent.transform.localPosition, _StartingCameraParentPosition, Time.deltaTime * 0.8f);
		}
	}

	public void DisableTurbulence()
	{
		if (_CamerasParent != null)
		{
			_CamerasParent.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			NoiseController nc = _CamerasParent.GetComponent<NoiseController>();
			if (nc != null && nc._Segments != null && nc._Segments.Length > 1 && nc._Segments[1] != null)
			{
				nc._Segments[1]._Enabled = false;
			}
		}
	}

	public void setState(EnvironmentState aState)
	{
		_ActiveEnvironmentState = aState;
	}

	private IEnumerator intro()
	{
		yield return null;
		while (_ActiveEnvironmentState == EnvironmentState.intro)
		{
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator idle()
	{
		yield return null;
		while (_ActiveEnvironmentState == EnvironmentState.idle)
		{
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator calm()
	{
		yield return null;
		while (_ActiveEnvironmentState == EnvironmentState.calm)
		{
			rotateAeroplane(0f, 15f, 0.25f);
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator crash()
	{
		iTween.ColorTo(_CloudsDomeStorm, iTween.Hash("a", 0, "time", 12, "easetype", iTween.EaseType.easeInOutCubic));
		iTween.ColorTo(_OutsideLight, iTween.Hash("color", _OutsideLightColor, "time", 12, "easetype", iTween.EaseType.easeInOutCubic));
		_verticalTiltSmooth = 0.075f;
		float timer = Time.time + Random.Range(0.1f, 5f);
		float timerII = Time.time + Random.Range(0.01f, 1f);
		yield return null;
		while (_ActiveEnvironmentState == EnvironmentState.crash)
		{
			rotateAeroplane(60f, 40f, 1.2f);
			if (Time.time > timer)
			{
				if (_CabinLights.activeSelf)
				{
					_CabinLights.SetActive(false);
				}
				else
				{
					_CabinLights.SetActive(true);
				}
				timer = Time.time + Random.Range(0.1f, 4.2f);
			}
			if (Time.time > timerII)
			{
				if (_PassengerLights.activeSelf)
				{
					_PassengerLights.SetActive(false);
					_OverheadLightMaterial.color = new Color(0f, 0f, 0f);
				}
				else
				{
					_PassengerLights.SetActive(true);
					_OverheadLightMaterial.color = _OVERHEADLIGHT_COLOR;
				}
				timerII = Time.time + Random.Range(0.01f, 0.1f);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator stormEnter()
	{
		iTween.ColorTo(_CloudsDomeStorm, iTween.Hash("a", 1, "time", _DURATION_TRANSITION_STORM, "easetype", iTween.EaseType.easeInOutCubic));
		iTween.ColorTo(_OutsideLight, iTween.Hash("color", new Color(0f, 0f, 0f), "time", _DURATION_TRANSITION_STORM, "easetype", iTween.EaseType.easeInOutCubic));
		yield return null;
		while (_ActiveEnvironmentState == EnvironmentState.stormEnter)
		{
			rotateAeroplane(0f, 20f, 0.95f);
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator stormEnterQuick()
	{
		iTween.ColorTo(_CloudsDomeStorm, iTween.Hash("a", 1, "time", 1f, "easetype", iTween.EaseType.easeInOutCubic));
		iTween.ColorTo(_OutsideLight, iTween.Hash("color", new Color(0f, 0f, 0f), "time", 1f, "easetype", iTween.EaseType.easeInOutCubic));
		yield return null;
		while (_ActiveEnvironmentState == EnvironmentState.stormEnterQuick)
		{
			rotateAeroplane(0f, 20f, 0.95f);
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator stormBlackout()
	{
		iTween.ColorTo(_CloudsDomeStormClose, iTween.Hash("a", 3, "time", _DURATION_TRANSITION_STORM_CLOSE, "easetype", iTween.EaseType.easeInOutCubic));
		yield return null;
		while (_ActiveEnvironmentState == EnvironmentState.stormBlackout)
		{
			rotateAeroplane(0f, 20f, 0.95f);
			yield return AsunderCoroutines.FsmWait;
		}
		iTween.ColorTo(_CloudsDomeStormClose, iTween.Hash("a", 0, "time", _DURATION_TRANSITION_STORM_CLOSE, "easetype", iTween.EaseType.easeInOutCubic));
	}

	private IEnumerator stormPhase1()
	{
		if (_City.activeSelf)
		{
			_City.SetActive(false);
		}
		float timer = 0f;
		bool isTurbulence = true;
		float timerII = 0f;
		yield return null;
		while (_ActiveEnvironmentState == EnvironmentState.stormPhase1)
		{
			rotateAeroplane(0f, 25f, 1.15f);
			if (isTurbulence)
			{
				if (Time.time > timerII)
				{
					if (_PassengerLights.activeSelf)
					{
						_PassengerLights.SetActive(false);
						_OverheadLightMaterial.color = new Color(0f, 0f, 0f);
					}
					else
					{
						_PassengerLights.SetActive(true);
						_OverheadLightMaterial.color = _OVERHEADLIGHT_COLOR;
					}
					timerII = Time.time + Random.Range(0.01f, 0.05f);
				}
				EnableTubulenceCycle(new Vector3(3.25f, 0f, 3.25f), 17f);
			}
			else if (!_PassengerLights.activeSelf)
			{
				_PassengerLights.SetActive(true);
				_OverheadLightMaterial.color = _OVERHEADLIGHT_COLOR;
			}
			if (Time.time > timer)
			{
				if (!isTurbulence)
				{
					switch (Random.Range(0, 2))
					{
					case 0:
						timer = Time.time + 2f;
						PlaySFX("turbulence-2sec", _TurbulenceAudioSource.GetComponent<AudioSource>());
						AsunderInput.TriggerHapticBoth(0.35f, 0.35f);
						break;
					case 1:
						timer = Time.time + 3f;
						PlaySFX("turbulence-3sec", _TurbulenceAudioSource.GetComponent<AudioSource>());
						AsunderInput.TriggerHapticBoth(0.45f, 0.45f);
						break;
					}
					isTurbulence = true;
				}
				else
				{
					DisableTurbulence();
					isTurbulence = false;
					if (!_PassengerLights.activeSelf)
					{
						_PassengerLights.SetActive(true);
						_OverheadLightMaterial.color = _OVERHEADLIGHT_COLOR;
					}
					timer = Time.time + Random.Range(4.5f, 12f);
				}
			}
			yield return AsunderCoroutines.FsmWait;
		}
		_PassengerLights.SetActive(true);
		_OverheadLightMaterial.color = _OVERHEADLIGHT_COLOR;
	}

	private IEnumerator stormPhase2()
	{
		iTween.ColorTo(_CloudsDomeStorm, iTween.Hash("a", 1, "time", 1f, "easetype", iTween.EaseType.easeInOutCubic));
		iTween.ColorTo(_OutsideLight, iTween.Hash("color", new Color(0f, 0f, 0f), "time", 1f, "easetype", iTween.EaseType.easeInOutCubic));
		if (_City.activeSelf)
		{
			_City.SetActive(false);
		}
		float timer = 0f;
		bool isTurbulence = true;
		float timer2 = Time.time + 28f;
		float timerLighs = Time.time + 10f;
		yield return null;
		while (_ActiveEnvironmentState == EnvironmentState.stormPhase2)
		{
			rotateAeroplane(0f, 30f, 1.35f);
			if (isTurbulence)
			{
				EnableTubulenceCycle(new Vector3(4.9f, 0f, 4.9f), 18f);
			}
			else
			{
				BalanceTurbulence();
			}
			if (Time.time > timer2)
			{
				_LightLightning.intensity = 1.5f;
				PlaySFX("Lightning-strike", _WingLeftAudiosource.GetComponent<AudioSource>());
				AsunderInput.TriggerHapticBoth(0.75f, 0.25f);
				timer2 = Time.time + 15f + (float)Random.Range(0, 10);
			}
			if (_LightLightning.intensity > 0f)
			{
				_LightLightning.intensity -= 0.075f;
			}
			if (Time.time > timer)
			{
				if (!isTurbulence)
				{
					switch (Random.Range(0, 5))
					{
					case 0:
						if (_CabinLights.activeSelf)
						{
							_CabinLights.SetActive(false);
						}
						timer = Time.time + 2f;
						PlaySFX("turbulence-2sec", _TurbulenceAudioSource.GetComponent<AudioSource>());
						AsunderInput.TriggerHapticBoth(0.4f, 0.4f);
						break;
					case 1:
						if (_CabinLights.activeSelf)
						{
							_CabinLights.SetActive(false);
						}
						timer = Time.time + 3f;
						PlaySFX("turbulence-3sec", _TurbulenceAudioSource.GetComponent<AudioSource>());
						AsunderInput.TriggerHapticBoth(0.5f, 0.5f);
						break;
					case 2:
						timer = Time.time + 4f;
						PlaySFX("turbulence-4sec", _TurbulenceAudioSource.GetComponent<AudioSource>());
						AsunderInput.TriggerHapticBoth(0.6f, 0.6f);
						break;
					case 3:
						timer = Time.time + 5f;
						PlaySFX("turbulence-5sec", _TurbulenceAudioSource.GetComponent<AudioSource>());
						AsunderInput.TriggerHapticBoth(0.7f, 0.7f);
						break;
					case 4:
						timer = Time.time + 8f;
						PlaySFX("turbulence-8sec", _TurbulenceAudioSource.GetComponent<AudioSource>());
						AsunderInput.TriggerHapticBoth(0.85f, 0.8f);
						break;
					}
					isTurbulence = true;
				}
				else
				{
					DisableTurbulence();
					isTurbulence = false;
					if (!_CabinLights.activeSelf)
					{
						_CabinLights.SetActive(true);
					}
					timer = Time.time + Random.Range(0.5f, 5f);
				}
			}
			yield return AsunderCoroutines.FsmWait;
		}
		if (!_CabinLights.activeSelf)
		{
			_CabinLights.SetActive(true);
		}
		_LightLightning.intensity = 0f;
		DisableTurbulence();
	}

	private IEnumerator cloudy()
	{
		iTween.ColorTo(_CloudsDomeCloudy, iTween.Hash("a", 1, "time", _DURATION_TRANSITION_CLOUDY, "easetype", iTween.EaseType.easeInOutCubic));
		iTween.ColorTo(_OutsideLight, iTween.Hash("color", new Color(0.5f, 0.5f, 0.5f), "time", _DURATION_TRANSITION_CLOUDY, "easetype", iTween.EaseType.easeInOutCubic));
		yield return null;
		while (_ActiveEnvironmentState == EnvironmentState.cloudy)
		{
			rotateAeroplane(0f, 15f, 0.25f);
			yield return AsunderCoroutines.FsmWait;
		}
		iTween.ColorTo(_CloudsDomeCloudy, iTween.Hash("a", 0, "time", _DURATION_TRANSITION_CLOUDY, "easetype", iTween.EaseType.easeInOutCubic));
		iTween.ColorTo(_OutsideLight, iTween.Hash("color", _OutsideLightColor, "time", _DURATION_TRANSITION_CLOUDY, "easetype", iTween.EaseType.easeInOutCubic));
	}

	private IEnumerator heavyClouds()
	{
		iTween.ColorTo(_CloudsDomeCloudy, iTween.Hash("a", 0.35f, "time", _DURATION_TRANSITION_HEAVYCLOUDS, "easetype", iTween.EaseType.easeInOutCubic));
		iTween.ColorTo(_CloudsDomeCloudyClose, iTween.Hash("a", 1, "time", _DURATION_TRANSITION_HEAVYCLOUDS, "easetype", iTween.EaseType.easeInOutCubic));
		iTween.ColorTo(_OutsideLight, iTween.Hash("color", new Color(0.5f, 0.5f, 0.5f), "time", _DURATION_TRANSITION_HEAVYCLOUDS, "easetype", iTween.EaseType.easeInOutCubic));
		yield return null;
		while (_ActiveEnvironmentState == EnvironmentState.heavyClouds)
		{
			rotateAeroplane(0f, 15f, 0.25f);
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator city()
	{
		float timer = Time.time + 150f;
		_City.SetActive(true);
		iTween.ColorTo(_CloudsDomeCloudy, iTween.Hash("a", 0, "time", _DURATION_TRANSITION_HEAVYCLOUDS, "easetype", iTween.EaseType.easeInOutCubic));
		iTween.ColorTo(_CloudsDomeCloudyClose, iTween.Hash("a", 0, "time", _DURATION_TRANSITION_HEAVYCLOUDS, "easetype", iTween.EaseType.easeInOutCubic));
		yield return null;
		while (_ActiveEnvironmentState == EnvironmentState.city)
		{
			rotateAeroplane(0f, 15f, 0.25f);
			if (Time.time > timer)
			{
				setState(EnvironmentState.heavyClouds);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator disableCloseWeather()
	{
		_CloudsDomeCloudy.SetActive(false);
		_CloudsDomeCloudyClose.SetActive(false);
		_CloudsDomeStorm.SetActive(false);
		_CloudsDomeStormClose.SetActive(false);
		yield return null;
		while (_ActiveEnvironmentState == EnvironmentState.disableCloseWeather)
		{
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator FSM()
	{
		while (true)
		{
			yield return StartCoroutine(_ActiveEnvironmentState.ToString());
		}
	}
}
