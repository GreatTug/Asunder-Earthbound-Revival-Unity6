using UnityEngine;

public class FollowRotation : MonoBehaviour
{
	public Transform _target;

	private Vector3 pos = Vector3.zero;

	private void Start()
	{
		pos = base.transform.localPosition;
	}

	private void Update()
	{
		if (AsunderInput.GetKeyDown(KeyCode.DownArrow))
		{
			pos.x++;
		}
		if (AsunderInput.GetKeyDown(KeyCode.UpArrow))
		{
			pos.x--;
		}
		base.transform.localPosition = pos;
	}
}
