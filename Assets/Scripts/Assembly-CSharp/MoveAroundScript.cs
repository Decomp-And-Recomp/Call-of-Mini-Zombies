using UnityEngine;

public class MoveAroundScript : MonoBehaviour
{
	public Transform pointA;

	public Transform pointB;

	public Transform target;

	private void Start()
	{
	}

	private void Update()
	{
		if ((double)(base.transform.position - pointA.position).sqrMagnitude < 1.0)
		{
			target = pointB;
		}
		else if ((double)(base.transform.position - pointB.position).sqrMagnitude < 1.0)
		{
			target = pointA;
		}
		Vector3 normalized = (target.position - base.transform.position).normalized;
		base.transform.Translate(normalized * Time.deltaTime * 10f);
	}
}
