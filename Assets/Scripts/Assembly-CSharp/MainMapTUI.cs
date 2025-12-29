using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Zombie3D;

public class MainMapTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	private TUIInput[] input;

	private AudioPlayer audioPlayer = new AudioPlayer();

	public static bool showNewbie1Intro;

	public static bool showNewbie2Intro;

	public TUIMeshText label_day;

	public TUIMeshText label_money;

	public TUIMeshText label_crystal;

	public GameObject MsgBox;

	public GameObject iapDiscountPanel;

	public GameObject iapDiscountEntry;

	public TUIMeshText iapDiscountPanelTimer;

	public TUIMeshText iapDiscountEntryTimer;

	public TUIMeshText[] iapDiscountInfo = new TUIMeshText[3];

	public GameObject newbiePanel;

	public TUIMeshSprite newbieContent;

	public TUIMeshSprite newbieMoney;

	public GameObject mask;

	private IAPName iapProcessing = IAPName.None;

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
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.transform.position = new Vector3(0f, 1f, -10f);
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.clip = GameApp.GetInstance().GetMenuResourceConfig().menuAudio;
			audioSource.loop = true;
			audioSource.bypassEffects = true;
			audioSource.rolloffMode = AudioRolloffMode.Linear;
			audioSource.mute = !GameApp.GetInstance().GetGameState().MusicOn;
			audioSource.Play();
		}
		if (GameObject.Find("IABAndroidManager") == null)
		{
			UnityEngine.Object.Instantiate(Resources.Load("Prefabs/IABAndroidManager"));
			string[] iapIds = new string[16]
			{
				"com.trinitigame.callofminizombies.199centsv40b", "com.trinitigame.callofminizombies.499centsv40b", "com.trinitigame.callofminizombies.999centsv40b", "com.trinitigame.callofminizombies.1999centsv40b", "com.trinitigame.callofminizombies.4999centsv40b", "com.trinitigame.callofminizombies.9999centsv43b", "com.trinitigame.callofminizombies.199centsv40", "com.trinitigame.callofminizombies.499centsv40", "com.trinitigame.callofminizombies.999centsv40", "com.trinitigame.callofminizombies.1999centsv40",
				"com.trinitigame.callofminizombies.4999centsv40", "com.trinitigame.callofminizombies.9999centsv43", "com.trinitigame.callofminizombies.099centsv40b", "com.trinitigame.callofminizombies.099centsv40c", "com.trinitigame.callofminizombies.099centsv40e", "com.trinitigame.callofminizombies.599centsv40"
			};
			GoogleIABManager.iapIds = iapIds;
			string publicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAvfxRqpA+fjKm64VbNaXM6offkWUUsgCRzZlJFJrjZD5MTcX2p2/nfyOYiNDAh9qrS6hoS7MfIvPYirc38oerql/die8eIsW5JtBkeVt2te9+ZCc2BjmOr2b3g+xirbE1bkReJP5JDARHColJA7lQ/6o4J8rvv9L1rGcYynrWeSdTegeBDRkuMPQjgNArMXzkw7hITPdLXhQtBgnn62tV7zvguxKMuYoqzmXpyMsSyyAFVGDQAvI7ITKXvRR+0LL2ybjmP0+0kwLu7NL+nshBm8msjHbCqclsiOwcEkaMFk/Jgqg8B2MLeL7Ff2PJIJA023FnMfPgzNJIde0hj20j6wIDAQAB";
			GoogleIAB.init(publicKey);
		}
		GameApp.GetInstance().ClearScene();
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	private void Start()
	{
		Cursor.visible = true;
        m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		label_day.text_Accessor = "DAY " + GameApp.GetInstance().GetGameState().LevelNum;
		label_money.text_Accessor = "$" + GameApp.GetInstance().GetGameState().GetCash()
			.ToString("N0");
		label_crystal.text_Accessor = GameApp.GetInstance().GetGameState().GetCrystal()
			.ToString("N0");
		if (!GameApp.GetInstance().GetGameState().show_zombies2_link)
		{
			GameObject.Find("TUI/TUIControl/COMZ/bk1").GetComponent<TUIMeshSprite>().frameName_Accessor = "halo1";
			GameObject.Find("TUI/TUIControl/COMZ/bk2").GetComponent<TUIMeshSprite>().frameName_Accessor = "halo2";
			GameObject.Find("TUI/TUIControl/COMZ/button").GetComponent<TUIMeshSpriteAnimation>().frameName_Accessor = "halo3";
			GameObject.Find("TUI/TUIControl/COMZ/button").GetComponent<TUIMeshSpriteAnimation>().frame_array[0] = "halo3";
			GameObject.Find("TUI/TUIControl/COMZ/button").GetComponent<TUIMeshSpriteAnimation>().frame_array[1] = "halo4";
			string text = Application.dataPath + "/../Documents/";
			if (Directory.Exists(text))
			{
				File.Open(text + "VideoLink.dat", FileMode.Create);
			}
		}
		else
		{
			string text2 = Application.dataPath + "/../Documents/";
			if (File.Exists(text2 + "VideoLink.dat"))
			{
				File.Delete(text2 + "VideoLink.dat");
			}
		}
		if (!CheckShowMakeUpCrystal())
		{
			if (!CheckShowIapDiscountPanel())
			{
				CheckShowNewbieIntroPanel();
			}
			else
			{
				GameApp.GetInstance().Save();
			}
		}
		Transform folderTrans = base.transform.Find("Audio");
		audioPlayer.AddAudio(folderTrans, "Button", true);
		audioPlayer.AddAudio(folderTrans, "Battle", true);
		OpenClikPlugin.Hide(false);
		NetworkObj.DestroyNetCom();
	}

	private bool CheckShowMakeUpCrystal()
	{
		if (GameApp.GetInstance().GetGameState().makeUpCrystal)
		{
			if (GameApp.GetInstance().GetGameState().LevelNum > 1)
			{
				int num = 0;
				num = ((!(GameApp.GetInstance().GetGameState().Game_last_ver < 4.19f)) ? (GameApp.GetInstance().GetGameState().LevelNum - 1 - GameApp.GetInstance().GetGameState().LevelNum / 5) : (GameApp.GetInstance().GetGameState().LevelNum - 1));
				MsgBox.transform.Find("BK/getCrystal").gameObject.SetActive(true);
				MsgBox.transform.Find("BK/getCrystal/crystalCount").GetComponent<TUIMeshText>().text_Accessor = num.ToString();
				MsgBox.GetComponent<MsgBoxDelegate>().SetOkButton("GET");
				MsgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.makeUpCrystal, MsgBoxType.SceneTutorial);
				MsgBox.GetComponent<MsgBoxDelegate>().SetContentOffset(new Vector2(-118f, 36f));
				return true;
			}
			GameApp.GetInstance().GetGameState().makeUpCrystal = false;
		}
		return false;
	}

	public bool CheckShowIapDiscountPanel()
	{
		if (GameApp.GetInstance().GetGameState().iapDiscount.discountInfo != null)
		{
			IapDiscountManager.DiscountInfo discountInfo = GameApp.GetInstance().GetGameState().iapDiscount.discountInfo;
			if (discountInfo.type == IapDiscountManager.DiscountType.Active)
			{
				if (discountInfo.beginTimer)
				{
					ShowIapDiscountEntry();
					HideIapDiscountPanel();
					return false;
				}
				if (GameApp.GetInstance().GetGameState().last_scene.StartsWith("Zombie3D_"))
				{
					discountInfo.startTime = DateTime.Now;
					discountInfo.beginTimer = true;
					ShowIapDiscountPanel(discountInfo.type);
					HideIapDiscountEntry();
					return true;
				}
			}
			else if (discountInfo.type != 0)
			{
				if (!discountInfo.beginTimer)
				{
					discountInfo.startTime = DateTime.Now;
					discountInfo.beginTimer = true;
					ShowIapDiscountPanel(discountInfo.type);
					HideIapDiscountEntry();
					return true;
				}
				ShowIapDiscountEntry();
				HideIapDiscountPanel();
				return false;
			}
		}
		return false;
	}

	private bool CheckShowNewbieIntroPanel()
	{
		if (showNewbie1Intro)
		{
			ShowNewbiePanel();
			newbieContent.frameName_Accessor = "newbie_info1";
			newbieMoney.frameName_Accessor = "newbie_money1";
			showNewbie1Intro = false;
			return true;
		}
		if (showNewbie2Intro)
		{
			ShowNewbiePanel();
			newbieContent.frameName_Accessor = "newbie_info2";
			newbieMoney.frameName_Accessor = "newbie_money2";
			showNewbie2Intro = false;
			return true;
		}
		return false;
	}

	private void Update()
	{
		input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
		UpdateIapDiscountTimer();
	}

	private void UpdateIapDiscountTimer()
	{
		if (GameApp.GetInstance().GetGameState().iapDiscount.discountInfo != null && GameApp.GetInstance().GetGameState().iapDiscount.discountInfo.beginTimer && GameApp.GetInstance().GetGameState().iapDiscount.CheckSystemTimeValid())
		{
			int minutes = DateTime.Now.Subtract(GameApp.GetInstance().GetGameState().iapDiscount.discountInfo.startTime).Minutes;
			int seconds = DateTime.Now.Subtract(GameApp.GetInstance().GetGameState().iapDiscount.discountInfo.startTime).Seconds;
			minutes = 9 - minutes;
			seconds = 59 - seconds;
			if (iapDiscountPanel.transform.position.y == 0f)
			{
				iapDiscountPanelTimer.text_Accessor = minutes.ToString("D2") + ":" + seconds.ToString("D2");
			}
			else if (iapDiscountEntry.transform.position.y == 112f)
			{
				iapDiscountEntryTimer.text_Accessor = minutes.ToString("D2") + ":" + seconds.ToString("D2");
			}
		}
		else
		{
			if (iapDiscountPanel.transform.position.y == 0f)
			{
				HideIapDiscountPanel();
			}
			if (iapDiscountEntry.transform.position.y == 112f)
			{
				HideIapDiscountEntry();
			}
		}
	}

	private IEnumerator IapDiscountBuy()
	{
		if (GameApp.GetInstance().GetGameState().iapDiscount.discountInfo != null && iapProcessing == IAPName.None)
		{
			IapDiscountManager.DiscountInfo mDiscount = GameApp.GetInstance().GetGameState().iapDiscount.discountInfo;
			if (mDiscount.type == IapDiscountManager.DiscountType.Active)
			{
				IAP.NowPurchaseProduct("com.trinitigame.callofminizombies.1199centsv40", "1");
				iapProcessing = IAPName.IapDiscountActive;
				ShowMask(true);
			}
			else if (mDiscount.type == IapDiscountManager.DiscountType.Inactive1)
			{
				IAP.NowPurchaseProduct("com.trinitigame.callofminizombies.299centsv40", "1");
				iapProcessing = IAPName.IapDiscountInactive1;
				ShowMask(true);
			}
			else if (mDiscount.type == IapDiscountManager.DiscountType.Inactive2)
			{
				IAP.NowPurchaseProduct("com.trinitigame.callofminizombies.199centsv40c", "1");
				iapProcessing = IAPName.IapDiscountInactive2;
				ShowMask(true);
			}
			else if (mDiscount.type == IapDiscountManager.DiscountType.Inactive3)
			{
				IAP.NowPurchaseProduct("com.trinitigame.callofminizombies.099centsv40d", "1");
				iapProcessing = IAPName.IapDiscountInactive3;
				ShowMask(true);
			}
			yield return 0;
		}
	}

	private void GetPurchaseStatus()
	{
		if (iapProcessing == IAPName.None)
		{
			return;
		}
		int num = IAP.purchaseStatus(null);
		Debug.Log("statusCode:" + num);
		switch (num)
		{
		case 0:
			break;
		case 1:
			GameApp.GetInstance().GetGameState().iapDiscount.SucceedBuyDiscount();
			GameApp.GetInstance().GetGameState().DeliverIAPItem(iapProcessing);
			HideIapDiscountPanel();
			HideIapDiscountEntry();
			HideMask(true);
			iapProcessing = IAPName.None;
			label_crystal.text_Accessor = GameApp.GetInstance().GetGameState().GetCrystal()
				.ToString("N0");
			break;
		default:
			HideMask(true);
			iapProcessing = IAPName.None;
			if (GameApp.GetInstance().GetGameState().iapDiscount.discountInfo != null)
			{
				ShowIapDiscountPanel(GameApp.GetInstance().GetGameState().iapDiscount.discountInfo.type);
			}
			break;
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Achi_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			SceneName.FadeOutLevel("AchievementTUI");
		}
		else if (control.name == "Shop_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			SceneName.FadeOutLevel("ShopMenuTUI");
			ShopMenuTUI.last_map_to_shop = "MainMapTUI";
			ShopMenuTUI.chosen_shop = ShopMenu.AvatarShop;
		}
		else if (control.name == "Option_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			SceneName.FadeOutLevel("OptionTUI");
		}
		else if (control.name == "Money_getMore" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			//SceneName.FadeOutLevel("IapShopTUI");
			//IapShopTUI.last_scene_to_iap = "MainMapTUI";
		}
		else if (control.name == "IapDiscount_Close" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			HideIapDiscountPanel();
			ShowIapDiscountEntry();
		}
		else if (control.name == "IapDiscount_Purchase" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			StartCoroutine(IapDiscountBuy());
		}
		else if (control.name == "TimeCount" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			HideIapDiscountEntry();
			ShowIapDiscountPanel(GameApp.GetInstance().GetGameState().iapDiscount.discountInfo.type);
		}
		else if (control.name == "Newbie_Close" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			HideNewbiePanel();
		}
		else if (control.name == "Newbie_Purchase" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			//SceneName.FadeOutLevel("IapShopTUI");
			//IapShopTUI.last_scene_to_iap = "MainMapTUI";
		}
		else if (control.name == "Button_COOP" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			GameApp.GetInstance().GetGameState().gameMode = GameMode.Coop;
			if (GameApp.GetInstance().GetGameState().nick_name == string.Empty || GameApp.GetInstance().GetGameState().nick_name == "None")
			{
				SceneName.FadeOutLevel("NickNameTUI");
				NickNameTUI.nickNameToCoop = true;
			}
			else
			{
				SceneName.FadeOutLevel("CoopHallTUI");
			}
		}
		else if (control.name == "Button_VS" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			GameApp.GetInstance().GetGameState().gameMode = GameMode.Vs;
			if (GameApp.GetInstance().GetGameState().nick_name == string.Empty || GameApp.GetInstance().GetGameState().nick_name == "None")
			{
				SceneName.FadeOutLevel("NickNameTUI");
				NickNameTUI.nickNameToCoop = false;
			}
			else
			{
				SceneName.FadeOutLevel("VSHallTUI");
			}
		}
		else if (control.name == "Button_Score" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			GameApp.GetInstance().GetGameState().gameMode = GameMode.Instance;
			SceneName.FadeOutLevel("CreateRoomTUI");
			if (GameApp.GetInstance().GetGameState().tutorialTriggers[9])
			{
				GameApp.GetInstance().GetGameState().tutorialTriggers[9] = false;
				GameApp.GetInstance().PlayerPrefsSave();
			}
		}
		else if (control.name.StartsWith(TutorialMainMap.tip_boss) && eventType == 3)
		{
			audioPlayer.PlayAudio("Battle");
			GameApp.GetInstance().GetGameState().gameMode = GameMode.SoloBoss;
			string scene = "Zombie3D_" + control.name.Substring((TutorialMainMap.tip_boss + "Button_").Length);
			SceneName.FadeOutLevel(scene);
			if (GameApp.GetInstance().GetGameState().tutorialTriggers[0])
			{
				GameApp.GetInstance().GetGameState().tutorialTriggers[0] = false;
				GameApp.GetInstance().PlayerPrefsSave();
			}
		}
		else if (control.name.StartsWith(TutorialMainMap.tip_hunting) && eventType == 3)
		{
			audioPlayer.PlayAudio("Battle");
			GameApp.GetInstance().GetGameState().gameMode = GameMode.Hunting;
			string scene2 = "Zombie3D_" + control.name.Substring((TutorialMainMap.tip_hunting + "Button_").Length);
			SceneName.FadeOutLevel(scene2);
			if (GameApp.GetInstance().GetGameState().tutorialTriggers[1])
			{
				GameApp.GetInstance().GetGameState().tutorialTriggers[1] = false;
				GameApp.GetInstance().PlayerPrefsSave();
			}
		}
		else if (control.name.StartsWith(TutorialMainMap.tip_dinoHunting) && eventType == 3)
		{
			audioPlayer.PlayAudio("Battle");
			GameApp.GetInstance().GetGameState().gameMode = GameMode.DinoHunting;
			string scene3 = "Zombie3D_" + control.name.Substring((TutorialMainMap.tip_dinoHunting + "Button_").Length);
			SceneName.FadeOutLevel(scene3);
			if (GameApp.GetInstance().GetGameState().tutorialTriggers[10])
			{
				GameApp.GetInstance().GetGameState().tutorialTriggers[10] = false;
				GameApp.GetInstance().PlayerPrefsSave();
			}
		}
		else if (control.name.StartsWith(TutorialMainMap.tip_normal) && eventType == 3)
		{
			audioPlayer.PlayAudio("Battle");
			string scene4 = "Zombie3D_" + control.name.Substring((TutorialMainMap.tip_normal + "Button_").Length);
			GameApp.GetInstance().GetGameState().gameMode = GameMode.Solo;
			SceneName.FadeOutLevel(scene4);
		}
		else if (control.name == "Msg_OK_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			MsgBox.GetComponent<MsgBoxDelegate>().Hide();
			if (MsgBox.GetComponent<MsgBoxDelegate>().m_type == MsgBoxType.SceneTutorial)
			{
				GameApp.GetInstance().GetGameState().makeUpCrystal = false;
				int crystalGot = int.Parse(control.transform.parent.Find("getCrystal/crystalCount").GetComponent<TUIMeshText>().text_Accessor);
				GameApp.GetInstance().GetGameState().AddCrystal(crystalGot);
				label_crystal.text_Accessor = GameApp.GetInstance().GetGameState().GetCrystal()
					.ToString("N0");
				CheckShowIapDiscountPanel();
				GameApp.GetInstance().Save();
			}
		}
		else if (control.name == "COMZ" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			if (GameApp.GetInstance().GetGameState().show_zombies2_link)
			{
				Application.OpenURL("https://itunes.apple.com/us/app/call-of-mini-zombies-2/id605681399?ls=1&mt=8");
			}
			else
			{
				Application.OpenURL("https://itunes.apple.com/us/app/call-of-mini-infinity/id605676336?ls=1&mt=8");
			}
		}
		else if (control.name == "DinoHunter" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			Application.OpenURL("https://itunes.apple.com/us/app/call-of-mini-dinohunter/id605678478?ls=1&mt=8");
		}
	}

	private void ShowIapDiscountPanel(IapDiscountManager.DiscountType type)
	{
		HideIapDiscountEntry();
		HideNewbiePanel();
		iapDiscountPanel.transform.position = new Vector3(0f, 0f, -30f);
		switch (type)
		{
		case IapDiscountManager.DiscountType.Active:
			iapDiscountInfo[0].text_Accessor = "298";
			iapDiscountInfo[1].text_Accessor = "$19.99";
			iapDiscountInfo[2].text_Accessor = "$11.99";
			break;
		case IapDiscountManager.DiscountType.Inactive1:
			iapDiscountInfo[0].text_Accessor = "150";
			iapDiscountInfo[1].text_Accessor = "$4.99";
			iapDiscountInfo[2].text_Accessor = "$2.99";
			break;
		case IapDiscountManager.DiscountType.Inactive2:
			iapDiscountInfo[0].text_Accessor = "150";
			iapDiscountInfo[1].text_Accessor = "$4.99";
			iapDiscountInfo[2].text_Accessor = "$1.99";
			break;
		case IapDiscountManager.DiscountType.Inactive3:
			iapDiscountInfo[0].text_Accessor = "150";
			iapDiscountInfo[1].text_Accessor = "$4.99";
			iapDiscountInfo[2].text_Accessor = "$0.99";
			break;
		}
	}

	private void HideIapDiscountPanel()
	{
		iapDiscountPanel.transform.position = new Vector3(0f, 4000f, -30f);
	}

	private void ShowIapDiscountEntry()
	{
		HideIapDiscountPanel();
		HideNewbiePanel();
		iapDiscountEntry.transform.position = new Vector3(192f, 112f, -20f);
	}

	private void HideIapDiscountEntry()
	{
		iapDiscountEntry.transform.position = new Vector3(192f, 4112f, -20f);
	}

	private void ShowNewbiePanel()
	{
		HideIapDiscountPanel();
		HideIapDiscountEntry();
		newbiePanel.transform.position = new Vector3(0f, 0f, -30f);
	}

	private void HideNewbiePanel()
	{
		newbiePanel.transform.position = new Vector3(0f, 4000f, -30f);
	}

	private void ShowMask(bool showIndicator)
	{
		mask.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		if (showIndicator)
		{
			int num = 0;
			num = ((AutoRect.GetPlatform() != Platform.IPad) ? 1 : 0);
			Utils.ShowIndicatorSystem(num, (int)AutoRect.GetPlatform(), 1f, 1f, 1f, 1f);
		}
	}

	private void HideMask(bool hideIndicator)
	{
		mask.transform.localPosition = new Vector3(0f, -1000f, base.transform.localPosition.z);
		if (hideIndicator)
		{
			Utils.HideIndicatorSystem();
		}
	}
}
