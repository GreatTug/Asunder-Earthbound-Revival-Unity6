using UnityEngine;

public class SoundFader : MonoBehaviour
{
	public float _Duration = 5f;

	public float _InitVolume;

	public float _EndGoal;

	private void Start()
	{
		base.gameObject.GetComponent<AudioSource>().volume = _InitVolume;
		iTween.AudioTo(base.gameObject, iTween.Hash("volume", _EndGoal, "time", _Duration, "easetype", iTween.EaseType.easeInSine));
	}

	private void Update()
	{
	}
}
