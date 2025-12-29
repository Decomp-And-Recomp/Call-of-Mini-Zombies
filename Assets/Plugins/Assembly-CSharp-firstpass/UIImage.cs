using UnityEngine;

public class UIImage : UIControlVisible
{
	public enum Command
	{
		Click = 0
	}

	protected int m_TouchFingerId = -1;

	public override Rect Rect
	{
		get
		{
			return base.Rect;
		}
		set
		{
			base.Rect = value;
			Vector2 position = new Vector2(value.x + value.width / 2f, value.y + value.height / 2f);
			SetSpritePosition(0, position);
		}
	}

	public UIImage()
	{
		CreateSprite(1);
	}

	public void SetTexture(Material material, Rect texture_rect, Vector2 size)
	{
		SetSpriteTexture(0, material, texture_rect, size);
	}

	public void SetTexture(Material material, Rect texture_rect)
	{
		SetSpriteTexture(0, material, texture_rect);
	}

	public void SetTextureSize(Vector2 size)
	{
		SetSpriteSize(0, size);
	}

	public void SetRotation(float rotation)
	{
		SetSpriteRotation(0, rotation);
	}

	public float GetRotation()
	{
		return GetSpriteRotation(0);
	}

	public void SetColor(Color color)
	{
		SetSpriteColor(0, color);
	}

	public override void Draw()
	{
		m_Parent.DrawSprite(m_Sprite[0]);
	}

	public override bool HandleInput(UITouchInner touch)
	{
		if (touch.phase == TouchPhase.Began)
		{
			m_TouchFingerId = touch.fingerId;
		}
		else if (touch.phase == TouchPhase.Ended && touch.fingerId == m_TouchFingerId && PtInRect(touch.position))
		{
			m_Parent.SendEvent(this, 0, 0f, 0f);
			m_TouchFingerId = -1;
			return true;
		}
		return false;
	}
}
