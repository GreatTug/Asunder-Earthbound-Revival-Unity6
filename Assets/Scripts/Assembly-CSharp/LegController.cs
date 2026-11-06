using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegController : MonoBehaviour
{
	public enum LegState
	{
		idle = 0,
		ready = 1,
		kick = 2,
		returnToFloor = 3
	}

	public Transform _LegTarget;

	public Transform _LegPresetPositionsRoot;

	public GameObject _FrontSeat;

	private List<Transform> _presetPositions = new List<Transform>();

	private int _activePresetID;

	private float _legSmoothing = 0.08f;

	private float _timer;

	public LegState _ActiveLegState;

	private float _fsmLoopFrequency = 0.01f;

	private void Start()
	{
		if (_LegPresetPositionsRoot != null)
		{
			for (int i = 0; i < _LegPresetPositionsRoot.childCount; i++)
			{
				_presetPositions.Add(_LegPresetPositionsRoot.GetChild(i));
			}
		}
		StartCoroutine(FSM());
	}

	private void Update()
	{
	}

	public void SetState(LegState aLegState)
	{
		_ActiveLegState = aLegState;
	}

	private IEnumerator idle()
	{
		yield return null;
		while (_ActiveLegState == LegState.idle)
		{
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator returnToFloor()
	{
		yield return null;
		while (_ActiveLegState == LegState.returnToFloor)
		{
			if (_LegTarget != null && _presetPositions.Count > 0 && _presetPositions[0] != null)
			{
				_LegTarget.position = Vector3.Lerp(_LegTarget.position, _presetPositions[0].position, _legSmoothing);
				_LegTarget.rotation = Quaternion.Lerp(_LegTarget.rotation, _presetPositions[0].rotation, _legSmoothing);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator ready()
	{
		yield return null;
		while (_ActiveLegState == LegState.ready)
		{
			if (_LegTarget != null && _presetPositions.Count > 1 && _presetPositions[1] != null)
			{
				_LegTarget.position = Vector3.Lerp(_LegTarget.position, _presetPositions[1].position, 0.08f);
				_LegTarget.rotation = Quaternion.Lerp(_LegTarget.rotation, _presetPositions[1].rotation, 0.2f);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator kick()
	{
		_timer = Time.time + 0.3f;
		if (_FrontSeat != null)
		{
			iTween.ShakeRotation(_FrontSeat, iTween.Hash("amount", new Vector3(5.01f, 0f, 0f), "time", 0.3f, "islocal", true));
		}
		if (_presetPositions.Count > 2 && _presetPositions[2] != null)
		{
			AudioSource audio = _presetPositions[2].GetComponent<AudioSource>();
			if (audio != null)
			{
				audio.Play();
			}
		}
		yield return null;
		while (_ActiveLegState == LegState.kick)
		{
			if (_LegTarget != null && _presetPositions.Count > 2 && _presetPositions[2] != null)
			{
				_LegTarget.position = Vector3.Lerp(_LegTarget.position, _presetPositions[2].position, 0.8f);
				_LegTarget.rotation = Quaternion.Lerp(_LegTarget.rotation, _presetPositions[2].rotation, 0.8f);
			}
			if (Time.time > _timer)
			{
				SetState(LegState.returnToFloor);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator FSM()
	{
		while (true)
		{
			yield return StartCoroutine(_ActiveLegState.ToString());
		}
	}
}
