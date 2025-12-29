using UnityEngine;

public class PopoAnimationScript : MonoBehaviour
{
	public bool u = true;

	public bool v = true;

	public float scrollSpeed = 1f;

	public float scaleSpeed = 0.1f;

	public string alphaPropertyName = "_Alpha";

	public string texturePropertyName = "_MainTex";

	protected float rndUV;

	protected int rndScale;

	protected float alpha;

	protected float startTime;

	protected float currentScale;

	private void Start()
	{
		alpha = base.GetComponent<Renderer>().material.GetFloat(alphaPropertyName);
		startTime = Time.time;
		rndUV = Random.Range(0f, 1f);
		rndScale = Random.Range(2, 5);
	}

	private void Update()
	{
		float num = (Time.time * scrollSpeed + rndUV) % 1f;
		if (u && v)
		{
			base.GetComponent<Renderer>().material.SetTextureOffset(texturePropertyName, new Vector2(num, num));
		}
		else if (u)
		{
			base.GetComponent<Renderer>().material.SetTextureOffset(texturePropertyName, new Vector2(num, 0f));
		}
		else if (v)
		{
			base.GetComponent<Renderer>().material.SetTextureOffset(texturePropertyName, new Vector2(0f, num));
		}
		currentScale += Time.deltaTime * scaleSpeed;
		currentScale = Mathf.Clamp(currentScale, 0.01f, 0.1f + 0.1f * (float)rndScale);
		base.transform.localScale = currentScale * new Vector3(1f, 1f, 1f);
	}
}
