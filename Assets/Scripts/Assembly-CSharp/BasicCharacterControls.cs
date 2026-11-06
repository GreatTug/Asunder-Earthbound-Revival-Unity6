using UnityEngine;

public class BasicCharacterControls : MonoBehaviour
{
	public USSequencer cutscene;

	public float strength = 10f;

	private void Update()
	{
		if (!cutscene || !cutscene.IsPlaying)
		{
			float num = strength * Time.deltaTime;
			if (AsunderInput.GetKey(KeyCode.W))
			{
				base.GetComponent<Rigidbody>().AddRelativeForce(0f - num, 0f, 0f);
			}
			if (AsunderInput.GetKey(KeyCode.S))
			{
				base.GetComponent<Rigidbody>().AddRelativeForce(num, 0f, 0f);
			}
			if (AsunderInput.GetKey(KeyCode.A))
			{
				base.GetComponent<Rigidbody>().AddRelativeForce(0f, 0f, 0f - num);
			}
			if (AsunderInput.GetKey(KeyCode.D))
			{
				base.GetComponent<Rigidbody>().AddRelativeForce(0f, 0f, num);
			}
		}
	}
}
