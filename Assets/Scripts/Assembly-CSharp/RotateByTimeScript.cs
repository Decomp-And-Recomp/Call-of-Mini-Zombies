using UnityEngine;

public class RotateByTimeScript : MonoBehaviour
{
	public Vector3 rotateSpeed = new Vector3(0f, 45f, 0f);

	protected float deltaTime;

	private void Start()
	{
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (!(deltaTime < 0.03f))
		{
			base.transform.Rotate(rotateSpeed * deltaTime, Space.Self);
			deltaTime = 0f;
		}
	}
}
