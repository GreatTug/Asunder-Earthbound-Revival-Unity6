using UnityEngine;

public class Rotate : MonoBehaviour
{
	public Vector3 rotateAxis = Vector3.up;

	public float degreesPerSecond = 45f;

	public bool worldSpace;

	private Transform xform;

	private void Awake()
	{
		xform = base.transform;
	}

	private void Update()
	{
		xform.Rotate(rotateAxis, degreesPerSecond * Time.deltaTime, (!worldSpace) ? Space.Self : Space.World);
	}
}
