using UnityEngine;

public class TUIFadeEx : TUIFade
{
	public OnFadeIn m_fadein;

	public OnFadeOut m_fadeout;

	public new void Start()
	{
		base.Start();
		if (mask != null)
		{
			mask.gameObject.SetActive(true);
		}
	}

	protected override void UpdateFadeIn(float deltaTime)
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
			if (m_fadein != null)
			{
				m_fadein();
			}
		}
	}

	protected override void UpdateFadeOut(float deltaTime)
	{
		time += deltaTime;
		if (mask != null)
		{
			float t = Mathf.Clamp(time / fadeOutTime, 0f, 1f);
			mask.color_Accessor = Color.Lerp(fadeOutColorBegin, fadeOutColorEnd, t);
		}
		if (!(time > fadeOutTime))
		{
			return;
		}
		state = State.Idle;
		if (mask != null)
		{
			mask.gameObject.SetActive(true);
			mask.color_Accessor = fadeOutColorEnd;
		}
		if (fadeOutScene.Length > 0)
		{
			if (m_fadeout != null)
			{
				m_fadeout();
			}
			Application.LoadLevel(fadeOutScene);
		}
	}
}
