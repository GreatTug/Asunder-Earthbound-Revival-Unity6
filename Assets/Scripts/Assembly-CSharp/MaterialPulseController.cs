using UnityEngine;

public class MaterialPulseController : MonoBehaviour
{
	public float _Speed = 5f;

	private float _randomStart;

	public Color _initColor;

	private Vector3 _initScale;

	private Renderer _rend;

	private Material _mat;

	private void Start()
	{
		_randomStart = Random.Range(0.01f, 2.93f);
		_initScale = base.transform.localScale;
		_rend = GetComponent<Renderer>();
		if (_rend != null)
		{
			_mat = _rend.material;
		}
	}

	private void Update()
	{
		float num = (Mathf.Sin((Time.time + _randomStart) * _Speed) + 1f) / 2f;
		if (num < 0.5f)
		{
			base.transform.localScale = new Vector3(num * _initScale.x, num * _initScale.y, num * _initScale.z);
		}
		else
		{
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, _initScale, 0.3f);
		}
		if (_mat == null && _rend != null)
		{
			_mat = _rend.material;
		}
		if (_mat != null)
		{
			_mat.color = new Color(_initColor.r * num, _initColor.g * num, _initColor.b * num);
		}
	}
}
