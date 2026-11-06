using UnityEngine;

public class ParentTo : MonoBehaviour
{
	public Transform _NewParent;

	private void Start()
	{
		base.transform.parent = _NewParent;
	}

	private void Update()
	{
	}
}
