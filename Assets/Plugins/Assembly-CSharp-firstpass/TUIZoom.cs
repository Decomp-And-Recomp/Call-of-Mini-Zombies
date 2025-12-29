using UnityEngine;

public class TUIZoom : TUIControlImpl
{
	protected struct TouchInfo
	{
		public int fingerId;

		public Vector2 position;
	}

	public const int CommandBegin = 1;

	public const int CommandZoom = 2;

	public const int CommandEnd = 3;

	protected TouchInfo[] touchInfo = new TouchInfo[2];

	protected int fingerIndex;

	protected float distance;

	protected bool zoom;

	public void Reset()
	{
		touchInfo[0].fingerId = -1;
		touchInfo[1].fingerId = -1;
		fingerIndex = 0;
		distance = 0f;
		zoom = false;
	}

	public override bool HandleInput(TUIInput input)
	{
		if (input.inputType == TUIInputType.Began)
		{
			if (PtInControl(input.position))
			{
				touchInfo[fingerIndex].fingerId = input.fingerId;
				touchInfo[fingerIndex].position = input.position;
				if (fingerIndex == 0)
				{
					fingerIndex = 1;
				}
				else
				{
					fingerIndex = 0;
				}
				if (touchInfo[0].fingerId != -1 && touchInfo[1].fingerId != -1)
				{
					if (zoom)
					{
						zoom = false;
						PostEvent(this, 3, 0f, 0f, null);
					}
					distance = (touchInfo[0].position - touchInfo[1].position).magnitude;
					zoom = true;
					PostEvent(this, 1, 0f, 0f, null);
				}
			}
			return false;
		}
		if (input.inputType == TUIInputType.Moved)
		{
			if (touchInfo[0].fingerId == -1 || touchInfo[1].fingerId == -1)
			{
				return false;
			}
			if (!PtInControl(input.position))
			{
				return false;
			}
			if (touchInfo[0].fingerId == input.fingerId)
			{
				touchInfo[0].position = input.position;
			}
			else
			{
				if (touchInfo[1].fingerId != input.fingerId)
				{
					return false;
				}
				touchInfo[1].position = input.position;
			}
			float magnitude = (touchInfo[0].position - touchInfo[1].position).magnitude;
			float wparam = magnitude - distance;
			distance = magnitude;
			PostEvent(this, 2, wparam, 0f, null);
			return true;
		}
		if (input.inputType == TUIInputType.Ended)
		{
			bool result = false;
			for (int i = 0; i < 2; i++)
			{
				if (touchInfo[i].fingerId == input.fingerId)
				{
					touchInfo[i].fingerId = -1;
					fingerIndex = i;
					if (zoom)
					{
						zoom = false;
						PostEvent(this, 3, 0f, 0f, null);
						result = true;
					}
				}
			}
			return result;
		}
		return false;
	}
}
