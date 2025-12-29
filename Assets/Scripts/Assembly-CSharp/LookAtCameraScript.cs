using UnityEngine;

public class LookAtCameraScript : MonoBehaviour
{
	protected Transform cameraTransform;

	protected float lastUpdateTime;

	public bool Flip_X;

	private void Start()
	{
		if (Camera.main != null)
		{
			cameraTransform = Camera.main.transform;
		}
	}

	private void Update()
	{
		if (!(Time.time - lastUpdateTime < 0.02f))
		{
			lastUpdateTime = Time.time;
			base.transform.LookAt(cameraTransform);
			if (Flip_X)
			{
				base.transform.forward = cameraTransform.forward;
			}
		}
	}
}
