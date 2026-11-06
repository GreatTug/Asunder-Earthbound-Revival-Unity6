using UnityEngine;

public class BiffController : MonoBehaviour
{
	public float _Speed = 0.08f;

	public GameObject _ArmRight;

	public GameObject _ForearmRight;

	private bool _activeStrangle;

	private Quaternion _initRotation = Quaternion.identity;

	private Quaternion _initForeArmRotation = Quaternion.identity;

	public GameObject _MeshRender1;

	public GameObject _MeshRender2;

	private void SetRendererEnabled(GameObject go, bool enabled)
	{
		if (go != null)
		{
			Renderer rend = go.GetComponent<Renderer>();
			if (rend != null)
			{
				rend.enabled = enabled;
			}
		}
	}

	private void OnEnable()
	{
		if (_ArmRight != null)
		{
			_initRotation = _ArmRight.transform.localRotation;
		}
		if (_ForearmRight != null)
		{
			_initForeArmRotation = _ForearmRight.transform.localRotation;
		}
		SetRendererEnabled(_MeshRender1, true);
		SetRendererEnabled(_MeshRender2, true);
	}

	private void OnDisable()
	{
		if (_ArmRight != null)
		{
			_ArmRight.transform.localRotation = _initRotation;
		}
		if (_ForearmRight != null)
		{
			_ForearmRight.transform.localRotation = _initForeArmRotation;
		}
		SetRendererEnabled(_MeshRender1, false);
		SetRendererEnabled(_MeshRender2, false);
	}

	private void Update()
	{
		if (AsunderInput.GetKeyDown(KeyCode.A))
		{
			_activeStrangle = true;
		}
		if (AsunderInput.GetKeyDown(KeyCode.S))
		{
			_activeStrangle = false;
		}
		if (!_activeStrangle)
		{
			if (_ArmRight != null)
			{
				_ArmRight.transform.localRotation = _initRotation;
			}
			if (_ForearmRight != null)
			{
				_ForearmRight.transform.localRotation = _initForeArmRotation;
			}
		}
		else
		{
			if (_ArmRight != null)
			{
				_ArmRight.transform.localRotation = Quaternion.Lerp(_ArmRight.transform.localRotation, Quaternion.Euler(55f, 77f, 190f), _Speed);
			}
			if (_ForearmRight != null)
			{
				_ForearmRight.transform.localRotation = Quaternion.Lerp(_ForearmRight.transform.localRotation, Quaternion.Euler(330f, 70f, 55f), _Speed);
			}
		}
	}

	public void Strangle()
	{
		_activeStrangle = true;
	}
}
