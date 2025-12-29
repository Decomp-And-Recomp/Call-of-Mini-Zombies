using UnityEngine;

public class RayCastScript : MonoBehaviour
{
	public float life;

	private void Start()
	{
	}

	private void Update()
	{
		GameObject gameObject = GameObject.Find("Begin");
		GameObject gameObject2 = GameObject.Find("End");
		Ray ray = new Ray(gameObject.transform.position, gameObject.transform.position - gameObject.transform.position);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f, 2048))
		{
			Debug.Log(string.Concat(gameObject.transform.position, ",", gameObject2.transform.position));
		}
	}

	private void OnDrawGizmos()
	{
		GameObject gameObject = GameObject.Find("Begin");
		GameObject gameObject2 = GameObject.Find("End");
		Ray ray = new Ray(gameObject.transform.position, gameObject.transform.position - gameObject.transform.position);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f, 2048))
		{
			Gizmos.DrawLine(gameObject.transform.position, gameObject2.transform.position);
		}
	}
}
