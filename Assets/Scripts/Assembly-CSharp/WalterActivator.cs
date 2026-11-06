using UnityEngine;

public class WalterActivator : MonoBehaviour
{
	public float _ActivationDelay = 1f;

	public GameObject _Walter;

	public GameObject _Spotlight;

	private Color _spotLightGoalColor;

	public AudioClip _Aclip;

	public bool _isLoop;

	private Vector3 _goalPosition = Vector3.zero;

	private void Start()
	{
		_Walter.SetActive(false);
		_Walter.GetComponent<AudioSource>().clip = _Aclip;
		_Walter.GetComponent<AudioSource>().loop = _isLoop;
		_goalPosition = _Walter.transform.localPosition;
		_Walter.transform.localScale = new Vector3(1f, 0f, 1f);
		Invoke("activateWalter", _ActivationDelay);
	}

	private void Update()
	{
	}

	private void activateWalter()
	{
		_Walter.SetActive(true);
		iTween.ScaleTo(_Walter, iTween.Hash("scale", new Vector3(1f, 1f, 1f), "islocal", true, "time", 2.2f));
	}
}
