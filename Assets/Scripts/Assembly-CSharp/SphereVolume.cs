using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SphereVolume : MonoBehaviour
{
	public Material volumetricMaterial;

	public float radius = 3f;

	public float visibility = 6f;

	public Color volumeColor = new Color(1f, 1f, 1f, 1f);

	public Texture2D texture;

	public float textureScale = 1f;

	public Vector3 textureMovement = new Vector3(0f, -0.1f, 0f);

	private Mesh meshInstance;

	private Material materialInstance;

	private Transform thisTransform;

	private float previousRadius = 1f;

	private float previousVisibility = 1f;

	private Color previousVolumeColor = new Color(1f, 1f, 1f, 1f);

	private Vector3 forcedLocalScale = Vector3.one;

	private Texture2D previousTexture;

	private float previousTextureScale = 10f;

	private Vector3 previousTextureMovement = new Vector3(0f, 0.1f, 0f);

	private Vector3[] unitVerts = new Vector3[8];

	private void Awake()
	{
		if (volumetricMaterial == null)
		{
			base.enabled = false;
		}
	}

	private void OnEnable()
	{
		SetupUnitVerts();
		thisTransform = base.transform;
		if (meshInstance != null)
		{
			Object.Destroy(meshInstance);
		}
		meshInstance = CreateCube();
		MeshFilter component = GetComponent<MeshFilter>();
		component.sharedMesh = meshInstance;
		if (materialInstance != null)
		{
			Object.Destroy(materialInstance);
		}
		MeshRenderer component2 = GetComponent<MeshRenderer>();
		component2.sharedMaterial = (Material)Object.Instantiate(volumetricMaterial);
		materialInstance = component2.sharedMaterial;
		if ((bool)Camera.current)
		{
			Camera.current.depthTextureMode |= DepthTextureMode.Depth;
		}
		if ((bool)Camera.main)
		{
			Camera.main.depthTextureMode |= DepthTextureMode.Depth;
		}
		UpdateVolume();
	}

	private void LateUpdate()
	{
		if (radius != previousRadius || visibility != previousVisibility || volumeColor != previousVolumeColor || thisTransform.localScale != forcedLocalScale || texture != previousTexture || textureScale != previousTextureScale || textureMovement != previousTextureMovement)
		{
			previousRadius = radius;
			previousVisibility = visibility;
			previousVolumeColor = volumeColor;
			thisTransform.localScale = forcedLocalScale;
			previousTexture = texture;
			previousTextureScale = textureScale;
			previousTextureMovement = textureMovement;
			UpdateVolume();
		}
	}

	public void UpdateVolume()
	{
		Vector3 vector = Vector3.one * radius;
		if ((bool)meshInstance)
		{
			ScaleMesh(meshInstance, vector);
			Bounds bounds = default(Bounds);
			bounds.SetMinMax(-vector, vector);
			meshInstance.bounds = bounds;
		}
		if ((bool)materialInstance)
		{
			materialInstance.SetVector("_TextureData", new Vector4(0f - textureMovement.x, 0f - textureMovement.y, 0f - textureMovement.z, 1f / textureScale));
			materialInstance.SetFloat("_RadiusSqr", radius * radius);
			materialInstance.SetFloat("_Visibility", visibility);
			materialInstance.SetColor("_Color", volumeColor);
			materialInstance.SetTexture("_MainTex", texture);
		}
	}

	public void SetupUnitVerts()
	{
		float num = 1f;
		unitVerts[0].x = 0f - num;
		unitVerts[0].y = 0f - num;
		unitVerts[0].z = 0f - num;
		unitVerts[1].x = num;
		unitVerts[1].y = 0f - num;
		unitVerts[1].z = 0f - num;
		unitVerts[2].x = num;
		unitVerts[2].y = num;
		unitVerts[2].z = 0f - num;
		unitVerts[3].x = num;
		unitVerts[3].y = 0f - num;
		unitVerts[3].z = num;
		unitVerts[4].x = num;
		unitVerts[4].y = num;
		unitVerts[4].z = num;
		unitVerts[5].x = 0f - num;
		unitVerts[5].y = num;
		unitVerts[5].z = 0f - num;
		unitVerts[6].x = 0f - num;
		unitVerts[6].y = num;
		unitVerts[6].z = num;
		unitVerts[7].x = 0f - num;
		unitVerts[7].y = 0f - num;
		unitVerts[7].z = num;
	}

	public Mesh CreateCube()
	{
		Mesh mesh = new Mesh();
		Vector3[] array = new Vector3[unitVerts.Length];
		unitVerts.CopyTo(array, 0);
		mesh.vertices = array;
		int[] array2 = new int[36];
		int num = 0;
		array2[num] = 0;
		num++;
		array2[num] = 2;
		num++;
		array2[num] = 1;
		num++;
		array2[num] = 0;
		num++;
		array2[num] = 5;
		num++;
		array2[num] = 2;
		num++;
		array2[num] = 3;
		num++;
		array2[num] = 6;
		num++;
		array2[num] = 7;
		num++;
		array2[num] = 3;
		num++;
		array2[num] = 4;
		num++;
		array2[num] = 6;
		num++;
		array2[num] = 1;
		num++;
		array2[num] = 4;
		num++;
		array2[num] = 3;
		num++;
		array2[num] = 1;
		num++;
		array2[num] = 2;
		num++;
		array2[num] = 4;
		num++;
		array2[num] = 7;
		num++;
		array2[num] = 5;
		num++;
		array2[num] = 0;
		num++;
		array2[num] = 7;
		num++;
		array2[num] = 6;
		num++;
		array2[num] = 5;
		num++;
		array2[num] = 7;
		num++;
		array2[num] = 1;
		num++;
		array2[num] = 3;
		num++;
		array2[num] = 7;
		num++;
		array2[num] = 0;
		num++;
		array2[num] = 1;
		num++;
		array2[num] = 5;
		num++;
		array2[num] = 4;
		num++;
		array2[num] = 2;
		num++;
		array2[num] = 5;
		num++;
		array2[num] = 6;
		num++;
		array2[num] = 4;
		num++;
		mesh.triangles = array2;
		mesh.RecalculateBounds();
		return mesh;
	}

	public void ScaleMesh(Mesh mesh, Vector3 scaleFactor)
	{
		Vector3[] array = new Vector3[mesh.vertexCount];
		for (int i = 0; i < mesh.vertexCount; i++)
		{
			array[i] = ScaleVector(unitVerts[i], scaleFactor);
		}
		mesh.vertices = array;
	}

	private Vector3 ScaleVector(Vector3 vector, Vector3 scale)
	{
		return new Vector3(vector.x * scale.x, vector.y * scale.y, vector.z * scale.z);
	}

	public Mesh CopyMesh(Mesh original)
	{
		Mesh mesh = new Mesh();
		Vector3[] array = new Vector3[original.vertices.Length];
		original.vertices.CopyTo(array, 0);
		mesh.vertices = array;
		Vector2[] array2 = new Vector2[original.uv.Length];
		original.uv.CopyTo(array2, 0);
		mesh.uv = array2;
		Vector2[] array4 = new Vector2[original.uv2.Length];
		original.uv2.CopyTo(array4, 0);
		mesh.uv2 = array4;
		Vector3[] array5 = new Vector3[original.normals.Length];
		original.normals.CopyTo(array5, 0);
		mesh.normals = array5;
		Vector4[] array6 = new Vector4[original.tangents.Length];
		original.tangents.CopyTo(array6, 0);
		mesh.tangents = array6;
		Color[] array7 = new Color[original.colors.Length];
		original.colors.CopyTo(array7, 0);
		mesh.colors = array7;
		mesh.subMeshCount = original.subMeshCount;
		for (int i = 0; i < original.subMeshCount; i++)
		{
			int[] triangles = original.GetTriangles(i);
			int[] array8 = new int[triangles.Length];
			triangles.CopyTo(array8, 0);
			mesh.SetTriangles(triangles, i);
		}
		mesh.RecalculateBounds();
		return mesh;
	}
}
