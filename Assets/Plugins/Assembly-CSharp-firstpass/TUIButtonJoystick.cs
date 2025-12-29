using System;
using UnityEngine;

public class TUIButtonJoystick : TUIButtonBase
{
	public const int CommandDown = 1;

	public const int CommandMove = 2;

	public const int CommandUp = 3;

	public GameObject m_JoyStickObj;

	public float m_MinDistance;

	public float m_MaxDistance;

	private float m_Direction;

	private float m_Distance;

	public override bool HandleInput(TUIInput input)
	{
		if (disabled)
		{
			return false;
		}
		if (input.inputType == TUIInputType.Began)
		{
			if (PtInControl(input.position))
			{
				pressed = true;
				fingerId = input.fingerId;
				float num = input.position.x - base.transform.position.x;
				float num2 = input.position.y - base.transform.position.y;
				m_Direction = ((!(num2 >= 0f)) ? (Mathf.Atan2(num2, num) + (float)Math.PI * 2f) : Mathf.Atan2(num2, num));
				m_Distance = Mathf.Sqrt(num * num + num2 * num2);
				if (m_Distance > m_MaxDistance)
				{
					m_Distance = m_MaxDistance;
				}
				float wparam = (m_Distance - m_MinDistance) / (m_MaxDistance - m_MinDistance);
				UpdateFrame();
				PostEvent(this, 1, wparam, m_Direction, null);
				return true;
			}
		}
		else if (input.fingerId == fingerId)
		{
			if (input.inputType == TUIInputType.Moved)
			{
				float num3 = input.position.x - base.transform.position.x;
				float num4 = input.position.y - base.transform.position.y;
				m_Direction = ((!(num4 >= 0f)) ? (Mathf.Atan2(num4, num3) + (float)Math.PI * 2f) : Mathf.Atan2(num4, num3));
				m_Distance = Mathf.Sqrt(num3 * num3 + num4 * num4);
				if (m_Distance > m_MaxDistance)
				{
					m_Distance = m_MaxDistance;
				}
				float wparam2 = (m_Distance - m_MinDistance) / (m_MaxDistance - m_MinDistance);
				UpdateFrame();
				PostEvent(this, 2, wparam2, m_Direction, null);
			}
			else if (input.inputType == TUIInputType.Ended)
			{
				pressed = false;
				fingerId = -1;
				m_Direction = 0f;
				m_Distance = 0f;
				UpdateFrame();
				PostEvent(this, 3, 0f, 0f, null);
			}
		}
		return false;
	}

	protected override void UpdateFrame()
	{
		base.UpdateFrame();
		if (null != m_JoyStickObj)
		{
			Vector3 localPosition = m_JoyStickObj.transform.localPosition;
			localPosition = new Vector3(m_Distance * Mathf.Cos(m_Direction), m_Distance * Mathf.Sin(m_Direction), localPosition.z);
			m_JoyStickObj.transform.localPosition = localPosition;
		}
	}
}
