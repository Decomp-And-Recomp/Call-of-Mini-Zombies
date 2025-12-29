using UnityEngine;

public class FireLineScript : MonoBehaviour
{
	public Vector3 beginPos;

	public Vector3 endPos;

	public float speed;

	protected float startTime;

	protected float interval_time;

	private void Start()
	{
	}

	public void Reset()
	{
		startTime = Time.time;
		interval_time = (endPos - beginPos).magnitude / speed;
	}

	private void Update()
	{
		base.transform.Translate(speed * (endPos - beginPos).normalized * Time.deltaTime, Space.World);
		if (Time.time - startTime >= interval_time)
		{
			base.gameObject.SetActive(false);
		}
	}
}
