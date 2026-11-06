using UnityEngine;

public class FacialController : MonoBehaviour
{
	public float _Valence;

	public float _Smile;

	public float _Peer;

	public float _RandomFrequency = 0.25f;

	public bool _randomize = true;

	private float _change = 20f;

	private float _expressionSmooth = 0.075f;

	private bool _activeMovement = true;

	public bool _randomizeValence;

	public bool _randomizeSmile;

	public bool _randomizePeer;

	public FacialSegment[] _FacialSegments;

	private void Start()
	{
		FacialSegment[] facialSegments = _FacialSegments;
		foreach (FacialSegment facialSegment in facialSegments)
		{
			facialSegment.name = facialSegment._Transform.name;
			facialSegment._TargetPosition = facialSegment._Transform.localPosition;
			facialSegment._StartPosition = facialSegment._Transform.localPosition;
		}
	}

	public void SetExpression(float aValence, float aSmile, float aPeer, float smooth = 0.075f, float delay = 0f)
	{
		_Valence = Mathf.Clamp(aValence, -1f, 1f);
		_Smile = Mathf.Clamp(aSmile, -1f, 1f);
		_Peer = Mathf.Clamp(aPeer, -1f, 1f);
		_expressionSmooth = smooth;
		_randomize = false;
		if (delay == 0f)
		{
			_activeMovement = true;
			return;
		}
		_activeMovement = false;
		Invoke("activateMovement", delay);
	}

	private void activateMovement()
	{
		_activeMovement = true;
	}

	public void SetRandomExpression(bool valance = true, bool smile = true, bool peer = true)
	{
		_randomize = true;
		_randomizeValence = valance;
		_randomizeSmile = smile;
		_randomizePeer = peer;
	}

	private void Update()
	{
		if (!_activeMovement)
		{
			return;
		}
		if (_randomize)
		{
			if (_randomizeValence)
			{
				_Valence = Mathf.PerlinNoise(Time.time * _RandomFrequency, 0f) * 2f - 1f;
				if (_Valence < 0f)
				{
					_Valence = 0f;
				}
			}
			if (_randomizeSmile)
			{
				_Smile = Mathf.PerlinNoise(Time.time * _RandomFrequency + 4f, 0f) * 2f - 1f;
			}
			if (_randomizePeer)
			{
				_Peer = Mathf.PerlinNoise(0f, Time.time * _RandomFrequency + 7f) * 2f - 1f;
			}
		}
		else
		{
			_Valence = Mathf.Clamp(_Valence, -1f, 1f);
			_Smile = Mathf.Clamp(_Smile, -1f, 1f);
			_Peer = Mathf.Clamp(_Peer, -1f, 1f);
		}
		FacialSegment[] facialSegments = _FacialSegments;
		foreach (FacialSegment facialSegment in facialSegments)
		{
			if (facialSegment._Type == ExpressiveType.Valence)
			{
				moveSegment(facialSegment, _Valence);
			}
			else if (facialSegment._Type == ExpressiveType.Smile)
			{
				moveSegment(facialSegment, _Smile);
			}
			else if (facialSegment._Type == ExpressiveType.Peer)
			{
				moveSegment(facialSegment, _Peer);
			}
		}
	}

	private void moveSegment(FacialSegment segment, float aMaxMin)
	{
		segment._TargetPosition.x = aMaxMin * segment._ChangeVector.x / _change + segment._StartPosition.x;
		segment._TargetPosition.y = aMaxMin * segment._ChangeVector.y / _change + segment._StartPosition.y;
		segment._TargetPosition.z = aMaxMin * segment._ChangeVector.z / _change + segment._StartPosition.z;
		segment._Transform.localPosition = Vector3.Lerp(segment._Transform.localPosition, segment._TargetPosition, _expressionSmooth);
	}
}
