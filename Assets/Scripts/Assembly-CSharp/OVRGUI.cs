using UnityEngine;

public class OVRGUI
{
	private Font FontReplace;

	private OVRCameraController CameraController;

	private float PixelWidth = 1280f;

	private float PixelHeight = 800f;

	private float DisplayWidth = 1280f;

	private float DisplayHeight = 800f;

	private Rect DrawRect;

	public void SetCameraController(ref OVRCameraController cameraController)
	{
		CameraController = cameraController;
	}

	public void GetFontReplace(ref Font fontReplace)
	{
		fontReplace = FontReplace;
	}

	public void SetFontReplace(Font fontReplace)
	{
		FontReplace = fontReplace;
	}

	public void GetPixelResolution(ref float pixelWidth, ref float pixelHeight)
	{
		pixelWidth = PixelWidth;
		pixelHeight = PixelHeight;
	}

	public void SetPixelResolution(float pixelWidth, float pixelHeight)
	{
		PixelWidth = pixelWidth;
		PixelHeight = pixelHeight;
	}

	public void GetDisplayResolution(ref float Width, ref float Height)
	{
		Width = DisplayWidth;
		Height = DisplayHeight;
	}

	public void SetDisplayResolution(float Width, float Height)
	{
		DisplayWidth = Width;
		DisplayHeight = Height;
	}

	public void StereoBox(int X, int Y, int wX, int hY, ref string text, Color color)
	{
		Font font = GUI.skin.font;
		GUI.color = color;
		if (GUI.skin.font != FontReplace)
		{
			GUI.skin.font = FontReplace;
		}
		float num = PixelWidth / DisplayWidth;
		CalcPositionAndSize((float)X * num, (float)Y * num, (float)wX * num, (float)hY * num, ref DrawRect);
		GUI.Box(DrawRect, text);
		GUI.skin.font = font;
	}

	public void StereoBox(float X, float Y, float wX, float hY, ref string text, Color color)
	{
		StereoBox((int)(X * PixelWidth), (int)(Y * PixelHeight), (int)(wX * PixelWidth), (int)(hY * PixelHeight), ref text, color);
	}

	public void StereoDrawTexture(int X, int Y, int wX, int hY, ref Texture image, Color color)
	{
		GUI.color = color;
		if (GUI.skin.font != FontReplace)
		{
			GUI.skin.font = FontReplace;
		}
		float num = PixelWidth / DisplayWidth;
		CalcPositionAndSize((float)X * num, (float)Y * num, (float)wX * num, (float)hY * num, ref DrawRect);
		GUI.DrawTexture(DrawRect, image);
	}

	public void StereoDrawTexture(float X, float Y, float wX, float hY, ref Texture image, Color color)
	{
		StereoDrawTexture((int)(X * PixelWidth), (int)(Y * PixelHeight), (int)(wX * PixelWidth), (int)(hY * PixelHeight), ref image, color);
	}

	private void CalcPositionAndSize(float X, float Y, float wX, float hY, ref Rect calcPosSize)
	{
		float num = (float)Screen.width / PixelWidth;
		float num2 = (float)Screen.height / PixelHeight;
		if (CameraController != null && CameraController.PortraitMode)
		{
			num = (float)Screen.height / PixelWidth;
			num2 = (float)Screen.width / PixelHeight;
		}
		calcPosSize.x = X * num;
		calcPosSize.width = wX * num;
		calcPosSize.y = Y * num2;
		calcPosSize.height = hY * num2;
	}
}
