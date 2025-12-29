using System;
using System.Collections;

namespace TNetSdk
{
	public class TEventDispatcher
	{
		private TNetObject target;

		private Hashtable listeners_sys = new Hashtable();

		private Hashtable listeners_room = new Hashtable();

		public TEventDispatcher(TNetObject target)
		{
			this.target = target;
		}

		public void AddEventListener(TNetEventSystem eventType, EventListenerDelegate listener)
		{
			EventListenerDelegate a = listeners_sys[eventType] as EventListenerDelegate;
			a = (EventListenerDelegate)Delegate.Combine(a, listener);
			listeners_sys[eventType] = a;
		}

		public void RemoveEventListener(TNetEventSystem eventType, EventListenerDelegate listener)
		{
			EventListenerDelegate eventListenerDelegate = listeners_sys[eventType] as EventListenerDelegate;
			if (eventListenerDelegate != null)
			{
				eventListenerDelegate = (EventListenerDelegate)Delegate.Remove(eventListenerDelegate, listener);
			}
			listeners_sys[eventType] = eventListenerDelegate;
		}

		public void AddEventListener(TNetEventRoom eventType, EventListenerDelegate listener)
		{
			EventListenerDelegate a = listeners_room[eventType] as EventListenerDelegate;
			a = (EventListenerDelegate)Delegate.Combine(a, listener);
			listeners_room[eventType] = a;
		}

		public void RemoveEventListener(TNetEventRoom eventType, EventListenerDelegate listener)
		{
			EventListenerDelegate eventListenerDelegate = listeners_room[eventType] as EventListenerDelegate;
			if (eventListenerDelegate != null)
			{
				eventListenerDelegate = (EventListenerDelegate)Delegate.Remove(eventListenerDelegate, listener);
			}
			listeners_room[eventType] = eventListenerDelegate;
		}

		public void DispatchEvent(PROTOCOLS protocol, CMD cmd, Packet packet)
		{
			switch (protocol)
			{
			case PROTOCOLS.version:
			{
				TNetEventData tEvent2 = TNetEventData.CreateDataWithPacket((TNetEventSystem)cmd, packet, target);
				EventListenerDelegate eventListenerDelegate2 = listeners_sys[(TNetEventSystem)cmd] as EventListenerDelegate;
				if (eventListenerDelegate2 != null)
				{
					eventListenerDelegate2(tEvent2);
				}
				break;
			}
			case PROTOCOLS.room:
			{
				TNetEventData tEvent = TNetEventData.CreateDataWithPacket((TNetEventRoom)cmd, packet, target);
				EventListenerDelegate eventListenerDelegate = listeners_room[(TNetEventRoom)cmd] as EventListenerDelegate;
				if (eventListenerDelegate != null)
				{
					eventListenerDelegate(tEvent);
				}
				break;
			}
			}
		}

		public void RemoveAll()
		{
			listeners_sys.Clear();
			listeners_room.Clear();
		}
	}
}
