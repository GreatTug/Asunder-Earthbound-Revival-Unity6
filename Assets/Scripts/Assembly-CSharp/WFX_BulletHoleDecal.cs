using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WFX_BulletHoleDecal : MonoBehaviour
{
	private static Vector2[] quadUVs = new Vector2[4]
	{
		new Vector2(0f, 0f),
		new Vector2(0f, 1f),
		new Vector2(1f, 0f),
		new Vector2(1f, 1f)
	};

	public float lifetime = 10f;

	public float fadeoutpercent = 80f;

	public Vector2 frames;

	public bool randomRotation;

	public bool deactivate;

	private float life;

	private float fadeout;

	private Color color = Color.white;

	private float orgAlpha = 1f;

	private Renderer _rend;

	private Material _mat;

	private MeshFilter _mf;

	private int _tintColorId;

	private void Awake()
	{
		_rend = GetComponent<Renderer>();
		_mf = GetComponent<MeshFilter>();
		_tintColorId = Shader.PropertyToID("_TintColor");
		if (_rend != null)
		{
			_mat = _rend.material;
			if (_mat != null && _mat.HasProperty(_tintColorId))
			{
				color = _mat.GetColor(_tintColorId);
				orgAlpha = color.a;
			}
		}
	}

	private void OnEnable()
	{
		if (frames.x > 0f && frames.y > 0f)
		{
			int num = Random.Range(0, (int)(frames.x * frames.y));
			int num2 = (int)((float)num % frames.x);
			int num3 = (int)((float)num / frames.y);
			Vector2[] array = new Vector2[4];
			for (int i = 0; i < 4; i++)
			{
				array[i].x = (quadUVs[i].x + (float)num2) * (1f / frames.x);
				array[i].y = (quadUVs[i].y + (float)num3) * (1f / frames.y);
			}
			if (_mf == null)
			{
				_mf = GetComponent<MeshFilter>();
			}
			if (_mf != null && _mf.mesh != null)
			{
				_mf.mesh.uv = array;
			}
		}
		if (randomRotation)
		{
			base.transform.Rotate(0f, 0f, Random.Range(0f, 360f), Space.Self);
		}
		life = lifetime;
		fadeout = life * (fadeoutpercent / 100f);
		color.a = orgAlpha;
		if (_mat == null && _rend != null)
		{
			_mat = _rend.material;
		}
		if (_mat != null && _mat.HasProperty(_tintColorId))
		{
			_mat.SetColor(_tintColorId, color);
		}
		StopAllCoroutines();
		StartCoroutine("holeUpdate");
	}

	private IEnumerator holeUpdate()
	{
		while (life > 0f)
		{
			life -= Time.deltaTime;
			if (life <= fadeout)
			{
				color.a = Mathf.Lerp(0f, orgAlpha, life / Mathf.Max(0.001f, fadeout));
				if (_mat != null && _mat.HasProperty(_tintColorId))
				{
					_mat.SetColor(_tintColorId, color);
				}
			}
			yield return null;
		}
	}
}
