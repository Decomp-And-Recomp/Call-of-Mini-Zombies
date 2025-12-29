using UnityEngine;

public class TUIButtonSlider : TUIButtonBase
{
	public const int CommandDown = 1;

	public const int CommandUp = 2;

	public const int CommandSlider = 3;

	public TUIRect sliderRect;

	protected Vector2 fingerPosition = Vector2.zero;

	protected Vector3 controlPosition = Vector3.zero;

	public void Reset()
	{
		pressed = false;
		fingerId = -1;
		UpdateFrame();
	}

	public override bool HandleInput(TUIInput input)
	{
		if (disabled)
		{
			return false;
		}
		if (input.inputType == TUIInputType.Began)
		{
			if (PtInControl(input.position))
			{
				pressed = true;
				fingerId = input.fingerId;
				fingerPosition = input.position;
				controlPosition = base.transform.position;
				UpdateFrame();
				PostEvent(this, 1, 0f, 0f, null);
				return true;
			}
			return false;
		}
		if (input.fingerId == fingerId)
		{
			if (input.inputType == TUIInputType.Moved)
			{
				if (sliderRect != null)
				{
					Vector3 position = controlPosition + new Vector3(input.position.x - fingerPosition.x, input.position.y - fingerPosition.y, 0f);
					Rect rect = sliderRect.GetRect();
					position.x = Mathf.Clamp(position.x, rect.xMin, rect.xMax);
					position.y = Mathf.Clamp(position.y, rect.yMin, rect.yMax);
					float wparam = 0f;
					float lparam = 0f;
					if (rect.width > 0f)
					{
						wparam = (position.x - rect.x) / rect.width;
					}
					if (rect.height > 0f)
					{
						lparam = (position.y - rect.y) / rect.height;
					}
					base.transform.position = position;
					PostEvent(this, 3, wparam, lparam, null);
				}
			}
			else if (input.inputType == TUIInputType.Ended)
			{
				pressed = false;
				fingerId = -1;
				UpdateFrame();
				PostEvent(this, 2, 0f, 0f, null);
			}
			return true;
		}
		return false;
	}
}
