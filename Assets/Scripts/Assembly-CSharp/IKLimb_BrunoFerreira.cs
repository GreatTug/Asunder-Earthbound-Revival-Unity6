using UnityEngine;

public class IKLimb_BrunoFerreira : MonoBehaviour
{
	public enum HandRotations
	{
		KeepLocalRotation = 0,
		KeepGlobalRotation = 1,
		UseTargetRotation = 2
	}

	public Transform upperArm;

	public Transform forearm;

	public Transform hand;

	public Vector3 offsetUpperArm;

	public Transform target;

	public Transform elbowTarget;

	public bool IsEnabled;

	public bool debug;

	public float transition = 1f;

	public bool idleOptimization;

	public HandRotations handRotationPolicy;

	private Quaternion upperArmStartRotation;

	private Quaternion forearmStartRotation;

	private Quaternion handStartRotation;

	private Vector3 targetRelativeStartPosition;

	private Vector3 elbowTargetRelativeStartPosition;

	private GameObject upperArmAxisCorrection;

	private GameObject forearmAxisCorrection;

	private GameObject handAxisCorrection;

	private Vector3 lastUpperArmPosition;

	private Vector3 lastTargetPosition;

	private Vector3 lastElbowTargetPosition;

	private void Start()
	{
		upperArmStartRotation = upperArm.rotation;
		forearmStartRotation = forearm.rotation;
		handStartRotation = hand.rotation;
		elbowTargetRelativeStartPosition = elbowTarget.position - upperArm.position;
		upperArmAxisCorrection = new GameObject("upperArmAxisCorrection");
		forearmAxisCorrection = new GameObject("forearmAxisCorrection");
		handAxisCorrection = new GameObject("handAxisCorrection");
		upperArmAxisCorrection.transform.parent = base.transform;
		forearmAxisCorrection.transform.parent = upperArmAxisCorrection.transform;
		handAxisCorrection.transform.parent = forearmAxisCorrection.transform;
		lastUpperArmPosition = upperArm.position + 5f * Vector3.up;
	}

	private void LateUpdate()
	{
		if (IsEnabled)
		{
			CalculateIK();
		}
	}

	private void CalculateIK()
	{
		if (target == null)
		{
			targetRelativeStartPosition = Vector3.zero;
			return;
		}
		if (targetRelativeStartPosition == Vector3.zero && target != null)
		{
			targetRelativeStartPosition = target.position - upperArm.position;
		}
		if (idleOptimization && lastUpperArmPosition == upperArm.position && lastTargetPosition == target.position && lastElbowTargetPosition == elbowTarget.position)
		{
			if (debug)
			{
				Debug.DrawLine(forearm.position, elbowTarget.position, Color.yellow);
				Debug.DrawLine(upperArm.position, target.position, Color.red);
			}
			return;
		}
		lastUpperArmPosition = upperArm.position;
		lastTargetPosition = target.position;
		lastElbowTargetPosition = elbowTarget.position;
		float num = Vector3.Distance(upperArm.position, forearm.position);
		float num2 = Vector3.Distance(forearm.position, hand.position);
		float num3 = num + num2;
		float num4 = num;
		float a = Vector3.Distance(upperArm.position, target.position);
		a = Mathf.Min(a, num3 - 0.0001f);
		float num5 = (num4 * num4 - num2 * num2 + a * a) / (2f * a);
		float x = Mathf.Acos(num5 / num4) * 57.29578f;
		Vector3 position = target.position;
		Vector3 position2 = elbowTarget.position;
		Transform parent = upperArm.parent;
		Transform parent2 = forearm.parent;
		Transform parent3 = hand.parent;
		Vector3 localScale = upperArm.localScale;
		Vector3 localScale2 = forearm.localScale;
		Vector3 localScale3 = hand.localScale;
		Vector3 localPosition = upperArm.localPosition;
		Vector3 localPosition2 = forearm.localPosition;
		Vector3 localPosition3 = hand.localPosition;
		Quaternion rotation = upperArm.rotation;
		Quaternion rotation2 = forearm.rotation;
		Quaternion rotation3 = hand.rotation;
		Quaternion localRotation = hand.localRotation;
		target.position = targetRelativeStartPosition + upperArm.position;
		elbowTarget.position = elbowTargetRelativeStartPosition + upperArm.position;
		upperArm.rotation = upperArmStartRotation;
		forearm.rotation = forearmStartRotation;
		hand.rotation = handStartRotation;
		base.transform.position = upperArm.position;
		base.transform.LookAt(position, position2 - base.transform.position);
		upperArmAxisCorrection.transform.position = upperArm.position;
		upperArmAxisCorrection.transform.LookAt(forearm.position, upperArm.up);
		upperArm.parent = upperArmAxisCorrection.transform;
		forearmAxisCorrection.transform.position = forearm.position;
		forearmAxisCorrection.transform.LookAt(hand.position, forearm.up);
		forearm.parent = forearmAxisCorrection.transform;
		handAxisCorrection.transform.position = hand.position;
		hand.parent = handAxisCorrection.transform;
		target.position = position;
		elbowTarget.position = position2;
		upperArmAxisCorrection.transform.LookAt(target, elbowTarget.position - upperArmAxisCorrection.transform.position);
		upperArmAxisCorrection.transform.localRotation = Quaternion.Euler(upperArmAxisCorrection.transform.localRotation.eulerAngles - new Vector3(x, 0f, 0f));
		forearmAxisCorrection.transform.LookAt(target, elbowTarget.position - upperArmAxisCorrection.transform.position);
		handAxisCorrection.transform.rotation = target.rotation;
		upperArm.parent = parent;
		forearm.parent = parent2;
		hand.parent = parent3;
		upperArm.localScale = localScale;
		forearm.localScale = localScale2;
		hand.localScale = localScale3;
		upperArm.localPosition = localPosition;
		forearm.localPosition = localPosition2;
		hand.localPosition = localPosition3;
		transition = Mathf.Clamp01(transition);
		upperArm.rotation = Quaternion.Slerp(rotation, upperArm.rotation, transition);
		forearm.rotation = Quaternion.Slerp(rotation2, forearm.rotation, transition);
		switch (handRotationPolicy)
		{
		case HandRotations.KeepLocalRotation:
			hand.localRotation = localRotation;
			break;
		case HandRotations.KeepGlobalRotation:
			hand.rotation = rotation3;
			break;
		case HandRotations.UseTargetRotation:
			hand.rotation = target.rotation;
			break;
		}
		if (debug)
		{
			Debug.DrawLine(forearm.position, elbowTarget.position, Color.yellow);
			Debug.DrawLine(upperArm.position, target.position, Color.red);
			Debug.Log("[IK Limb] adjacent: " + num5);
		}
	}
}
