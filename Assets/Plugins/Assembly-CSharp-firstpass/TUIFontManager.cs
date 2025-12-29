using System.Collections.Generic;
using UnityEngine;

public class TUIFontManager
{
	public class FontChar
	{
		public Vector2 size;

		public Vector4 uv;

		public float bearingX;

		public float advanceX;

		public float descentY;

		public FontChar(Vector2 size, Vector4 uv, float bearingX, float advanceX, float descentY)
		{
			this.size = size;
			this.uv = uv;
			this.bearingX = bearingX;
			this.advanceX = advanceX;
			this.descentY = descentY;
		}
	}

	public class Font
	{
		public Texture2D texture;

		public Material material;

		public float lineHeight;

		public Dictionary<char, FontChar> fontChars;

		public Font(Texture2D texture, Material material, float lineHeight)
		{
			this.texture = texture;
			this.material = material;
			this.lineHeight = lineHeight;
			fontChars = new Dictionary<char, FontChar>();
		}

		public void AddChar(char ch, FontChar fontChar)
		{
			if (!fontChars.ContainsKey(ch))
			{
				fontChars.Add(ch, fontChar);
			}
		}

		public FontChar GetChar(char ch)
		{
			FontChar value = null;
			fontChars.TryGetValue(ch, out value);
			return value;
		}
	}

	private static TUIFontManager instance;

	private Dictionary<string, Font> fontMap = new Dictionary<string, Font>();

	public static TUIFontManager Instance()
	{
		if (instance == null)
		{
			instance = new TUIFontManager();
		}
		return instance;
	}

	public void AddFont(string fontName, Font font)
	{
		if (!fontMap.ContainsKey(fontName))
		{
			fontMap.Add(fontName, font);
		}
	}

	public void RemoveFont(string fontName)
	{
		if (fontMap.ContainsKey(fontName))
		{
			fontMap.Remove(fontName);
		}
	}

	public Font GetFont(string fontName)
	{
		Font value = null;
		fontMap.TryGetValue(fontName, out value);
		return value;
	}
}
