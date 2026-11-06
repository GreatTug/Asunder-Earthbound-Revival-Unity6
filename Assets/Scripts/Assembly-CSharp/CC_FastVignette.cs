using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Colorful/Fast Vignette")]
public class CC_FastVignette : CC_Base
{
	public float sharpness = 10f;

	public float darkness = 30f;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Material mat = base.material;
		if (mat == null)
		{
			Graphics.Blit(source, destination);
			return;
		}
		mat.SetFloat("_sharpness", sharpness * 0.01f);
		mat.SetFloat("_darkness", darkness * 0.02f);
		Graphics.Blit(source, destination, mat);
	}
}
