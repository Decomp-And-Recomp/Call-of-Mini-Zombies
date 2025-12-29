using System;
using UnityEngine;

public class TUIRotate : TUIControlImpl
{
	public const int CommandBegin = 1;

	public const int CommandRotate = 2;

	public const int CommandEnd = 3;

	public float minRotate;

	protected int fingerId = -1;

	protected float direction;

	protected bool rotate;

	public override bool HandleInput(TUIInput input)
	{
		if (input.inputType == TUIInputType.Began)
		{
			if (PtInControl(input.position))
			{
				fingerId = input.fingerId;
				float x = input.position.x - base.transform.position.x;
				float num = input.position.y - base.transform.position.y;
				if (num >= 0f)
				{
					direction = Mathf.Atan2(num, x);
				}
				else
				{
					direction = Mathf.Atan2(num, x) + (float)Math.PI * 2f;
				}
				if (rotate)
				{
					rotate = false;
					PostEvent(this, 3, 0f, 0f, null);
				}
			}
			return false;
		}
		if (input.fingerId != fingerId)
		{
			return false;
		}
		if (!PtInControl(input.position))
		{
			return false;
		}
		if (input.inputType == TUIInputType.Moved)
		{
			float x2 = input.position.x - base.transform.position.x;
			float num2 = input.position.y - base.transform.position.y;
			float num3 = ((!(num2 >= 0f)) ? (Mathf.Atan2(num2, x2) + (float)Math.PI * 2f) : Mathf.Atan2(num2, x2));
			float num4 = num3 - direction;
			if (num4 < 0f)
			{
				num4 += (float)Math.PI * 2f;
			}
			if (num4 > (float)Math.PI)
			{
				num4 -= (float)Math.PI * 2f;
			}
			if (rotate)
			{
				direction = num3;
				PostEvent(this, 2, num4, 0f, null);
			}
			else
			{
				if (!(Mathf.Abs(num4) > minRotate))
				{
					return false;
				}
				direction = num3;
				rotate = true;
				PostEvent(this, 1, 0f, 0f, null);
				PostEvent(this, 2, num4, 0f, null);
			}
			return true;
		}
		if (input.inputType == TUIInputType.Ended)
		{
			bool flag = rotate;
			fingerId = -1;
			direction = 0f;
			rotate = false;
			if (flag)
			{
				PostEvent(this, 3, 0f, 0f, null);
				return true;
			}
			return false;
		}
		return false;
	}
}
