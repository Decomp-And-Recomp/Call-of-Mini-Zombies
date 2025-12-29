public class TUIButtonClickTextAni : TUIButtonClickText
{
	public void SetAnimationState(bool state)
	{
		TUIMeshSpriteAnimation component = frameNormal.GetComponent<TUIMeshSpriteAnimation>();
		if (component != null)
		{
			component.is_animate = state;
		}
		TUIMeshTextAnimation component2 = TextNormal.GetComponent<TUIMeshTextAnimation>();
		if (component2 != null)
		{
			component2.is_animate = state;
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
		TUIMeshTextAnimation component2 = TextNormal.GetComponent<TUIMeshTextAnimation>();
		if (component2 != null)
		{
			component2.color = component2.color_start;
		}
	}
}
