using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKCtrl : MonoBehaviour
{
	protected Animator animator;

	public bool ikActive;

	public Transform rightHandObj;

	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	private void OnAnimatorIK()
	{
		if (!animator)
		{
			return;
		}
		if (ikActive)
		{
			animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
			animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
			if (rightHandObj != null)
			{
				animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
				animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
			}
		}
		else
		{
			animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
			animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
		}
	}
}
