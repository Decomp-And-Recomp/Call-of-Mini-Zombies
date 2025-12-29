using UnityEngine;

public class TUIButtonSliderCallBack : TUIButtonSlider
{
	public OnButtonSliderBeginDelegate Begin_delegate;

	public OnButtonSliderMoveDelegate move_delegate;

	public OnButtonSliderEndDelegate end_delegate;

	public void SimCommandDown(TUIInput input)
	{
		pressed = true;
		fingerId = input.fingerId;
		fingerPosition = input.position;
		controlPosition = base.transform.position;
		UpdateFrame();
		PostEvent(this, 1, 0f, 0f, null);
		if (Begin_delegate != null)
		{
			Begin_delegate(input);
		}
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
				if (Begin_delegate != null)
				{
					Begin_delegate(input);
				}
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
					if (move_delegate != null)
					{
						move_delegate(input);
					}
				}
			}
			else if (input.inputType == TUIInputType.Ended)
			{
				pressed = false;
				fingerId = -1;
				UpdateFrame();
				PostEvent(this, 2, 0f, 0f, null);
				if (end_delegate != null)
				{
					end_delegate(input);
				}
			}
			return true;
		}
		return false;
	}
}
