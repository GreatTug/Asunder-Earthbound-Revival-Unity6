using UnityEngine;

public class WFX_Demo_Wall : MonoBehaviour
{
	public WFX_Demo demo;

	private void OnMouseDown()
	{
		RaycastHit hitInfo = default(RaycastHit);
		if (base.GetComponent<Collider>().Raycast(Camera.main.ScreenPointToRay(AsunderInput.mousePosition), out hitInfo, 9999f))
		{
			GameObject gameObject = demo.spawnParticle();
			gameObject.transform.position = hitInfo.point;
			gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hitInfo.normal);
		}
	}
}
