using UnityEngine;

public class FlybyMoverII : MonoBehaviour
{
	public Vector3 _Speed = Vector3.zero;

	private float lookX;

	private float lookY;

	public float lookSpeed = 40f;

	public float lookSmooth = 0.1f;

	public float sideLookSmooth = 0.08f;

	public float lookXMax = 20f;

	public float lookYMax = 20f;

	private Vector3 _eulers = Vector3.zero;

	private Vector3 _newLocal = Vector3.zero;

	private Vector3 _initLocal;

	private Vector3 _localEulers;


	private float orignal;

	private void Start()
	{
		_initLocal = base.transform.localPosition;
		_newLocal = _initLocal;
		_localEulers = base.transform.localEulerAngles;
		orignal = _localEulers.y;
	}

	private void Update()
	{
		base.transform.Translate(_Speed, Space.Self);
		lookX += AsunderInput.GetAxis("Mouse X") * lookSpeed * Time.deltaTime;
		lookY += AsunderInput.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;
		lookX = Mathf.Clamp(lookX, 0f - lookXMax, lookXMax);
		lookY = Mathf.Clamp(lookY, 0f - lookYMax, lookYMax);
		if (AsunderInput.GetMouseButtonDown(0))
		{
			_localEulers.y -= 25f;
		}
		if (AsunderInput.GetMouseButtonDown(1))
		{
			_localEulers.y += 25f;
		}
		if (AsunderInput.GetMouseButtonUp(0) || AsunderInput.GetMouseButtonUp(1))
		{
			_localEulers.y = orignal;
		}
		Quaternion to = Quaternion.Euler(0f - lookY, _localEulers.y, lookX);
		base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, to, lookSmooth);
	}
}
