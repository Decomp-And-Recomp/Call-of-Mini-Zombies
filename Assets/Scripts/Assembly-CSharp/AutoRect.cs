using UnityEngine;

public class AutoRect
{
    public static Rect AutoPos(Rect rect)
    {
        ResetResolution();

        float scaleX = (float)Screen.width / 960f;  // base reference
        float scaleY = (float)Screen.height / 640f; // base reference

        float width = rect.width * scaleX;
        float height = rect.height * scaleY;

        float x = (Screen.width - width) / 2f;
        float y = (Screen.height - height) / 2f;

        return new Rect(x, y, width, height);
    }


    public static Rect AutoValuePos(Rect rect)
	{
		ResetResolution();
		return new Rect(rect.x * ResolutionConstant.R, rect.y * ResolutionConstant.R, rect.width * ResolutionConstant.R, rect.height * ResolutionConstant.R);
	}

	public static Vector2 AutoValuePos(Vector2 v)
	{
		ResetResolution();
		return new Vector2(v.x * ResolutionConstant.R, v.y * ResolutionConstant.R);
	}

	public static Vector2 AutoSize(Rect rect)
	{
		ResetResolution();
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
		{
			return new Vector2(rect.width * ResolutionConstant.R, rect.height * ResolutionConstant.H);
		}
		if (ResolutionConstant.R == 1f)
		{
			return new Vector2(rect.width * ResolutionConstant.R, rect.height * ResolutionConstant.R);
		}
		if (ResolutionConstant.R == 2f)
		{
			return new Vector2(rect.width * ResolutionConstant.R, rect.height * ResolutionConstant.R);
		}
		return new Vector2(rect.width, rect.height);
	}

	public static Vector2 AutoSize(Rect rect, float zoom)
	{
		ResetResolution();
		if (ResolutionConstant.R == 1f || ResolutionConstant.R == 2f)
		{
			return new Vector2(rect.width * ResolutionConstant.R * zoom, rect.height * ResolutionConstant.R * zoom);
		}
		return new Vector2(rect.width * zoom, rect.height * zoom);
	}

	public static Vector2 AutoSize(Vector2 v)
	{
		ResetResolution();
		if (ResolutionConstant.R == 1f || ResolutionConstant.R == 2f)
		{
			return new Vector2(v.x * ResolutionConstant.R, v.y * ResolutionConstant.R);
		}
		return v;
	}

	public static Rect AutoTex(Rect rect)
	{
		ResetResolution();
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
		{
			return new Rect(rect.x * 1f, rect.y * 1f, rect.width * 1f, rect.height * 1f);
		}
		if (ResolutionConstant.R == 2f)
		{
			return new Rect(rect.x * 1f, rect.y * 1f, rect.width * 1f, rect.height * 1f);
		}
		return new Rect(rect.x * ResolutionConstant.R, rect.y * ResolutionConstant.R, rect.width * ResolutionConstant.R, rect.height * ResolutionConstant.R);
	}

	public static Vector2 AutoTex(Vector2 v)
	{
		ResetResolution();
		if (ResolutionConstant.R == 2f)
		{
			return new Vector2(v.x * 1f, v.y * 1f);
		}
		return new Vector2(v.x * ResolutionConstant.R, v.y * ResolutionConstant.R);
	}

	public static float AutoValue(float x)
	{
		return x * ResolutionConstant.R;
	}

	public static void ResetResolution()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
		{
			ResolutionConstant.R = (float)Screen.width / 960f;
			ResolutionConstant.H = (float)Screen.height / 640f;
		}
		else if (Mathf.Max(Screen.width, Screen.height) == 960)
		{
			ResolutionConstant.R = 1f;
		}
		else if (Mathf.Max(Screen.width, Screen.height) == 480)
		{
			ResolutionConstant.R = 0.5f;
		}
		else if (Mathf.Max(Screen.width, Screen.height) == 2048)
		{
			ResolutionConstant.R = 2f;
		}
		else if (Mathf.Max(Screen.width, Screen.height) == 1136)
		{
			ResolutionConstant.R = 1f;
		}
		else
		{
			ResolutionConstant.R = 1f;
		}
	}

	public static Platform GetPlatform()
	{
		return Platform.Android;
	}

	public static float GetFakeScreenWidth()
	{
		if (GetPlatform() == Platform.iPhone5)
		{
			return 960f;
		}
		return Screen.width;
	}

	public static float GetFakeScreenHeight()
	{
		if (GetPlatform() == Platform.iPhone5)
		{
			return 640f;
		}
		return Screen.height;
	}

	public static float GetFakeScreenWidthHeightRatio()
	{
		float num = 0f;
		float num2 = 0f;
		if (GetPlatform() == Platform.iPhone5)
		{
			num = 960f;
			num2 = 640f;
		}
		else
		{
			num = Screen.width;
			num2 = Screen.height;
		}
		return num2 / num;
	}
}
