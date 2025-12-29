using UnityEngine;

public class TUIFade : TUIControlImpl
{
	protected enum State
	{
		Idle = 0,
		FadeIn = 1,
		FadeOut = 2
	}

	public TUIMeshSprite mask;

	public Color fadeInColorBegin = new Color(0f, 0f, 0f, 1f);

	public Color fadeInColorEnd = new Color(0f, 0f, 0f, 0f);

	public float fadeInTime = 0.35f;

	public Color fadeOutColorBegin = new Color(0f, 0f, 0f, 0f);

	public Color fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);

	public float fadeOutTime = 0.35f;

	protected State state;

	protected float time;

	protected string fadeOutScene = string.Empty;

	public void FadeIn()
	{
		state = State.FadeIn;
		time = 0f;
		if (mask != null)
		{
			mask.gameObject.SetActive(true);
			mask.color_Accessor = fadeInColorBegin;
		}
	}

	public void FadeOut()
	{
		FadeOut(string.Empty);
	}

	public void FadeOut(string scene)
	{
		state = State.FadeOut;
		time = 0f;
		fadeOutScene = scene;
		if (mask != null)
		{
			mask.gameObject.SetActive(true);
			mask.color_Accessor = fadeOutColorBegin;
			mask.Static = false;
		}
	}

	public new void Start()
	{
		base.Start();
		if (mask != null)
		{
			mask.gameObject.SetActive(false);
		}
	}

	public void Update()
	{
		if (state != 0)
		{
			if (state == State.FadeIn)
			{
				UpdateFadeIn(Mathf.Clamp(Time.deltaTime, 0f, 0.05f));
			}
			else if (state == State.FadeOut)
			{
				UpdateFadeOut(Mathf.Clamp(Time.deltaTime, 0f, 0.05f));
			}
		}
	}

	public override bool HandleInput(TUIInput input)
	{
		if (state == State.Idle)
		{
			return false;
		}
		if (!PtInControl(input.position))
		{
			return false;
		}
		return true;
	}

	protected virtual void UpdateFadeIn(float deltaTime)
	{
		time += deltaTime;
		if (mask != null)
		{
			float t = Mathf.Clamp(time / fadeInTime, 0f, 1f);
			mask.color_Accessor = Color.Lerp(fadeInColorBegin, fadeInColorEnd, t);
		}
		if (time > fadeInTime)
		{
			state = State.Idle;
			if (mask != null)
			{
				mask.gameObject.SetActive(false);
				mask.Static = true;
			}
		}
	}

	protected virtual void UpdateFadeOut(float deltaTime)
	{
		time += deltaTime;
		if (mask != null)
		{
			float t = Mathf.Clamp(time / fadeOutTime, 0f, 1f);
			mask.color_Accessor = Color.Lerp(fadeOutColorBegin, fadeOutColorEnd, t);
		}
		if (time > fadeOutTime)
		{
			state = State.Idle;
			if (mask != null)
			{
				mask.gameObject.SetActive(true);
				mask.color_Accessor = fadeOutColorEnd;
			}
			if (fadeOutScene.Length > 0)
			{
				Application.LoadLevel(fadeOutScene);
			}
		}
	}
}
