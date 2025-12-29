public class TUIButtonSelectGroup : TUIContainer
{
	private TUIButtonSelect[] buttonSelectGroup;

	public new void Awake()
	{
		buttonSelectGroup = base.gameObject.GetComponentsInChildren<TUIButtonSelect>(false);
		base.Awake();
	}

	public override void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		bool flag = false;
		for (int i = 0; i < buttonSelectGroup.Length; i++)
		{
			if (buttonSelectGroup[i] == control && eventType == 1)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			for (int j = 0; j < buttonSelectGroup.Length; j++)
			{
				if (buttonSelectGroup[j] != control && buttonSelectGroup[j].IsSelected())
				{
					buttonSelectGroup[j].SetSelected(false);
				}
			}
		}
		base.HandleEvent(control, eventType, wparam, lparam, data);
	}
}
