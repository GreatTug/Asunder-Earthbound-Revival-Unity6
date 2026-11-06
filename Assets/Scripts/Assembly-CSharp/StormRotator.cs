using UnityEngine;

public class StormRotator : MonoBehaviour
{
	private Vector3 rot = new Vector3(0f, 0f, 1f);

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.Rotate(rot, 0.0004f * Mathf.Rad2Deg, Space.World);
	}
}
