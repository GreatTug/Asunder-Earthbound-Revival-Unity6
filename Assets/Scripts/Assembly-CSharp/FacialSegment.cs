using System;
using UnityEngine;

[Serializable]
public class FacialSegment
{
	[HideInInspector]
	public string name = string.Empty;

	public Transform _Transform;

	public ExpressiveType _Type;

	[HideInInspector]
	public Vector3 _TargetPosition = Vector3.zero;

	[HideInInspector]
	public Vector3 _StartPosition = Vector3.zero;

	public Vector3 _ChangeVector = Vector3.zero;
}
