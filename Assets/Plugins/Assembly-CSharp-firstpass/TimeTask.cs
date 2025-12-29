using UnityEngine;

public class TimeTask : Task
{
	private float m_fDuringTime;

	private float m_fCallBackTime;

	private int m_iMaxCallTimes;

	public float CallbackTime
	{
		set
		{
			m_fCallBackTime = value;
		}
	}

	public int MaxCallTimes
	{
		set
		{
			m_iMaxCallTimes = value;
		}
	}

	public new void Awake()
	{
		base.Awake();
		m_fDuringTime = 0f;
		m_fCallBackTime = 0f;
		m_iMaxCallTimes = 0;
	}

	public void Update()
	{
		if (base.State != enTaskState.kDoing)
		{
			return;
		}
		m_fDuringTime += Time.deltaTime;
		if (m_fDuringTime >= m_fCallBackTime)
		{
			if (m_iMaxCallTimes <= 0 || base.CallTimes < m_iMaxCallTimes)
			{
				ExecuteCallback(false);
				m_fDuringTime = 0f;
				base.State = enTaskState.kDoing;
			}
			else
			{
				ExecuteCallback(true);
			}
		}
	}
}
