using UnityEngine;

public class TUIControlImpl : TUIControl
{
	public Vector2 size = Vector2.zero;

	public new void Awake()
	{
		base.Awake();
	}

	public new void Start()
	{
		base.Start();
	}

	public void OnDrawGizmos()
	{
		float num = base.transform.lossyScale.x * size.x / 2f;
		float num2 = base.transform.lossyScale.y * size.y / 2f;
		Vector3[] array = new Vector3[4]
		{
			base.transform.position + new Vector3(0f - num, num2, base.transform.position.z),
			base.transform.position + new Vector3(num, num2, base.transform.position.z),
			base.transform.position + new Vector3(num, 0f - num2, base.transform.position.z),
			base.transform.position + new Vector3(0f - num, 0f - num2, base.transform.position.z)
		};
		Gizmos.color = Color.white;
		Gizmos.DrawLine(array[0], array[1]);
		Gizmos.DrawLine(array[1], array[2]);
		Gizmos.DrawLine(array[2], array[3]);
		Gizmos.DrawLine(array[3], array[0]);
		Gizmos.DrawLine(array[0], array[2]);
	}

	public void OnDrawGizmosSelected()
	{
		float num = base.transform.lossyScale.x * size.x / 2f;
		float num2 = base.transform.lossyScale.y * size.y / 2f;
		Vector3[] array = new Vector3[4]
		{
			base.transform.position + new Vector3(0f - num, num2, base.transform.position.z),
			base.transform.position + new Vector3(num, num2, base.transform.position.z),
			base.transform.position + new Vector3(num, 0f - num2, base.transform.position.z),
			base.transform.position + new Vector3(0f - num, 0f - num2, base.transform.position.z)
		};
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(array[0], array[1]);
		Gizmos.DrawLine(array[1], array[2]);
		Gizmos.DrawLine(array[2], array[3]);
		Gizmos.DrawLine(array[3], array[0]);
		Gizmos.DrawLine(array[0], array[2]);
	}

	public virtual bool PtInControl(Vector2 point)
	{
		float num = base.transform.lossyScale.x * size.x / 2f;
		float num2 = base.transform.lossyScale.y * size.y / 2f;
		Vector3 position = base.transform.position;
		return point.x >= position.x - num && point.x < position.x + num && point.y >= position.y - num2 && point.y < position.y + num2;
	}
}
