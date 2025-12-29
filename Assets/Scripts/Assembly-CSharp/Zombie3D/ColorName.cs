using UnityEngine;

namespace Zombie3D
{
	public class ColorName
	{
		public static Color fontColor_darkred = new Color(1f, 6f / 85f, 0.0627451f, 1f);

		public static Color fontColor_red = new Color(84f / 85f, 0.20392157f, 1f / 85f, 1f);

		public static Color fontColor_yellow = new Color(1f, 73f / 85f, 2f / 15f, 1f);

		public static Color fontColor_orange = new Color(0.7921569f, 0.5294118f, 7f / 85f, 1f);

		public static Color fontColor_darkorange = new Color(0.6039216f, 0.41960785f, 0.101960786f, 1f);

		public static Color fontColor_green = new Color(3f / 85f, 76f / 85f, 2f / 51f, 1f);

		public static Color fontColor_blue = new Color(0.003921569f, 71f / 85f, 0.99607843f, 1f);

		public static Color modelEdgeColor_purple = new Color(0.5019608f, 0.5019608f, 1f, 1f);

		public static Color modelEdgeColor_blue_max = new Color(49f / 85f, 0.81960785f, 1f, 1f);

		public static Color modelEdgeColor_blue_min = new Color(0.41568628f, 0.59607846f, 37f / 51f, 1f);

		public static Color modelEdgeColor_gold_max = new Color(1f, 44f / 51f, 19f / 85f, 1f);

		public static Color modelEdgeColor_gold_min = new Color(2f / 3f, 29f / 51f, 0.14509805f, 1f);

		public static Color32 GetPlayerMarkColor(int index)
		{
			Color32 result = Color.white;
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				result = new Color32(246, 218, 22, byte.MaxValue);
			}
			else
			{
				switch (index)
				{
				case 0:
					result = new Color32(246, 218, 22, byte.MaxValue);
					break;
				case 1:
					result = new Color32(22, 234, 11, byte.MaxValue);
					break;
				case 2:
					result = new Color32(220, 80, 245, byte.MaxValue);
					break;
				case 3:
					result = new Color32(39, 93, 204, byte.MaxValue);
					break;
				}
			}
			return result;
		}
	}
}
