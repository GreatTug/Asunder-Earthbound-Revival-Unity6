using UnityEngine;

[AddComponentMenu("Colorful/Grayscale")]
[ExecuteInEditMode]
public class CC_Grayscale : CC_Base
{
	public float redLuminance = 0.3f;

	public float greenLuminance = 0.59f;

	public float blueLuminance = 0.11f;

	public float amount = 1f;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Material mat = base.material;
		if (mat == null)
		{
			Graphics.Blit(source, destination);
			return;
		}
		mat.SetFloat("_rLum", redLuminance);
		mat.SetFloat("_gLum", greenLuminance);
		mat.SetFloat("_bLum", blueLuminance);
		mat.SetFloat("_amount", amount);
		Graphics.Blit(source, destination, mat);
	}
}
