using UnityEngine;
using Zombie3D;

public class PaperJoyAdTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	private TUIInput[] input;

	private AudioPlayer audioPlayer;

	public GameObject buyButton;

	public GameObject useButton;

	public GameObject restoreButton;

	public GameObject mask;

	private IAPName iap_Processing = IAPName.None;

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
		GameApp.GetInstance().PlayerPrefsLoad();
		CheckPaperModelStatus();
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
		if (control.name == "Back" && eventType == 3)
		{
			audioPlayer.PlayAudio("Audio_Back");
			//SceneName.FadeOutLevel("IapShopTUI");
		}
		else if (control.name == "BuyThem" && eventType == 3)
		{
			audioPlayer.PlayAudio("Audio_Button");
			if (iap_Processing == IAPName.None)
			{
				IAP.NowPurchaseProduct("com.trinitigame.callofminizombies.new099cents2", "1");
				iap_Processing = IAPName.PaperJoy;
				ShowMask();
			}
		}
		else if (control.name == "MakeThem" && eventType == 3)
		{
			audioPlayer.PlayAudio("Audio_Button");
			SceneName.FadeOutLevel("PaperJoyMakeTUI");
		}
		else if (control.name == "Restore" && eventType == 3)
		{
			audioPlayer.PlayAudio("Audio_Button");
			if (iap_Processing == IAPName.None)
			{
				IAP.DoRestoreProduct();
				iap_Processing = IAPName.PaperJoyRestore;
				ShowMask();
			}
		}
	}

	private void CheckPaperModelStatus()
	{
		if (GameApp.GetInstance().GetGameState().PaperModelShow)
		{
			buyButton.SetActive(false);
			useButton.SetActive(true);
			restoreButton.SetActive(false);
		}
		else
		{
			buyButton.SetActive(true);
			useButton.SetActive(false);
			restoreButton.SetActive(true);
		}
	}

	public void GetPurchaseStatus()
	{
		if (iap_Processing == IAPName.PaperJoy)
		{
			switch (IAP.purchaseStatus(null))
			{
			case 0:
				break;
			case 1:
				GameApp.GetInstance().GetGameState().PaperModelShow = true;
				GameApp.GetInstance().PlayerPrefsSave();
				HideMask();
				iap_Processing = IAPName.None;
				CheckPaperModelStatus();
				break;
			default:
				HideMask();
				iap_Processing = IAPName.None;
				break;
			}
		}
		else if (iap_Processing == IAPName.PaperJoyRestore)
		{
			switch (IAP.DoRestoreStatus())
			{
			case 0:
				break;
			case 1:
				Restore();
				HideMask();
				iap_Processing = IAPName.None;
				CheckPaperModelStatus();
				break;
			default:
				HideMask();
				iap_Processing = IAPName.None;
				break;
			}
		}
	}

	private void Restore()
	{
		string[] array = IAP.DoRestoreGetProductId();
		Debug.Log(array.Length);
		foreach (string text in array)
		{
			Debug.Log(text);
			if (text == "com.trinitigame.callofminizombies.new099cents2")
			{
				GameApp.GetInstance().GetGameState().PaperModelShow = true;
				GameApp.GetInstance().PlayerPrefsSave();
				Utils.ShowMessageBox1(string.Empty, "Item restored.", "OK");
				return;
			}
		}
		Utils.ShowMessageBox1(string.Empty, "You can't restore unpurchased items.", "OK");
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
