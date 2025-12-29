using UnityEngine;

public class TUIControl : MonoBehaviour
{
	public void Awake()
	{
	}

	public void Start()
	{
	}

	public virtual bool HandleInput(TUIInput input)
	{
		return false;
	}

	public virtual void PostEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		TUIContainer component = base.transform.parent.gameObject.GetComponent<TUIContainer>();
		if ((bool)component)
		{
			component.HandleEvent(control, eventType, wparam, lparam, data);
		}
	}
}
