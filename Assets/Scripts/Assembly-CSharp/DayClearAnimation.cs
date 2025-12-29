using UnityEngine;

public class DayClearAnimation : MonoBehaviour
{
	private float delta_time;

	private Vector3 targetScale;

	private void Start()
	{
		targetScale = base.gameObject.transform.localScale * 1.2f;
		base.gameObject.transform.localScale = base.gameObject.transform.localScale * 0.5f;
		delta_time = 0f;
	}

	private void Update()
	{
		delta_time += Time.deltaTime;
		if ((double)delta_time >= 0.02)
		{
			base.gameObject.transform.localScale += base.gameObject.transform.localScale * delta_time;
			if (base.gameObject.transform.localScale.magnitude >= targetScale.magnitude)
			{
				base.enabled = false;
			}
			delta_time = 0f;
		}
	}
}
