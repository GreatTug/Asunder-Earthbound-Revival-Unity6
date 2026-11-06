using UnityEngine;

public class DC3_Flap : MonoBehaviour
{
	public Transform _Flap;

	public Transform _Airplane;

	private Vector3 newRotation = Vector3.zero;

	private void Start()
	{
		_Flap.parent = base.transform;
		newRotation = base.transform.localEulerAngles;
	}

	private void Update()
	{
		newRotation.x = _Airplane.localEulerAngles.z * 2f;
		base.transform.localEulerAngles = newRotation;
	}
}
