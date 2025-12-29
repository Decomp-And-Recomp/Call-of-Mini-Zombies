using UnityEngine;

public class MapBorderScript : MonoBehaviour
{
	public MapBorderScript[] nodes;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		MapBorderScript[] array = nodes;
		foreach (MapBorderScript mapBorderScript in array)
		{
			Gizmos.DrawLine(base.transform.position, mapBorderScript.transform.position);
		}
	}
}
