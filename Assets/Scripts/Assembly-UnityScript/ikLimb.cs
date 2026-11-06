using System;
using UnityEngine;

[Serializable]
public class ikLimb : MonoBehaviour
{
	public Transform upperArm;

	public Transform forearm;

	public Transform hand;

	public Transform target;

	public Transform elbowTarget;

	public bool IsEnabled;

	public bool debug;

	public float transition;

	private Quaternion upperArmStartRotation;

	private Quaternion forearmStartRotation;

	private Quaternion handStartRotation;

	private Vector3 targetRelativeStartPosition;

	private Vector3 elbowTargetRelativeStartPosition;

	public ikLimb()
	{
		IsEnabled = true;
		debug = true;
		transition = 1f;
	}

	public virtual void Start()
	{
		upperArmStartRotation = upperArm.rotation;
		forearmStartRotation = forearm.rotation;
		handStartRotation = hand.rotation;
		targetRelativeStartPosition = target.position - upperArm.position;
		elbowTargetRelativeStartPosition = elbowTarget.position - upperArm.position;
	}

	public virtual void LateUpdate()
	{
		if (IsEnabled)
		{
			CalculateIK();
		}
	}

	public virtual void CalculateIK()
	{
		float num = Vector3.Distance(upperArm.position, forearm.position);
		float num2 = Vector3.Distance(forearm.position, hand.position);
		float num3 = num + num2;
		float num4 = num;
		float a = Vector3.Distance(upperArm.position, target.position);
		a = Mathf.Min(a, num3 - 0.0001f);
		float num5 = (Mathf.Pow(num4, 2f) - Mathf.Pow(num2, 2f) + Mathf.Pow(a, 2f)) / (2f * a);
		Debug.Log(num5);
		float num6 = Mathf.Acos(num5 / num4) * 57.29578f;
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
		target.position = targetRelativeStartPosition + upperArm.position;
		elbowTarget.position = elbowTargetRelativeStartPosition + upperArm.position;
		upperArm.rotation = upperArmStartRotation;
		forearm.rotation = forearmStartRotation;
		hand.rotation = handStartRotation;
		transform.position = upperArm.position;
		transform.LookAt(position, position2 - transform.position);
		GameObject gameObject = new GameObject("upperArmAxisCorrection");
		GameObject gameObject2 = new GameObject("forearmAxisCorrection");
		GameObject gameObject3 = new GameObject("handAxisCorrection");
		gameObject.transform.position = upperArm.position;
		gameObject.transform.LookAt(forearm.position, transform.root.up);
		gameObject.transform.parent = transform;
		upperArm.parent = gameObject.transform;
		gameObject2.transform.position = forearm.position;
		gameObject2.transform.LookAt(hand.position, transform.root.up);
		gameObject2.transform.parent = gameObject.transform;
		forearm.parent = gameObject2.transform;
		gameObject3.transform.position = hand.position;
		gameObject3.transform.parent = gameObject2.transform;
		hand.parent = gameObject3.transform;
		target.position = position;
		elbowTarget.position = position2;
		gameObject.transform.LookAt(target, elbowTarget.position - gameObject.transform.position);
		float x = gameObject.transform.localRotation.eulerAngles.x - num6;
		Quaternion localRotation = gameObject.transform.localRotation;
		Vector3 eulerAngles = localRotation.eulerAngles;
		float num7 = (eulerAngles.x = x);
		Vector3 vector = (localRotation.eulerAngles = eulerAngles);
		Quaternion quaternion = (gameObject.transform.localRotation = localRotation);
		gameObject2.transform.LookAt(target, elbowTarget.position - gameObject.transform.position);
		gameObject3.transform.rotation = target.rotation;
		upperArm.parent = parent;
		forearm.parent = parent2;
		hand.parent = parent3;
		upperArm.localScale = localScale;
		forearm.localScale = localScale2;
		hand.localScale = localScale3;
		upperArm.localPosition = localPosition;
		forearm.localPosition = localPosition2;
		hand.localPosition = localPosition3;
		UnityEngine.Object.Destroy(gameObject);
		UnityEngine.Object.Destroy(gameObject2);
		UnityEngine.Object.Destroy(gameObject3);
		transition = Mathf.Clamp01(transition);
		upperArm.rotation = Quaternion.Slerp(rotation, upperArm.rotation, transition);
		forearm.rotation = Quaternion.Slerp(rotation2, forearm.rotation, transition);
		hand.rotation = Quaternion.Slerp(rotation3, hand.rotation, transition);
		if (debug)
		{
			Debug.DrawLine(forearm.position, elbowTarget.position, Color.yellow);
			Debug.DrawLine(upperArm.position, target.position, Color.red);
		}
	}

	public virtual void Main()
	{
	}
}
