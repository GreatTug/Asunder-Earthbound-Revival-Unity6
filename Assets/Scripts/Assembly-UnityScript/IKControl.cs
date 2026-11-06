using System;
using UnityEngine;

[Serializable]
public class IKControl : MonoBehaviour
{
	public Transform forearm;

	public Transform hand;

	public Transform target;

	public float transition;

	public float elbowAngle;

	private Transform armIK;

	private Transform armRotation;

	private float upperArmLength;

	private float forearmLength;

	private float armLength;

	public IKControl()
	{
		transition = 1f;
	}

	public virtual void Start()
	{
		GameObject gameObject = new GameObject("Arm IK");
		armIK = gameObject.transform;
		armIK.parent = transform;
		GameObject gameObject2 = new GameObject("Arm Rotation");
		armRotation = gameObject2.transform;
		armRotation.parent = armIK;
		upperArmLength = Vector3.Distance(transform.position, forearm.position);
		forearmLength = Vector3.Distance(forearm.position, hand.position);
		armLength = upperArmLength + forearmLength;
	}

	public virtual void Update()
	{
	}

	public virtual void LateUpdate()
	{
		Quaternion rotation = transform.rotation;
		Quaternion rotation2 = forearm.rotation;
		armIK.position = transform.position;
		armIK.LookAt(forearm);
		armRotation.position = transform.position;
		armRotation.rotation = transform.rotation;
		armIK.LookAt(target);
		transform.rotation = armRotation.rotation;
		float a = Vector3.Distance(transform.position, target.position);
		a = Mathf.Min(a, armLength - 1E-05f);
		float num = (upperArmLength * upperArmLength - forearmLength * forearmLength + a * a) / (2f * a);
		float num2 = Mathf.Acos(num / upperArmLength) * 57.29578f;
		transform.RotateAround(transform.position, transform.forward, 0f - num2);
		armIK.position = forearm.position;
		armIK.LookAt(hand);
		armRotation.position = forearm.position;
		armRotation.rotation = forearm.rotation;
		armIK.LookAt(target);
		forearm.rotation = armRotation.rotation;
		transform.RotateAround(transform.position, target.position - transform.position, elbowAngle);
		transition = Mathf.Clamp01(transition);
		transform.rotation = Quaternion.Slerp(rotation, transform.rotation, transition);
		forearm.rotation = Quaternion.Slerp(rotation2, forearm.rotation, transition);
	}

	public virtual void Main()
	{
	}
}
