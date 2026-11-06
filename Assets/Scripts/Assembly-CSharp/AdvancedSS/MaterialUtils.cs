using UnityEngine;

namespace AdvancedSS
{
	public class MaterialUtils
	{
		public static void SetTextureOnMaterials(Material[] materials, string propertyName, Texture texture)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				materials[i].SetTexture(propertyName, texture);
			}
		}
	}
}
