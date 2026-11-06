using UnityEngine;

public class CameraMover : MonoBehaviour
{
	public Vector3 _MoveVector = Vector3.zero;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.Translate(_MoveVector * Time.deltaTime, Space.Self);
	}
}
