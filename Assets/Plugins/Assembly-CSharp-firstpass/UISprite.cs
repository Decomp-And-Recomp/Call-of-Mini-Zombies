using UnityEngine;

public class UISprite : Sprite
{
	protected bool m_Clip;

	protected Rect m_ClipRect;

	public new int Layer
	{
		get
		{
			return base.Layer;
		}
	}

	public new Vector2 Size
	{
		get
		{
			return m_Size;
		}
		set
		{
			m_Size = value;
			m_UpdateVertices = true;
			if (m_Clip)
			{
				m_UpdateUV = true;
			}
		}
	}

	public new Vector2 Position
	{
		get
		{
			return m_Position;
		}
		set
		{
			m_Position = value;
			m_UpdateVertices = true;
			if (m_Clip)
			{
				m_UpdateUV = true;
			}
		}
	}

	public UISprite()
	{
		base.Layer = 0;
	}

	public void SetClip(Rect clip_rect)
	{
		m_Clip = true;
		m_ClipRect = clip_rect;
		m_UpdateVertices = true;
		m_UpdateUV = true;
	}

	public void ClearClip()
	{
		m_Clip = false;
		m_UpdateVertices = true;
		m_UpdateUV = true;
	}

	protected override void UpdateVertices()
	{
		if (m_Clip)
		{
			UpdateClipVertices();
		}
		else
		{
			base.UpdateVertices();
		}
	}

	protected override void UpdateUV()
	{
		if (m_Clip)
		{
			UpdateClipUV();
		}
		else
		{
			base.UpdateUV();
		}
	}

	protected void UpdateClipVertices()
	{
		Rect rect = new Rect((int)(m_Position.x - m_Size.x / 2f), (int)(m_Position.y - m_Size.y / 2f), m_Size.x, m_Size.y);
		if (m_ClipRect.xMin > rect.xMax || m_ClipRect.xMax < rect.xMin || m_ClipRect.yMin > rect.yMax || m_ClipRect.yMax < rect.yMin)
		{
			m_Vertices[0] = (m_Vertices[1] = (m_Vertices[2] = (m_Vertices[3] = new Vector3(-1f, -1f, 0f))));
			m_UpdateVertices = false;
			return;
		}
		if (m_ClipRect.xMin > rect.xMin)
		{
			rect.xMin = m_ClipRect.xMin;
		}
		if (m_ClipRect.xMax < rect.xMax)
		{
			rect.xMax = m_ClipRect.xMax;
		}
		if (m_ClipRect.yMin > rect.yMin)
		{
			rect.yMin = m_ClipRect.yMin;
		}
		if (m_ClipRect.yMax < rect.yMax)
		{
			rect.yMax = m_ClipRect.yMax;
		}
		if (m_Rotation == 0f)
		{
			m_Vertices[0] = new Vector3(rect.xMin, rect.yMax, 0f);
			m_Vertices[1] = new Vector3(rect.xMax, rect.yMax, 0f);
			m_Vertices[2] = new Vector3(rect.xMax, rect.yMin, 0f);
			m_Vertices[3] = new Vector3(rect.xMin, rect.yMin, 0f);
		}
		else
		{
			float num = Mathf.Sin(m_Rotation);
			float num2 = Mathf.Cos(m_Rotation);
			float num3 = m_Position.x - rect.xMin;
			float num4 = rect.yMax - m_Position.y;
			m_Vertices[0] = new Vector3(m_Position.x + ((0f - num3) * num2 - num4 * num), m_Position.y + ((0f - num3) * num + num4 * num2), 0f);
			num3 = rect.xMax - m_Position.x;
			num4 = rect.yMax - m_Position.y;
			m_Vertices[1] = new Vector3(m_Position.x + (num3 * num2 - num4 * num), m_Position.y + (num3 * num + num4 * num2), 0f);
			num3 = rect.xMax - m_Position.x;
			num4 = m_Position.y - rect.yMin;
			m_Vertices[2] = new Vector3(m_Position.x + (num3 * num2 + num4 * num), m_Position.y + (num3 * num - num4 * num2), 0f);
			num3 = m_Position.x - rect.xMin;
			num4 = m_Position.y - rect.yMin;
			m_Vertices[3] = new Vector3(m_Position.x + ((0f - num3) * num2 + num4 * num), m_Position.y + ((0f - num3) * num - num4 * num2), 0f);
		}
		m_UpdateVertices = false;
	}

	protected void UpdateClipUV()
	{
		Rect rect = new Rect((int)(m_Position.x - m_Size.x / 2f), (int)(m_Position.y - m_Size.y / 2f), m_Size.x, m_Size.y);
		if (m_ClipRect.xMin > rect.xMax || m_ClipRect.xMax < rect.xMin || m_ClipRect.yMin > rect.yMax || m_ClipRect.yMax < rect.yMin)
		{
			m_UV[0] = (m_UV[1] = (m_UV[2] = (m_UV[3] = new Vector2(0f, 0f))));
			m_UpdateVertices = false;
			return;
		}
		Rect textureRect = m_TextureRect;
		float num = m_ClipRect.xMin - rect.xMin;
		if (num > 0f)
		{
			textureRect.xMin += num;
		}
		num = rect.xMax - m_ClipRect.xMax;
		if (num > 0f)
		{
			textureRect.xMax -= num;
		}
		num = m_ClipRect.yMin - rect.yMin;
		if (num > 0f)
		{
			textureRect.yMax -= num;
		}
		num = rect.yMax - m_ClipRect.yMax;
		if (num > 0f)
		{
			textureRect.yMin += num;
		}
		float num2 = 1f / (float)m_Material.mainTexture.width;
		float num3 = 1f / (float)m_Material.mainTexture.height;
		float x = textureRect.xMin * num2;
		float x2 = textureRect.xMax * num2;
		float y = 1f - textureRect.yMax * num3;
		float y2 = 1f - textureRect.yMin * num3;
		if (!m_FlipX && !m_FlipY)
		{
			m_UV[0] = new Vector2(x, y2);
			m_UV[1] = new Vector2(x2, y2);
			m_UV[2] = new Vector2(x2, y);
			m_UV[3] = new Vector2(x, y);
		}
		else if (m_FlipX && !m_FlipY)
		{
			m_UV[0] = new Vector2(x2, y2);
			m_UV[1] = new Vector2(x, y2);
			m_UV[2] = new Vector2(x, y);
			m_UV[3] = new Vector2(x2, y);
		}
		else if (!m_FlipX && m_FlipY)
		{
			m_UV[0] = new Vector2(x, y);
			m_UV[1] = new Vector2(x2, y);
			m_UV[2] = new Vector2(x2, y2);
			m_UV[3] = new Vector2(x, y2);
		}
		else
		{
			m_UV[0] = new Vector2(x2, y);
			m_UV[1] = new Vector2(x, y);
			m_UV[2] = new Vector2(x, y2);
			m_UV[3] = new Vector2(x2, y2);
		}
		m_UpdateUV = false;
	}
}
