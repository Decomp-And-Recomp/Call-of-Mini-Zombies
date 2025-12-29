using System;
using UnityEngine;

[ExecuteInEditMode]
public class TUIFont : MonoBehaviour
{
	[Serializable]
	public class FontCharInfo
	{
		public string ch;

		public Vector2 size;

		public Vector4 uv;

		public float bearingX;

		public float advanceX;

		public float descentY;
	}

	[Serializable]
	public class FontInfo
	{
		public string textureFile;

		public float lineHeight;

		public FontCharInfo[] chars;
	}

	public string fontName;

	public string fontPath;

	public FontInfo fontInfo;

	public FontInfo fontInfoHD;

	public Material material;

	private Material materialClone;

	public void Awake()
	{
		FontInfo fontInfo = ChooseFont();
		if (fontInfo != null)
		{
			Texture2D texture2D = Resources.Load(fontInfo.textureFile, typeof(Texture2D)) as Texture2D;
			if ((bool)material)
			{
				materialClone = UnityEngine.Object.Instantiate(material) as Material;
				materialClone.hideFlags = HideFlags.DontSave;
				materialClone.mainTexture = texture2D;
			}
			else
			{
				materialClone = null;
			}
			TUIFontManager.Font font = new TUIFontManager.Font(texture2D, materialClone, fontInfo.lineHeight);
			for (int i = 0; i < fontInfo.chars.Length; i++)
			{
				FontCharInfo fontCharInfo = fontInfo.chars[i];
				TUIFontManager.FontChar fontChar = new TUIFontManager.FontChar(fontCharInfo.size, fontCharInfo.uv, fontCharInfo.bearingX, fontCharInfo.advanceX, fontCharInfo.descentY);
                if (fontCharInfo.ch.Length < 1) continue;
                font.AddChar(fontCharInfo.ch[0], fontChar);
			}
			TUIFontManager.Instance().AddFont(fontName, font);
		}
	}

	public void OnDestroy()
	{
		TUIFontManager.Instance().RemoveFont(fontName);
		if ((bool)materialClone)
		{
			UnityEngine.Object.DestroyImmediate(materialClone);
		}
	}

	private FontInfo ChooseFont()
	{
		if (fontInfoHD == null || !HD())
		{
			return fontInfo;
		}
		return fontInfoHD;
	}

	private bool HD()
	{
		if (Application.isPlaying)
		{
			if (Mathf.Max(Screen.width, Screen.height) > 900)
			{
				return true;
			}
			return false;
		}
		return false;
	}
}
