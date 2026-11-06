using UnityEngine;

public class RavenFlight : MonoBehaviour
{
	private float _turningSpeed = 8f;

	private float _flyingSpeed = 1000f;

	private float _speedDampTime = 0.25f;

	private float _directionDampTime = 0.25f;

	private float _preferredHeight = 50f;


	private float _distanceToGround;

	public Animator _Animator;

	public Rigidbody _Rigidbody;

	public Transform _FollowTarget;

	private float _maxDistanceToTarget = 15f;

	private bool _ascending = true;

	private float randnumber = 1f;

	private float timer;

	private float randomTimerNumber;

	private Transform _WingBoneRight;

	private Transform _WingBoneLeft;

	private float _flapSpeed = 7f;

	private float _flapAmplification = 50f;

	private void Start()
	{
		if (_Animator == null)
		{
			_Animator = GetComponent<Animator>();
		}
		if (_Rigidbody == null)
		{
			_Rigidbody = GetComponent<Rigidbody>();
		}
		_flyingSpeed = Random.Range(650, 1000);
		randnumber = Random.Range(0, 10);
		randomTimerNumber = Random.Range(0.000231f, 57.003433f);
		timer = Time.time + randnumber;
		if (randnumber > 5f)
		{
			_ascending = false;
		}
		else
		{
			_ascending = true;
		}
		if (_Animator != null)
		{
			_Animator.SetBool("Glide", true);
		}
		if (base.transform.childCount > 1 && base.transform.GetChild(1).childCount > 0 && base.transform.GetChild(1).GetChild(0).childCount > 3)
		{
			_WingBoneRight = base.transform.GetChild(1).GetChild(0).GetChild(3);
			_WingBoneLeft = base.transform.GetChild(1).GetChild(0).GetChild(2);
		}
		if (_WingBoneRight == null || _WingBoneLeft == null)
		{
			Transform[] allChildren = GetComponentsInChildren<Transform>(true);
			for (int i = 0; i < allChildren.Length; i++)
			{
				string childName = allChildren[i].name.ToLowerInvariant();
				if (_WingBoneRight == null && childName.Contains("wing") && (childName.Contains("r") || childName.Contains("right")))
				{
					_WingBoneRight = allChildren[i];
				}
				if (_WingBoneLeft == null && childName.Contains("wing") && (childName.Contains("l") || childName.Contains("left")))
				{
					_WingBoneLeft = allChildren[i];
				}
			}
		}
	}

	private void LateUpdate()
	{
		if (_Rigidbody == null)
		{
			return;
		}
		CheckHeight();
		FollowTarget();
		_Rigidbody.AddForce(base.transform.forward * _flyingSpeed);
		if (_WingBoneRight != null && _WingBoneLeft != null)
		{
			if (_ascending)
			{
				float num = Mathf.Sin((Time.time + randomTimerNumber) * _flapSpeed) * _flapAmplification + 150f;
				_WingBoneRight.localRotation = Quaternion.Euler(0f, 90f, 0f - num);
				_WingBoneLeft.localRotation = Quaternion.Euler(-180f, 90f, 0f - num);
			}
			else
			{
				_WingBoneRight.localRotation = Quaternion.Lerp(_WingBoneRight.localRotation, Quaternion.Euler(0f, 90f, 220f), 0.2f);
				_WingBoneLeft.localRotation = Quaternion.Lerp(_WingBoneLeft.localRotation, Quaternion.Euler(-180f, 90f, 220f), 0.2f);
			}
		}
		if (Time.time > timer)
		{
			if (_ascending)
			{
				_ascending = false;
			}
			else
			{
				_ascending = true;
			}
			timer = Time.time + (float)Random.Range(0, 10);
		}
		if (_ascending && _distanceToGround <= _preferredHeight)
		{
			_Rigidbody.AddForce(Vector3.up * 100f);
		}
	}

	private void FollowTarget()
	{
		if (_FollowTarget == null || _Animator == null || _Rigidbody == null)
		{
			return;
		}
		Vector3 lhs = _Animator.rootRotation * Vector3.forward;
		Vector3 normalized = (_FollowTarget.position - _Animator.rootPosition).normalized;
		float y = Vector3.Cross(lhs, normalized).y;
		if (Vector3.Distance(_FollowTarget.position, _Animator.rootPosition) > _maxDistanceToTarget)
		{
			if (Vector3.Dot(lhs, normalized) > 0f)
			{
				_Animator.SetFloat("Direction", y, _directionDampTime, Time.deltaTime);
			}
			else
			{
				_Animator.SetFloat("Direction", (y > 0f) ? 1 : (-1), _directionDampTime, Time.deltaTime);
			}
			if (_Animator.GetFloat("Direction") != 0f)
			{
				float yAngle = _turningSpeed * y * _directionDampTime;
				base.transform.Rotate(0f, yAngle, 0f);
				_Rigidbody.AddForce(base.transform.forward * 100f * _speedDampTime);
			}
			else
			{
				base.transform.Rotate(0f, 0f, 0f);
			}
		}
	}

	private void CheckHeight()
	{
		Ray ray = new Ray(base.transform.position + Vector3.up, -Vector3.up);
		RaycastHit hitInfo = default(RaycastHit);
		if (Physics.Raycast(ray, out hitInfo))
		{
			_distanceToGround = hitInfo.distance;
		}
	}
}
