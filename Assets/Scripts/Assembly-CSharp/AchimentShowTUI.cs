using UnityEngine;
using Zombie3D;

public class AchimentShowTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	private TUIInput[] input;

	private AudioPlayer audioPlayer = new AudioPlayer();

	public GameObject Content_Panel;

	public GameObject Back_button;

	public GameObject panel_back_button;

	public GameObject Background_Coop;

	public GameObject Background_Vs;

	public GameObject ModeChoose_Button;

	private bool isCoopAchievement = true;

	private string lastScene = string.Empty;

	public void Awake()
	{
		GameScript.CheckFillBlack();
		GameApp.GetInstance().LoadMultiAchievementConf();
		GameApp.GetInstance().LoadVsAchievementConf();
	}

	public void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		Transform folderTrans = base.transform.Find("Audio");
		audioPlayer.AddAudio(folderTrans, "Button", true);
		audioPlayer.AddAudio(folderTrans, "Back", true);
		lastScene = GameApp.GetInstance().GetGameState().last_scene;
		if (lastScene == "VSHallTUI")
		{
			isCoopAchievement = false;
			Background_Vs.transform.position = Vector3.zero;
			Background_Coop.transform.position = new Vector3(0f, -1000f, 0f);
			ModeChoose_Button.transform.Find("Button_Vs").GetComponent<TUIButtonSelect>().SetSelected(true);
		}
		else
		{
			isCoopAchievement = true;
			ModeChoose_Button.transform.Find("Button_Coop").GetComponent<TUIButtonSelect>().SetSelected(true);
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
		if (control.name == "Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Back");
			SceneName.FadeOutLevel(lastScene);
		}
		else if (control.name.StartsWith("Achivment_Button") && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			if (isCoopAchievement)
			{
				ShowAchivmentContent(control.GetComponent<AchievementData>().achievement_data);
			}
			else
			{
				ShowAchivmentContent(control.GetComponent<AchievementData>().vs_achievement_data);
			}
		}
		else if (control.name == "Button_Coop" && eventType == 1)
		{
			audioPlayer.PlayAudio("Button");
			isCoopAchievement = true;
			Background_Vs.transform.position = new Vector3(0f, -1000f, 0f);
			Background_Coop.transform.position = Vector3.zero;
		}
		else if (control.name == "Button_Vs" && eventType == 1)
		{
			audioPlayer.PlayAudio("Button");
			isCoopAchievement = false;
			Background_Coop.transform.position = new Vector3(0f, -1000f, 0f);
			Background_Vs.transform.position = Vector3.zero;
		}
		else if (control.name == "Panel_Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Back");
			Content_Panel.transform.localPosition = new Vector3(0f, 500f, -10f);
			Back_button.SetActive(true);
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
			Content_Panel.transform.localPosition = new Vector3(0f, 0f, -10f);
			Back_button.SetActive(false);
		}
	}

	public void ShowAchivmentContent(VsAchievementCfg achi)
	{
		if ((bool)Content_Panel)
		{
			Content_Panel.transform.Find("PanelBK/Icon").GetComponent<TUIMeshSprite>().frameName_Accessor = achi.icon;
			string content = achi.content;
			content = content.Replace("\\n", "\n");
			Content_Panel.transform.Find("PanelBK/Label_Content").GetComponent<TUIMeshTextMx>().ResetColor();
			Content_Panel.transform.Find("PanelBK/Label_Content").GetComponent<TUIMeshTextMx>().text_Accessor = content;
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
			Content_Panel.transform.Find("PanelBK/Label_Reward").GetComponent<TUIMeshText>().text_Accessor = text;
			Content_Panel.transform.localPosition = new Vector3(0f, 0f, -10f);
			Back_button.SetActive(false);
		}
	}
}
