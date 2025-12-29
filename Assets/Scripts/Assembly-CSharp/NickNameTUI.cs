using System.Text.RegularExpressions;
using UnityEngine;
using Zombie3D;

public class NickNameTUI : MonoBehaviour, TUIHandler
{
	public static bool nickNameToCoop = true;

	private TUI m_tui;

	protected TUIInput[] input;

	private AudioPlayer audioPlayer = new AudioPlayer();

	public GameObject Msg_box;

	public GameObject Indicator_Panel;

	public GameObject Editor_Panel;

	public GameObject OK_Button;

	protected Regex myRex;

	private void Awake()
	{
		GameScript.CheckFillBlack();
		GameScript.CheckMenuResourceConfig();
		GameScript.CheckGlobalResourceConfig();
		if (GameObject.Find("Music") == null)
		{
			GameApp.GetInstance().InitForMenu();
			GameObject gameObject = new GameObject("Music");
			Object.DontDestroyOnLoad(gameObject);
			gameObject.transform.position = new Vector3(0f, 1f, -10f);
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.clip = GameApp.GetInstance().GetMenuResourceConfig().menuAudio;
			audioSource.loop = true;
			audioSource.bypassEffects = true;
			audioSource.rolloffMode = AudioRolloffMode.Linear;
			audioSource.mute = !GameApp.GetInstance().GetGameState().MusicOn;
			audioSource.Play();
		}
	}

	public void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		audioPlayer.AddAudio(base.transform, "Button", true);
		myRex = new Regex("^[A-Za-z0-9]+$");
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
		if (control.name == "Msg_OK_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			MsgBoxType type = Msg_box.GetComponent<MsgBoxDelegate>().m_type;
			Msg_box.GetComponent<MsgBoxDelegate>().Hide();
			switch (type)
			{
			case MsgBoxType.ContectingTimeout:
			case MsgBoxType.ContectingLost:
				SceneName.FadeOutLevel("MainMapTUI");
				break;
			case MsgBoxType.JoinRommFailed:
				break;
			}
		}
		else
		{
			if (!(control.name == "OK") || eventType != 3)
			{
				return;
			}
			audioPlayer.PlayAudio("Button");
			Match match = myRex.Match(Editor_Panel.GetComponent<TUITextField>().GetText());
			if (Editor_Panel.GetComponent<TUITextField>().GetText().Length > 0 && match.Success && Editor_Panel.GetComponent<TUITextField>().GetText() != "None")
			{
				GameApp.GetInstance().GetGameState().nick_name = Editor_Panel.GetComponent<TUITextField>().GetText();
				GameApp.GetInstance().Save();
				Editor_Panel.transform.localPosition = new Vector3(0f, 1000f, Editor_Panel.transform.localPosition.z);
				if (nickNameToCoop)
				{
					SceneName.FadeOutLevel("CoopHallTUI");
				}
				else
				{
					SceneName.FadeOutLevel("VSHallTUI");
				}
			}
			else if ((bool)Msg_box)
			{
				Msg_box.transform.localPosition = new Vector3(0f, 0f, Msg_box.transform.localPosition.z);
				Msg_box.GetComponent<MsgBoxDelegate>().SetMsgContent("Invalid name. Please try again!", MsgBoxType.WrongTextFormat);
				Editor_Panel.GetComponent<TUITextField>().ResetText();
			}
		}
	}
}
