using System;
using UnityEngine;

[Serializable]
public class MuscleSegment
{
	public string Name;

	public Transform _MuscleTransform;

	public Transform _MuscleGraphic;

	public Transform _Attachment;

	[HideInInspector]
	public bool _EnablePhysics;

	[HideInInspector]
	public float _timeOffset;

	public bool _EnableMuscles;

	public bool _ConstrainPosition;

	public bool _ConstrainRotation;

	public Vector3 _Direction = Vector3.zero;

	public float _Strength;
}
