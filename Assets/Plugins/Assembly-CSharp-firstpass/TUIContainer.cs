using System;
using System.Collections.Generic;
using UnityEngine;

public class TUIContainer : TUIControl
{
	public new void Awake()
	{
		base.Awake();
	}

	public new void Start()
	{
		base.Start();
	}

	public T GetControl<T>(string name) where T : TUIControl
	{
		Transform transform = base.transform.Find(name);
		if (!transform)
		{
			return (T)null;
		}
		return transform.gameObject.GetComponent<T>();
	}

	public override bool HandleInput(TUIInput input)
	{
		List<TUIControl> list = new List<TUIControl>();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			TUIControl component = base.transform.GetChild(i).gameObject.GetComponent<TUIControl>();
			if ((bool)component && component.gameObject.activeInHierarchy && component.enabled)
			{
				list.Add(component);
			}
		}
		TUIControl[] array = list.ToArray();
		Array.Sort(array, CompareControl);
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].HandleInput(input))
			{
				return true;
			}
		}
		return base.HandleInput(input);
	}

	public virtual void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		PostEvent(control, eventType, wparam, lparam, data);
	}

	private static int CompareControl(TUIControl l, TUIControl r)
	{
		if (l.transform.position.z < r.transform.position.z)
		{
			return -1;
		}
		if (l.transform.position.z > r.transform.position.z)
		{
			return 1;
		}
		return 0;
	}
}
