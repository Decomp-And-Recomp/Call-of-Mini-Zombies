using UnityEngine;

public class MapEffect : MonoBehaviour
{
	private float deltaTime;

	private void Start()
	{
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (!((double)deltaTime < 0.02))
		{
			base.transform.localPosition -= deltaTime * new Vector3(0f, 200f, 0f);
			if (base.transform.localPosition.y <= -320f)
			{
				base.transform.localPosition += new Vector3(0f, 640f, 0f);
			}
			deltaTime = 0f;
		}
	}
}
