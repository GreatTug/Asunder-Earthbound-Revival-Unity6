using UnityEngine;

public class ActivateAfterDelay : MonoBehaviour
{
	public float _Delay = 1f;

	private Renderer _renderer;

	private void Start()
	{
		_renderer = GetComponent<Renderer>();
		if (_renderer != null)
		{
			_renderer.enabled = false;
		}
		Invoke("activateNow", _Delay);
	}

	private void Update()
	{
	}

	private void activateNow()
	{
		if (_renderer != null)
		{
			_renderer.enabled = true;
		}
	}
}
