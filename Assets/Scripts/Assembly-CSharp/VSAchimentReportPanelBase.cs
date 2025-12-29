using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class VSAchimentReportPanelBase : MonoBehaviour
{
	public List<GameObject> achievement_buttons = new List<GameObject>();

	public void Awake()
	{
	}

	public void Start()
	{
	}

	public void Update()
	{
	}

	public void InitAchivments()
	{
		int num = 0;
		string empty = string.Empty;
		foreach (VsAchievementCfg item in GameApp.GetInstance().GetGameConfig().Vs_AchievementConfTable)
		{
			if (item.level == 1)
			{
				GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/Achivment_Button")) as GameObject;
				gameObject.name = "Achivment_Button" + item.m_class;
				gameObject.transform.parent = base.gameObject.transform;
				gameObject.GetComponent<AchievementData>().vs_achievement_data = item;
				achievement_buttons.Add(gameObject);
				int num2 = item.icon.IndexOf('_');
				empty = item.icon.Substring(0, num2 + 1) + "0";
				gameObject.GetComponent<TUIButtonClick>().frameNormal.GetComponent<TUIMeshSprite>().frameName_Accessor = empty;
				gameObject.GetComponent<TUIButtonClick>().framePressed.GetComponent<TUIMeshSprite>().frameName_Accessor = empty;
				gameObject.GetComponent<TUIButtonClick>().frameDisabled.GetComponent<TUIMeshSprite>().frameName_Accessor = empty;
				gameObject.transform.localPosition = new Vector3(-120 + num % 4 * 80, 78 - num / 4 * 80, -1f);
				num++;
			}
		}
		foreach (VsAchievementCfg item2 in GameApp.GetInstance().GetGameConfig().Vs_AchievementConfTable)
		{
			AddAchivmentItem(item2);
		}
	}

	public GameObject FindAchivmentItem(string class_name)
	{
		GameObject result = null;
		foreach (GameObject achievement_button in achievement_buttons)
		{
			if (achievement_button.GetComponent<AchievementData>().vs_achievement_data.m_class == class_name)
			{
				result = achievement_button;
				break;
			}
		}
		return result;
	}

	public void AddAchivmentItem(VsAchievementCfg achi)
	{
		GameObject gameObject = null;
		gameObject = FindAchivmentItem(achi.m_class);
		if (gameObject != null && achi.finish)
		{
			gameObject.GetComponent<AchievementData>().vs_achievement_data = achi;
			gameObject.GetComponent<TUIButtonClick>().frameNormal.GetComponent<TUIMeshSprite>().frameName_Accessor = achi.icon;
			gameObject.GetComponent<TUIButtonClick>().framePressed.GetComponent<TUIMeshSprite>().frameName_Accessor = achi.icon;
			gameObject.GetComponent<TUIButtonClick>().frameDisabled.GetComponent<TUIMeshSprite>().frameName_Accessor = achi.icon;
		}
		else if (gameObject != null && !achi.finish && gameObject.GetComponent<AchievementData>().vs_achievement_data.finish && achi.level - gameObject.GetComponent<AchievementData>().vs_achievement_data.level == 1)
		{
			gameObject.GetComponent<AchievementData>().vs_achievement_data = achi;
		}
	}

	public void AddAchivmentItemAni(VsAchievementCfg achi)
	{
		GameObject gameObject = null;
		gameObject = FindAchivmentItem(achi.m_class);
		if (gameObject != null)
		{
			gameObject.GetComponent<AchievementData>().vs_achievement_data = achi;
			GameObject gameObject2 = Object.Instantiate(Resources.Load("Prefabs/TUI/Achivment_Effact")) as GameObject;
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = new Vector3(0f, 0f, -2f - (float)achi.level / 100f);
			gameObject2.GetComponent<AchivAnimateEff>().EffGo(achi.icon);
			gameObject2.GetComponent<AudioSource>().mute = !GameApp.GetInstance().GetGameState().SoundOn;
		}
	}

	public void CheckAchivmentItems()
	{
		foreach (VsAchievementCfg item in GameApp.GetInstance().GetGameConfig().Vs_AchievementConfTable)
		{
			GameObject gameObject = null;
			gameObject = FindAchivmentItem(item.m_class);
			if (gameObject != null && !item.finish && gameObject.GetComponent<AchievementData>().vs_achievement_data.finish && item.level - gameObject.GetComponent<AchievementData>().vs_achievement_data.level == 1)
			{
				gameObject.GetComponent<AchievementData>().vs_achievement_data = item;
			}
		}
	}
}
