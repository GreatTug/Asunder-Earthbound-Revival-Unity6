using System;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraFade : MonoBehaviour
{
	private static CameraFade mInstance;

	private static Material s_StereoFadeMat;

	public GUIStyle m_BackgroundStyle = new GUIStyle();

	public Texture2D m_FadeTexture;

	public Color m_CurrentScreenOverlayColor = new Color(0f, 0f, 0f, 0f);

	public Color m_TargetScreenOverlayColor = new Color(0f, 0f, 0f, 0f);

	public Color m_DeltaColor = new Color(0f, 0f, 0f, 0f);

	public int m_FadeGUIDepth = -1000;

	public float m_FadeDelay;

	public Action m_OnFadeFinish;

	private bool _IsToBeDestroyed;

	private static CameraFade instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = UnityEngine.Object.FindAnyObjectByType<CameraFade>();
				if (mInstance == null)
				{
					mInstance = new GameObject("CameraFade").AddComponent<CameraFade>();
				}
			}
			return mInstance;
		}
	}

	private static Material GetStereoFadeMaterial()
	{
		if (s_StereoFadeMat == null)
		{
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			if (shader != null)
			{
				s_StereoFadeMat = new Material(shader);
				s_StereoFadeMat.hideFlags = HideFlags.HideAndDontSave;
				s_StereoFadeMat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
				s_StereoFadeMat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
				s_StereoFadeMat.SetInt("_Cull", (int)CullMode.Off);
				s_StereoFadeMat.SetInt("_ZWrite", 0);
				s_StereoFadeMat.SetInt("_ZTest", (int)CompareFunction.Always);
			}
		}
		return s_StereoFadeMat;
	}

	public static void DrawStereoOverlayQuad(Color col)
	{
		if (col.a <= 0.001f)
		{
			return;
		}
		Material mat = GetStereoFadeMaterial();
		if (mat == null)
		{
			return;
		}
		GL.PushMatrix();
		mat.SetPass(0);
		GL.LoadOrtho();
		GL.Begin(GL.QUADS);
		GL.Color(col);
		GL.Vertex3(0f, 0f, 0f);
		GL.Vertex3(0f, 1f, 0f);
		GL.Vertex3(1f, 1f, 0f);
		GL.Vertex3(1f, 0f, 0f);
		GL.End();
		GL.PopMatrix();
	}

	private void Awake()
	{
		if (mInstance == null)
		{
			mInstance = this;
			init();
		}
	}

	private void OnEnable()
	{
		Camera.onPostRender += OnCameraPostRender;
	}

	private void OnDisable()
	{
		Camera.onPostRender -= OnCameraPostRender;
	}

	public void init()
	{
		if (m_FadeTexture == null)
		{
			m_FadeTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		}
		m_BackgroundStyle.normal.background = m_FadeTexture;
	}

	private void Update()
	{
		if (mInstance != this)
		{
			return;
		}
		if (Time.time > m_FadeDelay && m_CurrentScreenOverlayColor != m_TargetScreenOverlayColor)
		{
			if (Mathf.Abs(m_CurrentScreenOverlayColor.a - m_TargetScreenOverlayColor.a) < Mathf.Abs(m_DeltaColor.a) * Time.deltaTime)
			{
				m_CurrentScreenOverlayColor = m_TargetScreenOverlayColor;
				SetScreenOverlayColor(m_CurrentScreenOverlayColor);
				m_DeltaColor = new Color(0f, 0f, 0f, 0f);
				Action cb = m_OnFadeFinish;
				m_OnFadeFinish = null;
				if (cb != null)
				{
					cb();
				}
				Die();
			}
			else
			{
				SetScreenOverlayColor(m_CurrentScreenOverlayColor + m_DeltaColor * Time.deltaTime);
			}
		}
	}

	private void OnCameraPostRender(Camera cam)
	{
		if (mInstance == this && m_CurrentScreenOverlayColor.a > 0.001f && cam != null)
		{
			DrawStereoOverlayQuad(m_CurrentScreenOverlayColor);
		}
	}

	private void OnGUI()
	{
		if (mInstance == this && m_CurrentScreenOverlayColor.a > 0f && m_FadeTexture != null)
		{
			GUI.depth = m_FadeGUIDepth;
			GUI.Label(new Rect(-10f, -10f, Screen.width + 20, Screen.height + 20), m_FadeTexture, m_BackgroundStyle);
		}
	}

	public static void SetScreenOverlayColor(Color newScreenOverlayColor)
	{
		instance.init();
		instance.m_CurrentScreenOverlayColor = newScreenOverlayColor;
		instance.m_FadeTexture.SetPixel(0, 0, instance.m_CurrentScreenOverlayColor);
		instance.m_FadeTexture.Apply();
	}

	public static void StartAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration)
	{
		instance._IsToBeDestroyed = false;
		instance.m_FadeDelay = 0f;
		instance.m_OnFadeFinish = null;
		if (fadeDuration <= 0f)
		{
			SetScreenOverlayColor(newScreenOverlayColor);
			return;
		}
		if (isFadeIn)
		{
			instance.m_TargetScreenOverlayColor = new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0f);
			SetScreenOverlayColor(newScreenOverlayColor);
		}
		else
		{
			instance.m_TargetScreenOverlayColor = newScreenOverlayColor;
			SetScreenOverlayColor(new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0f));
		}
		instance.m_DeltaColor = (instance.m_TargetScreenOverlayColor - instance.m_CurrentScreenOverlayColor) / fadeDuration;
	}

	public static void StartTeleAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration)
	{
		instance._IsToBeDestroyed = true;
		instance.m_FadeDelay = 0f;
		instance.m_OnFadeFinish = null;
		if (fadeDuration <= 0f)
		{
			SetScreenOverlayColor(newScreenOverlayColor);
			return;
		}
		if (isFadeIn)
		{
			instance.m_TargetScreenOverlayColor = new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0f);
			SetScreenOverlayColor(newScreenOverlayColor);
		}
		else
		{
			instance.m_TargetScreenOverlayColor = newScreenOverlayColor;
			SetScreenOverlayColor(new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0f));
		}
		instance.m_DeltaColor = (instance.m_TargetScreenOverlayColor - instance.m_CurrentScreenOverlayColor) / fadeDuration;
	}

	public static void StartAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration, float fadeDelay)
	{
		instance._IsToBeDestroyed = false;
		instance.m_OnFadeFinish = null;
		if (fadeDuration <= 0f)
		{
			SetScreenOverlayColor(newScreenOverlayColor);
			return;
		}
		instance.m_FadeDelay = Time.time + fadeDelay;
		if (isFadeIn)
		{
			instance.m_TargetScreenOverlayColor = new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0f);
			SetScreenOverlayColor(newScreenOverlayColor);
		}
		else
		{
			instance.m_TargetScreenOverlayColor = newScreenOverlayColor;
			SetScreenOverlayColor(new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0f));
		}
		instance.m_DeltaColor = (instance.m_TargetScreenOverlayColor - instance.m_CurrentScreenOverlayColor) / fadeDuration;
	}

	public static void StartAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration, float fadeDelay, Action OnFadeFinish)
	{
		instance._IsToBeDestroyed = false;
		if (fadeDuration <= 0f)
		{
			SetScreenOverlayColor(newScreenOverlayColor);
			OnFadeFinish?.Invoke();
			return;
		}
		instance.m_OnFadeFinish = OnFadeFinish;
		instance.m_FadeDelay = Time.time + fadeDelay;
		if (isFadeIn)
		{
			instance.m_TargetScreenOverlayColor = new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0f);
			SetScreenOverlayColor(newScreenOverlayColor);
		}
		else
		{
			instance.m_TargetScreenOverlayColor = newScreenOverlayColor;
			SetScreenOverlayColor(new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0f));
		}
		instance.m_DeltaColor = (instance.m_TargetScreenOverlayColor - instance.m_CurrentScreenOverlayColor) / fadeDuration;
	}

	private void Die()
	{
		if (_IsToBeDestroyed)
		{
			mInstance = null;
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnApplicationQuit()
	{
		mInstance = null;
	}
}
