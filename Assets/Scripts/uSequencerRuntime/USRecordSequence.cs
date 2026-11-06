using UnityEngine;

public class USRecordSequence : MonoBehaviour
{
	private bool isRecording;

	private bool recordOnStart;

	private int imageCount;

	private int captureFrameRate = 60;

	private int upscaleAmount = 1;

	private string capturePath = string.Empty;

	public int CaptureFrameRate
	{
		set
		{
			captureFrameRate = Mathf.Clamp(value, 1, 60);
		}
	}

	public int UpscaleAmount
	{
		set
		{
			upscaleAmount = Mathf.Clamp(value, 1, 8);
		}
	}

	public string CapturePath
	{
		set
		{
			capturePath = value;
		}
	}

	public bool RecordOnStart
	{
		set
		{
			recordOnStart = value;
		}
	}

	private void Start()
	{
		imageCount = 0;
		isRecording = recordOnStart;
	}

	private void LateUpdate()
	{
		Time.captureFramerate = captureFrameRate;
		if (isRecording)
		{
			ScreenCapture.CaptureScreenshot(capturePath + "/Screenshot_" + imageCount + ".png", upscaleAmount);
			imageCount++;
		}
	}

	public void StartRecording()
	{
		isRecording = true;
	}

	public void PauseRecording()
	{
		isRecording = false;
	}

	public void StopRecording()
	{
		isRecording = false;
		imageCount = 0;
	}
}
