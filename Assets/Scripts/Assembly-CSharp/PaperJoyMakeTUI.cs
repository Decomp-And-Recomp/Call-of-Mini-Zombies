using UnityEngine;
using Zombie3D;

public class PaperJoyMakeTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	private TUIInput[] input;

	private AudioPlayer audioPlayer;

	public GameObject MsgBox;

	public GameObject mask;

	private bool m_is_save_photo;

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

	private void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		audioPlayer = new AudioPlayer();
		audioPlayer.AddAudio(base.transform, "Audio_Button", true);
		audioPlayer.AddAudio(base.transform, "Audio_Back", true);
		OpenClikPlugin.Hide();
		Resources.UnloadUnusedAssets();
	}

	private void Update()
	{
		input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
		GetPhotoSaveStatus();
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "BackButton" && eventType == 3)
		{
			audioPlayer.PlayAudio("Audio_Back");
			SceneName.FadeOutLevel("PaperJoyAdTUI");
		}
		else if (control.name.StartsWith("paper") && eventType == 3)
		{
			int photo_index = int.Parse(control.name.Substring("paper".Length)) - 1;
			Utils.SavePhoto(photo_index, 2480, 3508);
			ShowMask();
			m_is_save_photo = true;
		}
		else if (control.name == "okButton" && eventType == 3)
		{
			MsgBox.transform.localPosition = new Vector3(0f, 1000f, MsgBox.transform.localPosition.z);
		}
	}

	public void GetPhotoSaveStatus()
	{
		if (m_is_save_photo)
		{
			switch (Utils.OnCheckPhotoSaveStatus())
			{
			case 0:
				break;
			case 1:
				HideMask();
				Utils.OnResetPhotoSaveStatus();
				m_is_save_photo = false;
				MsgBox.transform.localPosition = new Vector3(0f, 0f, MsgBox.transform.localPosition.z);
				break;
			default:
				HideMask();
				m_is_save_photo = false;
				break;
			}
		}
	}

	public void ShowMask()
	{
		mask.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		int num = 0;
		num = ((AutoRect.GetPlatform() != Platform.IPad) ? 1 : 0);
		Utils.ShowIndicatorSystem(num, (int)AutoRect.GetPlatform(), 1f, 1f, 1f, 1f);
	}

	private void HideMask()
	{
		mask.transform.localPosition = new Vector3(0f, -1000f, base.transform.localPosition.z);
		Utils.HideIndicatorSystem();
	}
}
