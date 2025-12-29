public interface UIContainer
{
	void DrawSprite(UISprite sprite);

	void SendEvent(UIControl control, int command, float wparam, float lparam);

	void Add(UIControl control);
}
