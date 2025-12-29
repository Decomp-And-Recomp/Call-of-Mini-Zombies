using UnityEngine;

public class UIButtonBase : UIControlVisible
{
	public enum State
	{
		Normal = 0,
		Pressed = 1,
		Disabled = 2
	}

	protected State m_State;

	protected UISprite m_HoverSprite;

	protected Vector2 m_HoverCenterDelta;

	public UIButtonBase()
	{
		CreateSprite(3);
		m_State = State.Normal;
	}

	public void SetTexture(State state, Material material, Rect texture_rect)
	{
		SetSpriteTexture((int)state, material, texture_rect);
	}

	public void SetTexture(State state, Material material, Rect texture_rect, Vector2 size)
	{
		SetSpriteTexture((int)state, material, texture_rect, size);
	}

	public void SetTextureSize(State state, Vector2 size)
	{
		SetSpriteSize((int)state, size);
	}

	public void SetColor(State state, Color color)
	{
		SetSpriteColor((int)state, color);
	}

	public void SetRotate(float rotate)
	{
		SetSpriteRotation(0, rotate);
		SetSpriteRotation(1, rotate);
		SetSpriteRotation(2, rotate);
	}

	public float GetRotate()
	{
		return GetSpriteRotation((int)m_State);
	}

	public void SetHoverSprite(Material material, Rect texture_rect)
	{
		SetHoverSprite(material, texture_rect, new Vector2(0f, 0f));
	}

	public void SetHoverSprite(Material material, Rect texture_rect, Vector2 center_deta)
	{
		m_HoverSprite = new UISprite();
		m_HoverSprite.Material = material;
		m_HoverSprite.TextureRect = texture_rect;
		m_HoverSprite.Size = new Vector2(texture_rect.width, texture_rect.height);
		m_HoverSprite.Position = new Vector2(Rect.x + Rect.width / 2f, Rect.y + Rect.height / 2f) + center_deta;
		m_HoverCenterDelta = center_deta;
	}

	public override void SetClip(Rect clip_rect)
	{
		base.SetClip(clip_rect);
		if (m_HoverSprite != null)
		{
			m_HoverSprite.SetClip(clip_rect);
		}
	}

	public override void Draw()
	{
		if (!m_Enable)
		{
			m_Parent.DrawSprite(m_Sprite[2]);
			return;
		}
		switch (m_State)
		{
		case State.Normal:
			m_Parent.DrawSprite(m_Sprite[0]);
			break;
		case State.Pressed:
			m_Parent.DrawSprite(m_Sprite[1]);
			if (m_HoverSprite != null)
			{
				m_HoverSprite.Position = new Vector2(Rect.x + Rect.width / 2f, Rect.y + Rect.height / 2f) + m_HoverCenterDelta;
				m_Parent.DrawSprite(m_HoverSprite);
			}
			break;
		}
	}
}
