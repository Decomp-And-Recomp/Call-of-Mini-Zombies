using UnityEngine;
using Zombie3D;

public class OptionTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public TUIButtonSelectText music_off_button;

	public TUIButtonSelectText music_on_button;

	public TUIButtonSelectText sound_off_button;

	public TUIButtonSelectText sound_on_button;

	public TUIMeshText label_day;

	public TUIMeshText label_money;

	public TUIMeshText label_crystal;

	public GameObject credits_panel;

	public GameObject Sensitivity_Ori;

	public GameObject Sensitivity_Button;

	public GameObject Sensitivity_BK;

	public GameObject Sensitivity_Label;

	public GameObject Sensitivity_Label_BK;

	protected AudioPlayer audioPlayer = new AudioPlayer();

	private void Awake()
	{
		GameScript.CheckFillBlack();
		GameScript.CheckMenuResourceConfig();
		GameScript.CheckGlobalResourceConfig();
		if (GameObject.Find("Music") == null)
		{
			GameApp.GetInstance().InitForMenu();
			GameApp.GetInstance().GetGameState().InitWeapons();
			GameApp.GetInstance().GetGameState().InitItems();
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
		music_off_button.SetSelected(!GameApp.GetInstance().GetGameState().MusicOn);
		music_on_button.SetSelected(GameApp.GetInstance().GetGameState().MusicOn);
		sound_off_button.SetSelected(!GameApp.GetInstance().GetGameState().SoundOn);
		sound_on_button.SetSelected(GameApp.GetInstance().GetGameState().SoundOn);
		label_day.text_Accessor = "DAY " + GameApp.GetInstance().GetGameState().LevelNum;
		label_money.text_Accessor = "$" + GameApp.GetInstance().GetGameState().GetCash()
			.ToString("N0");
		label_crystal.text_Accessor = GameApp.GetInstance().GetGameState().GetCrystal()
			.ToString("N0");
		Transform folderTrans = base.transform.Find("Audio");
		audioPlayer.AddAudio(folderTrans, "Button", true);
		audioPlayer.AddAudio(folderTrans, "Back", true);
		GameObject gameObject = GameObject.Find("Music");
		if (gameObject != null)
		{
			AudioSource component = gameObject.GetComponent<AudioSource>();
			if (component != null)
			{
				component.mute = !GameApp.GetInstance().GetGameState().MusicOn;
			}
		}
		if (Sensitivity_Button != null)
		{
			Sensitivity_Button.transform.localPosition = new Vector3(GameApp.GetInstance().GetGameState().macos_sen - 1f + Sensitivity_Ori.transform.localPosition.x, Sensitivity_Ori.transform.localPosition.y, Sensitivity_Ori.transform.localPosition.z);
		}
		GameObject gameObject2 = GameObject.Find("GameCenter");
		if (gameObject2 != null)
		{
			gameObject2.SetActive(false);
		}
		Sensitivity_Ori.SetActive(false);
		Sensitivity_Button.SetActive(false);
		Sensitivity_BK.SetActive(false);
		Sensitivity_Label.SetActive(false);
		Sensitivity_Label_BK.SetActive(false);
		GameObject gameObject3 = GameObject.Find("Share_Button");
		if (gameObject3 != null)
		{
			gameObject3.SetActive(false);
		}
		GameObject gameObject4 = GameObject.Find("Support_Button");
		if (gameObject4 != null)
		{
			gameObject4.transform.localPosition = new Vector3(0f, gameObject4.transform.localPosition.y, gameObject4.transform.localPosition.z);
		}
		NetworkObj.DestroyNetCom();
		OpenClikPlugin.Hide();
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
			SceneName.FadeOutLevel(GameApp.GetInstance().GetGameState().last_scene);
		}
		else if (control.name == "Music_Off_Button" && eventType == 1)
		{
			GameApp.GetInstance().GetGameState().MusicOn = false;
			audioPlayer.PlayAudio("Button");
			GameApp.GetInstance().PlayerPrefsSave();
			GameObject gameObject = GameObject.Find("Music");
			if (gameObject != null)
			{
				AudioSource component = gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.mute = !GameApp.GetInstance().GetGameState().MusicOn;
				}
			}
		}
		else if (control.name == "Music_On_Button" && eventType == 1)
		{
			GameApp.GetInstance().GetGameState().MusicOn = true;
			audioPlayer.PlayAudio("Button");
			GameApp.GetInstance().PlayerPrefsSave();
			GameObject gameObject2 = GameObject.Find("Music");
			if (gameObject2 != null)
			{
				AudioSource component2 = gameObject2.GetComponent<AudioSource>();
				if (component2 != null)
				{
					component2.mute = !GameApp.GetInstance().GetGameState().MusicOn;
				}
			}
		}
		else if (control.name == "Sound_Off_Button" && eventType == 1)
		{
			GameApp.GetInstance().GetGameState().SoundOn = false;
			audioPlayer.PlayAudio("Button");
			GameApp.GetInstance().PlayerPrefsSave();
		}
		else if (control.name == "Sound_On_Button" && eventType == 1)
		{
			GameApp.GetInstance().GetGameState().SoundOn = true;
			audioPlayer.PlayAudio("Button");
			GameApp.GetInstance().PlayerPrefsSave();
		}
		else if (control.name == "Share_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			string text = GameApp.GetInstance().GetGloabResourceConfig().shareHtm.text;
			Utils.ToSendMail(string.Empty, "Call Of Mini: Zombies", text);
		}
		else if (control.name == "Support_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			Application.OpenURL("http://www.trinitigame.com/support?game=comz&version=4.3.4");
		}
		else if (control.name == "Review_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			Application.OpenURL("market://details?id=com.trinitigame.callofminiandroid");
		}
		else if (control.name == "Credits_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			credits_panel.transform.localPosition = new Vector3(0f, 0f, credits_panel.transform.localPosition.z);
		}
		else if (control.name == "Credits_Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			credits_panel.transform.localPosition = new Vector3(0f, -1000f, credits_panel.transform.localPosition.z);
		}
		else if (control.name == "Sensitivity_Button" && eventType == 2)
		{
			float num = control.transform.localPosition.x - Sensitivity_Ori.transform.localPosition.x + 1f;
			GameApp.GetInstance().GetGameState().macos_sen = num;
			Debug.Log("delta_x:" + num);
			GameApp.GetInstance().PlayerPrefsSave();
		}
		else if (control.name == "game_center" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			GameCenterPlugin.OpenAchievement();
		}
		else if (control.name == "game_rankings" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			GameCenterPlugin.OpenLeaderboard();
		}
	}
}
