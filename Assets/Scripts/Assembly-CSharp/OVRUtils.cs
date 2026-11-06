using UnityEngine;

public class OVRUtils
{
	public static void SetLocalTransformIdentity(ref GameObject gameObject)
	{
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
	}

	public static void SetLocalTransform(ref GameObject gameObject, ref Transform xfrm)
	{
		gameObject.transform.localPosition = xfrm.position;
		gameObject.transform.localRotation = xfrm.rotation;
		gameObject.transform.localScale = xfrm.localScale;
	}

	public void Blit(RenderTexture source, RenderTexture dest, Material m, bool flip)
	{
		RenderTexture.active = dest;
		source.SetGlobalShaderProperty("_MainTex");
		GL.PushMatrix();
		GL.LoadOrtho();
		for (int i = 0; i < m.passCount; i++)
		{
			m.SetPass(i);
			DrawQuad(flip);
		}
		GL.PopMatrix();
	}

	public void DrawQuad(bool flip)
	{
		GL.Begin(7);
		if (flip)
		{
			GL.TexCoord2(0f, 1f);
			GL.Vertex3(0f, 0f, 0.1f);
			GL.TexCoord2(1f, 1f);
			GL.Vertex3(1f, 0f, 0.1f);
			GL.TexCoord2(1f, 0f);
			GL.Vertex3(1f, 1f, 0.1f);
			GL.TexCoord2(0f, 0f);
			GL.Vertex3(0f, 1f, 0.1f);
		}
		else
		{
			GL.TexCoord2(0f, 0f);
			GL.Vertex3(0f, 0f, 0.1f);
			GL.TexCoord2(1f, 0f);
			GL.Vertex3(1f, 0f, 0.1f);
			GL.TexCoord2(1f, 1f);
			GL.Vertex3(1f, 1f, 0.1f);
			GL.TexCoord2(0f, 1f);
			GL.Vertex3(0f, 1f, 0.1f);
		}
		GL.End();
	}
}
