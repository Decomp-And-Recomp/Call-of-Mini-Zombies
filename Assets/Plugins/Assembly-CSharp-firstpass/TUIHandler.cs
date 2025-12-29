public interface TUIHandler
{
	void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data);
}
