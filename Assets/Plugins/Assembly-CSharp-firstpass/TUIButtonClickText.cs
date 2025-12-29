using UnityEngine;

public class TUIButtonClickText : TUIButtonClick
{
	public GameObject TextNormal;

	public GameObject TextPressed;

	public GameObject TextDisabled;

	public void ResetFrame()
	{
		if (pressed)
		{
			pressed = false;
			fingerId = -1;
			UpdateFrame();
		}
	}

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
}
