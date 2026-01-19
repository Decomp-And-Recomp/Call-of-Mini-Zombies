using System.Collections.Generic;
using UnityEngine;

public class ScaleScroller : MonoBehaviour
{
	public float page_distance = 30f;

	public GameObject tar_transform;

	public GameObject scoll;

	public List<GameObject> item_buttons = new List<GameObject>();

	protected GameObject selected_item;

	protected bool flip_y;

	public void Awake()
	{
		if (scoll != null)
		{
			flip_y = scoll.GetComponent<TUIScroll>().scroll_flip_y;
		}
		GameObject gameObject = null;
		int num = (flip_y ? 1 : (-1));
		for (int i = 0; i < item_buttons.Count; i++)
		{
			gameObject = item_buttons[i];
			gameObject.transform.localPosition = new Vector3(0f, (float)num * page_distance * (float)i, -1f);
		}
		if (null != scoll)
		{
			if (!flip_y)
			{
				scoll.GetComponent<TUIScroll>().borderYMax = (scoll.GetComponent<TUIScroll>().rangeYMax = (float)(-num) * page_distance * (float)(item_buttons.Count - 1));
			}
			else
			{
				scoll.GetComponent<TUIScroll>().borderYMin = (scoll.GetComponent<TUIScroll>().rangeYMin = (float)(-num) * page_distance * (float)(item_buttons.Count - 1));
			}
			scoll.GetComponent<TUIScroll>().pageY = new float[item_buttons.Count];
			for (int j = 0; j < item_buttons.Count; j++)
			{
				scoll.GetComponent<TUIScroll>().pageY[j] = (float)(-num) * page_distance * (float)j;
			}
		}
		UpdateItemsPosition();
	}

	public void Start()
	{
	}

	public void Update()
	{
	}

	public void SetPositionWithItem(GameObject item)
	{
		foreach (GameObject item_button in item_buttons)
		{
			if (item == item_button)
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.z, 0f - item.transform.localPosition.y, base.transform.localPosition.z);
				if ((bool)scoll)
				{
					scoll.GetComponent<TUIScroll>().position = new Vector2(0f, 0f - item.transform.localPosition.y);
				}
				UpdateItemsPosition();
				break;
			}
		}
	}

	public void UpdateItemsPosition()
	{
		int num = Mathf.RoundToInt(Mathf.Abs(base.transform.localPosition.y - tar_transform.transform.localPosition.y) / page_distance);
		if (num < item_buttons.Count)
		{
			selected_item = item_buttons[num];
			selected_item.transform.localPosition = new Vector3(selected_item.transform.localPosition.x, selected_item.transform.localPosition.y, tar_transform.transform.position.z - 1f);
			selected_item.GetComponent<TUIMeshSprite>().color_Accessor = new Color(1f, 1f, 1f, 1f);
			selected_item.transform.localScale = new Vector3(1f, 1f, 1f);
			for (int i = 0; i < selected_item.transform.childCount; i++)
			{
				if ((bool)selected_item.transform.GetChild(i).gameObject.GetComponent<TUIMeshSprite>())
				{
					selected_item.transform.GetChild(i).gameObject.GetComponent<TUIMeshSprite>().color_Accessor = new Color(1f, 1f, 1f, 1f);
				}
			}
			GameObject gameObject = null;
			float num2 = 0f;
			float num3 = 0f;
			for (int j = 0; j < item_buttons.Count; j++)
			{
				if (j == num)
				{
					continue;
				}
				gameObject = item_buttons[j];
				num2 = Mathf.Abs(j - num);
				gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, tar_transform.transform.position.z + num2);
				gameObject.GetComponent<TUIMeshSprite>().color_Accessor = new Color(0.3f, 0.3f, 0.3f, 1f);
				num3 = Mathf.Abs(gameObject.transform.position.y - tar_transform.transform.position.y) / 300f;
				gameObject.transform.localScale = new Vector3(1f - num3, 1f - num3, 1f);
				for (int k = 0; k < gameObject.transform.childCount; k++)
				{
					if ((bool)gameObject.transform.GetChild(k).gameObject.GetComponent<TUIMeshSprite>())
					{
						gameObject.transform.GetChild(k).gameObject.GetComponent<TUIMeshSprite>().color_Accessor = new Color(0.3f, 0.3f, 0.3f, 1f);
					}
				}
			}
		}
		else
		{
			Debug.Log("out border~~");
		}
	}

	public void OnScrollBegin()
	{
		UpdateItemsPosition();
	}

	public void OnScrollMove()
	{
		UpdateItemsPosition();
	}

	public void OnScrollEnd()
	{
		UpdateItemsPosition();
	}

	public GameObject GetSelectedButton()
	{
		return selected_item;
	}
}
