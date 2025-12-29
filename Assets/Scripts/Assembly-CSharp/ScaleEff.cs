using System.Collections;
using UnityEngine;

public class ScaleEff : MonoBehaviour
{
	public float time_step1;

	public float time_step2;

	public float time_step3;

	public Vector3 scale_step0 = new Vector3(0f, 0f, 0f);

	public Vector3 scale_step1 = Vector3.zero;

	public Vector3 scale_step2 = Vector3.zero;

	public Vector3 scale_step3 = Vector3.zero;

	private int step;

	private float start_time;

	private float time;

	public bool destroyAfterFinished;

	private IEnumerator Start()
	{
		start_time = Time.time;
		yield return new WaitForSeconds(time_step1);
		step++;
		start_time = Time.time;
		yield return new WaitForSeconds(time_step2);
		step++;
		start_time = Time.time;
		yield return new WaitForSeconds(time_step3);
		step++;
		if (destroyAfterFinished)
		{
			Object.Destroy(this);
		}
	}

	private void Update()
	{
		time = Time.time - start_time;
		if (step == 0)
		{
			float t = Mathf.Clamp(time / time_step1, 0f, 1f);
			base.gameObject.transform.localScale = Vector3.Lerp(scale_step0, scale_step1, t);
		}
		else if (step == 1)
		{
			float t2 = Mathf.Clamp(time / time_step1, 0f, 1f);
			base.gameObject.transform.localScale = Vector3.Lerp(scale_step1, scale_step2, t2);
		}
		else if (step == 2)
		{
			float t3 = Mathf.Clamp(time / time_step1, 0f, 1f);
			base.gameObject.transform.localScale = Vector3.Lerp(scale_step2, scale_step3, t3);
		}
	}
}
