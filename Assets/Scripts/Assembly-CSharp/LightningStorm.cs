using UnityEngine;

public class LightningStorm : MonoBehaviour
{
	private float _timer;

	private bool _isLightning;

	private void Start()
	{
		base.transform.GetComponent<Renderer>().enabled = false;
		_timer = Time.time + Random.Range(0.25f, 3.2f);
	}

	private void Update()
	{
		if (Time.time > _timer)
		{
			if (_isLightning)
			{
				base.transform.GetComponent<Renderer>().enabled = true;
				_timer = Time.time + 0.18f;
				_isLightning = false;
			}
			else
			{
				base.transform.GetComponent<Renderer>().enabled = false;
				_timer = Time.time + Random.Range(0.25f, 3.2f);
				_isLightning = true;
			}
		}
	}
}
