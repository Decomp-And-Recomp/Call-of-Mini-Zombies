using UnityEngine;

public class DelayCameraDisplayScript : MonoBehaviour
{
	public float delayTime = 0.5f;

	protected float startTime;

	private void Start()
	{
		base.GetComponent<Camera>().enabled = false;
		startTime = Time.time;
	}

	private void Update()
	{
		if (Time.time - startTime > delayTime)
		{
			base.GetComponent<Camera>().enabled = true;
		}
	}
}
