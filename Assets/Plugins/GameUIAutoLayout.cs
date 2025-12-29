using System.Collections;
using UnityEngine;

public class GameUIAutoLayout : MonoBehaviour
{
	public bool left;

	public bool right;

	public bool top;

	public bool bottom;

	public IEnumerator Start()
	{
		yield return null;
		yield return null;

        int width = 0;
		int height = 0;

		if (TUIVirtualScreen.use)
		{
			width = TUIVirtualScreen.res.x;
			height = TUIVirtualScreen.res.y;
		}
		else
		{
			width = Screen.width;
			height = Screen.height;
		}

		if (Mathf.Max(width, height) >= 960)
		{
			float x = 0f;
			float y = 0f;
			int num = width;
			int num2 = height;
			if (Mathf.Max(width, height) == 2048)
			{
				num /= 2;
				num2 /= 2;
			}
			if (left)
			{
				x = base.transform.position.x - (float)(num - 960) / 4f;
			}
			if (right)
			{
				x = base.transform.position.x + (float)(num - 960) / 4f;
			}
			if (top)
			{
				y = base.transform.position.y + (float)(num2 - 640) / 4f;
			}
			if (bottom)
			{
				y = base.transform.position.y - (float)(num2 - 640) / 4f;
			}
			base.transform.position = new Vector3(x, y, base.transform.position.z);
		}
	}
}
