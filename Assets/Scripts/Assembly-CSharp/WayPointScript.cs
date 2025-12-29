using UnityEngine;

public class WayPointScript : MonoBehaviour
{
	public WayPointScript[] nodes;

	public WayPointScript parent;

	public float cost { get; set; }

	private void OnDrawGizmos()
	{
		if (base.transform.position.y < 10005f)
		{
			Gizmos.color = Color.white;
		}
		else
		{
			Gizmos.color = Color.magenta;
		}
		Gizmos.DrawSphere(base.transform.position, 1f);
		WayPointScript[] array = nodes;
		foreach (WayPointScript wayPointScript in array)
		{
			Gizmos.DrawLine(base.transform.position, wayPointScript.transform.position);
		}
	}
}
