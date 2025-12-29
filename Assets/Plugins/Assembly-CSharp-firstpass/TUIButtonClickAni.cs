public class TUIButtonClickAni : TUIButtonClick
{
	public void SetAnimationState(bool state)
	{
		TUIMeshSpriteAnimation component = frameNormal.GetComponent<TUIMeshSpriteAnimation>();
		if (component != null)
		{
			component.is_animate = state;
		}
	}

	public override void OnButtonClick()
	{
		SetAnimationState(false);
		TUIMeshSpriteAnimation component = frameNormal.GetComponent<TUIMeshSpriteAnimation>();
		if (component != null)
		{
			component.frameName_Accessor = component.frame_array[0];
		}
	}

	public void AddAnimationFrame(int index, string frame)
	{
		TUIMeshSpriteAnimation component = frameNormal.GetComponent<TUIMeshSpriteAnimation>();
		component.frame_array[index] = frame;
	}

	public void PlayAnimation()
	{
		SetAnimationState(true);
	}
}
