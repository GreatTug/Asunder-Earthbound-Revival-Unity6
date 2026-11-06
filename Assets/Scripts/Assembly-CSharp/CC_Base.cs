using UnityEngine;

[AddComponentMenu("")]
[RequireComponent(typeof(Camera))]
public class CC_Base : MonoBehaviour
{
	public Shader shader;

	protected Material _material;

	protected Material material
	{
		get
		{
			if (_material == null)
			{
				if (shader == null)
				{
					shader = Shader.Find("Hidden/" + GetType().Name);
				}
				if (shader != null)
				{
					_material = new Material(shader);
					_material.hideFlags = HideFlags.HideAndDontSave;
				}
			}
			return _material;
		}
	}

	protected virtual void Start()
	{
		if (shader == null)
		{
			shader = Shader.Find("Hidden/" + GetType().Name);
		}
		if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
		{
			base.enabled = false;
		}
		else if (!shader || !shader.isSupported)
		{
			base.enabled = false;
		}
	}

	protected virtual void OnDisable()
	{
		if ((bool)_material)
		{
			Object.DestroyImmediate(_material);
		}
	}
}
