using UnityEngine;
using Zombie3D;

public class InstanceCreateRoomTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	private AudioPlayer audioPlayer = new AudioPlayer();

	public GameObject ChosenFrame;

	public TUIMeshText Title;

	public GameObject maskBlock;

	private int currentChosenScene;

	public void Awake()
	{
		if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Instance)
		{
			base.enabled = false;
			return;
		}
		base.enabled = true;
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
		audioPlayer.AddAudio(base.transform, "Button", true);
		audioPlayer.AddAudio(base.transform, "Back", true);
		Title.text_Accessor = "SURVIVAL";
		Title.color_Accessor = ColorName.fontColor_orange;
		GameObject.Find("scene10").SetActive(false);
		ChooseScene(GameObject.Find("scene1"));
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
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Back");
			SceneName.FadeOutLevel("MainMapTUI");
		}
		else if (control.name.StartsWith("scene") && eventType == 1)
		{
			audioPlayer.PlayAudio("Button");
			ChooseScene(control.gameObject);
		}
		else if (control.name == "Create_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			SceneName.FadeOutLevel(SceneName.GetNetMapName(currentChosenScene));
			maskBlock.transform.localPosition = new Vector3(0f, 0f, maskBlock.transform.localPosition.z);
		}
	}

	private void ChooseScene(GameObject button)
	{
		currentChosenScene = int.Parse(button.name.Substring("scene".Length));
		ChosenFrame.GetComponent<ChosenFrameManager>().ChosenObject = button;
	}
}
