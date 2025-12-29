using UnityEngine;

public class Sprite2DStatic : Sprite2D
{
	public struct Frame
	{
		public Material Material;

		public Rect TextureRect;
	}

	public Frame ImageFrame
	{
		set
		{
			base.Material = value.Material;
			base.TextureRect = value.TextureRect;
		}
	}
}
