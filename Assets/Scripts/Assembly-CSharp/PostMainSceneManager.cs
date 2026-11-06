using UnityEngine;
using UnityEngine.SceneManagement;

public class PostMainSceneManager : MonoBehaviour
{
	public GameObject _AudioBeach;

	private float _timer;

	private bool _isDone;

	private float _skipTimer;

	private void Start()
	{
		if ((bool)GameObject.Find("* App Manager"))
		{
			GameObject.Find("* App Manager").GetComponent<AppManager>().InitateCameras();
		}
		_timer = Time.time + 60f;
		_skipTimer = Time.time + 15f;
		CameraFade.StartAlphaFade(Color.black, true, 10f);
	}

	private void Update()
	{
		if (Time.time > _skipTimer && !_isDone && AsunderInput.GetInteractUp())
		{
			Invoke("loadEndScene", 10f);
			iTween.AudioTo(_AudioBeach, iTween.Hash("volume", 0, "time", 10));
			CameraFade.StartAlphaFade(Color.black, false, 10f);
			_isDone = true;
		}
		if (Time.time > _timer && !_isDone)
		{
			Invoke("loadEndScene", 10f);
			CameraFade.StartAlphaFade(Color.black, false, 10f);
			_isDone = true;
		}
	}

	private void loadEndScene()
	{
		SceneManager.LoadScene("EndScene");
	}
}
