using UnityEngine;

[AddComponentMenu("Colorful/Brightness, Contrast, Gamma")]
[ExecuteInEditMode]
public class CC_BrightnessContrastGamma : CC_Base
{
	public float redCoeff = 0.5f;

	public float greenCoeff = 0.5f;

	public float blueCoeff = 0.5f;

	public float brightness;

	public float contrast;

	public float gamma = 1f;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Material mat = base.material;
		if (mat == null)
		{
			Graphics.Blit(source, destination);
			return;
		}
		mat.SetFloat("_rCoeff", redCoeff);
		mat.SetFloat("_gCoeff", greenCoeff);
		mat.SetFloat("_bCoeff", blueCoeff);
		mat.SetFloat("_brightness", (brightness + 100f) * 0.01f);
		mat.SetFloat("_contrast", (contrast + 100f) * 0.01f);
		mat.SetFloat("_gamma", 1f / gamma);
		Graphics.Blit(source, destination, mat);
	}
}
