using System.Collections.Generic;
using UnityEngine;

public class TUITextureManager
{
	public struct Frame
	{
		public Texture2D texture;

		public Material material;

		public Vector2 size;

		public Vector4 uv;

		public Frame(Texture2D texture, Material material, Vector2 size, Vector4 uv)
		{
			this.texture = texture;
			this.material = material;
			this.size = size;
			this.uv = uv;
		}
	}

	private static TUITextureManager instance;

	private Dictionary<string, Frame> frameMap = new Dictionary<string, Frame>();

	public static TUITextureManager Instance()
	{
		if (instance == null)
		{
			instance = new TUITextureManager();
		}
		return instance;
	}

	public void AddFrame(string frameName, Texture2D texture, Material material, Vector2 size, Vector4 uv)
	{
		if (!frameMap.ContainsKey(frameName))
		{
			frameMap.Add(frameName, new Frame(texture, material, size, uv));
		}
	}

	public void RemoveFrame(string frameName)
	{
		if (frameMap.ContainsKey(frameName))
		{
			frameMap.Remove(frameName);
		}
	}

	public Frame GetFrame(string frameName)
	{
		Frame value = new Frame(null, null, Vector2.zero, Vector4.zero);
		frameMap.TryGetValue(frameName, out value);
		return value;
	}
}
