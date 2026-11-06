using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IK : MonoBehaviour
{
	public float smoother = 0.1f;

	protected Animator avatar;

	public bool bla;

	private float _timer;

	public bool IKlerp;

	public bool ikActive;

	public Transform bodyObj;

	public Transform leftFootObj;

	public Transform rightFootObj;

	public Transform leftHandObj;

	private bool switcher = true;

	public Transform rightHandObj;

	public Transform lookAtObj;

	public float leftFootWeightPosition = 1f;

	public float leftFootWeightRotation = 1f;

	public float rightFootWeightPosition = 1f;

	public float rightFootWeightRotation = 1f;

	public float leftHandWeightPosition = 1f;

	public float leftHandWeightRotation = 1f;

	public float rightHandWeightPosition = 1f;

	public float rightHandWeightRotation = 1f;

	public float lookAtWeight = 1f;

	private void Start()
	{
		avatar = GetComponent<Animator>();
	}

	private void OnGUI()
	{
	}

	private void OnAnimatorIK(int layerIndex)
	{
		if (!avatar)
		{
			return;
		}
		_timer += Time.deltaTime;
		if (ikActive)
		{
			if (IKlerp)
			{
				if (switcher)
				{
					_timer = 0f;
					switcher = false;
				}
			}
			else if (!switcher)
			{
				_timer = 0f;
				switcher = true;
			}
			if (IKlerp)
			{
				avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand, Mathf.Lerp(0f, 1f, _timer * smoother));
				avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand, Mathf.Lerp(0f, 1f, _timer * smoother));
				avatar.SetIKPositionWeight(AvatarIKGoal.RightHand, Mathf.Lerp(0f, 1f, _timer * smoother));
				avatar.SetIKRotationWeight(AvatarIKGoal.RightHand, Mathf.Lerp(0f, 1f, _timer * smoother));
				avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot, Mathf.Lerp(0f, 1f, _timer * smoother));
				avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot, Mathf.Lerp(0f, 1f, _timer * smoother));
				avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot, Mathf.Lerp(0f, 1f, _timer * smoother));
				avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot, Mathf.Lerp(0f, 1f, _timer * smoother));
			}
			else
			{
				avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand, Mathf.Lerp(1f, 0f, _timer * smoother));
				avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand, Mathf.Lerp(1f, 0f, _timer * smoother));
				avatar.SetIKPositionWeight(AvatarIKGoal.RightHand, Mathf.Lerp(1f, 0f, _timer * smoother));
				avatar.SetIKRotationWeight(AvatarIKGoal.RightHand, Mathf.Lerp(1f, 0f, _timer * smoother));
				avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot, Mathf.Lerp(1f, 0f, _timer * smoother));
				avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot, Mathf.Lerp(1f, 0f, _timer * smoother));
				avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot, Mathf.Lerp(1f, 0f, _timer * smoother));
				avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot, Mathf.Lerp(1f, 0f, _timer * smoother));
			}
			avatar.SetLookAtWeight(lookAtWeight, 0.3f, 0.6f, 1f, 0.5f);
			if (bodyObj != null)
			{
				avatar.bodyPosition = bodyObj.position;
				avatar.bodyRotation = bodyObj.rotation;
			}
			if (leftFootObj != null)
			{
				avatar.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootObj.position);
				avatar.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootObj.rotation);
			}
			if (rightFootObj != null)
			{
				avatar.SetIKPosition(AvatarIKGoal.RightFoot, rightFootObj.position);
				avatar.SetIKRotation(AvatarIKGoal.RightFoot, rightFootObj.rotation);
			}
			if (leftHandObj != null)
			{
				avatar.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
				avatar.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
			}
			if (rightHandObj != null)
			{
				avatar.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
				avatar.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
			}
			if (lookAtObj != null)
			{
				avatar.SetLookAtPosition(lookAtObj.position);
			}
		}
		else
		{
			avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0f);
			avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0f);
			avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0f);
			avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0f);
		}
	}

	private void Update()
	{
		if ((bool)avatar && !ikActive)
		{
			if (bodyObj != null)
			{
				bodyObj.position = avatar.bodyPosition;
				bodyObj.rotation = avatar.bodyRotation;
			}
			if (leftFootObj != null)
			{
				leftFootObj.position = avatar.GetIKPosition(AvatarIKGoal.LeftFoot);
				leftFootObj.rotation = avatar.GetIKRotation(AvatarIKGoal.LeftFoot);
			}
			if (rightFootObj != null)
			{
				rightFootObj.position = avatar.GetIKPosition(AvatarIKGoal.RightFoot);
				rightFootObj.rotation = avatar.GetIKRotation(AvatarIKGoal.RightFoot);
			}
			if (leftHandObj != null)
			{
			}
			if (rightHandObj != null)
			{
				rightHandObj.position = avatar.GetIKPosition(AvatarIKGoal.RightHand);
				rightHandObj.rotation = avatar.GetIKRotation(AvatarIKGoal.RightHand);
			}
			if (lookAtObj != null)
			{
				lookAtObj.position = avatar.bodyPosition + avatar.bodyRotation * new Vector3(0f, 0.5f, 1f);
			}
		}
	}
}
