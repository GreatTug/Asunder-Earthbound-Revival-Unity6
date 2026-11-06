using UnityEngine;

public class CreatureClankerController : MonoBehaviour
{
	public Vector3 _WalkScale;

	public float _WalkSpeed;

	private Vector3 _initPosition;

	private void Start()
	{
		_initPosition = base.transform.localPosition;
	}

	private void Update()
	{
		base.transform.localPosition = _initPosition + new Vector3(Mathf.Sin(Time.time * _WalkSpeed) * _WalkScale.x, Mathf.Sin(Time.time * _WalkSpeed) * _WalkScale.y, Mathf.Sin(Time.time * _WalkSpeed) * _WalkScale.z);
	}
}
