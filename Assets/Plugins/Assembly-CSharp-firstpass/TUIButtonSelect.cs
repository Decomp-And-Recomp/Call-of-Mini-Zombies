public class TUIButtonSelect : TUIButtonBase
{
	public const int CommandSelect = 1;

	public void Reset()
	{
		fingerId = -1;
	}

	public virtual void SetSelected(bool selected)
	{
		pressed = selected;
		fingerId = -1;
		UpdateFrame();
	}

	public bool IsSelected()
	{
		return pressed;
	}

	public override bool HandleInput(TUIInput input)
	{
		if (input.inputType == TUIInputType.Began)
		{
			if (PtInControl(input.position))
			{
				fingerId = input.fingerId;
				return true;
			}
			return false;
		}
		if (input.fingerId == fingerId)
		{
			if (input.inputType == TUIInputType.Ended)
			{
				fingerId = -1;
				if (PtInControl(input.position) && !pressed)
				{
					pressed = true;
					UpdateFrame();
					PostEvent(this, 1, 0f, 0f, null);
				}
			}
			return true;
		}
		return false;
	}
}
