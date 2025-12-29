using UnityEngine;

public class SliderManager : MonoBehaviour
{
	public TUIScroll scroll;

	public bool isVertical = true;

	public float offset;

	public float range;

	private void Start()
	{
	}

	private void Update()
	{
		if (isVertical)
		{
			float num = offset + scroll.position.y / scroll.rangeYMax * range;
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, 0f - num, base.transform.localPosition.z);
		}
		else
		{
			float x = offset + scroll.position.x / scroll.rangeXMin * range;
			base.transform.localPosition = new Vector3(x, base.transform.localPosition.y, base.transform.localPosition.z);
		}
	}
}
