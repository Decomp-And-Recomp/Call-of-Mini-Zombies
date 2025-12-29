public class TUIButtonPush : TUIButtonBase
{
	public const int CommandDown = 1;

	public const int CommandUp = 2;

	public void Reset()
	{
		fingerId = -1;
	}

	public void SetPressed(bool pressed)
	{
		base.pressed = pressed;
		fingerId = -1;
		UpdateFrame();
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
				if (PtInControl(input.position))
				{
					if (!pressed)
					{
						pressed = true;
						UpdateFrame();
						PostEvent(this, 1, 0f, 0f, null);
					}
					else if (pressed)
					{
						pressed = false;
						UpdateFrame();
						PostEvent(this, 2, 0f, 0f, null);
					}
				}
			}
			return true;
		}
		return false;
	}
}
