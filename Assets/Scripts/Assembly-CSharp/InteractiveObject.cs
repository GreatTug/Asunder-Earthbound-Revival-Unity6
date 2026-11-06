using System;
using UnityEngine;

[Serializable]
public class InteractiveObject
{
	public GameObject _InteractiveObject;

	public bool _enabled;

	public void SetInteractive(bool aBool)
	{
		_enabled = aBool;
		if (_enabled)
		{
			_InteractiveObject.GetComponent<Collider>().enabled = true;
		}
		else
		{
			_InteractiveObject.GetComponent<Collider>().enabled = false;
		}
	}
}
