using UnityEngine;

public class LuggageSpawner : MonoBehaviour
{
	public GameObject[] _LuggageDebris;

	private float _timer;

	public float _Drag = 0.75f;

	public Vector3 _ForceDirection = new Vector3(0f, 0f, 0f);

	public float _SpawnIntervalMin = 1f;

	public float _SpawnIntervalMax = 5f;

	public int _BurstSizeMin = 1;

	public int _BurstSizeMax = 5;

	private void Start()
	{
		_timer = Time.time;
	}

	private void Update()
	{
		if (Time.time > _timer)
		{
			int num = Random.Range(_BurstSizeMin, _BurstSizeMax);
			for (int i = 0; i < num; i++)
			{
				int num2 = Random.Range(0, _LuggageDebris.Length);
				GameObject gameObject = (GameObject)Object.Instantiate(_LuggageDebris[num2]);
				gameObject.transform.position = base.transform.position;
				gameObject.transform.localRotation = Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
				gameObject.GetComponent<Rigidbody>().linearDamping = Random.Range(0f, _Drag);
				gameObject.GetComponent<Collider>().enabled = false;
				gameObject.GetComponent<Rigidbody>().AddForce(_ForceDirection);
				gameObject.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
			}
			_timer = Time.time + Random.Range(_SpawnIntervalMin, _SpawnIntervalMax);
		}
	}
}
