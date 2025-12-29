using System;
using System.Collections;
using UnityEngine;
using Zombie3D;

public class MultiReportUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	private AudioPlayer audioPlayer = new AudioPlayer();

	public GameObject Label_Time;

	public TUIMeshSprite zombieBK;

	public GameObject Report_Content;

	public GameObject Scoll_Obj;

	public GameObject Report_Panel;

	public GameObject Achievement_Panel;

	public GameObject Content_Panel;

	public GameObject Achievement_Panel_Base;

	public GameObject Achievement_Panel_Scroll;

	public GameObject OK_Button;

	public GameObject Report_Button;

	public GameObject Msg_box;

	protected MultiReportData multiReport_data;

	protected int finish_achievement_count;

	protected Vector3 button_ori_pos;

	private void Awake()
	{
		GameScript.CheckFillBlack();
	}

	private IEnumerator Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		Transform audioFolder = base.transform.Find("Audio");
		audioPlayer.AddAudio(audioFolder, "Button", true);
		audioPlayer.AddAudio(audioFolder, "Achiv", true);
		audioPlayer.AddAudio(audioFolder, "Back", true);
		GameObject dataObj = GameObject.Find("MultiReportData");
		if ((bool)dataObj)
		{
			multiReport_data = dataObj.GetComponent<MultiReportData>();
			TimeSpan ts = new TimeSpan(0, 0, (int)multiReport_data.play_time);
			SetLabelContent(Label_Time, ts.ToString());
			zombieBK.frameName_Accessor = GameApp.GetInstance().GetGameState().cur_net_map;
			int reward_cash = SceneName.GetRewardFromMap(GameApp.GetInstance().GetGameState().cur_net_map);
			int index = 0;
			foreach (string keyName in multiReport_data.userReport.Keys)
			{
				GameObject Report_Item = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/TUI/ReportItem")) as GameObject;
				if (multiReport_data.userReport[keyName] == 2 * reward_cash)
				{
					Report_Item.GetComponent<ReportItem>().SetContent(keyName.Substring(1), multiReport_data.userReport[keyName], true);
				}
				else
				{
					Report_Item.GetComponent<ReportItem>().SetContent(keyName.Substring(1), multiReport_data.userReport[keyName], false);
				}
				Report_Item.transform.parent = Report_Content.transform;
				Report_Item.transform.localPosition = new Vector3(0f, 35 - index * 40, -1f);
				index++;
			}
			yield return 1;
			if (Achievement_Panel_Base != null)
			{
				Achievement_Panel_Base.GetComponent<AchimentReportPanelBase>().InitAchivments();
			}
			multiReport_data.CheckMultiAchievementFinishStatus();
			GameApp.GetInstance().Save();
			foreach (MultiAchievementCfg achi in multiReport_data.Achievement_Finished_Array)
			{
				if ((bool)Scoll_Obj)
				{
					Scoll_Obj.GetComponent<AchimentReportPanel>().AddAchievementToList(achi);
				}
			}
			yield return 1;
			button_ori_pos = Report_Button.transform.localPosition;
			if (multiReport_data.Achievement_Finished_Array.Count > 0)
			{
				Content_Panel.GetComponent<PanelParameter>().HidePanel();
				Report_Panel.GetComponent<PanelParameter>().HidePanel();
				Achievement_Panel.GetComponent<PanelParameter>().ShowPanel();
				audioPlayer.PlayAudio("Achiv");
				OK_Button.transform.localPosition = new Vector3(button_ori_pos.x, 1000f, button_ori_pos.z);
				Report_Button.transform.localPosition = new Vector3(button_ori_pos.x, 1000f, button_ori_pos.z);
			}
			else
			{
				OK_Button.transform.localPosition = button_ori_pos;
				Report_Button.transform.localPosition = new Vector3(button_ori_pos.x, 1000f, button_ori_pos.z);
			}
			UnityEngine.Object.Destroy(dataObj);
		}
		OpenClikPlugin.Hide();
		NetworkObj.DestroyNetCom();
	}

	public void Update()
	{
		input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "OK_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			SceneName.LoadLevel("CoopHallTUI");
		}
		else if (control.name == "Report_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			Report_Button.transform.localPosition = new Vector3(button_ori_pos.x, 1000f, button_ori_pos.z);
			OK_Button.transform.localPosition = button_ori_pos;
			Report_Panel.GetComponent<PanelParameter>().ShowPanel();
			Achievement_Panel.GetComponent<PanelParameter>().HidePanel();
			Content_Panel.GetComponent<PanelParameter>().HidePanel();
		}
		else if (control.name == "Panel_Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Back");
			Content_Panel.GetComponent<PanelParameter>().HidePanel();
		}
		else if (control.name == "Achievement_Close_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			Achievement_Panel_Scroll.transform.localPosition = new Vector3(0f, 2000f, Achievement_Panel_Scroll.transform.localPosition.z);
			Report_Button.transform.localPosition = button_ori_pos;
			if (!(Achievement_Panel_Base != null))
			{
				return;
			}
			foreach (MultiAchievementCfg item in multiReport_data.Achievement_Finished_Array)
			{
				Achievement_Panel_Base.GetComponent<AchimentReportPanelBase>().AddAchivmentItemAni(item);
			}
			Achievement_Panel_Base.GetComponent<AchimentReportPanelBase>().CheckAchivmentItems();
		}
		else if (control.name.StartsWith("Achivment_Button") && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			Debug.Log(control.name + " level:" + control.gameObject.GetComponent<AchievementData>().achievement_data.level);
			ShowAchivmentContent(control.GetComponent<AchievementData>().achievement_data);
		}
		else if (control.name == "Msg_OK_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			Msg_box.GetComponent<MsgBoxDelegate>().Hide();
		}
	}

	public void SetReportTime(string str)
	{
		if ((bool)Label_Time)
		{
			TUIMeshText component = Label_Time.GetComponent<TUIMeshText>();
			if ((bool)component)
			{
				Debug.Log(str);
				component.text_Accessor = str;
			}
		}
	}

	public void SetLabelContent(GameObject label, string str)
	{
		if ((bool)label)
		{
			TUIMeshText component = label.GetComponent<TUIMeshText>();
			component.text_Accessor = str;
		}
	}

	public void ShowAchivmentContent(MultiAchievementCfg achi)
	{
		if ((bool)Content_Panel)
		{
			Content_Panel.transform.Find("PanelBK/Icon").GetComponent<TUIMeshSprite>().frameName_Accessor = achi.icon;
			string text = achi.content + "\n";
			text = text.Replace("\\n", "\n");
			int length = text.Length;
			string text2 = text;
			text = text2 + achi.finishGameCount + "/" + achi.gameCount;
			Content_Panel.transform.Find("PanelBK/Label_Content").GetComponent<TUIMeshTextMx>().ChangeColor(length, length + achi.finishGameCount.ToString().Length, Color.red);
			Content_Panel.transform.Find("PanelBK/Label_Content").GetComponent<TUIMeshTextMx>().text_Accessor = text;
			string text_Accessor = string.Empty;
			if (achi.rewardCash > 0)
			{
				text_Accessor = "REWARD: $" + achi.rewardCash;
			}
			Content_Panel.transform.Find("PanelBK/Label_Reward").GetComponent<TUIMeshText>().text_Accessor = text_Accessor;
			Content_Panel.GetComponent<PanelParameter>().ShowPanel();
		}
	}
}
