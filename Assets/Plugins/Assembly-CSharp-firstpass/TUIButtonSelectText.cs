using UnityEngine;

public class TUIButtonSelectText : TUIButtonSelect
{
	public GameObject TextNormal;

	public GameObject TextPressed;

	public GameObject TextDisabled;

	public OnButtonButtonSelectTrueDelegate select_true_delegate;

	public OnButtonButtonSelectFalseDelegate select_false_delegate;

	protected override void HideFrame()
	{
		base.HideFrame();
		HideText();
	}

	protected void HideText()
	{
		if ((bool)TextNormal)
		{
			TextNormal.SetActive(false);
		}
		if ((bool)TextPressed)
		{
			TextPressed.SetActive(false);
		}
		if ((bool)TextDisabled)
		{
			TextDisabled.SetActive(false);
		}
	}

	protected override void ShowFrame()
	{
		base.ShowFrame();
		ShowText();
	}

	protected void ShowText()
	{
		if (disabled)
		{
			if ((bool)TextDisabled)
			{
				TextDisabled.SetActive(true);
			}
		}
		else if (pressed)
		{
			if ((bool)TextPressed)
			{
				TextPressed.SetActive(true);
			}
		}
		else if ((bool)TextNormal)
		{
			TextNormal.SetActive(true);
		}
	}

	public override void SetSelected(bool selected)
	{
		pressed = selected;
		fingerId = -1;
		UpdateFrame();
		if (selected)
		{
			if (select_true_delegate != null)
			{
				select_true_delegate();
			}
		}
		else if (select_false_delegate != null)
		{
			select_false_delegate();
		}
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
					if (!disabled && select_true_delegate != null)
					{
						select_true_delegate();
					}
				}
			}
			return true;
		}
		return false;
	}
}
