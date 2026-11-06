using System;
using UnityEngine;

[Serializable]
public class GlowPlaneAngleFade : MonoBehaviour
{
	public Transform cameraTransform;

	public Color glowColor;

	private float dot;

	public GlowPlaneAngleFade()
	{
		glowColor = Color.grey;
		dot = 0.5f;
	}

	public virtual void Start()
	{
		if (!cameraTransform)
		{
			cameraTransform = Camera.main.transform;
		}
	}

	public virtual void Update()
	{
		dot = 1.5f * Mathf.Clamp01(Vector3.Dot(cameraTransform.forward, -transform.up) - 0.25f);
	}

	public virtual void OnWillRenderObject()
	{
		GetComponent<Renderer>().sharedMaterial.SetColor("_TintColor", glowColor * dot);
	}

	public virtual void Main()
	{
	}
}
