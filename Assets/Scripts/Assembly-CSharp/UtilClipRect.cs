using System.Collections.Generic;
using UnityEngine;

public class UtilClipRect : MonoBehaviour
{
	public Camera ClipCamera;

	public int ClipLayer;

	public int ClipDepth;

	public Rect ClipRect;

	protected float ClipCenterX;

	protected float ClipCenterY;

	protected float ClipRate;

	protected Camera newCamera;

	private void Start()
	{
		ClipCenterX = (ClipRect.x + ClipRect.xMax) / 2f;
		ClipCenterY = (ClipRect.y + ClipRect.yMax) / 2f;
		newCamera = Object.Instantiate(ClipCamera) as Camera;
		newCamera.name = "ClipCamera";
		newCamera.transform.parent = ClipCamera.transform.parent;
		newCamera.cullingMask = 1 << ClipLayer;
		newCamera.depth = ClipDepth;
		newCamera.transform.localPosition += new Vector3(ClipCenterX, ClipCenterY, 0f);
		ClipRate = ClipRect.width / 480f;
		newCamera.rect = new Rect((ClipRect.x + 240f) / 480f, (ClipRect.y + 160f) / 320f, ClipRate, ClipRate);
	}

	private void Update()
	{
		UpdateClip();
	}

	private void UpdateClip()
	{
		newCamera.orthographicSize = ClipCamera.orthographicSize * ClipRate;
		List<Component> list = new List<Component>(base.gameObject.GetComponentsInChildren(typeof(Transform)));
		List<Transform> list2 = list.ConvertAll((Component c) => (Transform)c);
		foreach (Transform item in list2)
		{
			item.gameObject.layer = ClipLayer;
		}
	}
}
