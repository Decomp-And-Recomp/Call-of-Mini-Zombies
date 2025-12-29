using UnityEngine;

public class TUIScroll : TUIControlImpl
{
	public const int CommandBegin = 1;

	public const int CommandMove = 2;

	public const int CommandEnd = 3;

	public GameObject scrollObject;

	public bool scroll_flip_x;

	public bool scroll_flip_y;

	public bool is_inertance = true;

	public float rangeXMin;

	public float rangeXMax;

	public float rangeYMin;

	public float rangeYMax;

	public float borderXMin;

	public float borderXMax;

	public float borderYMin;

	public float borderYMax;

	public float[] pageX;

	public float[] pageY;

	public float thresholdX = 3f;

	public float thresholdY = 3f;

	public float inertance_val = 20f;

	public Vector2 position = Vector2.zero;

	protected int fingerId = -1;

	protected Vector2 fingerPosition = Vector2.zero;

	protected bool touch;

	protected bool move;

	protected bool scroll;

	protected Vector2 lastPosition = Vector2.zero;

	protected Vector2 moveSpeed = Vector2.zero;

	protected float pagePositionX;

	protected float pagePositionY;

	protected Vector2 eventPosition;

	public int cur_page;

	public void Update()
	{
		if (!(Time.deltaTime < 0.0001f))
		{
			if (touch)
			{
				Vector2 vector = (fingerPosition - lastPosition) / Time.deltaTime;
				lastPosition = fingerPosition;
				float num = Mathf.Clamp(Time.deltaTime / 0.5f, 0f, 1f);
				moveSpeed = moveSpeed * (1f - num) + vector * num;
			}
			if (scroll)
			{
				UpdateScroll(Time.deltaTime);
			}
		}
	}

	public override bool HandleInput(TUIInput input)
	{
		if (input.inputType == TUIInputType.Began)
		{
			if (PtInControl(input.position))
			{
				fingerId = input.fingerId;
				fingerPosition = input.position;
				touch = true;
				move = false;
				scroll = false;
				lastPosition = fingerPosition;
				moveSpeed = Vector2.zero;
			}
			return false;
		}
		if (input.fingerId != fingerId)
		{
			return false;
		}
		if (input.inputType == TUIInputType.Moved)
		{
			if (!PtInControl(input.position))
			{
				return false;
			}
			float num = input.position.x - fingerPosition.x;
			float num2 = input.position.y - fingerPosition.y;
			if (!move && (Mathf.Abs(num) > thresholdX || Mathf.Abs(num2) > thresholdY))
			{
				move = true;
				PostEvent(this, 1, 0f, 0f, null);
				ScrollObjectBegin();
			}
			if (move)
			{
				if (position.x > rangeXMax && num > 0f)
				{
					num *= 0.25f;
				}
				if (position.x < rangeXMin && num < 0f)
				{
					num *= 0.25f;
				}
				if (position.y > rangeYMax && num2 > 0f)
				{
					num2 *= 0.25f;
				}
				if (position.y < rangeYMin && num2 < 0f)
				{
					num2 *= 0.25f;
				}
				position.x += num;
				position.y += num2;
				position.x = Mathf.Clamp(position.x, borderXMin, borderXMax);
				position.y = Mathf.Clamp(position.y, borderYMin, borderYMax);
				fingerPosition = input.position;
				PostEvent(this, 2, position.x, position.y, null);
				ScrollObjectMove();
			}
		}
		else if (input.inputType == TUIInputType.Ended)
		{
			fingerId = -1;
			fingerPosition = Vector2.zero;
			touch = false;
			if (is_inertance)
			{
				StartScroll();
				if (move)
				{
					move = false;
					return true;
				}
				return false;
			}
			ScrollObjectEnd();
			return true;
		}
		return false;
	}

	protected void StartScroll()
	{
		scroll = true;
		if (pageX != null && pageX.Length > 0)
		{
			int num = 0;
			float num2 = Mathf.Abs(pageX[0] - position.x);
			for (int i = 1; i < pageX.Length; i++)
			{
				float num3 = Mathf.Abs(pageX[i] - position.x);
				if (num3 < num2)
				{
					num = i;
					num2 = num3;
				}
			}
			if (scroll_flip_x)
			{
				moveSpeed.x *= -1f;
			}
			if (cur_page == num)
			{
				if (moveSpeed.x > 50f && num + 1 < pageX.Length)
				{
					num++;
				}
				if (moveSpeed.x < -50f && num - 1 >= 0)
				{
					num--;
				}
			}
			cur_page = num;
			pagePositionX = pageX[num];
		}
		if (pageY != null && pageY.Length > 0)
		{
			int num4 = 0;
			float num5 = Mathf.Abs(pageY[0] - position.y);
			for (int j = 1; j < pageY.Length; j++)
			{
				float num6 = Mathf.Abs(pageY[j] - position.y);
				if (num6 < num5)
				{
					num4 = j;
					num5 = num6;
				}
			}
			if (scroll_flip_y)
			{
				moveSpeed.y *= -1f;
			}
			if (cur_page == num4)
			{
				if (moveSpeed.y > 50f && num4 + 1 < pageY.Length)
				{
					num4++;
				}
				if (moveSpeed.y < -50f && num4 - 1 >= 0)
				{
					num4--;
				}
			}
			cur_page = num4;
			pagePositionY = pageY[num4];
		}
		eventPosition = position;
	}

	protected void UpdateScroll(float delta_time)
	{
		bool flag = false;
		if (pageX != null && pageX.Length > 0)
		{
			float num = pagePositionX - position.x;
			if (Mathf.Abs(num) > 0.5f)
			{
				float num2 = num * inertance_val;
				position.x += num2 * delta_time;
				float num3 = pagePositionX - position.x;
				if (num * num3 <= 0f)
				{
					position.x = pagePositionX;
				}
				flag = true;
			}
			else
			{
				position.x = pagePositionX;
			}
		}
		else if (Mathf.Abs(moveSpeed.x) > 0.5f)
		{
			float num4 = 1f;
			if (position.x < rangeXMin || moveSpeed.x < 0f)
			{
				num4 = 3f;
			}
			if (position.x > rangeXMax || moveSpeed.x > 0f)
			{
				num4 = 3f;
			}
			num4 = Mathf.Clamp(num4 * delta_time, 0.01f, 0.9f);
			moveSpeed.x -= num4 * moveSpeed.x;
			position.x += moveSpeed.x * delta_time;
			if (position.x <= borderXMin)
			{
				position.x = borderXMin;
				moveSpeed.x = 0f;
			}
			if (position.x >= borderXMax)
			{
				position.x = borderXMax;
				moveSpeed.x = 0f;
			}
			if (Mathf.Abs(moveSpeed.x) < 5f)
			{
				moveSpeed.x = 0f;
			}
			flag = true;
		}
		else if (position.x < rangeXMin)
		{
			float num5 = (rangeXMin - position.x) * 1.5f;
			position.x += num5 * delta_time;
			position.x = Mathf.Clamp(position.x, borderXMin, borderXMax);
			if (Mathf.Abs(position.x - rangeXMin) < 0.5f)
			{
				position.x = rangeXMin;
			}
			else
			{
				flag = true;
			}
		}
		else if (position.x > rangeXMax)
		{
			float num6 = (rangeXMax - position.x) * 1.5f;
			position.x += num6 * delta_time;
			position.x = Mathf.Clamp(position.x, borderXMin, borderXMax);
			if (Mathf.Abs(position.x - rangeXMax) < 0.5f)
			{
				position.x = rangeXMax;
			}
			else
			{
				flag = true;
			}
		}
		if (pageY != null && pageY.Length > 0)
		{
			float num7 = pagePositionY - position.y;
			if (Mathf.Abs(num7) > 0.5f)
			{
				float num8 = num7 * inertance_val;
				position.y += num8 * delta_time;
				float num9 = pagePositionY - position.y;
				if (num7 * num9 <= 0f)
				{
					position.y = pagePositionY;
				}
				flag = true;
			}
			else
			{
				position.y = pagePositionY;
			}
		}
		else if (Mathf.Abs(moveSpeed.y) > 0.5f)
		{
			float num10 = 1f;
			if (position.y < rangeYMin || moveSpeed.y < 0f)
			{
				num10 = 3f;
			}
			if (position.y > rangeYMax || moveSpeed.y > 0f)
			{
				num10 = 3f;
			}
			num10 = Mathf.Clamp(num10 * delta_time, 0.01f, 0.9f);
			moveSpeed.y -= num10 * moveSpeed.y;
			position.y += moveSpeed.y * delta_time;
			if (position.y <= borderYMin)
			{
				position.y = borderYMin;
				moveSpeed.y = 0f;
			}
			if (position.y >= borderYMax)
			{
				position.y = borderYMax;
				moveSpeed.y = 0f;
			}
			if (Mathf.Abs(moveSpeed.y) < 5f)
			{
				moveSpeed.y = 0f;
			}
			flag = true;
		}
		else if (position.y < rangeYMin)
		{
			float num11 = (rangeYMin - position.y) * 1.5f;
			position.y += num11 * delta_time;
			position.y = Mathf.Clamp(position.y, borderYMin, borderYMax);
			if (Mathf.Abs(position.y - rangeYMin) < 0.5f)
			{
				position.y = rangeYMin;
			}
			else
			{
				flag = true;
			}
		}
		else if (position.y > rangeYMax)
		{
			float num12 = (rangeYMax - position.y) * 1.5f;
			position.y += num12 * delta_time;
			position.y = Mathf.Clamp(position.y, borderYMin, borderYMax);
			if (Mathf.Abs(position.y - rangeYMax) < 0.5f)
			{
				position.y = rangeYMax;
			}
			else
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (Mathf.Abs(eventPosition.x - position.x) >= 0.4f || Mathf.Abs(eventPosition.y - position.y) >= 0.4f)
			{
				eventPosition = position;
				PostEvent(this, 2, position.x, position.y, null);
				ScrollObjectMove();
			}
		}
		else
		{
			scroll = false;
			PostEvent(this, 2, position.x, position.y, null);
			ScrollObjectMove();
			PostEvent(this, 3, 0f, 0f, null);
			ScrollObjectEnd();
		}
	}

	protected void ScrollObjectBegin()
	{
		if ((bool)scrollObject)
		{
			scrollObject.SendMessage("OnScrollBegin", SendMessageOptions.DontRequireReceiver);
		}
	}

	protected void ScrollObjectMove()
	{
		if ((bool)scrollObject)
		{
			scrollObject.transform.localPosition = new Vector3(position.x, position.y, scrollObject.transform.localPosition.z);
			scrollObject.SendMessage("OnScrollMove", SendMessageOptions.DontRequireReceiver);
		}
	}

	protected void ScrollObjectEnd()
	{
		if ((bool)scrollObject)
		{
			scrollObject.transform.localPosition = new Vector3((int)scrollObject.transform.localPosition.x, (int)scrollObject.transform.localPosition.y, scrollObject.transform.localPosition.z);
			scrollObject.SendMessage("OnScrollEnd", SendMessageOptions.DontRequireReceiver);
		}
	}
}
