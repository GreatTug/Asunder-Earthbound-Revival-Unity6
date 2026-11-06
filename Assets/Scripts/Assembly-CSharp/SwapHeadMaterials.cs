using UnityEngine;

public class SwapHeadMaterials : MonoBehaviour
{
	public Light directionalLight;

	public Rotate[] toggleRotateScripts;

	public bool infoShowing = true;

	public bool lightsPaused = true;

	private void Awake()
	{
		Rotate[] array = toggleRotateScripts;
		foreach (Rotate rotate in array)
		{
			rotate.enabled = !lightsPaused;
		}
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
		if (infoShowing)
		{
			GUILayout.Label("Press P key to toggle light movement");
			GUILayout.Label("Click and drag or use arrow keys to orbit camera around the head");
			GUILayout.Label("Zoom with the mouse scrollwheel");
			GUILayout.Label("Press O key to toggle mouse cursor display");
			GUILayout.Label("\nPress Tab to toggle this information\n");
		}
	}
}
