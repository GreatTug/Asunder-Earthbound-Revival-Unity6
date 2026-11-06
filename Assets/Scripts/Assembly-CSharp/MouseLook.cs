using UnityEngine;

public class MouseLook : MonoBehaviour
{
	public float lookSpeed = 40f;

	public float lookSmooth = 0.1f;

	public float sideLookSmooth = 0.08f;

	public float lookXMax = 20f;

	public float lookYMax = 20f;

	public float sideTitltMax = 20f;

	private float initLookXMax;

	private float initLookYMax;

	private float initLookSpeed;

	private float initLookSmooth;

	private float initSideLookSmooth;

	private float initSideTitltMax;

	private float initRatioMove;

	private float lookX;

	private float lookY;

	private Vector3 _eulers = Vector3.zero;

	private Vector3 _newLocal = Vector3.zero;

	private Vector3 _initLocal;

	private float _RatioMove = 0.125f;

	private void Start()
	{
		_initLocal = base.transform.localPosition;
		_newLocal = _initLocal;
		initLookXMax = lookXMax;
		initLookYMax = lookYMax;
		initLookSpeed = lookSpeed;
		initLookSmooth = lookSmooth;
		initSideLookSmooth = sideLookSmooth;
		initSideTitltMax = sideTitltMax;
		initRatioMove = _RatioMove;
	}

	public void ConstrainNeck()
	{
		lookXMax = 8f;
		lookYMax = 12f;
		sideTitltMax = 5f;
		lookSmooth = 0.1f;
		_RatioMove = 0.05f;
	}

	public void UnconstrainNeck()
	{
		lookXMax = initLookXMax;
		lookYMax = initLookYMax;
		sideTitltMax = initSideTitltMax;
		lookSmooth = initLookSmooth;
		_RatioMove = initRatioMove;
	}

	private void Update()
	{
		if (AsunderInput.IsOpenXRActive)
		{
			return;
		}
		lookX += AsunderInput.GetAxis("Mouse X") * lookSpeed * Time.deltaTime;
		lookY += AsunderInput.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;
		lookX = Mathf.Clamp(lookX, 0f - lookXMax, lookXMax);
		lookY = Mathf.Clamp(lookY, 0f - lookYMax, lookYMax);
		Quaternion to = Quaternion.Euler(0f - lookY, lookX, _eulers.z);
		base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, to, lookSmooth);
	}
}
