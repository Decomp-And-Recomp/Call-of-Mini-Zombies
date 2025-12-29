using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TUIMeshText : MonoBehaviour
{
	public enum HorizontalAlignment
	{
		Left = 0,
		Right = 1,
		Center = 2,
		CenterLine = 3
	}

	public enum VerticalAlignment
	{
		Top = 0,
		Bottom = 1,
		Center = 2
	}

	protected class Sprite
	{
		public int line;

		public Vector2 position;

		public Vector2 size;

		public Vector4 uv;
	}

	public bool Static = true;

	public Material material;

	public string fontName;

	public Color color = Color.white;

	public float characterSpacing;

	public float lineSpacing;

	public HorizontalAlignment horizontalAlignment;

	public VerticalAlignment verticalAlignment;

	public string text;

	protected Material materialClone;

	protected MeshFilter meshFilter;

	protected MeshRenderer meshRender;

	public bool Static_Accessor
	{
		get
		{
			return Static;
		}
		set
		{
			Static = value;
		}
	}

	public Material material_Accessor
	{
		get
		{
			return material;
		}
		set
		{
			material = value;
			Static = false;
		}
	}

	public string fontName_Accessor
	{
		get
		{
			return fontName;
		}
		set
		{
			fontName = value;
			Static = false;
		}
	}

	public Color color_Accessor
	{
		get
		{
			return color;
		}
		set
		{
			color = value;
			Static = false;
		}
	}

	public float characterSpacing_Accessor
	{
		get
		{
			return characterSpacing;
		}
		set
		{
			characterSpacing = value;
			Static = false;
		}
	}

	public float lineSpacing_Accessor
	{
		get
		{
			return lineSpacing;
		}
		set
		{
			lineSpacing = value;
			Static = false;
		}
	}

	public HorizontalAlignment horizontalAlignment_Accessor
	{
		get
		{
			return horizontalAlignment;
		}
		set
		{
			horizontalAlignment = value;
			Static = false;
		}
	}

	public VerticalAlignment verticalAlignment_Accessor
	{
		get
		{
			return verticalAlignment;
		}
		set
		{
			verticalAlignment = value;
			Static = false;
		}
	}

	public string text_Accessor
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
			Static = false;
		}
	}

	public void Awake()
	{
		if ((bool)material)
		{
			materialClone = Object.Instantiate(material) as Material;
			materialClone.hideFlags = HideFlags.DontSave;
		}
		else
		{
			materialClone = null;
		}
		meshFilter = base.gameObject.GetComponent<MeshFilter>();
		meshRender = base.gameObject.GetComponent<MeshRenderer>();
		meshFilter.sharedMesh = new Mesh();
		meshFilter.sharedMesh.hideFlags = HideFlags.DontSave;
		meshRender.castShadows = false;
		meshRender.receiveShadows = false;
	}

	public void Start()
	{
		UpdateMesh();
	}

	private void OnDestroy()
	{
		if ((bool)meshFilter && (bool)meshFilter.sharedMesh)
		{
			Object.DestroyImmediate(meshFilter.sharedMesh);
		}
		if ((bool)materialClone)
		{
			Object.DestroyImmediate(materialClone);
		}
	}

	public void Update()
	{
		if (!Static)
		{
			UpdateMesh();
		}
	}

	public virtual void UpdateMesh()
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
		Vector3[] array = new Vector3[list.Count * 4];
		Vector2[] array2 = new Vector2[list.Count * 4];
		Color[] array3 = new Color[list.Count * 4];
		int[] array4 = new int[list.Count * 6];
		for (int l = 0; l < list.Count; l++)
		{
			Sprite sprite3 = list[l];
			int num6 = l * 4;
			int num7 = l * 6;
			Vector2 vector = sprite3.size * 0.5f;
			Vector3 vector2 = new Vector3(sprite3.position.x, sprite3.position.y, 0f);
			array[num6] = vector2 + new Vector3(0f - vector.x, vector.y, 0f);
			array[num6 + 1] = vector2 + new Vector3(vector.x, vector.y, 0f);
			array[num6 + 2] = vector2 + new Vector3(vector.x, 0f - vector.y, 0f);
			array[num6 + 3] = vector2 + new Vector3(0f - vector.x, 0f - vector.y, 0f);
			array2[num6] = new Vector2(sprite3.uv.x, sprite3.uv.y);
			array2[num6 + 1] = new Vector2(sprite3.uv.z, sprite3.uv.y);
			array2[num6 + 2] = new Vector2(sprite3.uv.z, sprite3.uv.w);
			array2[num6 + 3] = new Vector2(sprite3.uv.x, sprite3.uv.w);
			array3[num6] = color;
			array3[num6 + 1] = color;
			array3[num6 + 2] = color;
			array3[num6 + 3] = color;
			array4[num7] = num6;
			array4[num7 + 1] = num6 + 1;
			array4[num7 + 2] = num6 + 2;
			array4[num7 + 3] = num6;
			array4[num7 + 4] = num6 + 2;
			array4[num7 + 5] = num6 + 3;
		}
		meshFilter.sharedMesh.Clear();
		meshFilter.sharedMesh.vertices = array;
		meshFilter.sharedMesh.uv = array2;
		meshFilter.sharedMesh.colors = array3;
		meshFilter.sharedMesh.triangles = array4;
		Static = true;
	}
}
