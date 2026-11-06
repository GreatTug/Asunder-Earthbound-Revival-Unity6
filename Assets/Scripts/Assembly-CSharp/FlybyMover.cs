using UnityEngine;

public class FlybyMover : MonoBehaviour
{
	public Vector3 _Speed = Vector3.zero;

	private Vector3 _location = Vector3.zero;

	public Vector3 _Eulers = Vector3.zero;

	private Quaternion _rotation;

	public float _lookSmooth = 0.5f;

	public float _moveAmount = 5f;

	private void Start()
	{
		_location = base.transform.position;
		_Eulers = base.transform.localRotation.eulerAngles;
	}

	private void Update()
	{
		if (AsunderInput.GetKeyDown(KeyCode.W))
		{
			_Eulers.x -= _moveAmount;
		}
		else if (AsunderInput.GetKeyDown(KeyCode.S))
		{
			_Eulers.x += _moveAmount;
		}
		if (AsunderInput.GetKeyDown(KeyCode.A))
		{
			_Eulers.y -= _moveAmount;
		}
		else if (AsunderInput.GetKeyDown(KeyCode.D))
		{
			_Eulers.y += _moveAmount;
		}
		if (AsunderInput.GetKeyDown(KeyCode.Q))
		{
			_Eulers.z = _moveAmount;
		}
		else if (AsunderInput.GetKeyDown(KeyCode.E))
		{
			_Eulers.z = 0f - _moveAmount;
		}
		_rotation = Quaternion.Euler(_Eulers.x, _Eulers.y, _Eulers.z);
		base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, _rotation, _lookSmooth);
		base.transform.Translate(_Speed, Space.Self);
	}
}
