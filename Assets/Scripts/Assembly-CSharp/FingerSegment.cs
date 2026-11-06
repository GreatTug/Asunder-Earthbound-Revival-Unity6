using System;
using UnityEngine;

[Serializable]
public class FingerSegment
{
	public Transform _Transform;

	public Vector3 _rotationOffset = Vector3.zero;

	public float _GrabAmount;

	public float _RandomNumber;
}
