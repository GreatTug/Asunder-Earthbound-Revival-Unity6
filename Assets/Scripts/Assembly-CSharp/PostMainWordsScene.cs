using UnityEngine;
using UnityEngine.SceneManagement;

public class PostMainWordsScene : MonoBehaviour
{
	public GameObject Slide1;

	public GameObject Slide2;

	public GameObject Slide3;

	private float _intervalShow = 2f;

	private float _intervalWait = 4f;

	private float _timer;

	private int _stateID = 1;

	private bool _showing = true;

	private void Start()
	{
		_timer = Time.time + _intervalWait;
		if ((bool)GameObject.Find("* App Manager"))
		{
			GameObject.Find("* App Manager").GetComponent<AppManager>().InitateCameras();
		}
	}

	private void Update()
	{
		if (_showing)
		{
			if (Time.time > _timer)
			{
				if (_stateID == 0)
				{
					Slide1.SetActive(true);
				}
				else if (_stateID == 1)
				{
					Slide2.SetActive(true);
				}
				else if (_stateID == 2)
				{
					Slide3.SetActive(true);
					_intervalWait += 5f;
					iTween.AudioTo(base.gameObject, iTween.Hash("volume", 0, "time", 10));
				}
				else if (_stateID == 3)
				{
					SceneManager.LoadScene("EndScene");
				}
				_stateID++;
				_timer = Time.time + _intervalWait;
				_showing = false;
			}
		}
		else if (Time.time > _timer)
		{
			if (_stateID == 1)
			{
				Slide1.SetActive(false);
			}
			else if (_stateID == 2)
			{
				Slide2.SetActive(false);
			}
			else if (_stateID == 3)
			{
				Slide3.SetActive(false);
			}
			_timer = Time.time + _intervalShow;
			_showing = true;
		}
	}
}
