using UnityEngine;

public class Deactivator : MonoBehaviour
{
	public float _Delay = 1f;

	private void Start()
	{
		Invoke("stopall", _Delay);
	}

	private void Update()
	{
	}

	private void stopall()
	{
		base.gameObject.SetActive(false);
	}
}
