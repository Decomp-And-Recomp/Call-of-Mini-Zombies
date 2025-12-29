using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class VSAchimentReportPanel : MonoBehaviour
{
	public GameObject scoll;

	public List<GameObject> achievement_items = new List<GameObject>();

	protected int cur_index;

	public void Awake()
	{
	}

	public void Start()
	{
	}

	public void Update()
	{
	}

	public void AddAchievementToList(VsAchievementCfg achi)
	{
		int num = 49;
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/AchimentItemPanel")) as GameObject;
		gameObject.name = "AchimentItem_" + achi.m_class;
		gameObject.transform.parent = base.gameObject.transform;
		gameObject.GetComponent<AchievementData>().vs_achievement_data = achi;
		GameObject gameObject2 = gameObject.transform.Find("Icon").gameObject;
		gameObject2.GetComponent<TUIMeshSprite>().frameName_Accessor = achi.icon;
		string text = string.Empty;
		if (achi.reward_cash > 0)
		{
			text = "REWARD:$" + achi.reward_cash;
		}
		if (achi.reward_avata != AvatarType.None)
		{
			text = ((text.Length <= 1) ? (text + "REWARD:" + achi.reward_avata) : (text + "     REWARD:" + achi.reward_avata));
		}
		if (achi.reward_weapon != "none")
		{
			text = ((text.Length <= 1) ? (text + "REWARD:UNLOCK " + achi.reward_weapon.ToString()) : (text + "     REWARD:UNLOCK " + achi.reward_weapon.ToString()));
		}
		GameObject gameObject3 = gameObject.transform.Find("Label_Reward").gameObject;
		gameObject3.GetComponent<TUIMeshText>().text_Accessor = text;
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
