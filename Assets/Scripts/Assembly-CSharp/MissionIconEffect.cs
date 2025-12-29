using UnityEngine;

public class MissionIconEffect : MonoBehaviour
{
	public bool updown = true;

	public bool brightness;

	public bool frameBrightness;

	public float speed = 200f;

	public float minValue;

	public float maxValue = 10f;

	private float deltaTime;

	private bool positiveDirection = true;

	private float startValue;

	public OnFrameBrightnessMax onFrameBrightnessMax;

	public OnFrameBrightnessMin onFrameBrightnessMin;

	private void Start()
	{
		if (updown)
		{
			startValue = base.transform.localPosition.y;
		}
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if ((double)deltaTime < 0.02)
		{
			return;
		}
		if (updown)
		{
			float y = base.transform.localPosition.y;
			if (positiveDirection)
			{
				y += deltaTime * speed;
				y = Mathf.Clamp(y, startValue + minValue, startValue + maxValue);
				if (y == startValue + maxValue)
				{
					positiveDirection = false;
				}
			}
			else
			{
				y -= deltaTime * speed;
				y = Mathf.Clamp(y, startValue + minValue, startValue + maxValue);
				if (y == startValue + minValue)
				{
					positiveDirection = true;
				}
			}
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, y, base.transform.localPosition.z);
		}
		if (brightness)
		{
			float a = base.GetComponent<Renderer>().material.GetColor("_TintColor").a;
			if (positiveDirection)
			{
				a += deltaTime * speed;
				a = Mathf.Clamp(a, minValue, maxValue);
				if (a == maxValue)
				{
					positiveDirection = false;
				}
			}
			else
			{
				a -= deltaTime * speed;
				a = Mathf.Clamp(a, minValue, maxValue);
				if (a == minValue)
				{
					positiveDirection = true;
				}
			}
			base.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1f, 1f, 1f, a));
		}
		if (frameBrightness)
		{
			float a2 = base.gameObject.GetComponent<TUIMeshSprite>().color_Accessor.a;
			if (positiveDirection)
			{
				a2 += deltaTime * speed;
				a2 = Mathf.Clamp(a2, minValue, maxValue);
				if (a2 == maxValue)
				{
					positiveDirection = false;
					if (onFrameBrightnessMax != null)
					{
						onFrameBrightnessMax();
					}
				}
			}
			else
			{
				a2 -= deltaTime * speed;
				a2 = Mathf.Clamp(a2, minValue, maxValue);
				if (a2 == minValue)
				{
					positiveDirection = true;
					if (onFrameBrightnessMin != null)
					{
						onFrameBrightnessMin();
					}
				}
			}
			base.gameObject.GetComponent<TUIMeshSprite>().color_Accessor = new Color(1f, 1f, 1f, a2);
		}
		deltaTime = 0f;
	}
}
