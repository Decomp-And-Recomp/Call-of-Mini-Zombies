using UnityEngine;

public class TimerManager
{
	protected static TimerManager instance;

	public TimerInfo[] timerInfos = new TimerInfo[100];

	public static TimerManager GetInstance()
	{
		if (instance == null)
		{
			instance = new TimerManager();
		}
		return instance;
	}

	public void SetTimer(int index, float interval, bool doAtStart)
	{
		TimerInfo timerInfo = new TimerInfo();
		if (doAtStart)
		{
			timerInfo.lastDoTime = -9999f;
		}
		else
		{
			timerInfo.lastDoTime = Time.time;
		}
		timerInfo.interval = interval;
		timerInfos[index] = timerInfo;
	}

	public void Do(int index)
	{
		timerInfos[index].lastDoTime = Time.time;
	}

	public bool Ready(int index)
	{
		if (Time.time - timerInfos[index].lastDoTime > timerInfos[index].interval)
		{
			return true;
		}
		return false;
	}
}
