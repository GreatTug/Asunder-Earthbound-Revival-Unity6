using System;
using AdvancedSS;
using UnityEngine;

public class ShaderSelector : MonoBehaviour
{
	public Renderer objectRenderer;

	public Light directionalLight;

	public Rotate[] toggleRotateScripts;

	public bool infoShowing = true;

	public bool lightsPaused = true;

	private AdvancedShader advancedShader;

	private void Awake()
	{
		Rotate[] array = toggleRotateScripts;
		foreach (Rotate rotate in array)
		{
			rotate.enabled = !lightsPaused;
		}
	}

	private void OnEnable()
	{
		advancedShader = new AdvancedShader();
	}

	private void Update()
	{
		if (AsunderInput.GetKeyUp(KeyCode.P))
		{
			lightsPaused = !lightsPaused;
			Rotate[] array = toggleRotateScripts;
			foreach (Rotate rotate in array)
			{
				rotate.enabled = !lightsPaused;
			}
		}
		if (AsunderInput.GetKeyUp(KeyCode.O))
		{
			Cursor.visible = !Cursor.visible;
		}
		if (AsunderInput.GetKeyUp(KeyCode.Tab))
		{
			infoShowing = !infoShowing;
		}
	}

	private void OnGUI()
	{
		if (!infoShowing)
		{
			return;
		}
		GUILayout.Label("Press P key to toggle light movement");
		GUILayout.Label("Click and drag or use arrow keys to orbit camera");
		GUILayout.Label("Zoom with the mouse scrollwheel");
		GUILayout.Label("Press O key to toggle mouse cursor display");
		GUILayout.Label("\nPress Tab to toggle this information\n");
		GUILayout.Space(20f);
		bool flag = false;
		bool flag2 = false;
		AdvancedShader.ShaderEffect[] array = (AdvancedShader.ShaderEffect[])Enum.GetValues(typeof(AdvancedShader.ShaderEffect));
		for (int i = 0; i < array.Length; i++)
		{
			flag = advancedShader.HasEffect(array[i]);
			bool flag3 = GUILayout.Toggle(flag, array[i].ToString());
			if (flag != flag3)
			{
				flag2 = true;
				if (flag3)
				{
					advancedShader.shaderEffects.Add(array[i]);
				}
				else
				{
					advancedShader.shaderEffects.Remove(array[i]);
				}
			}
		}
		if (flag2)
		{
			if (Shader.Find(advancedShader.ShaderName()) == null)
			{
				GUILayout.Space(20f);
				GUILayout.Label("That effect combination is a built-in shader");
			}
			else
			{
				objectRenderer.material.shader = Shader.Find(advancedShader.ShaderName());
			}
		}
	}
}
