using System.Collections;
using UnityEngine;

public class UILabel : UIControlVisible
{
	private UIFontInfo m_Font;

	protected Color m_Color;

	protected float m_CharacterSpacing;

	protected float m_LineSpacing;

	protected string m_Text;

	public override Rect Rect
	{
		get
		{
			return base.Rect;
		}
		set
		{
			base.Rect = value;
			UpdateText();
		}
	}

	public UILabel()
	{
		m_Font = null;
		m_Color = Color.white;
		m_CharacterSpacing = 0f;
		m_LineSpacing = 0f;
		m_Text = null;
	}

	public void SetFont(UIFontInfo font)
	{
		m_Font = font;
		UpdateText();
	}

	public void SetColor(Color color)
	{
		m_Color = color;
		if (m_Sprite != null)
		{
			for (int i = 0; i < m_Sprite.Length; i++)
			{
				SetSpriteColor(i, color);
			}
		}
	}

	public void SetCharacterSpacing(float character_spacing)
	{
		m_CharacterSpacing = character_spacing;
		UpdateText();
	}

	public void SetLineSpacing(float line_spacing)
	{
		m_LineSpacing = line_spacing;
		UpdateText();
	}

	public void SetText(string text)
	{
		m_Text = text;
		UpdateText();
	}

	public override void Draw()
	{
		if (m_Sprite != null)
		{
			for (int i = 0; i < m_Sprite.Length; i++)
			{
				m_Parent.DrawSprite(m_Sprite[i]);
			}
		}
	}

	private void UpdateText()
	{
		m_Sprite = null;
		if (m_Font == null || m_Text == null || m_Text.Length <= 0)
		{
			return;
		}
		ArrayList arrayList = new ArrayList();
		float num = 0f;
		float num2 = 0f;
		if (num2 + m_Font.GetHeight() > m_Rect.height)
		{
			return;
		}
		for (int i = 0; i < m_Text.Length; i++)
		{
			char c = m_Text[i];
			if (c == '\n' || c == '\r')
			{
				num = 0f;
				num2 += m_Font.GetHeight() + m_LineSpacing;
				if (num2 + m_Font.GetHeight() > m_Rect.height)
				{
					break;
				}
				continue;
			}
			float width = m_Font.GetWidth(c);
			if (num + width > m_Rect.width)
			{
				break;
			}
			UISprite uISprite = new UISprite();
			uISprite.Position = new Vector2(m_Rect.x + num + width / 2f, m_Rect.y + num2 + m_Font.GetHeight() / 2f);
			uISprite.Size = new Vector2(width, m_Font.GetHeight());
			uISprite.Material = m_Font.GetMaterial();
			uISprite.TextureRect = m_Font.GetUVRect(c);
			uISprite.Color = m_Color;
			if (m_Clip)
			{
				uISprite.SetClip(m_ClipRect);
			}
			arrayList.Add(uISprite);
			num += width + m_CharacterSpacing;
		}
		m_Sprite = new UISprite[arrayList.Count];
		for (int j = 0; j < arrayList.Count; j++)
		{
			m_Sprite[j] = (UISprite)arrayList[j];
		}
	}

	public int GetTextWidth(string text)
	{
		if (m_Font == null)
		{
			return 0;
		}
		return m_Font.GetTextWidth(text);
	}
}
