using System.Collections;
using UnityEngine;

public class WalterController : MonoBehaviour
{
	public enum WalterState
	{
		idle = 0,
		reading = 1,
		lookAtPlayer = 2,
		lookAtPlayerWithAnger = 3,
		lookAtBiff = 4,
		lookAtPlayerPeeking = 5,
		lookAtPlayerHappy = 6,
		lookAtPlayerWindow = 7,
		lookAtPlayerWithConcern = 8,
		lookAtPlayerAsking = 9,
		reachForPlayer = 10,
		lookOverhead = 11,
		lookOutside = 12,
		lookAtPlayerLeftHand = 13,
		upsetAtPlayer = 14,
		lookAtEd = 15,
		lookAtLydia = 16,
		lookForSounds = 17
	}

	public GameObject _GazeTarget;

	public GameObject[] _GazePositions;

	public GameObject _HandTarget;

	public GameObject[] _HandPositions;

	public FacialController _FacialController;

	private Transform _HeadTarget;

	private float noiseSpeed = 0.2f;

	public WalterState _WalterState;

	private float _fsmLoopFrequency = 0.01f;

	private void Start()
	{
		HeadLookController hlc = GetComponent<HeadLookController>();
		if (hlc != null && hlc.segments != null && hlc.segments.Length > 0)
		{
			_HeadTarget = hlc.segments[0].target;
		}
		StartCoroutine(FSM());
	}

	private void Update()
	{
		if (AsunderInput.GetKeyDown(KeyCode.Q))
		{
			_WalterState = WalterState.idle;
		}
		if (AsunderInput.GetKeyDown(KeyCode.W))
		{
			_WalterState = WalterState.lookAtPlayer;
		}
		if (AsunderInput.GetKeyDown(KeyCode.E))
		{
			_WalterState = WalterState.reading;
		}
		if (AsunderInput.GetKeyDown(KeyCode.R))
		{
			_WalterState = WalterState.lookOverhead;
		}
		if (AsunderInput.GetKeyDown(KeyCode.T))
		{
			_WalterState = WalterState.lookOutside;
		}
	}

	public void SetState(WalterState aState)
	{
		_WalterState = aState;
	}

	private Vector3 GetGazePos(int index, Vector3 fallback = default)
	{
		if (_GazePositions != null && index >= 0 && index < _GazePositions.Length && _GazePositions[index] != null)
		{
			return _GazePositions[index].transform.position;
		}
		return fallback;
	}

	private Transform GetHandPos(int index)
	{
		if (_HandPositions != null && index >= 0 && index < _HandPositions.Length && _HandPositions[index] != null)
		{
			return _HandPositions[index].transform;
		}
		return null;
	}

	private void SetFacialExpression(float a, float b, float c, float d, float e)
	{
		if (_FacialController != null)
		{
			_FacialController.SetExpression(a, b, c, d, e);
		}
	}

	private void SetFacialRandomExpression(bool a = false, bool b = false)
	{
		if (_FacialController != null)
		{
			_FacialController.SetRandomExpression(a, b);
		}
	}

	private void setGazeTargetBehavior(Vector3 aGoalPosition, float aHeadMoveAmount, float aEyeMoveAmount, float headResponsivness = 0.75f)
	{
		if (_GazeTarget != null)
		{
			_GazeTarget.transform.position = aGoalPosition;
			if (_GazeTarget.transform.childCount > 0)
			{
				Transform c0 = _GazeTarget.transform.GetChild(0);
				c0.localPosition = new Vector3(0f, aHeadMoveAmount, 0f);
				if (c0.childCount > 0)
				{
					c0.GetChild(0).localPosition = new Vector3(0f, 0f, aEyeMoveAmount);
				}
			}
		}
		HeadLookController hlc = GetComponent<HeadLookController>();
		if (hlc != null && hlc.segments != null && hlc.segments.Length > 0)
		{
			hlc.segments[0].responsiveness = headResponsivness;
		}
	}

	private void loopGazeTargetBehavior(float aHeadFrequency, float aHeadMaxAngle, float aEyesFrequency, float aEyesMaxAngle)
	{
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		zero.x = Mathf.PerlinNoise(Time.time * aHeadFrequency, 0f) * (0f - aHeadMaxAngle);
		zero2.x = Mathf.PerlinNoise(Time.time * aEyesFrequency, 0f) * aEyesMaxAngle;
		zero.y = Mathf.PerlinNoise(Time.time * aHeadFrequency, 0f) * (0f - aHeadMaxAngle);
		zero2.y = Mathf.PerlinNoise(Time.time * aEyesFrequency, 0f) * aEyesMaxAngle;
		if (_GazeTarget != null)
		{
			_GazeTarget.transform.localRotation = Quaternion.Euler(zero);
			if (_GazeTarget.transform.childCount > 0)
			{
				_GazeTarget.transform.GetChild(0).localRotation = Quaternion.Euler(zero2);
			}
		}
	}

	private void lockHeadOnTarget(Vector3 aTarget)
	{
		if (_HeadTarget != null)
		{
			_HeadTarget.position = aTarget;
		}
	}

	private void lockEyesOnTarget(Vector3 aTarget)
	{
		if (_GazeTarget != null && _GazeTarget.transform.childCount > 0)
		{
			Transform c0 = _GazeTarget.transform.GetChild(0);
			if (c0.childCount > 0)
			{
				c0.GetChild(0).position = aTarget;
			}
		}
	}

	private void loopHandTargetBehavior(Transform aGoal)
	{
		if (aGoal == null || _HandTarget == null)
		{
			return;
		}
		Vector3 position = aGoal.position;
		position.y = aGoal.position.y + (Mathf.PerlinNoise(Time.time * 0.1f, 0f) / 2f - 1f) / 25f;
		position.z = aGoal.position.z + (Mathf.PerlinNoise((Time.time + 2.87876f) * 0.1f, 0f) / 2f - 1f) / 25f;
		_HandTarget.transform.position = Vector3.Lerp(_HandTarget.transform.position, position, 0.02f);
	}

	private IEnumerator idle()
	{
		yield return null;
		while (_WalterState == WalterState.idle)
		{
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator reading()
	{
		setGazeTargetBehavior(GetGazePos(0), 0f, 0f);
		SetFacialRandomExpression(true, false);
		yield return null;
		while (_WalterState == WalterState.reading)
		{
			cycleRandomReadingGaze();
			loopHandTargetBehavior(GetHandPos(0));
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private void cycleRandomReadingGaze()
	{
		if (_HeadTarget == null)
		{
			return;
		}
		Vector3 localPosition = _HeadTarget.localPosition;
		localPosition.x = (Mathf.PerlinNoise(Time.time * noiseSpeed, 0f) - 0.5f) * 2.4f * ((Mathf.Sin(Time.time * 0.2f) + 1f) / 2f);
		localPosition.y = (Mathf.PerlinNoise((Time.time + 0.23421f) * noiseSpeed, 0f) - 0.5f) * 1.1f * ((Mathf.Sin(Time.time * 0.2f) + 1f) / 2f);
		localPosition.z = (Mathf.PerlinNoise((Time.time + 0.713431f) * noiseSpeed, 0f) - 0.5f) * 0.2f * ((Mathf.Sin(Time.time * 0.2f) + 1f) / 2f);
		_HeadTarget.localPosition = Vector3.Lerp(_HeadTarget.localPosition, localPosition, (Mathf.Sin(Time.time * 0.3f) + 1f) / 2f);
	}

	private IEnumerator lookAtLydia()
	{
		float timer = Time.time + 3.5f;
		bool isLooking = true;
		setGazeTargetBehavior(GetGazePos(6), 0f, 0f, 1.3f);
		SetFacialExpression(0.75f, 0.75f, 0.5f, 0.2f, 0f);
		yield return null;
		while (_WalterState == WalterState.lookAtLydia)
		{
			if (Time.time > timer)
			{
				if (isLooking)
				{
					SetFacialExpression(0f, 0f, 0f, 0.05f, 2f);
					setGazeTargetBehavior(GetGazePos(0), 0f, 0f, 0.12f);
					cycleRandomReadingGaze();
					timer = Time.time + 8f;
					isLooking = false;
				}
				else
				{
					SetFacialExpression(0.75f, 0.75f, 0.5f, 0.1f, 1f);
					setGazeTargetBehavior(GetGazePos(6), 0f, 0f, 1.3f);
					timer = Time.time + 3.5f;
					isLooking = true;
				}
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator lookAtPlayerAsking()
	{
		SetFacialExpression(0.2f, -0.2f, 0.4f, 0.08f, 0f);
		float timer = Time.time + 4f;
		float timerII = Time.time + 10f;
		yield return null;
		while (_WalterState == WalterState.lookAtPlayerAsking)
		{
			if (Time.time > timer && Time.time < timerII)
			{
				SetFacialExpression(-0.8f, -1f, -0.8f, 0.06f, 0f);
			}
			if (Time.time > timerII)
			{
				SetFacialRandomExpression();
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator lookAtPlayerWithAnger()
	{
		SetFacialExpression(1f, 1f, 0f, 0.45f, 0f);
		setGazeTargetBehavior(GetGazePos(1), -0.4f, 0f);
		float timer = Time.time + 4f;
		yield return null;
		while (_WalterState == WalterState.lookAtPlayerWithAnger)
		{
			loopGazeTargetBehavior(0.2f, 100f, 0f, 0f);
			if (Time.time > timer)
			{
				SetState(WalterState.reading);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator lookAtBiff()
	{
		setGazeTargetBehavior(GetGazePos(9), 0f, 0f, 3f);
		float timer = Time.time + 1f;
		bool isLookingAtPlayer = true;
		yield return null;
		while (_WalterState == WalterState.lookAtBiff)
		{
			if (Time.time > timer)
			{
				if (isLookingAtPlayer)
				{
					lockEyesOnTarget(GetGazePos(9));
					isLookingAtPlayer = false;
				}
				else
				{
					lockEyesOnTarget(GetGazePos(1));
					isLookingAtPlayer = true;
				}
				timer = Time.time + 1f;
			}
			loopHandTargetBehavior(GetHandPos(0));
			if (_HandTarget != null)
			{
				Vector3 newPosition = new Vector3(_HandTarget.transform.localPosition.x - Mathf.PerlinNoise(Time.time * 9f + 4.23f - 0.5f, 0f) / 300f, _HandTarget.transform.localPosition.y - Mathf.PerlinNoise(Time.time * 9f + 1.63f - 0.5f, 0f) / 300f, _HandTarget.transform.localPosition.z);
				_HandTarget.transform.localPosition = newPosition;
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator lookAtPlayerHappy()
	{
		setGazeTargetBehavior(GetGazePos(8), -0.4f, 0f);
		float timer = Time.time + 2f;
		bool _hasLooked = false;
		yield return null;
		while (_WalterState == WalterState.lookAtPlayerHappy)
		{
			if (!_hasLooked && Time.time > timer)
			{
				SetFacialExpression(-0.75f, -0.85f, -0.1f, 0.05f, 0f);
				setGazeTargetBehavior(GetGazePos(1), -0.4f, 0f);
				_hasLooked = true;
			}
			lockEyesOnTarget(GetGazePos(1));
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator lookAtPlayerWithConcern()
	{
		SetFacialExpression(0.65f, 0.7f, 0.2f, 0.45f, 0f);
		setGazeTargetBehavior(GetGazePos(1), -0.4f, 0f);
		float timer = Time.time + 4f;
		yield return null;
		while (_WalterState == WalterState.lookAtPlayerWithConcern)
		{
			loopGazeTargetBehavior(0.2f, 100f, 0f, 0f);
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator lookAtPlayer()
	{
		setGazeTargetBehavior(GetGazePos(1), -0.4f, 0f);
		SetFacialExpression(-0.7f, -0.7f, 0f, 0.06f, 0f);
		float timer = Time.time + 3f;
		bool isLookingAtYou = true;
		yield return null;
		while (_WalterState == WalterState.lookAtPlayer)
		{
			loopGazeTargetBehavior(0.2f, 100f, 0f, 0f);
			if (Time.time > timer)
			{
				if (isLookingAtYou)
				{
					lockEyesOnTarget(GetGazePos(1));
					isLookingAtYou = false;
					timer = Time.time + 2f;
				}
				else
				{
					Vector3 newLookPos = GetGazePos(1);
					newLookPos.z += Random.Range(-1.2f, 1.2f);
					newLookPos.y += Random.Range(-1.2f, 1.2f);
					lockEyesOnTarget(newLookPos);
					timer = Time.time + Random.Range(0.01f, 2f);
					isLookingAtYou = true;
				}
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator lookOverhead()
	{
		float timer = Time.time + 5f;
		SetFacialRandomExpression();
		setGazeTargetBehavior(GetGazePos(2), 0f, 0f);
		yield return null;
		while (_WalterState == WalterState.lookOverhead)
		{
			if (Time.time > timer)
			{
				SetState(WalterState.reading);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator lookForSounds()
	{
		float timer = Time.time + 5f;
		SetFacialExpression(1f, 1f, 0f, 0.009f, 0f);
		SetFacialRandomExpression();
		setGazeTargetBehavior(GetGazePos(2), 0f, 0f);
		yield return null;
		while (_WalterState == WalterState.lookForSounds)
		{
			if (Time.time > timer)
			{
				Vector3 gp = GetGazePos(2);
				lockEyesOnTarget(new Vector3(gp.x + Random.Range(-0.3f, 0.3f), gp.y + Random.Range(-0.8f, 0.8f), gp.z + Random.Range(-1.3f, 1.3f)));
				timer = Time.time + (float)Random.Range(3, 8);
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator lookOutside()
	{
		setGazeTargetBehavior(GetGazePos(3), 0f, 0f);
		yield return null;
		while (_WalterState == WalterState.lookOutside)
		{
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator lookAtPlayerLeftHand()
	{
		lockEyesOnTarget(GetGazePos(4));
		float timer = Time.time + 3f;
		yield return null;
		while (_WalterState == WalterState.lookAtPlayerLeftHand)
		{
			if (Time.time > timer)
			{
				setGazeTargetBehavior(GetGazePos(4), 0f, 0f);
				SetFacialExpression(1f, 1f, 0f, 0.005f, 0f);
			}
			yield return AsunderCoroutines.FsmWait;
		}
		SetFacialExpression(1f, 1f, 0f, 0.075f, 0f);
	}

	private IEnumerator lookAtPlayerPeeking()
	{
		setGazeTargetBehavior(GetGazePos(10), 0f, 0f, 1.25f);
		yield return null;
		while (_WalterState == WalterState.lookAtPlayerPeeking)
		{
			lockEyesOnTarget(GetGazePos(1));
			yield return AsunderCoroutines.FsmWait;
		}
		setGazeTargetBehavior(GetGazePos(4), 0f, 0f);
	}

	private IEnumerator lookAtPlayerWindow()
	{
		lockEyesOnTarget(GetGazePos(7));
		float timer = Time.time + 3f;
		yield return null;
		while (_WalterState == WalterState.lookAtPlayerWindow)
		{
			if (Time.time > timer)
			{
				setGazeTargetBehavior(GetGazePos(7), 0f, 0f);
				SetFacialExpression(-0.75f, -0.75f, -0.5f, 0.01f, 0f);
			}
			yield return AsunderCoroutines.FsmWait;
		}
		SetFacialExpression(1f, 1f, 0f, 0.075f, 0f);
	}

	private IEnumerator lookAtEd()
	{
		float timer = Time.time + 3.5f;
		bool isLooking = true;
		setGazeTargetBehavior(GetGazePos(5), 0f, 0f, 1.3f);
		SetFacialExpression(0.75f, 0.75f, 0.5f, 0.2f, 0f);
		yield return null;
		while (_WalterState == WalterState.lookAtEd)
		{
			if (Time.time > timer)
			{
				if (isLooking)
				{
					SetFacialExpression(0f, 0f, 0f, 0.05f, 2f);
					cycleRandomReadingGaze();
					setGazeTargetBehavior(GetGazePos(0), 0f, 0f, 0.12f);
					timer = Time.time + 8f;
					isLooking = false;
				}
				else
				{
					SetFacialExpression(0.75f, 0.75f, 0.5f, 0.1f, 1f);
					setGazeTargetBehavior(GetGazePos(5), 0f, 0f, 1.3f);
					timer = Time.time + 3.5f;
					isLooking = true;
				}
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator upsetAtPlayer()
	{
		SetFacialExpression(-1f, 1f, -1f, 0.1f, 0f);
		setGazeTargetBehavior(GetGazePos(1), -0.2f, 0f);
		float timer = Time.time + 3f;
		bool isLookingAtYou = true;
		yield return null;
		while (_WalterState == WalterState.upsetAtPlayer)
		{
			loopGazeTargetBehavior(0.9f, 200f, 0.9f, 100f);
			loopHandTargetBehavior(GetHandPos(0));
			if (_HandTarget != null)
			{
				Vector3 newPosition = new Vector3(_HandTarget.transform.localPosition.x - Mathf.PerlinNoise(Time.time * 9f + 4.23f - 0.5f, 0f) / 300f, _HandTarget.transform.localPosition.y - Mathf.PerlinNoise(Time.time * 9f + 1.63f - 0.5f, 0f) / 300f, _HandTarget.transform.localPosition.z);
				_HandTarget.transform.localPosition = newPosition;
			}
			if (Time.time > timer)
			{
				if (isLookingAtYou)
				{
					lockEyesOnTarget(GetGazePos(1));
					isLookingAtYou = false;
					timer = Time.time + 1f;
				}
				else
				{
					Vector3 newLookPos = GetGazePos(1);
					newLookPos.z += Random.Range(-2.2f, 2.2f);
					newLookPos.x += Random.Range(-2.2f, 2.2f);
					lockEyesOnTarget(newLookPos);
					timer = Time.time + Random.Range(0.01f, 0.5f);
					isLookingAtYou = true;
				}
			}
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator reachForPlayer()
	{
		SetFacialExpression(-1f, 1f, 1f, 0.08f, 0f);
		setGazeTargetBehavior(GetGazePos(1), -0.2f, 0f);
		float timer = Time.time + 3f;
		yield return null;
		while (_WalterState == WalterState.reachForPlayer)
		{
			loopGazeTargetBehavior(0.9f, 200f, 0.9f, 100f);
			loopHandTargetBehavior(GetHandPos(2));
			Transform h2 = GetHandPos(2);
			if (_HandTarget != null && h2 != null)
			{
				_HandTarget.transform.rotation = Quaternion.Lerp(_HandTarget.transform.rotation, h2.rotation, 0.1f);
			}
			if (_HandTarget != null)
			{
				Vector3 newPosition = new Vector3(_HandTarget.transform.localPosition.x - Mathf.PerlinNoise(Time.time * 9f + 4.23f - 0.5f, 0f) / 300f, _HandTarget.transform.localPosition.y - Mathf.PerlinNoise(Time.time * 9f + 1.63f - 0.5f, 0f) / 300f, _HandTarget.transform.localPosition.z);
				_HandTarget.transform.localPosition = newPosition;
			}
			lockEyesOnTarget(GetGazePos(1));
			yield return AsunderCoroutines.FsmWait;
		}
	}

	private IEnumerator FSM()
	{
		while (true)
		{
			yield return StartCoroutine(_WalterState.ToString());
		}
	}
}
