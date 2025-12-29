using UnityEngine;

public class TUIControlManager : TUIContainer
{
	private TUIHandler handler;

	public new void Awake()
	{
		base.Awake();
	}

	public new void Start()
	{
		base.Start();
	}

	public void Initialize()
	{
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
	}

	public void SetHandler(TUIHandler handler)
	{
		this.handler = handler;
	}

	public override void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (handler != null)
		{
			handler.HandleEvent(control, eventType, wparam, lparam, data);
		}
	}
}
