using UnityEngine;

public class AlphaAnimationScript : MonoBehaviour
{
	public float maxAlpha = 1f;

	public float minAlpha;

	public float animationSpeed = 5.5f;

	public Color maxBright = Color.white;

	public Color minBright = Color.black;

	public bool enableAlphaAnimation;

	public bool enableBrightAnimation;

	public string colorPropertyName = "_TintColor";

	protected float alpha;

	protected float startTime;

	protected bool increasing = true;

	public Color startColor = Color.yellow;

	protected float lastUpdateTime;

	protected float deltaTime;

	private void Start()
	{
		startTime = Time.time;
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime < 0.02f)
		{
			return;
		}
		Color white = Color.white;
		if (enableAlphaAnimation || enableBrightAnimation)
		{
			white = base.GetComponent<Renderer>().material.GetColor(colorPropertyName);
			if (enableAlphaAnimation)
			{
				if (increasing)
				{
					white.a += animationSpeed * deltaTime;
					white.a = Mathf.Clamp(white.a, minAlpha, maxAlpha);
					if (white.a == maxAlpha)
					{
						increasing = false;
					}
				}
				else
				{
					white.a -= animationSpeed * deltaTime;
					white.a = Mathf.Clamp(white.a, minAlpha, maxAlpha);
					if (white.a == minAlpha)
					{
						increasing = true;
					}
				}
			}
			if (enableBrightAnimation)
			{
				if (increasing)
				{
					white.r += animationSpeed * deltaTime * (maxBright.r - minBright.r);
					white.g += animationSpeed * deltaTime * (maxBright.g - minBright.g);
					white.b += animationSpeed * deltaTime * (maxBright.b - minBright.b);
					if (white.r > maxBright.r || white.g > maxBright.g || white.b > maxBright.b)
					{
						increasing = false;
					}
				}
				else
				{
					white.r -= animationSpeed * deltaTime * (maxBright.r - minBright.r);
					white.g -= animationSpeed * deltaTime * (maxBright.g - minBright.g);
					white.b -= animationSpeed * deltaTime * (maxBright.b - minBright.b);
					if (white.r < minBright.r || white.g < minBright.g || white.b < minBright.b)
					{
						increasing = true;
					}
				}
			}
			base.GetComponent<Renderer>().material.SetColor(colorPropertyName, white);
		}
		deltaTime = 0f;
	}
}
