using UnityEngine;

public class Task : MonoBehaviour
{
	private enTaskState m_State;

	private object m_AttachParam;

	private object m_CallbackParam;

	private CallbackFunc m_Callback;

	private int m_iCallTimes;

	private bool m_Global;

	public object AttachParam
	{
		set
		{
			m_AttachParam = value;
		}
	}

	public object CallbackParam
	{
		set
		{
			m_CallbackParam = value;
		}
	}

	public CallbackFunc Callback
	{
		set
		{
			m_Callback = value;
		}
	}

	public enTaskState State
	{
		get
		{
			return m_State;
		}
		set
		{
			m_State = value;
		}
	}

	public int CallTimes
	{
		get
		{
			return m_iCallTimes;
		}
	}

	public bool Global
	{
		get
		{
			return m_Global;
		}
		set
		{
			m_Global = value;
		}
	}

	public void Awake()
	{
		AttachParam = null;
		CallbackParam = null;
		Callback = null;
		m_iCallTimes = 0;
		State = enTaskState.kDoing;
		m_Global = false;
	}

	protected void ExecuteCallback(bool bFinish)
	{
		if (m_Callback != null)
		{
			m_Callback(m_CallbackParam, m_AttachParam, bFinish);
			m_iCallTimes++;
		}
		State = enTaskState.kFinish;
	}
}
