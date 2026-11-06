using UnityEngine;

public class ShipAI : MonoBehaviour
{
	public Vector3 _Movement = Vector3.zero;

	public Vector3 _Rotation = Vector3.zero;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.Translate(_Movement, Space.Self);
		base.transform.Rotate(_Rotation);
	}
}
