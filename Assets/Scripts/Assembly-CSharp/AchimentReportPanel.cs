using System.Collections.Generic;
using UnityEngine;

public class AchimentReportPanel : MonoBehaviour
{
	public GameObject scoll;

	public List<GameObject> achievement_items = new List<GameObject>();

	protected int cur_index;

	public void AddAchievementToList(MultiAchievementCfg achi)
	{
		int num = 49;
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/AchimentItemPanel")) as GameObject;
		gameObject.name = "AchimentItem_" + achi.m_class;
		gameObject.transform.parent = base.gameObject.transform;
		gameObject.GetComponent<AchievementData>().achievement_data = achi;
		GameObject gameObject2 = gameObject.transform.Find("Icon").gameObject;
		gameObject2.GetComponent<TUIMeshSprite>().frameName_Accessor = achi.icon;
		string text_Accessor = string.Empty;
		if (achi.rewardCash > 0)
		{
			text_Accessor = "REWARD:$" + achi.rewardCash;
		}
		GameObject gameObject3 = gameObject.transform.Find("Label_Reward").gameObject;
		gameObject3.GetComponent<TUIMeshText>().text_Accessor = text_Accessor;
		achievement_items.Add(gameObject);
		gameObject.transform.localPosition = new Vector3(0f, 60 - cur_index * num, 0f);
		cur_index++;
		if ((bool)scoll)
		{
			scoll.GetComponent<TUIScroll>().rangeYMax = (float)(cur_index * num) - scoll.GetComponent<TUIScroll>().size.y + 60f;
			scoll.GetComponent<TUIScroll>().borderYMax = (float)(cur_index * num) - scoll.GetComponent<TUIScroll>().size.y + 60f;
			scoll.GetComponent<TUIScroll>().rangeYMax = Mathf.Clamp(scoll.GetComponent<TUIScroll>().rangeYMax, 0f, 5000f);
			scoll.GetComponent<TUIScroll>().borderYMax = Mathf.Clamp(scoll.GetComponent<TUIScroll>().borderYMax, 0f, 5000f);
		}
	}

	public void OnScrollBegin()
	{
	}

	public void OnScrollMove()
	{
	}

	public void OnScrollEnd()
	{
	}
}
