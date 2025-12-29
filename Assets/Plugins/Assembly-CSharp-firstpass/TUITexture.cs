using System;
using UnityEngine;

[ExecuteInEditMode]
public class TUITexture : MonoBehaviour
{
	[Serializable]
	public class FrameInfo
	{
		public string frameName;

		public Vector2 size;

		public Vector4 uv;
	}

	[Serializable]
	public class TextureInfo
	{
		public string textureFile;

		public FrameInfo[] frames;
	}

	public string path;

	public bool hd = true;

	public string atlasPath;

	public string atlasFile;

	public TextureInfo[] textureInfo;

	public TextureInfo[] textureInfoHD;

	public Material material;

	private Material[] materialClone;

	public void Awake()
	{
		TextureInfo[] array = ChooseTexture();
		materialClone = new Material[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			Texture2D texture2D = Resources.Load(array[i].textureFile, typeof(Texture2D)) as Texture2D;
			if ((bool)material)
			{
				materialClone[i] = UnityEngine.Object.Instantiate(material) as Material;
				materialClone[i].hideFlags = HideFlags.DontSave;
				materialClone[i].mainTexture = texture2D;
			}
			else
			{
				materialClone[i] = null;
			}
			for (int j = 0; j < array[i].frames.Length; j++)
			{
				TUITextureManager.Instance().AddFrame(array[i].frames[j].frameName, texture2D, materialClone[i], array[i].frames[j].size, array[i].frames[j].uv);
			}
		}
	}

	public void OnDestroy()
	{
		TextureInfo[] array = ChooseTexture();
		for (int i = 0; i < array.Length; i++)
		{
			for (int j = 0; j < array[i].frames.Length; j++)
			{
				TUITextureManager.Instance().RemoveFrame(array[i].frames[j].frameName);
			}
		}
		for (int k = 0; k < materialClone.Length; k++)
		{
			UnityEngine.Object.DestroyImmediate(materialClone[k]);
		}
	}

	private TextureInfo[] ChooseTexture()
	{
		if (!hd || !HD())
		{
			return textureInfo;
		}
		return textureInfoHD;
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
