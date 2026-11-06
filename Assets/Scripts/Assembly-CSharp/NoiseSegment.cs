using System;
using UnityEngine;

[Serializable]
public class NoiseSegment
{
	public Transform _Segment;

	public bool _Enabled = true;

	public float _Speed;

	public Vector3 _Scale;

	private Vector3 _orginalAngles;

	private Vector3 _currentAngles;

	private Vector3 _orginalNoise;

	public float _RandomTiming;

	public void SetOriginalAngels(Vector3 aVector3)
	{
		_orginalAngles = aVector3;
	}

	public Vector3 GetOriginialAngels()
	{
		return _orginalAngles;
	}

	public void SetCurrentAngels(Vector3 aVector3)
	{
		_currentAngles = aVector3;
	}

	public Vector3 GetCurrentAngels()
	{
		return _currentAngles;
	}

	public void SetOriginalNoise(Vector3 aVector3)
	{
		_orginalNoise = aVector3;
	}

	public Vector3 GetOriginialNoise()
	{
		return _orginalNoise;
	}
}
