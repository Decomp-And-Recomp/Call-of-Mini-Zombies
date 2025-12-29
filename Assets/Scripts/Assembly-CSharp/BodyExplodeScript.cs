using UnityEngine;

public class BodyExplodeScript : MonoBehaviour
{
	protected Vector3[] dir = new Vector3[7];

	protected Transform[] trans;

	private void Start()
	{
		trans = GetComponentsInChildren<Transform>();
		Transform[] array = trans;
		foreach (Transform transform in array)
		{
			Debug.Log(transform.name);
		}
		for (int j = 0; j < 7; j++)
		{
			base.transform.rotation = Quaternion.AngleAxis(51.42857f, Vector3.up) * base.transform.rotation;
			dir[j] = base.transform.forward;
		}
	}

	private void Update()
	{
		for (int i = 0; i < 7; i++)
		{
			trans[i].Translate(dir[i] * Time.deltaTime, Space.World);
		}
	}
}
