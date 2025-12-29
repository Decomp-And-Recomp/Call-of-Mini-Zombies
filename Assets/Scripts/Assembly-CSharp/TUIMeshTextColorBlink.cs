using UnityEngine;

public class TUIMeshTextColorBlink : MonoBehaviour
{
	protected enum State
	{
		In = 0,
		Out = 1
	}

	public Color BlinkColorBegin = new Color(0f, 0f, 0f, 1f);

	public Color BlinkColorEnd = new Color(0f, 0f, 0f, 0f);

	public float BlinkRate = 0.35f;

	public bool is_blink;

	protected float time;

	protected TUIMeshText text;

	protected State state;

	private void Start()
	{
		text = GetComponent<TUIMeshText>();
	}

	private void Update()
	{
		if (!(text != null) || !is_blink)
		{
			return;
		}
		time += Time.deltaTime;
		if (state == State.In)
		{
			float t = Mathf.Clamp(time / BlinkRate, 0f, 1f);
			text.color_Accessor = Color.Lerp(BlinkColorBegin, BlinkColorEnd, t);
		}
		else if (state == State.Out)
		{
			float t2 = Mathf.Clamp(time / BlinkRate, 0f, 1f);
			text.color_Accessor = Color.Lerp(BlinkColorEnd, BlinkColorBegin, t2);
		}
		if (time >= BlinkRate)
		{
			time = 0f;
			if (state == State.In)
			{
				state = State.Out;
			}
			else if (state == State.Out)
			{
				state = State.In;
			}
		}
	}
}
