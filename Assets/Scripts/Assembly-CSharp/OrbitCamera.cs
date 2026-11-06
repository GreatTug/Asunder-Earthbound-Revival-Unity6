using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
	public Transform target;

	public float distance = 10f;

	public float speed = 2f;

	public float zoomSpeed = 2f;

	public float minDistanceSpeed = 1f;

	public float maxDistanceSpeed = 100f;

	public float minDistance = 1f;

	public float maxDistance = 200f;

	public float shiftKeyModifier = 2.5f;

	private float x;

	private float y;

	private Transform thisTransform;

	private Vector3 previousTargetPos = Vector3.zero;

	public Transform xform
	{
		get
		{
			if (thisTransform == null)
			{
				thisTransform = base.transform;
			}
			return thisTransform;
		}
	}

	private void Start()
	{
		Setup();
	}

	private void LateUpdate()
	{
		float num = 1f;
		if (AsunderInput.GetKey(KeyCode.LeftShift) || AsunderInput.GetKey(KeyCode.RightShift))
		{
			num = shiftKeyModifier;
		}
		if (target != null)
		{
			bool flag = false;
			if (target.position != previousTargetPos)
			{
				flag = true;
			}
			float axis = AsunderInput.GetAxis("Mouse ScrollWheel");
			if (axis != 0f)
			{
				distance += (0f - axis) * Mathf.Lerp(minDistanceSpeed, maxDistanceSpeed, distance / maxDistance) * zoomSpeed * num;
				distance = Mathf.Clamp(distance, minDistance, maxDistance);
				flag = true;
			}
			float num2 = 0f;
			float num3 = 0f;
			num2 += AsunderInput.GetAxis("Horizontal");
			num3 += AsunderInput.GetAxis("Vertical");
			if (AsunderInput.GetMouseButton(0) || AsunderInput.GetMouseButton(1))
			{
				num2 += AsunderInput.GetAxis("Mouse X");
				num3 -= AsunderInput.GetAxis("Mouse Y");
			}
			if (num2 != 0f || num3 != 0f)
			{
				flag = true;
				x += Mathf.Clamp(num2, -1f, 1f) * speed * num;
				y += Mathf.Clamp(num3, -1f, 1f) * speed * num;
			}
			if (flag)
			{
				SetCamera();
			}
			previousTargetPos = target.position;
		}
	}

	public void Setup()
	{
		Vector3 eulerAngles = xform.eulerAngles;
		x = eulerAngles.y;
		y = eulerAngles.x;
		SetCamera();
	}

	public void SetOrbitPoint(float xAngle, float yAngle, float distance)
	{
		x = xAngle;
		y = yAngle;
		this.distance = distance;
		SetCamera();
	}

	public void SetOrbitPosition(Vector3 position, Vector3 up)
	{
		xform.position = position;
		xform.LookAt(target.position, up);
		x = xform.localEulerAngles.y;
		y = xform.localEulerAngles.x;
		distance = Vector3.Distance(position, target.position);
		SetCamera();
	}

	private void SetCamera()
	{
		Vector3 vector = Vector3.zero;
		if (target != null)
		{
			vector = target.position;
		}
		Quaternion quaternion = Quaternion.Euler(y, x, 0f);
		Vector3 position = quaternion * new Vector3(0f, 0f, 0f - distance) + vector;
		xform.rotation = quaternion;
		xform.position = position;
	}
}
