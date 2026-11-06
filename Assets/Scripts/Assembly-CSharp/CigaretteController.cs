using UnityEngine;

public class CigaretteController : MonoBehaviour
{
	public Transform _FollowTarget;

	public ParticleSystem _SmokePSys;

	private void Start()
	{
	}

	private void Update()
	{
		if (_FollowTarget != null)
		{
			base.transform.position = _FollowTarget.position;
			base.transform.rotation = _FollowTarget.rotation;
		}
	}
}
