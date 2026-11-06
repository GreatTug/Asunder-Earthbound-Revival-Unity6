using System.Collections;
using UnityEngine;

public class LydiaController : MonoBehaviour
{
	public enum LydiaState
	{
		idle = 0,
		lookAtYou = 1,
		returnToSeat = 2
	}

	public LydiaState _ActiveLydiaState;

	private Vector3 _initGazePosition = Vector3.zero;

	public GameObject _Spine;

	public GameObject _GazeTarget;

	public float _spineSmooth = 3.06f;

	private float _fsmLoopFrequency = 0.01f;

	private HeadLookController _headLookController;

	private FacialController _facialController;

	private void Start()
	{
		_headLookController = GetComponent<HeadLookController>();
		_facialController = GetComponent<FacialController>();
		if (_GazeTarget != null)
		{
			_initGazePosition = _GazeTarget.transform.localPosition;
		}
		StartCoroutine(FSM());
	}

	public void SetState(LydiaState aState)
	{
		_ActiveLydiaState = aState;
	}

	private void Update()
	{
	}

	private IEnumerator idle()
	{
		yield return null;
		while (_ActiveLydiaState == LydiaState.idle)
		{
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator lookAtYou()
	{
		if (_headLookController != null)
		{
			_headLookController.enabled = true;
		}
		if (_GazeTarget != null)
		{
			_GazeTarget.transform.localPosition = new Vector3(-1.6f, 1.345f, -2.284f);
		}
		if (_facialController != null)
		{
			_facialController.enabled = true;
		}
		yield return null;
		while (_ActiveLydiaState == LydiaState.lookAtYou)
		{
			if (_GazeTarget != null)
			{
				Vector3 newLocalPos = _GazeTarget.transform.localPosition;
				newLocalPos.y = _GazeTarget.transform.localPosition.y + (Mathf.PerlinNoise(Time.time * 0.6f, 0f) - 0.5f) * 0.05f;
				_GazeTarget.transform.localPosition = newLocalPos;
			}
			if (_Spine != null)
			{
				_Spine.transform.localRotation = Quaternion.Lerp(_Spine.transform.localRotation, Quaternion.Euler(30.1f, 339.6f, 318.6f), _spineSmooth * Time.deltaTime);
			}
			yield return AsunderCoroutines.FsmWait;
		}
		if (_facialController != null)
		{
			_facialController.enabled = false;
		}
	}

	private IEnumerator returnToSeat()
	{
		if (_GazeTarget != null)
		{
			_GazeTarget.transform.localPosition = _initGazePosition;
		}
		float timer = Time.time + 3.5f;
		yield return null;
		while (_ActiveLydiaState == LydiaState.returnToSeat)
		{
			if (_Spine != null)
			{
				_Spine.transform.localRotation = Quaternion.Lerp(_Spine.transform.localRotation, Quaternion.Euler(0f, 17.75f, 0f), _spineSmooth * Time.deltaTime);
			}
			if (Time.time > timer)
			{
				SetState(LydiaState.idle);
			}
			yield return AsunderCoroutines.FsmWait;
		}
		if (_headLookController != null)
		{
			_headLookController.enabled = false;
		}
	}

	private IEnumerator FSM()
	{
		while (true)
		{
			yield return StartCoroutine(_ActiveLydiaState.ToString());
		}
	}
}
