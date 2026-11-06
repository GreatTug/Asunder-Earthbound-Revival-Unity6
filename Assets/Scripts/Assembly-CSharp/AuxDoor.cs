using UnityEngine;

public class AuxDoor : MonoBehaviour
{
	public Transform _AuxDoor;

	private void Start()
	{
		if (_AuxDoor != null)
		{
			_AuxDoor.parent = base.transform;
		}
	}

	private void Update()
	{
	}
}
