using UnityEngine;

public class OVRComponent : MonoBehaviour
{
	protected float DeltaTime = 1f;

	public virtual void Awake()
	{
	}

	public virtual void Start()
	{
	}

	public virtual void Update()
	{
		DeltaTime = Time.deltaTime * 60f;
	}

	public virtual void LateUpdate()
	{
	}
}
