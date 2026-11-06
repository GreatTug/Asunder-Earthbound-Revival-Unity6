using UnityEngine;

public class MoveBackgroundCity : MonoBehaviour
{

	public float _Speed = -0.5f;

	public float _SpeedRotation = -0.4f;

	private Vector3 _location = Vector3.zero;

	private float _NewAxisRotation;

	private void Start()
	{
		_location = base.gameObject.transform.localPosition;
	}

	private void Update()
	{
		_location.z += _Speed * Time.deltaTime;
		base.gameObject.transform.localPosition = _location;
		_NewAxisRotation += _SpeedRotation * Time.deltaTime;
		base.gameObject.transform.localRotation = Quaternion.Euler(0f, _NewAxisRotation, 0f);
	}

	private void RemoveThis()
	{
	}
}
