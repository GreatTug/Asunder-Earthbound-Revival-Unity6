using UnityEngine;

public class PingPong : MonoBehaviour
{
	public float lerpTime = 5f;

	public Vector3 posMin = Vector3.zero;

	public Vector3 posMax = Vector3.zero;

	private Transform thisTransform;

	private void OnEnable()
	{
		thisTransform = base.transform;
	}

	private void Update()
	{
		thisTransform.localPosition = Vector3.Lerp(posMin, posMax, Mathf.PingPong(Time.time * (1f / lerpTime), 1f));
	}
}
