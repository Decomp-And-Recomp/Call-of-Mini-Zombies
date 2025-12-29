using UnityEngine;

public class iPhoneInputMgr
{
	private static UITouchInner[] touches = new UITouchInner[0];

	private static UITouchInner[] touches0 = new UITouchInner[0];

	private static UITouchInner[] touches1 = new UITouchInner[1];

	private static UITouchInner[] touches2 = new UITouchInner[2];

	public static UITouchInner[] MockTouches()
	{
		if (Input.touches.Length == 0)
		{
			return touches0;
		}
		if (Input.touches.Length == 1)
		{
			int num = 0;
			Touch[] array = Input.touches;
			for (int i = 0; i < array.Length; i++)
			{
				Touch touch = array[i];
				touches1[num].deltaPosition = touch.deltaPosition;
				touches1[num].deltaTime = touch.deltaTime;
				touches1[num].fingerId = touch.fingerId;
				touches1[num].phase = touch.phase;
				touches1[num].position = touch.position;
				touches1[num].tapCount = touch.tapCount;
				num++;
			}
			return touches1;
		}
		if (Input.touches.Length == 2)
		{
			int num2 = 0;
			Touch[] array2 = Input.touches;
			for (int j = 0; j < array2.Length; j++)
			{
				Touch touch2 = array2[j];
				touches2[num2].deltaPosition = touch2.deltaPosition;
				touches2[num2].deltaTime = touch2.deltaTime;
				touches2[num2].fingerId = touch2.fingerId;
				touches2[num2].phase = touch2.phase;
				touches2[num2].position = touch2.position;
				touches2[num2].tapCount = touch2.tapCount;
				num2++;
			}
			return touches2;
		}
		touches = new UITouchInner[Input.touches.Length];
		int num3 = 0;
		Touch[] array3 = Input.touches;
		for (int k = 0; k < array3.Length; k++)
		{
			Touch touch3 = array3[k];
			touches[num3].deltaPosition = touch3.deltaPosition;
			touches[num3].deltaTime = touch3.deltaTime;
			touches[num3].fingerId = touch3.fingerId;
			touches[num3].phase = touch3.phase;
			touches[num3].position = touch3.position;
			touches[num3].tapCount = touch3.tapCount;
			num3++;
		}
		return touches;
	}
}
