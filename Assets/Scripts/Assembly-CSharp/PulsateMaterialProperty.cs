using UnityEngine;

public class PulsateMaterialProperty : MonoBehaviour
{
	public Material material;

	public string propertyName = string.Empty;

	public float minVal;

	public float maxVal = 1f;

	public float pulsateSpeed = 1f;

	private void Update()
	{
		material.SetFloat(propertyName, Mathf.Lerp(minVal, maxVal, Mathf.PingPong(Time.time * pulsateSpeed, 1f)));
	}
}
