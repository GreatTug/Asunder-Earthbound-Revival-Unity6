using UnityEngine;

[AddComponentMenu("Colorful/Posterize")]
[ExecuteInEditMode]
public class CC_Posterize : CC_Base
{
	public int levels = 4;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		base.material.SetFloat("_levels", levels);
		Graphics.Blit(source, destination, base.material);
	}
}
