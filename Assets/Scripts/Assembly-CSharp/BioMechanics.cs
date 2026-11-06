using UnityEngine;

public class BioMechanics : MonoBehaviour
{
	public bool _EnablePhysics;

	public bool _EnableGravity;

	public bool _UseSinWave = true;

	public float _InitialDrag = 0.5f;

	public float _UnifiedStrength = 100f;

	public float _UnifiedForce = 1f;

	public float _UnifiedFreq = 1f;

	public float _SyncOffset;

	public bool _OverwriteLimits;

	public float _Limit;

	public float _Spring = 0.1f;

	public float _Damper = 0.1f;

	public MuscleSegment[] _MuscleSegments;

	private float _timer;

	private void Start()
	{
		MuscleSegment[] muscleSegments = _MuscleSegments;
		foreach (MuscleSegment muscleSegment in muscleSegments)
		{
			muscleSegment.Name = muscleSegment._MuscleTransform.name;
			if (!muscleSegment._MuscleTransform.gameObject.GetComponent<ConstantForce>())
			{
				muscleSegment._MuscleTransform.gameObject.AddComponent<ConstantForce>();
			}
			if (muscleSegment._MuscleGraphic != null)
			{
				muscleSegment._MuscleGraphic.parent = muscleSegment._MuscleTransform;
			}
			if (!muscleSegment._EnablePhysics)
			{
				muscleSegment._MuscleTransform.GetComponent<Rigidbody>().isKinematic = true;
			}
			muscleSegment._timeOffset = Random.Range(0f, _SyncOffset);
			if (_EnableGravity)
			{
				muscleSegment._MuscleTransform.GetComponent<Rigidbody>().useGravity = true;
			}
			else
			{
				muscleSegment._MuscleTransform.GetComponent<Rigidbody>().useGravity = false;
			}
			muscleSegment._MuscleTransform.GetComponent<Rigidbody>().linearDamping = _InitialDrag;
		}
	}

	private void Update()
	{
		_UnifiedFreq = Mathf.PerlinNoise(Time.time * 0.1f, 0f) - 0.5f;
		if (_EnablePhysics)
		{
			MuscleSegment[] muscleSegments = _MuscleSegments;
			foreach (MuscleSegment muscleSegment in muscleSegments)
			{
				muscleSegment._MuscleTransform.GetComponent<Rigidbody>().isKinematic = false;
			}
		}
		else
		{
			MuscleSegment[] muscleSegments2 = _MuscleSegments;
			foreach (MuscleSegment muscleSegment2 in muscleSegments2)
			{
				muscleSegment2._MuscleTransform.GetComponent<Rigidbody>().isKinematic = true;
			}
		}
		MuscleSegment[] muscleSegments3 = _MuscleSegments;
		foreach (MuscleSegment muscleSegment3 in muscleSegments3)
		{
			CharacterJoint cj = muscleSegment3._MuscleTransform.GetComponent<CharacterJoint>();
			if (_OverwriteLimits && cj != null)
			{
				SoftJointLimit lowTwistLimit = cj.lowTwistLimit;
				lowTwistLimit.limit = _Limit;
				cj.lowTwistLimit = lowTwistLimit;
				SoftJointLimitSpring twistLimitSpring = cj.twistLimitSpring;
				twistLimitSpring.spring = _Spring;
				twistLimitSpring.damper = _Damper;
				cj.twistLimitSpring = twistLimitSpring;
			}
			ConstantForce cf = muscleSegment3._MuscleTransform.GetComponent<ConstantForce>();
			if (muscleSegment3._EnableMuscles)
			{
				if (muscleSegment3._Attachment != null)
				{
					muscleSegment3._MuscleTransform.position = muscleSegment3._Attachment.position;
					muscleSegment3._MuscleTransform.rotation = muscleSegment3._Attachment.rotation;
				}
				if (_UseSinWave && cf != null)
				{
					float num = Mathf.PerlinNoise((Time.time + muscleSegment3._timeOffset) * _UnifiedFreq, 0f) - 0.5f;
					cf.relativeTorque = muscleSegment3._Direction * _UnifiedStrength * num;
				}
				if (muscleSegment3._MuscleGraphic != null && !muscleSegment3._MuscleGraphic.gameObject.activeSelf)
				{
					muscleSegment3._MuscleGraphic.gameObject.SetActive(true);
				}
				if (muscleSegment3._ConstrainPosition)
				{
					muscleSegment3._MuscleTransform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
				}
				else if (muscleSegment3._ConstrainRotation)
				{
					muscleSegment3._MuscleTransform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
				}
				if (muscleSegment3._ConstrainPosition && muscleSegment3._ConstrainRotation)
				{
					muscleSegment3._MuscleTransform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
				}
				if (!muscleSegment3._ConstrainPosition && !muscleSegment3._ConstrainRotation)
				{
					muscleSegment3._MuscleTransform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
				}
			}
			else
			{
				if (cf != null)
				{
					cf.relativeTorque = Vector3.zero;
				}
				if (muscleSegment3._MuscleGraphic != null && muscleSegment3._MuscleGraphic.gameObject.activeSelf)
				{
					muscleSegment3._MuscleGraphic.gameObject.SetActive(false);
				}
			}
		}
	}
}
