using System.Collections.Generic;
using UnityEngine;

public class TUIMeshTextClip : TUIMeshText
{
	public TUIRect clip;

	public override void UpdateMesh()
	{
		if (clip == null)
		{
			base.UpdateMesh();
		}
		else
		{
			if (meshFilter == null || meshRender == null)
			{
				return;
			}
			TUIFontManager.Font font = TUIFontManager.Instance().GetFont(fontName);
			if (font == null)
			{
				return;
			}
			List<Sprite> list = new List<Sprite>();
			List<float> list2 = new List<float>();
			int num = 0;
			Vector2 zero = Vector2.zero;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (c == '\n')
				{
					list2.Add(zero.x);
					num++;
					zero.x = 0f;
					zero.y -= font.lineHeight + lineSpacing;
					continue;
				}
				TUIFontManager.FontChar @char = font.GetChar(c);
				if (@char != null)
				{
					Sprite sprite = new Sprite();
					sprite.line = num;
					sprite.position = zero + new Vector2(@char.size.x / 2f + @char.bearingX, @char.size.y / 2f + @char.descentY - font.lineHeight);
					sprite.size = @char.size;
					sprite.uv = @char.uv;
					list.Add(sprite);
					zero.x += @char.advanceX + characterSpacing;
				}
			}
			list2.Add(zero.x);
			num++;
			float num2 = 0f;
			for (int j = 0; j < list2.Count; j++)
			{
				if (list2[j] > num2)
				{
					num2 = list2[j];
				}
			}
			float num3 = num2 / 2f;
			float num4 = (font.lineHeight + lineSpacing) * (float)num - lineSpacing;
			float num5 = num4 / 2f;
			for (int k = 0; k < list.Count; k++)
			{
				Sprite sprite2 = list[k];
				if (horizontalAlignment != 0)
				{
					if (horizontalAlignment == HorizontalAlignment.Right)
					{
						sprite2.position.x -= num2;
					}
					else if (horizontalAlignment == HorizontalAlignment.Center)
					{
						sprite2.position.x -= num3;
					}
					else if (horizontalAlignment == HorizontalAlignment.CenterLine)
					{
						sprite2.position.x -= list2[sprite2.line] / 2f;
					}
				}
				if (verticalAlignment != 0)
				{
					if (verticalAlignment == VerticalAlignment.Bottom)
					{
						sprite2.position.y += num4;
					}
					else if (verticalAlignment == VerticalAlignment.Center)
					{
						sprite2.position.y += num5;
					}
				}
			}
			if ((bool)materialClone)
			{
				materialClone.mainTexture = font.texture;
				meshRender.sharedMaterial = materialClone;
			}
			else if ((bool)font.material)
			{
				meshRender.sharedMaterial = font.material;
			}
			List<Vector3> list3 = new List<Vector3>();
			List<Vector2> list4 = new List<Vector2>();
			List<Color> list5 = new List<Color>();
			List<int> list6 = new List<int>();
			Rect rectLocal = clip.GetRectLocal(base.transform);
			int num6 = 0;
			for (int l = 0; l < list.Count; l++)
			{
				Sprite sprite3 = list[l];
				Rect rect = new Rect(sprite3.position.x + sprite3.size.x * -0.5f, sprite3.position.y + sprite3.size.y * -0.5f, sprite3.size.x, sprite3.size.y);
				Rect rect2 = TUIRect.RectIntersect(rect, rectLocal);
				if (!(rect2.width <= 0f) && !(rect2.height <= 0f))
				{
					list3.Add(new Vector3(rect2.xMin, rect2.yMax, 0f));
					list3.Add(new Vector3(rect2.xMax, rect2.yMax, 0f));
					list3.Add(new Vector3(rect2.xMax, rect2.yMin, 0f));
					list3.Add(new Vector3(rect2.xMin, rect2.yMin, 0f));
					Vector4 zero2 = Vector4.zero;
					float num7 = (sprite3.uv.z - sprite3.uv.x) / sprite3.size.x;
					float num8 = (sprite3.uv.x + sprite3.uv.z) * 0.5f - num7 * sprite3.position.x;
					float num9 = (sprite3.uv.y - sprite3.uv.w) / sprite3.size.y;
					float num10 = (sprite3.uv.y + sprite3.uv.w) * 0.5f - num9 * sprite3.position.y;
					zero2.x = num8 + num7 * rect2.xMin;
					zero2.z = num8 + num7 * rect2.xMax;
					zero2.w = num10 + num9 * rect2.yMin;
					zero2.y = num10 + num9 * rect2.yMax;
					list4.Add(new Vector2(zero2.x, zero2.y));
					list4.Add(new Vector2(zero2.z, zero2.y));
					list4.Add(new Vector2(zero2.z, zero2.w));
					list4.Add(new Vector2(zero2.x, zero2.w));
					list5.Add(color);
					list5.Add(color);
					list5.Add(color);
					list5.Add(color);
					int num11 = num6 * 4;
					list6.Add(num11);
					list6.Add(num11 + 1);
					list6.Add(num11 + 2);
					list6.Add(num11);
					list6.Add(num11 + 2);
					list6.Add(num11 + 3);
					num6++;
				}
			}
			meshFilter.sharedMesh.Clear();
			if (num6 > 0)
			{
				meshFilter.sharedMesh.vertices = list3.ToArray();
				meshFilter.sharedMesh.uv = list4.ToArray();
				meshFilter.sharedMesh.colors = list5.ToArray();
				meshFilter.sharedMesh.triangles = list6.ToArray();
			}
		}
	}
}
