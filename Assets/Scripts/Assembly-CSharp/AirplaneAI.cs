using UnityEngine;

public class AirplaneAI : MonoBehaviour
{
	public AudioSource _WomanVoice;

	public AudioSource _OldManVoice;

	public AudioSource _WalterVoice;

	public Transform _PlanePartToShake;

	public AudioSource _OverheadAudioSource;

	public Material _WarningLightSeatBelt;



	private Quaternion _initRotation;

	private Vector3 _initPosition;

	private void Start()
	{
		_initPosition = _PlanePartToShake.position;
	}

	private void Update()
	{
	}
}
