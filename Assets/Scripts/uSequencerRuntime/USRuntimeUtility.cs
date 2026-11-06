using UnityEngine;

public class USRuntimeUtility
{
	public static void StartRecordingSequence(USSequencer sequence, string capturePath, int captureFramerate, int upScaleAmount)
	{
		USRecordSequence orSpawnRecorder = GetOrSpawnRecorder();
		orSpawnRecorder.StartRecording();
		orSpawnRecorder.RecordOnStart = true;
		orSpawnRecorder.CapturePath = capturePath;
		orSpawnRecorder.CaptureFrameRate = captureFramerate;
		orSpawnRecorder.UpscaleAmount = upScaleAmount;
	}

	public static void StopRecordingSequence()
	{
		USRecordSequence orSpawnRecorder = GetOrSpawnRecorder();
		if ((bool)orSpawnRecorder)
		{
			Object.DestroyImmediate(orSpawnRecorder.gameObject);
		}
	}

	private static USRecordSequence GetOrSpawnRecorder()
	{
		USRecordSequence uSRecordSequence = Object.FindAnyObjectByType<USRecordSequence>();
		if ((bool)uSRecordSequence)
		{
			return uSRecordSequence;
		}
		string path = "EmptyPrefab";
		GameObject gameObject = Resources.Load(path, typeof(GameObject)) as GameObject;
		if (!gameObject)
		{
			Debug.LogError("Couldn't find prefab for USWindow::StartRecording, it is possible it has been moved from the resources folder");
			return null;
		}
		GameObject gameObject2 = Object.Instantiate(gameObject) as GameObject;
		gameObject2.name = "Recording Object";
		return gameObject2.AddComponent<USRecordSequence>();
	}
}
