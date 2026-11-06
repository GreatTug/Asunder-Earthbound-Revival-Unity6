using UnityEngine;

public class DC3_Rotation : MonoBehaviour
{
	public Transform _Engine;

	private Vector3 newRotation = Vector3.zero;

	private void Start()
	{
		_Engine.parent = base.transform;
	}

	private void Update()
	{
		base.transform.Rotate(new Vector3(0f, 0f, 25f));
	}
}
