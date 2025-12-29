using UnityEngine;

[ExecuteInEditMode]
public class TUIRect : MonoBehaviour
{
	public bool Static = true;

	public Rect rect = new Rect(0f, 0f, 0f, 0f);

	private Rect rectWorld = new Rect(0f, 0f, 0f, 0f);

	public void Awake()
	{
		UpdateRect();
	}

	public void Update()
	{
		if (!Static)
		{
			UpdateRect();
		}
	}

	public void UpdateRect()
	{
		Vector3 v = new Vector3(rect.xMin, rect.yMin, 0f);
		Vector3 v2 = new Vector3(rect.xMax, rect.yMax, 0f);
		v = base.transform.localToWorldMatrix.MultiplyPoint3x4(v);
		v2 = base.transform.localToWorldMatrix.MultiplyPoint3x4(v2);
		rectWorld = new Rect(v.x, v.y, v2.x - v.x, v2.y - v.y);
	}

	public Rect GetRect()
	{
		return rectWorld;
	}

	public Rect GetRectLocal(Transform x)
	{
		Vector3 v = new Vector3(rectWorld.xMin, rectWorld.yMin, 0f);
		Vector3 v2 = new Vector3(rectWorld.xMax, rectWorld.yMax, 0f);
		v = x.worldToLocalMatrix.MultiplyPoint3x4(v);
		v2 = x.worldToLocalMatrix.MultiplyPoint3x4(v2);
		return new Rect(v.x, v.y, v2.x - v.x, v2.y - v.y);
	}

	public static Rect RectIntersect(Rect rect1, Rect rect2)
	{
		float num = Mathf.Max(rect1.xMin, rect2.xMin);
		float num2 = Mathf.Min(rect1.xMax, rect2.xMax);
		float num3 = Mathf.Max(rect1.yMin, rect2.yMin);
		float num4 = Mathf.Min(rect1.yMax, rect2.yMax);
		float num5 = num2 - num;
		float num6 = num4 - num3;
		return new Rect(num, num3, (!(num5 < 0f)) ? num5 : 0f, (!(num6 < 0f)) ? num6 : 0f);
	}

	public void OnDrawGizmos()
	{
		float x = rect.xMin * base.transform.lossyScale.x;
		float x2 = rect.xMax * base.transform.lossyScale.x;
		float y = rect.yMin * base.transform.lossyScale.y;
		float y2 = rect.yMax * base.transform.lossyScale.y;
		Vector3[] array = new Vector3[4]
		{
			base.transform.position + new Vector3(x, y2, base.transform.position.z),
			base.transform.position + new Vector3(x2, y2, base.transform.position.z),
			base.transform.position + new Vector3(x2, y, base.transform.position.z),
			base.transform.position + new Vector3(x, y, base.transform.position.z)
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
		float x = rect.xMin * base.transform.lossyScale.x;
		float x2 = rect.xMax * base.transform.lossyScale.x;
		float y = rect.yMin * base.transform.lossyScale.y;
		float y2 = rect.yMax * base.transform.lossyScale.y;
		Vector3[] array = new Vector3[4]
		{
			base.transform.position + new Vector3(x, y2, base.transform.position.z),
			base.transform.position + new Vector3(x2, y2, base.transform.position.z),
			base.transform.position + new Vector3(x2, y, base.transform.position.z),
			base.transform.position + new Vector3(x, y, base.transform.position.z)
		};
		Gizmos.color = Color.green;
		Gizmos.DrawLine(array[0], array[1]);
		Gizmos.DrawLine(array[1], array[2]);
		Gizmos.DrawLine(array[2], array[3]);
		Gizmos.DrawLine(array[3], array[0]);
		Gizmos.DrawLine(array[0], array[2]);
	}
}
