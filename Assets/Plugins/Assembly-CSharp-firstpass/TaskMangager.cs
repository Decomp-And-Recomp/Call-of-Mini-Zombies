using UnityEngine;

public class TaskMangager
{
	private static TaskMangager s_intance;

	private PropUtils m_TaskMap = new PropUtils();

	public static TaskMangager Instance()
	{
		if (s_intance == null)
		{
			s_intance = new TaskMangager();
		}
		return s_intance;
	}

	public bool AddTimeTask(string taskname, float calltime, int maxCallNum, CallbackFunc func, object param, object attach, bool bGlobal)
	{
		if (m_TaskMap.GetObject(taskname) != null)
		{
			return false;
		}
		GameObject gameObject = new GameObject(taskname);
		m_TaskMap.SetProp(taskname, gameObject);
		TimeTask timeTask = gameObject.AddComponent<TimeTask>() as TimeTask;
		timeTask.Callback = func;
		timeTask.CallbackParam = param;
		timeTask.Global = bGlobal;
		timeTask.CallbackTime = calltime;
		timeTask.MaxCallTimes = maxCallNum;
		if (bGlobal)
		{
			Object.DontDestroyOnLoad(gameObject);
		}
		return true;
	}

	public Task GetTask(string taskname)
	{
		GameObject gameObject = m_TaskMap.GetGameObject(taskname);
		if (null != gameObject)
		{
			return gameObject.GetComponent("Task") as Task;
		}
		return null;
	}

	public void RemoveTask(string taskname)
	{
		GameObject gameObject = m_TaskMap.GetGameObject(taskname);
		if (null != gameObject)
		{
			m_TaskMap.RemoveProp(taskname);
			Task task = gameObject.GetComponent("Task") as Task;
			if (null != task && task.Global)
			{
				Object.Destroy(gameObject);
			}
		}
		else
		{
			Debug.Log("dont fint the Task = " + taskname + " : " + Random.Range(0, 1000));
		}
	}
}
