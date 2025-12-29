using UnityEngine;

public class UIControlVisible : UIControl
{
	protected UISprite[] m_Sprite;

	public UIControlVisible()
	{
		m_Sprite = null;
	}

	protected void CreateSprite(int number)
	{
		m_Sprite = new UISprite[number];
		for (int i = 0; i < number; i++)
		{
			m_Sprite[i] = new UISprite();
		}
	}

	protected void SetSpriteTexture(int index, Material material, Rect texture_rect, Vector2 size)
	{
		m_Sprite[index].Material = material;
		m_Sprite[index].TextureRect = texture_rect;
		m_Sprite[index].Size = size;
	}

	protected void SetSpriteTexture(int index, Material material, Rect texture_rect)
	{
		m_Sprite[index].Material = material;
		m_Sprite[index].TextureRect = texture_rect;
		m_Sprite[index].Size = new Vector2(texture_rect.width, texture_rect.height);
	}

	protected void SetSpriteSize(int index, Vector2 size)
	{
		m_Sprite[index].Size = size;
	}

	protected void SetSpriteColor(int index, Color color)
	{
		m_Sprite[index].Color = color;
	}

	protected void SetSpritePosition(int index, Vector2 position)
	{
		m_Sprite[index].Position = position;
	}

	protected void SetSpriteRotation(int index, float rotation)
	{
		m_Sprite[index].Rotation = rotation;
	}

	protected float GetSpriteRotation(int index)
	{
		return m_Sprite[index].Rotation;
	}

	public override void SetClip(Rect clip_rect)
	{
		base.SetClip(clip_rect);
		if (m_Sprite != null)
		{
			for (int i = 0; i < m_Sprite.Length; i++)
			{
				m_Sprite[i].SetClip(clip_rect);
			}
		}
	}

	public override void ClearClip()
	{
		base.ClearClip();
		if (m_Sprite != null)
		{
			for (int i = 0; i < m_Sprite.Length; i++)
			{
				m_Sprite[i].ClearClip();
			}
		}
	}
}
