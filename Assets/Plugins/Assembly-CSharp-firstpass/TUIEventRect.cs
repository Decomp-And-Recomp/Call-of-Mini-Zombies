using UnityEngine;

public class TUIEventRect : TUIControlImpl
{
	public const int CommandBegin = 1;

	public const int CommandMove = 2;

	public const int CommandEnd = 3;

	public float minX;

	public float minY;

	protected int fingerId = -1;

	protected Vector2 position = Vector2.zero;

	public override bool HandleInput(TUIInput input)
	{
		if (input.inputType == TUIInputType.Began)
		{
			if (PtInControl(input.position))
			{
				PostEvent(this, 1, 0f, 0f, input);
				fingerId = input.fingerId;
				position = input.position;
				return true;
			}
			return false;
		}
		if (input.fingerId != fingerId)
		{
			return false;
		}
		if (input.inputType == TUIInputType.Moved)
		{
			PostEvent(this, 2, 0f, 0f, input);
		}
		else if (input.inputType == TUIInputType.Ended)
		{
			fingerId = -1;
			position = Vector2.zero;
			PostEvent(this, 3, 0f, 0f, input);
		}
		return false;
	}
}
