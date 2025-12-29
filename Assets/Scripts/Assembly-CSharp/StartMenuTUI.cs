using UnityEngine;
using Zombie3D;

public class StartMenuTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	private TUIInput[] input;

	private AudioPlayer audioPlayer;

	public TUIMeshSprite playButton;

	public GameObject background;

	public GameObject background_iphone;

	public GameObject background_ipad;

	private float animatePlayButtonTimer;

	private float animatePlayButtonAlpha = 1f;

	private bool animatePlayButtonIncreaseAlpha;

	private void Awake()
	{
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
			audioSource.playOnAwake = false;
			audioSource.bypassEffects = true;
			audioSource.rolloffMode = AudioRolloffMode.Linear;
			audioSource.Play();
		}
		GameCenterPlugin.Login();
	}

	private void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		if (AutoRect.GetPlatform() == Platform.iPhone5)
		{
			background_iphone.SetActive(true);
			background_ipad.SetActive(false);
			background.SetActive(false);
		}
		else if (AutoRect.GetPlatform() == Platform.IPad)
		{
			background_iphone.SetActive(false);
			background_ipad.SetActive(true);
			background.SetActive(false);
		}
		else
		{
			background_iphone.SetActive(false);
			background_ipad.SetActive(false);
			background.SetActive(true);
		}
		GameApp.GetInstance().PlayerPrefsLoad();
		GameObject.Find("Music").GetComponent<AudioSource>().mute = !GameApp.GetInstance().GetGameState().MusicOn;
		audioPlayer = new AudioPlayer();
		audioPlayer.AddAudio(base.transform, "Audio_Button", true);
		Resources.UnloadUnusedAssets();
	}

	private void Update()
	{
		input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
		if (animatePlayButtonTimer < 0.02f)
		{
			animatePlayButtonTimer += Time.deltaTime;
		}
		else if (animatePlayButtonIncreaseAlpha)
		{
			animatePlayButtonAlpha += animatePlayButtonTimer * Time.deltaTime * 50;
			animatePlayButtonAlpha = Mathf.Clamp(animatePlayButtonAlpha, 0f, 1f);
			if (animatePlayButtonAlpha >= 1f)
			{
				animatePlayButtonIncreaseAlpha = false;
			}
			playButton.color_Accessor = new Color(1f, 1f, 1f, animatePlayButtonAlpha);
		}
		else
		{
			animatePlayButtonAlpha -= animatePlayButtonTimer * Time.deltaTime * 50;
			animatePlayButtonAlpha = Mathf.Clamp(animatePlayButtonAlpha, 0f, 1f);
			if (animatePlayButtonAlpha <= 0f)
			{
				animatePlayButtonIncreaseAlpha = true;
			}
			playButton.color_Accessor = new Color(1f, 1f, 1f, animatePlayButtonAlpha);
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "StartButton" && eventType == 3)
		{
			audioPlayer.PlayAudio("Audio_Button");
			GameApp.GetInstance().Load();
			if (GameApp.GetInstance().GetGameState().LevelNum == 0)
			{
				GameApp.GetInstance().GetGameState().gameMode = GameMode.Tutorial;
				SceneName.FadeOutLevel("Zombie3D_Tutorial");
			}
			else
			{
				SceneName.FadeOutLevel("MainMapTUI");
			}
			OpenClikPlugin.Show(true);
		}
	}
}
