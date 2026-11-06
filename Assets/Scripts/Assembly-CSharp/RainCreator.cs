using UnityEngine;

public class RainCreator : MonoBehaviour
{
	public int rainLevel;

	public Vector3 localPosition;

	public GameObject softRain;

	public GameObject rain;

	public GameObject heavyRain;

	public void OnEnable()
	{
		switch (rainLevel)
		{
		default:
			softRain = Object.Instantiate(softRain) as GameObject;
			softRain.transform.DetachChildren();
			softRain.transform.parent = base.transform;
			softRain.transform.localPosition = localPosition;
			break;
		case 1:
			rain = Object.Instantiate(rain) as GameObject;
			rain.transform.DetachChildren();
			rain.transform.parent = base.transform;
			rain.transform.localPosition = localPosition;
			break;
		case 2:
			heavyRain = Object.Instantiate(heavyRain) as GameObject;
			heavyRain.transform.DetachChildren();
			heavyRain.transform.parent = base.transform;
			heavyRain.transform.localPosition = localPosition;
			break;
		}
	}
}
