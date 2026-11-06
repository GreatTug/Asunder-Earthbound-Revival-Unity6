using UnityEngine;

[ExecuteInEditMode]
public class AmbientLightAdjuster : MonoBehaviour
{
	public Color ambientLightColor = Color.red;

	private void Update()
	{
		RenderSettings.ambientLight = ambientLightColor;
	}
}
