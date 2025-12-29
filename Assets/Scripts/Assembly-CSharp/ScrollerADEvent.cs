using UnityEngine;

public class ScrollerADEvent : MonoBehaviour
{
	public TUIScroll scroll;

	public GameObject[] labels;

	public VSAdUI adTUI;

	public void Start()
	{
	}

	private void OnScrollEnd()
	{
		if (adTUI != null)
		{
			for (int i = 0; i < adTUI.allPages.Length - 1; i++)
			{
				if (i == scroll.cur_page)
				{
					adTUI.allPages[i].transform.Find("arrow").gameObject.SetActive(true);
				}
				else
				{
					adTUI.allPages[i].transform.Find("arrow").gameObject.SetActive(false);
				}
			}
			return;
		}
		for (int j = 0; j < labels.Length; j++)
		{
			if (j == scroll.cur_page)
			{
				labels[j].transform.Find("label_active").gameObject.SetActive(true);
				labels[j].transform.Find("label_deactive").gameObject.SetActive(false);
			}
			else
			{
				labels[j].transform.Find("label_active").gameObject.SetActive(false);
				labels[j].transform.Find("label_deactive").gameObject.SetActive(true);
			}
		}
	}

	public void OnAutoScrollEnd(int currentPage)
	{
		for (int i = 0; i < labels.Length; i++)
		{
			if (i == currentPage)
			{
				labels[i].transform.Find("label_active").gameObject.SetActive(true);
				labels[i].transform.Find("label_deactive").gameObject.SetActive(false);
			}
			else
			{
				labels[i].transform.Find("label_active").gameObject.SetActive(false);
				labels[i].transform.Find("label_deactive").gameObject.SetActive(true);
			}
		}
	}
}
