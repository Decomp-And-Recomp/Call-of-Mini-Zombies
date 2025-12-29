using UnityEngine;

public class FadeAnimationScript : MonoBehaviour
{
	private Color startColor = Color.black;

	private Color endColor = new Color(0f, 0f, 0f, 0f);

	public float animationSpeed = 0.5f;

	public bool enableAlphaAnimation;

	public string colorPropertyName = "_TintColor";

	protected float deltaTime;

	public OnFadeEnd onEnd;

	private void Start()
	{
		base.GetComponent<Renderer>().material.SetColor(colorPropertyName, startColor);
	}

	public void StartFade(Color startColor, Color endColor)
	{
		this.startColor = startColor;
		this.endColor = endColor;
		base.GetComponent<Renderer>().material.SetColor(colorPropertyName, startColor);
		enableAlphaAnimation = true;
	}

	public void StartFade(Color startColor, Color endColor, float time)
	{
		StartFade(startColor, endColor);
		if (time != 0f)
		{
			animationSpeed = 1f / time;
		}
	}

	public bool FadeOutComplete()
	{
		return base.GetComponent<Renderer>().material.GetColor(colorPropertyName).a == 0f;
	}

	public bool FadeOutComplete(float target)
	{
		return base.GetComponent<Renderer>().material.GetColor(colorPropertyName).a <= target;
	}

	public bool FadeInComplete()
	{
		return base.GetComponent<Renderer>().material.GetColor(colorPropertyName).a == 1f;
	}

	public static FadeAnimationScript GetInstance()
	{
		return GameObject.Find("CameraFade").GetComponent<FadeAnimationScript>();
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime < 0.02f)
		{
			return;
		}
		if (enableAlphaAnimation)
		{
			float a = startColor.a;
			float a2 = endColor.a;
			float num = Mathf.Sign(a2 - a);
			Color color = base.GetComponent<Renderer>().material.GetColor(colorPropertyName);
			color.a += num * animationSpeed * deltaTime;
			if (Mathf.Sign(a2 - color.a) != num)
			{
				color.a = a2;
				enableAlphaAnimation = false;
				if (onEnd != null)
				{
					onEnd();
				}
			}
			base.GetComponent<Renderer>().material.SetColor(colorPropertyName, color);
		}
		deltaTime = 0f;
	}
}
