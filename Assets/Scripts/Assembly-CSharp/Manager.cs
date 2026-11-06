using UnityEngine;

public class Manager : MonoBehaviour
{
	public Transform _GazeTarget;

	public Transform _HandIKTarget;

	public Transform _UserTarget;

	public Transform _PaperReadTarget;

	public Transform _PaperIdleTarget;

	public Transform _PaperAwayTarget;

	public NoiseController _HandNoise;


	private void Start()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
	}

	private void moveDone()
	{
		MonoBehaviour.print("done?");
		_HandNoise._Segments[0]._Enabled = true;
	}
}
