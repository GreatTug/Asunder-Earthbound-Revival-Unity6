using UnityEngine;

public class WhiskeyAI : MonoBehaviour
{
	public Transform _WhiskeyBone;

	public Transform _Airplane;

	public Vector3 initRotation = new Vector3(90f, 90f, 0f);

	public Vector3 airplaneEulerEffect = new Vector3(0f, 0f, 2f);

	private void Start()
	{
	}

	private void Update()
	{
		initRotation.y = _Airplane.localEulerAngles.z * airplaneEulerEffect.z;
		_WhiskeyBone.localEulerAngles = initRotation;
	}
}
