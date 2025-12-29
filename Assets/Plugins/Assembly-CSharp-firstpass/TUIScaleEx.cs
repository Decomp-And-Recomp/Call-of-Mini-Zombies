using UnityEngine;

public class TUIScaleEx : TUIScale
{
	public OnScaleIn m_scalein;

	public OnScaleOut m_scaleout;

	public new void Start()
	{
		base.Start();
		if (scale_sprite != null)
		{
			scale_sprite.gameObject.SetActive(true);
		}
	}

	protected override void UpdateFadeIn(float deltaTime)
	{
		time += deltaTime;
		if (scale_sprite != null)
		{
			float t = Mathf.Clamp(time / scaleInTime, 0f, 1f);
			scale_sprite.transform.localScale = Vector3.Lerp(scaleInBegin, scaleInEnd, t);
		}
		if (time > scaleInTime)
		{
			state = State.Idle;
			if (scale_sprite != null)
			{
				scale_sprite.Static = true;
			}
			if (m_scalein != null)
			{
				m_scalein();
			}
		}
	}

	protected override void UpdateFadeOut(float deltaTime)
	{
		time += deltaTime;
		if (scale_sprite != null)
		{
			float t = Mathf.Clamp(time / scaleOutTime, 0f, 1f);
			scale_sprite.transform.localScale = Vector3.Lerp(scaleOutBegin, scaleOutEnd, t);
		}
		if (time > scaleOutTime)
		{
			state = State.Idle;
			if (scale_sprite != null)
			{
				scale_sprite.transform.localScale = scaleOutEnd;
			}
			if (m_scaleout != null)
			{
				m_scaleout();
			}
			if (scaleOutScene.Length > 0)
			{
				Application.LoadLevel(scaleOutScene);
			}
		}
	}
}
