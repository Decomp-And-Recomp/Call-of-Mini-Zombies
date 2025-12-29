using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prime31
{
	public class ThreadingCallbackHelper : MonoBehaviour
	{
		private Queue<Action> _actionQueue = new Queue<Action>();

		public void addActionToQueue(Action action)
		{
			lock (_actionQueue)
			{
				_actionQueue.Enqueue(action);
			}
		}

		private void Update()
		{
			lock (_actionQueue)
			{
				foreach (Action item in _actionQueue)
				{
					item();
				}
				_actionQueue.Clear();
			}
		}

		public void disableIfEmpty()
		{
			lock (_actionQueue)
			{
				if (_actionQueue.Count == 0)
				{
					base.enabled = false;
				}
			}
		}
	}
}
