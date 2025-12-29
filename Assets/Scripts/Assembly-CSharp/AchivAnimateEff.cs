using UnityEngine;

public class AchivAnimateEff : MonoBehaviour
{
	public TUIFadeEx ani0;

	public TUIMeshSpriteAnimation ani1;

	public TUIMeshSpriteAnimation ani2;

	public TUIScaleEx ani3;

	protected int m_step;

	public float m_life = 1f;

	protected float m_life_time;

	protected bool active_eff;

	private void Start()
	{
		ani0.m_fadein = OnFadeIn;
		ani0.m_fadeout = OnFadeOut;
		ani3.m_scalein = OScaleIn;
		ani3.m_scaleout = OnScaleOut;
		ani1.on_animation_end = OnAnimationEnd;
		ani2.on_animation_end = OnAnimationEnd;
	}

	private void Update()
	{
		if (active_eff)
		{
			m_life_time += Time.deltaTime;
			if (m_life_time >= m_life)
			{
				active_eff = false;
				Object.Destroy(base.gameObject);
			}
		}
	}

	public void SetScaleFrame(string frame)
	{
		ani3.scale_sprite.frameName_Accessor = frame;
	}

	public void EffGo(string frame)
	{
		SetScaleFrame(frame);
		active_eff = true;
		m_step = 0;
		ani0.fadeInTime = 1f / 3f;
		ani0.fadeOutTime = 7f / 30f;
		ani0.FadeIn();
		m_step++;
		ani3.scaleInBegin = new Vector3(0.1f, 0.1f, 1f);
		ani3.scaleInEnd = new Vector3(1.1f, 1.1f, 1f);
		ani3.scaleInTime = 0.1f;
		ani3.ScaleIn();
	}

	private void OnFadeIn()
	{
		ani0.FadeOut();
	}

	private void OnFadeOut()
	{
	}

	private void OScaleIn()
	{
		if (m_step == 1)
		{
			m_step++;
			ani3.scaleOutBegin = new Vector3(1.1f, 1.1f, 1f);
			ani3.scaleOutEnd = new Vector3(0.9f, 0.9f, 1f);
			ani3.scaleOutTime = 2f / 15f;
			ani3.ScaleOut();
			ani1.Play();
			ani2.Play();
		}
		else if (m_step == 3 && (bool)base.transform.parent.GetComponent<TUIButtonClick>())
		{
			base.transform.parent.GetComponent<TUIButtonClick>().frameNormal.GetComponent<TUIMeshSprite>().frameName_Accessor = ani3.scale_sprite.frameName_Accessor;
			base.transform.parent.GetComponent<TUIButtonClick>().framePressed.GetComponent<TUIMeshSprite>().frameName_Accessor = ani3.scale_sprite.frameName_Accessor;
			base.transform.parent.GetComponent<TUIButtonClick>().frameDisabled.GetComponent<TUIMeshSprite>().frameName_Accessor = ani3.scale_sprite.frameName_Accessor;
		}
	}

	private void OnScaleOut()
	{
		if (m_step == 2)
		{
			m_step++;
			ani3.scaleInBegin = new Vector3(0.9f, 0.9f, 1f);
			ani3.scaleInEnd = new Vector3(1f, 1f, 1f);
			ani3.scaleInTime = 0.1f;
			ani3.ScaleIn();
		}
	}

	private void OnAnimationEnd(TUIMeshSpriteAnimation sprite)
	{
		sprite.gameObject.SetActive(false);
	}
}
