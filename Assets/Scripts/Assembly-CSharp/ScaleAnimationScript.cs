using UnityEngine;

public class ScaleAnimationScript : MonoBehaviour
{
	public float destScaleMax = 0.4f;

	public float destScaleMin = 0.2f;

	protected float destScale;

	public float scaleSpeed = 1f;

	private void Start()
	{
		destScale = Random.Range(destScaleMin, destScaleMax);
	}

	private void Update()
	{
		if (base.transform.localScale.x < destScale)
		{
			base.transform.localScale += Vector3.one * Time.deltaTime * scaleSpeed;
		}
	}
}
