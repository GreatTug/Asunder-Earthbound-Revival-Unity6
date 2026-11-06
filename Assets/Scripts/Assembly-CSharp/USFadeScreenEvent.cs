using UnityEngine;

[USequencerEvent("Fullscreen/Fade Screen")]
[USequencerFriendlyName("Fade Screen")]
public class USFadeScreenEvent : USEventBase
{
	public UILayer uiLayer;

	public AnimationCurve fadeCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f), new Keyframe(3f, 1f), new Keyframe(4f, 0f));

	public Color fadeColour = Color.black;

	private float currentCurveSampleTime;

	private Color _currentOverlayColor = new Color(0f, 0f, 0f, 0f);

	public static Texture2D texture;

	public override void FireEvent()
	{
	}

	public override void ProcessEvent(float deltaTime)
	{
		currentCurveSampleTime = deltaTime;
		if (!texture)
		{
			texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		}
		float b = fadeCurve.Evaluate(currentCurveSampleTime);
		b = Mathf.Min(Mathf.Max(0f, b), 1f);
		_currentOverlayColor = new Color(fadeColour.r, fadeColour.g, fadeColour.b, b);
		texture.SetPixel(0, 0, _currentOverlayColor);
		texture.Apply();
	}

	public override void EndEvent()
	{
		if (!texture)
		{
			texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		}
		float b = fadeCurve.Evaluate(fadeCurve.keys[fadeCurve.length - 1].time);
		b = Mathf.Min(Mathf.Max(0f, b), 1f);
		_currentOverlayColor = new Color(fadeColour.r, fadeColour.g, fadeColour.b, b);
		texture.SetPixel(0, 0, _currentOverlayColor);
		texture.Apply();
	}

	public override void StopEvent()
	{
		UndoEvent();
	}

	public override void UndoEvent()
	{
		currentCurveSampleTime = 0f;
		if (!texture)
		{
			texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		}
		_currentOverlayColor = new Color(fadeColour.r, fadeColour.g, fadeColour.b, 0f);
		texture.SetPixel(0, 0, _currentOverlayColor);
		texture.Apply();
	}

	private void OnCameraPostRender(Camera cam)
	{
		if (base.Sequence != null && base.Sequence.IsPlaying && _currentOverlayColor.a > 0.001f)
		{
			CameraFade.DrawStereoOverlayQuad(_currentOverlayColor);
		}
	}

	private void OnGUI()
	{
		if (base.Sequence == null || !base.Sequence.IsPlaying)
		{
			return;
		}
		float num = 0f;
		Keyframe[] keys = fadeCurve.keys;
		for (int i = 0; i < keys.Length; i++)
		{
			Keyframe keyframe = keys[i];
			if (keyframe.time > num)
			{
				num = keyframe.time;
			}
		}
		base.Duration = num;
		if ((bool)texture && _currentOverlayColor.a > 0.001f)
		{
			int depth = GUI.depth;
			GUI.depth = (int)uiLayer;
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), texture);
			GUI.depth = depth;
		}
	}

	private void OnEnable()
	{
		Camera.onPostRender += OnCameraPostRender;
		if (texture == null)
		{
			texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		}
		_currentOverlayColor = new Color(fadeColour.r, fadeColour.g, fadeColour.b, 0f);
		texture.SetPixel(0, 0, _currentOverlayColor);
		texture.Apply();
	}

	private void OnDisable()
	{
		Camera.onPostRender -= OnCameraPostRender;
	}
}
