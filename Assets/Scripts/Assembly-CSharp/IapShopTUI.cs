using System.Collections;
using UnityEngine;
using Zombie3D;

public class IapShopTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public static string last_scene_to_iap = string.Empty;

	public TUIMeshText label_day;

	public TUIMeshText label_money;

	public TUIMeshText label_crystal;

	public GameObject mask;

	public TUIMeshText description_text;

	public TUIMeshText msgBox_text;

	public GameObject msgBoxOK;

	protected AudioPlayer audioPlayer = new AudioPlayer();

	protected IAPName iapProcessing = IAPName.None;

	protected GameObject iapChosenObj;

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
		Transform folderTrans = base.transform.Find("Audio");
		audioPlayer.AddAudio(folderTrans, "Button", true);
		audioPlayer.AddAudio(folderTrans, "Back", true);
		RefreshCashLebel();
		label_day.text_Accessor = "DAY " + GameApp.GetInstance().GetGameState().LevelNum;
		CheckNiewbieSoldout();
		NetworkObj.DestroyNetCom();
		OpenClikPlugin.Hide();
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/IABAndroidEventListener")) as GameObject;
		gameObject.GetComponent<GoogleIABEventListener>().iap_panel = this;
		Object.Destroy(GameObject.Find("TapJoyObj"));
		Object.Destroy(GameObject.Find("Tapjoy"));
		Resources.UnloadUnusedAssets();
	}

	private void CheckNiewbieSoldout()
	{
		if (GameApp.GetInstance().GetGameState().soldOutNewbie1)
		{
			m_tui.transform.Find("TUIControl/TUIScrollObj/iap2/Newbie1").GetComponent<TUIButtonClick>().enabled = false;
			m_tui.transform.Find("TUIControl/TUIScrollObj/iap2/Newbie1/soldout").gameObject.SetActive(true);
		}
		else
		{
			m_tui.transform.Find("TUIControl/TUIScrollObj/iap2/Newbie1").GetComponent<TUIButtonClick>().enabled = true;
			m_tui.transform.Find("TUIControl/TUIScrollObj/iap2/Newbie1/soldout").gameObject.SetActive(false);
		}
		if (GameApp.GetInstance().GetGameState().soldOutNewbie2)
		{
			m_tui.transform.Find("TUIControl/TUIScrollObj/iap2/Newbie2").GetComponent<TUIButtonClick>().enabled = false;
			m_tui.transform.Find("TUIControl/TUIScrollObj/iap2/Newbie2/soldout").gameObject.SetActive(true);
		}
		else
		{
			m_tui.transform.Find("TUIControl/TUIScrollObj/iap2/Newbie2").GetComponent<TUIButtonClick>().enabled = true;
			m_tui.transform.Find("TUIControl/TUIScrollObj/iap2/Newbie2/soldout").gameObject.SetActive(false);
		}
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
			SceneName.FadeOutLevel(last_scene_to_iap);
		}
		else if (control.name == "Achi_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			SceneName.FadeOutLevel("AchievementTUI");
		}
		else if (control.name == "Option_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			SceneName.FadeOutLevel("OptionTUI");
		}
		else if (control.name.StartsWith("buy_") && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			StartCoroutine(IapBuy(control.GetComponent<IapItemData>().GetIapItem()));
		}
		else if (control.name == "Buy_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			description_text.transform.parent.position = new Vector3(0f, -1000f, -30f);
			if (!CheckBuyReminder())
			{
				StartCoroutine(IapBuy(iapChosenObj.GetComponent<IapItemData>().GetIapItem()));
			}
		}
		else if (control.name == "CancleBuy_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			HideMask(false);
			description_text.transform.parent.position = new Vector3(0f, -1000f, -30f);
		}
		else if (control.name == "Yes_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			HideMsgBox();
			StartCoroutine(IapBuy(iapChosenObj.GetComponent<IapItemData>().GetIapItem()));
		}
		else if (control.name == "Cancel_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			HideMsgBox();
		}
		else if (control.name == "OK_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			msgBoxOK.transform.localPosition = new Vector3(0f, -3000f, msgBoxOK.transform.localPosition.z);
			HideMask(false);
		}
		else if (control.name == "PaperJoy" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			SceneName.FadeOutLevel("PaperJoyAdTUI");
		}
		else if (control.name == "Tapjoy" && eventType == 3)
		{
			if (TapJoyScript.CheckTapjoyObjectExist())
			{
				TapjoyPlugin.ShowOffers();
			}
		}
		else if (control.name == "Experience15" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			StartCoroutine(IapBuy(control.GetComponent<IapItemData>().GetIapItem()));
		}
		else if (control.name == "Experience25" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			StartCoroutine(IapBuy(control.GetComponent<IapItemData>().GetIapItem()));
		}
		else if (control.name == "Newbie1" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			StartCoroutine(IapBuy(control.GetComponent<IapItemData>().GetIapItem()));
		}
		else if (control.name == "Newbie2" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			StartCoroutine(IapBuy(control.GetComponent<IapItemData>().GetIapItem()));
		}
	}

	private bool CheckBuyReminder()
	{
		if (iapChosenObj.GetComponent<IapItemData>().GetIapItem().Name == IAPName.Exp15 || iapChosenObj.GetComponent<IapItemData>().GetIapItem().Name == IAPName.Exp25 || iapChosenObj.GetComponent<IapItemData>().GetIapItem().Name == IAPName.Newbie1 || iapChosenObj.GetComponent<IapItemData>().GetIapItem().Name == IAPName.Newbie2)
		{
			int gameCounterForPlayerExpBuff = GameApp.GetInstance().GetGameState().gameCounterForPlayerExpBuff;
			if ((gameCounterForPlayerExpBuff > 0 && gameCounterForPlayerExpBuff <= 10) || (gameCounterForPlayerExpBuff > 100 && gameCounterForPlayerExpBuff <= 115))
			{
				ShowMsgBox(DialogString.cover_exp_reminder);
				return true;
			}
		}
		return false;
	}

	private void RefreshCashLebel()
	{
		label_money.text_Accessor = "$" + GameApp.GetInstance().GetGameState().GetCash()
			.ToString("N0");
		label_crystal.text_Accessor = GameApp.GetInstance().GetGameState().GetCrystal()
			.ToString("N0");
	}

	public void GetPurchaseStatus()
	{
		if (iapProcessing != IAPName.None)
		{
			int num = IAP.purchaseStatus(null);
			switch (num)
			{
			case 0:
				break;
			case 1:
				Debug.Log("statusCode:" + num);
				GameApp.GetInstance().GetGameState().DeliverIAPItem(iapProcessing);
				HideMask(true);
				iapProcessing = IAPName.None;
				RefreshCashLebel();
				CheckNiewbieSoldout();
				break;
			default:
				Debug.Log("statusCode:" + num);
				HideMask(true);
				iapProcessing = IAPName.None;
				break;
			}
		}
	}

	private IEnumerator IapBuy(IAPItem item)
	{
		if (iapProcessing == IAPName.None)
		{
			IAP.NowPurchaseProduct(item.ID, "1");
			iapProcessing = item.Name;
			Debug.Log("IAP ID:" + item.ID);
			ShowMask(true);
		}
		yield return 0;
	}

	private void ShowMsgBox(string text)
	{
		msgBox_text.transform.parent.position = new Vector3(0f, 0f, -30f);
		msgBox_text.text_Accessor = text;
		ShowMask(false);
	}

	private void HideMsgBox()
	{
		msgBox_text.transform.parent.position = new Vector3(0f, -2000f, -30f);
		HideMask(false);
	}

	private void ShowMask(bool showIndicator)
	{
		mask.transform.localPosition = new Vector3(0f, 0f, mask.transform.localPosition.z);
		if (showIndicator)
		{
			int num = 0;
			num = ((AutoRect.GetPlatform() != Platform.IPad) ? 1 : 0);
			Utils.ShowIndicatorSystem(num, (int)AutoRect.GetPlatform(), 1f, 1f, 1f, 1f);
		}
	}

	private void HideMask(bool hideIndicator)
	{
		mask.transform.localPosition = new Vector3(0f, -1000f, mask.transform.localPosition.z);
		if (hideIndicator)
		{
			Utils.HideIndicatorSystem();
		}
	}

	public void PurchaseFinished(string productId)
	{
		Debug.Log("Panel PurchaseFinished:" + productId);
		GameApp.GetInstance().GetGameState().DeliverIAPItem(iapProcessing);
		HideMask(false);
		iapProcessing = IAPName.None;
		RefreshCashLebel();
	}

	public void PurchaseCanceled(string productId)
	{
		Debug.Log("Panel PurchaseCanceled:" + productId);
		HideMask(false);
		iapProcessing = IAPName.None;
	}
}
