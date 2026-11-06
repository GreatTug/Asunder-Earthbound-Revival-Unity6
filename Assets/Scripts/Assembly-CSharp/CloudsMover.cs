using UnityEngine;

public class CloudsMover : MonoBehaviour
{
	private Vector2 _MoveVector = Vector2.zero;

	public Vector2 _MoveSetVector = new Vector2(0f, 0.05f);

	public bool _DoOnce;

	private Renderer _renderer;

	private Material _material;

	private void Start()
	{
		_renderer = GetComponent<Renderer>();
		if (_renderer != null)
		{
			_material = _renderer.material;
		}
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void Update()
	{
		if (_material == null)
		{
			if (_renderer == null)
			{
				_renderer = GetComponent<Renderer>();
			}
			if (_renderer != null)
			{
				_material = _renderer.material;
			}
			if (_material == null)
			{
				return;
			}
		}

		if (!_DoOnce)
		{
			_MoveVector.x += _MoveSetVector.x;
			_MoveVector.y += _MoveSetVector.y;
			_material.SetTextureOffset("_MainTex", _MoveVector);
		}
		else if (_MoveVector.y > -0.8375f)
		{
			_MoveVector.y += _MoveSetVector.y * Time.deltaTime;
			_material.SetTextureOffset("_MainTex", _MoveVector);
		}
	}
}
