using UnityEngine;

public class ScreenBloodScript : MonoBehaviour
{
	public float scrollSpeed = 1f;

	protected float alpha;

	protected float startTime;

	protected float deltaTime;

	public string alphaPropertyName = "_Alpha";

	private void Start()
	{
		alpha = base.GetComponent<Renderer>().material.GetFloat(alphaPropertyName);
		startTime = Time.time;
		if (Mathf.Max(Screen.width, Screen.height) >= 960)
		{
			float num = (float)Screen.width / 960f;
			float num2 = (float)Screen.height / 640f;
			base.transform.localScale = new Vector3(4.2f * num, 2.8f * num2, 0.5f);
		}
	}

	public void NewBlood(float damage)
	{
		base.GetComponent<Renderer>().enabled = true;
		alpha = damage;
		alpha = Mathf.Clamp(alpha, 0f, 1f);
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (!(deltaTime < 0.03f))
		{
			alpha -= 0.5f * deltaTime;
			if (alpha <= 0f)
			{
				base.GetComponent<Renderer>().enabled = false;
			}
			base.GetComponent<Renderer>().material.SetFloat(alphaPropertyName, alpha);
			deltaTime = 0f;
		}
	}
}
