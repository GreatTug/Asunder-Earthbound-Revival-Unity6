using UnityEngine;

public class WalterAI : MonoBehaviour
{
	public AudioSource _WalterVoice;

	public GameObject _GazeTargetPaper;

	public GameObject _GazeTargetPlayer;

	public GameObject _GazeTargetOverhead;

	public GameObject _GazeTarget;

	public GameObject _CityFromAFar;

	public GameObject _RadioAudio;

	private void Start()
	{
	}

	private void Update()
	{
		if (AsunderInput.GetKeyDown(KeyCode.Alpha1))
		{
			_GazeTarget.transform.position = _GazeTargetPlayer.transform.position;
		}
		if (AsunderInput.GetKeyDown(KeyCode.Alpha2))
		{
			_WalterVoice.Play();
		}
		if (AsunderInput.GetKeyDown(KeyCode.Alpha3))
		{
			_GazeTarget.transform.position = _GazeTargetPaper.transform.position;
		}
		if (AsunderInput.GetKeyDown(KeyCode.Alpha5))
		{
			_GazeTarget.transform.position = _GazeTargetOverhead.transform.position;
		}
		if (AsunderInput.GetKeyDown(KeyCode.Alpha4))
		{
			_RadioAudio.GetComponent<AudioSource>().Play();
		}
		if (AsunderInput.GetKeyDown(KeyCode.Alpha6))
		{
			_CityFromAFar.SetActive(true);
			_GazeTarget.transform.position = _GazeTargetPaper.transform.position;
		}
	}
}
