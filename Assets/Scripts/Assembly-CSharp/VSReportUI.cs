using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class VSReportUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	private AudioPlayer audioPlayer = new AudioPlayer();

	public GameObject ReportPanel;

	public GameObject Report_Content;

	public GameObject AchiReportPanel_Get;

	public GameObject AchiReportPanel_Base;

	public GameObject AchiReportPanel;

	public GameObject AchiReportPanel_Content;

	public GameObject Scoll_Obj;

	public GameObject OK_button;

	public GameObject Continue_button;

	protected VSReportData report_data;

	protected List<VSPlayerReport> sorted_report_data = new List<VSPlayerReport>();

	protected List<VSPlayerReport> unsorted_report_data = new List<VSPlayerReport>();

	private Vector3 hide_pos = new Vector3(0f, -2000f, -8f);

	private Vector3 show_pos = new Vector3(0f, -136f, -8f);

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
		GameObject report_obj = GameObject.Find("VSReprotObj");
		if ((bool)report_obj)
		{
			report_data = report_obj.GetComponent<VSReportData>();
			foreach (VSPlayerReport report_tem in report_data.player_reports)
			{
				unsorted_report_data.Add(report_tem);
			}
			while (unsorted_report_data.Count > 0)
			{
				VSPlayerReport data = FindMaxOneReportData();
				unsorted_report_data.Remove(data);
				sorted_report_data.Add(data);
			}
			GameObject VSReportItem2 = null;
			for (int i = 0; i < sorted_report_data.Count; i++)
			{
				VSReportItem2 = Object.Instantiate(Resources.Load("Prefabs/TUI/VSReportItem")) as GameObject;
				VSReportItem2.transform.parent = Report_Content.transform;
				VSReportItem2.transform.localPosition = new Vector3(0f, 34 - i * 30, VSReportItem2.transform.localPosition.z);
				if (i == 0 && (sorted_report_data[i].kill_cout != 0 || sorted_report_data[i].death_count != 0))
				{
					VSReportItem2.GetComponent<ReportItem>().SetContent(sorted_report_data[i].nick_name, sorted_report_data[i].kill_cout, sorted_report_data[i].death_count, sorted_report_data[i].loot_cash, sorted_report_data[i].combo_kill, true);
				}
				else
				{
					VSReportItem2.GetComponent<ReportItem>().SetContent(sorted_report_data[i].nick_name, sorted_report_data[i].kill_cout, sorted_report_data[i].death_count, sorted_report_data[i].loot_cash, sorted_report_data[i].combo_kill, false);
				}
			}
			yield return 1;
			if (AchiReportPanel_Base != null)
			{
				AchiReportPanel_Base.GetComponent<VSAchimentReportPanelBase>().InitAchivments();
			}
			report_data.CheckMultiAchievementFinishStatus();
			report_data.GiveReward();
			GameApp.GetInstance().Save();
			foreach (VsAchievementCfg achi in report_data.Achievement_Finished_Array)
			{
				if ((bool)Scoll_Obj)
				{
					Scoll_Obj.GetComponent<VSAchimentReportPanel>().AddAchievementToList(achi);
				}
			}
			yield return 1;
			if (report_data.Achievement_Finished_Array.Count > 0)
			{
				ReportPanel.GetComponent<PanelParameter>().HidePanel();
				AchiReportPanel_Content.GetComponent<PanelParameter>().HidePanel();
				AchiReportPanel.GetComponent<PanelParameter>().ShowPanel();
				audioPlayer.PlayAudio("Achiv");
				OK_button.transform.localPosition = hide_pos;
				Continue_button.transform.localPosition = hide_pos;
			}
			else
			{
				OK_button.transform.localPosition = show_pos;
				Continue_button.transform.localPosition = hide_pos;
			}
			Object.Destroy(report_obj);
		}
		TNetConnection.UnregisterSFSSceneCallbacks();
		TNetConnection.Disconnect();
		OpenClikPlugin.Hide();
	}

	private void Update()
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
			SceneName.LoadLevel("VSHallTUI");
		}
		if (control.name == "Continue_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			AchiReportPanel_Content.GetComponent<PanelParameter>().HidePanel();
			AchiReportPanel.GetComponent<PanelParameter>().HidePanel();
			ReportPanel.GetComponent<PanelParameter>().ShowPanel();
			OK_button.transform.localPosition = show_pos;
			Continue_button.transform.localPosition = hide_pos;
		}
		else if (control.name == "Achievement_Close_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			AchiReportPanel_Get.transform.localPosition = new Vector3(0f, 2000f, AchiReportPanel_Get.transform.localPosition.z);
			OK_button.transform.localPosition = hide_pos;
			Continue_button.transform.localPosition = show_pos;
			if (!(AchiReportPanel_Base != null))
			{
				return;
			}
			foreach (VsAchievementCfg item in report_data.Achievement_Finished_Array)
			{
				AchiReportPanel_Base.GetComponent<VSAchimentReportPanelBase>().AddAchivmentItemAni(item);
			}
			AchiReportPanel_Base.GetComponent<VSAchimentReportPanelBase>().CheckAchivmentItems();
		}
		else if (control.name.StartsWith("Achivment_Button") && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			Debug.Log(control.name + " level:" + control.gameObject.GetComponent<AchievementData>().vs_achievement_data.level);
			ShowAchivmentContent(control.GetComponent<AchievementData>().vs_achievement_data);
		}
		else if (control.name == "Panel_Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Back");
			AchiReportPanel_Content.GetComponent<PanelParameter>().HidePanel();
			AchiReportPanel.GetComponent<PanelParameter>().ShowPanel();
		}
	}

	public VSPlayerReport FindMaxOneReportData()
	{
		VSPlayerReport result = null;
		float num = -9999f;
		foreach (VSPlayerReport unsorted_report_datum in unsorted_report_data)
		{
			float num2 = (float)unsorted_report_datum.kill_cout - 0.6f * (float)unsorted_report_datum.death_count + 1.5f * (float)unsorted_report_datum.combo_kill;
			if (num2 > num)
			{
				result = unsorted_report_datum;
				num = num2;
			}
		}
		return result;
	}

	public void ShowAchivmentContent(VsAchievementCfg achi)
	{
		if ((bool)AchiReportPanel_Content)
		{
			AchiReportPanel_Content.transform.Find("PanelBK/Icon").GetComponent<TUIMeshSprite>().frameName_Accessor = achi.icon;
			string content = achi.content;
			content = content.Replace("\\n", "\n");
			AchiReportPanel_Content.transform.Find("PanelBK/Label_Content").GetComponent<TUIMeshText>().text_Accessor = content;
			string text = string.Empty;
			if (achi.reward_cash > 0)
			{
				text = "REWARD: $" + achi.reward_cash;
			}
			if (achi.reward_avata != AvatarType.None)
			{
				text = text + "    REWARD: " + achi.reward_avata.ToString().ToUpper();
			}
			if (achi.reward_weapon != "none")
			{
				string text2 = achi.reward_weapon.ToString();
				text = text + "     REWARD: UNLOCK " + text2;
			}
			AchiReportPanel_Content.transform.Find("PanelBK/Label_Reward").GetComponent<TUIMeshText>().text_Accessor = text;
			AchiReportPanel_Content.transform.localPosition = new Vector3(0f, 0f, -10f);
		}
	}
}
