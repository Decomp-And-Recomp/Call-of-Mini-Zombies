namespace TNetSdk
{
	public interface TDispatchable
	{
		TEventDispatcher Dispatcher { get; }

		void AddEventListener(TNetEventSystem eventType, EventListenerDelegate listener);

		void AddEventListener(TNetEventRoom eventType, EventListenerDelegate listener);
	}
}
