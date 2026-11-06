using UnityEngine;

public class WarningLight : MonoBehaviour
{
	public Light _Light;

	private float strength;

	private Color _color;

	public Color _colorChange;

	private Renderer _rend;

	private Material _mat;

	private void Start()
	{
		_rend = GetComponent<Renderer>();
		if (_rend != null)
		{
			_mat = _rend.material;
		}
	}

	private void Update()
	{
		strength = (Mathf.Sin(Time.time * 2.7f) + 1f) / 2f;
		if ((double)strength > 0.97)
		{
			_color = _colorChange;
		}
		else
		{
			_color.r = 0f;
			_color.g = 0f;
			_color.b = 0f;
		}
		if (_mat == null && _rend != null)
		{
			_mat = _rend.material;
		}
		if (_mat != null)
		{
			_mat.color = _color;
		}
		if (_Light != null)
		{
			_Light.color = _color;
		}
	}
}
