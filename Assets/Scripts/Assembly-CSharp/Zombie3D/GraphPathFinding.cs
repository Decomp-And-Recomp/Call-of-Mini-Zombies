using System.Collections.Generic;
using UnityEngine;

namespace Zombie3D
{
	public class GraphPathFinding : IPathFinding
	{
		protected List<WayPointScript> openStack = new List<WayPointScript>();

		protected List<WayPointScript> closeStack = new List<WayPointScript>();

		protected List<Transform> path;

		protected GameObject[] points;

		public void InitPath(GameObject[] scene_points)
		{
			points = scene_points;
		}

		public void FindPath(Vector3 enemyPos, Vector3 playerPos)
		{
			float num = float.MaxValue;
			WayPointScript wayPointScript = null;
			WayPointScript wayPointScript2 = GameApp.GetInstance().GetGameScene().GetPlayer()
				.NearestWayPoint();
			GameObject[] array = points;
			foreach (GameObject gameObject in array)
			{
				WayPointScript component = gameObject.GetComponent<WayPointScript>();
				component.parent = null;
				float magnitude = (component.transform.position - enemyPos).magnitude;
				if (magnitude < num)
				{
					Ray ray = new Ray(enemyPos + new Vector3(0f, 0.5f, 0f), component.transform.position - enemyPos);
					RaycastHit hitInfo;
					if (!Physics.Raycast(ray, out hitInfo, magnitude, 100352))
					{
						wayPointScript = component;
						num = magnitude;
					}
				}
			}
			if (wayPointScript != null && wayPointScript2 != null)
			{
				path = SearchPath(wayPointScript, wayPointScript2);
			}
			if (wayPointScript2 == null)
			{
				Debug.Log("to null");
			}
			if (wayPointScript == null)
			{
				Debug.Log("from null");
			}
		}

		public Transform GetNextWayPoint(Vector3 enemyPos, Vector3 playerPos)
		{
			if (path != null && path.Count > 0)
			{
				return path[path.Count - 1];
			}
			path = null;
			FindPath(enemyPos, playerPos);
			if (path != null && path.Count > 0)
			{
				return path[path.Count - 1];
			}
			return null;
		}

		public void PopNode()
		{
			if (path != null && path.Count > 0)
			{
				path.RemoveAt(path.Count - 1);
			}
		}

		public bool HavePath()
		{
			if (path != null && path.Count > 0)
			{
				return true;
			}
			return false;
		}

		public void ClearPath()
		{
			if (path != null)
			{
				path.Clear();
			}
		}

		public Transform PeekPath()
		{
			if (path != null && path.Count > 0)
			{
				return path[path.Count - 1];
			}
			return null;
		}

		public List<Transform> SearchPath(WayPointScript from, WayPointScript to)
		{
			List<Transform> list = new List<Transform>();
			openStack.Clear();
			closeStack.Clear();
			from.cost = 0f;
			openStack.Add(from);
			while (openStack.Count > 0)
			{
				WayPointScript wayPointScript = openStack[openStack.Count - 1];
				openStack.RemoveAt(openStack.Count - 1);
				closeStack.Add(wayPointScript);
				if (wayPointScript.transform == to.transform)
				{
					break;
				}
				WayPointScript[] nodes = wayPointScript.nodes;
				foreach (WayPointScript wayPointScript2 in nodes)
				{
					float num = wayPointScript.cost + (wayPointScript2.transform.position - wayPointScript.transform.position).magnitude + (to.transform.position - wayPointScript.transform.position).magnitude;
					if (closeStack.Contains(wayPointScript2))
					{
						if (wayPointScript2.cost > num)
						{
							wayPointScript2.cost = num;
							wayPointScript2.parent = wayPointScript;
						}
						continue;
					}
					if (openStack.Contains(wayPointScript2))
					{
						if (wayPointScript2.cost > num)
						{
							wayPointScript2.cost = num;
							wayPointScript2.parent = wayPointScript;
						}
						continue;
					}
					wayPointScript2.cost = num;
					wayPointScript2.parent = wayPointScript;
					openStack.Add(wayPointScript2);
					openStack.Sort((WayPointScript a, WayPointScript b) => b.cost.CompareTo(a.cost));
				}
			}
			WayPointScript wayPointScript3 = to;
			list.Add(to.transform);
			while (wayPointScript3.parent != null)
			{
				wayPointScript3 = wayPointScript3.parent;
				if (list.Count > 30)
				{
					Debug.Log("Memeroy Explode! Parent Forever..");
					Debug.Break();
					break;
				}
				list.Add(wayPointScript3.transform);
			}
			return list;
		}
	}
}
