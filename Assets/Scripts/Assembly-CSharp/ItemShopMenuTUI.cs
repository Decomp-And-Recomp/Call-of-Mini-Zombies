using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class ItemShopMenuTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	private ItemType itemChosenBeforeEntering;

	public static int ItemShopIndex;

	public TUIMeshText label_day;

	public TUIMeshText label_money;

	public TUIMeshText label_crystal;

	public GameObject item_mover;

	public GameObject scroll_obj;

	public TUIScroll scroll;

	public TUIMeshText item_Info;

	public GameObject chosen_frame;

	public TUIMeshText item_owned_count;

	public GameObject crystal_in_info;

	public TUIMeshText item_cost;

	public GameObject buy_button;

	public GameObject select_button;

	public GameObject Slider;

	public GetMoneyPanel get_money_panel;

	public Camera FrameCamera;

	protected AudioPlayer audioPlayer = new AudioPlayer();

	protected Quaternion cur_rotation = Quaternion.identity;

	protected Vector3 availably_postion = new Vector3(0f, -1000f, 0f);

	protected Vector3 invalid_postion = new Vector3(0f, -2000f, 0f);

	protected GameObject item;

	protected List<GameObject> item_set = new List<GameObject>();

	protected AlignFrameCamera[] alignCameras;

	private void Awake()
	{
		GameScript.CheckFillBlack();
		GameScript.CheckMenuResourceConfig();
		GameScript.CheckGlobalResourceConfig();
		if (GameObject.Find("Music") == null)
		{
			GameApp.GetInstance().InitForMenu();
			GameApp.GetInstance().GetGameState().InitItems();
			GameApp.GetInstance().GetGameState().InitWeapons();
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
		FrameCamera.orthographicSize = AutoRect.GetFakeScreenWidthHeightRatio() * 2.25f;
		FrameCamera.depth = -5f;
	}

	public void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFadeEx>()
			.FadeIn();
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFadeEx>()
			.m_fadein = OnSceneIn;
		Transform folderTrans = base.transform.Find("Audio");
		audioPlayer.AddAudio(folderTrans, "Button", true);
		audioPlayer.AddAudio(folderTrans, "Back", true);
		RefreshCashLebel();
		label_day.text_Accessor = "DAY " + GameApp.GetInstance().GetGameState().LevelNum;
		alignCameras = GameObject.Find("ScrollObjects").GetComponentsInChildren<AlignFrameCamera>(true);
		for (int i = 0; i < alignCameras.Length; i++)
		{
			for (int j = i; j < alignCameras.Length; j++)
			{
				if (int.Parse(alignCameras[j].gameObject.name.Substring("Item_".Length)) == i)
				{
					if (i != j)
					{
						AlignFrameCamera alignFrameCamera = alignCameras[i];
						alignCameras[i] = alignCameras[j];
						alignCameras[j] = alignFrameCamera;
					}
					break;
				}
			}
		}
		if (ItemShopIndex >= GameApp.GetInstance().GetGameState().GetItemsCarried()
			.Count)
		{
			itemChosenBeforeEntering = ItemType.SmallHp;
		}
		else
		{
			itemChosenBeforeEntering = GameApp.GetInstance().GetGameState().GetItemsCarried()[ItemShopIndex].iType;
		}
		InitDisplayAndButtons();
		OpenClikPlugin.Hide();
		Resources.UnloadUnusedAssets();
	}

	private void AdaptWeaponModelDisplay(GameObject m_obj)
	{
		switch (m_obj.name)
		{
		case "Power":
			m_obj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
			break;
		case "SuicideGun":
			m_obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			break;
		default:
			m_obj.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
			break;
		}
	}

	private void InitDisplayAndButtons()
	{
		for (int i = 0; i < Item.GameItemCollection.Length; i++)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/Item/" + Item.GameItemCollection[i])) as GameObject;
			GameObject gameObject2 = Object.Instantiate(gameObject.GetComponent<ItemConfigScript>().Item_Instance, invalid_postion, Quaternion.identity) as GameObject;
			gameObject2.name = Item.GameItemCollection[i].ToString();
			if (gameObject2.GetComponent<ItemScript>() != null)
			{
				Object.Destroy(gameObject2.GetComponent<ItemScript>());
			}
			Object.Destroy(gameObject);
			gameObject2.AddComponent<ItemItemData>().SetItem(GameApp.GetInstance().GetGameState().GetItemByType(Item.GameItemCollection[i]));
			AdaptWeaponModelDisplay(gameObject2);
			item_set.Add(gameObject2);
			if (itemChosenBeforeEntering == Item.GameItemCollection[i])
			{
				ChangeItem(itemChosenBeforeEntering, alignCameras[i].gameObject);
				AdaptChosenFrameFirstDisplay(alignCameras[i].gameObject);
				alignCameras[i].transform.Find("tip").GetComponent<TUIMeshSprite>().frameName_Accessor = "logo_equipped";
			}
			else if (GameApp.GetInstance().GetGameState().GetItemByType(Item.GameItemCollection[i])
				.isCarryInGame)
			{
				alignCameras[i].transform.Find("tip").GetComponent<TUIMeshSprite>().frameName_Accessor = "logo_equipped";
			}
		}
	}

	private void OnSceneOut()
	{
	}

	private void OnSceneIn()
	{
		FrameCamera.depth = 1f;
	}

	private void ChangeItem(ItemType type, GameObject control)
	{
		bool flag = false;
		if (item != null)
		{
			cur_rotation = item.transform.rotation;
			flag = true;
		}
		foreach (GameObject item in item_set)
		{
			if (item.GetComponent<ItemItemData>().GetItem().iType == type)
			{
				item.transform.position = availably_postion;
				if (flag)
				{
					item.transform.rotation = cur_rotation;
				}
				this.item = item;
				if (item_mover != null)
				{
					item_mover.GetComponent<ShopUIAvatarMover>().slider_obj = this.item;
				}
			}
			else
			{
				item.transform.position = invalid_postion;
			}
		}
		AlignFrameCamera[] array = alignCameras;
		foreach (AlignFrameCamera alignFrameCamera in array)
		{
			if (alignFrameCamera.gameObject.name != control.name)
			{
				alignFrameCamera.enabled = false;
				continue;
			}
			alignFrameCamera.enabled = true;
			alignFrameCamera.SetCamera();
		}
		chosen_frame.GetComponent<ChosenFrameManager>().ChosenObject = control;
		RefreshDescription(type);
	}

	public void Update()
	{
		input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
		UpdateSlider();
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Back");
			FrameCamera.depth = -5f;
			SceneName.FadeOutLevel("ShopMenuTUI");
		}
		else if (control.name == "Money_getMore" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			//FrameCamera.depth = -5f;
			//SceneName.FadeOutLevel("IapShopTUI");
			//IapShopTUI.last_scene_to_iap = "ItemShopMenuTUI";
		}
		else if (control.name == "Achi_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			SceneName.FadeOutLevel("AchievementTUI");
		}
		else if (control.name == "Option_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			SceneName.FadeOutLevel("OptionTUI");
		}
		else if (control.name == "Buy_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			switch (GameApp.GetInstance().GetGameState().BuyItem(item.GetComponent<ItemItemData>().GetItem()))
			{
			case BuyStatus.NotEnoughCash:
				get_money_panel.SetContent(DialogString.lack_of_money);
				//get_money_panel.Show();
				//FrameCamera.depth = -5f;
				break;
			case BuyStatus.NotEnoughCrystal:
				get_money_panel.SetContent(DialogString.lack_of_crystal);
				//get_money_panel.Show();
				//FrameCamera.depth = -5f;
				break;
			case BuyStatus.Succeed:
				RefreshOwnedCount();
				GameApp.GetInstance().Save();
				FlurryStatistics.BuyItems(item.GetComponent<ItemItemData>().GetItem());
				break;
			}
			RefreshCashLebel();
		}
		else if (control.name == "Select_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			SelectItem();
		}
		else if (control.name == "Iap_yes" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			//FrameCamera.depth = -5f;
			//SceneName.FadeOutLevel("IapShopTUI");
			//IapShopTUI.last_scene_to_iap = "ItemShopMenuTUI";
		}
		else if (control.name == "Iap_no" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			get_money_panel.Hide();
			FrameCamera.depth = 1f;
		}
		else if (control.name.StartsWith("Item_") && eventType == 1)
		{
			audioPlayer.PlayAudio("Button");
			ChangeItem(Item.GameItemCollection[int.Parse(control.name.Substring("Item_".Length))], control.gameObject);
		}
	}

	private void SelectItem()
	{
		Item item = GameApp.GetInstance().GetGameState().GetItemsCarried()[ItemShopIndex];
		Item item2 = this.item.GetComponent<ItemItemData>().GetItem();
		if (item.iType == item2.iType)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < 2; i++)
		{
			if (GameApp.GetInstance().GetGameState().GetItemsCarried()[i].iType == item2.iType)
			{
				item.carryPacketIndex = i + 1;
				item2.carryPacketIndex = ItemShopIndex + 1;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			item2.isCarryInGame = true;
			item2.carryPacketIndex = ItemShopIndex + 1;
			alignCameras[Item.GetItemCollectionIndexByType(item2.iType)].transform.Find("tip").GetComponent<TUIMeshSprite>().frameName_Accessor = "logo_equipped";
			item.isCarryInGame = false;
			item.carryPacketIndex = 0;
			alignCameras[Item.GetItemCollectionIndexByType(item.iType)].transform.Find("tip").GetComponent<TUIMeshSprite>().frameName_Accessor = string.Empty;
		}
		GameApp.GetInstance().Save();
	}

	private void RefreshCashLebel()
	{
		label_money.text_Accessor = "$" + GameApp.GetInstance().GetGameState().GetCash()
			.ToString("N0");
		label_crystal.text_Accessor = GameApp.GetInstance().GetGameState().GetCrystal()
			.ToString("N0");
	}

	private void RefreshDescription(ItemType type)
	{
		ItemConfig iConf = item.GetComponent<ItemItemData>().GetItem().iConf;
		switch (type)
		{
		case ItemType.SmallHp:
			item_Info.text_Accessor = "RESTORES 300 HP.";
			item_cost.text_Accessor = "\n$" + iConf.price.ToString("N0") + "   x" + iConf.buyCount;
			crystal_in_info.SetActive(false);
			break;
		case ItemType.BigHp:
			item_Info.text_Accessor = "RESTORES FULL HP.";
			item_cost.text_Accessor = "\n   " + iConf.price.ToString("N0") + "   x" + iConf.buyCount;
			crystal_in_info.SetActive(true);
			break;
		case ItemType.Shield:
			item_Info.text_Accessor = "REDUCES DAMAGE TAKEN BY \n90% FOR 10 SECONDS.";
			item_cost.text_Accessor = "\n   " + iConf.price.ToString("N0") + "   x" + iConf.buyCount;
			crystal_in_info.SetActive(true);
			break;
		case ItemType.SuicideGun:
			item_Info.text_Accessor = "RAIN BOMBS ON EVERYONE. \nTAKE COVER, YOU'LL GET \nHURT TOO.";
			item_cost.text_Accessor = "\n   " + iConf.price.ToString("N0") + "   x" + iConf.buyCount;
			crystal_in_info.SetActive(true);
			break;
		case ItemType.Power:
			item_Info.text_Accessor = "DOUBLES DAMAGE.";
			item_cost.text_Accessor = "\n$" + iConf.price.ToString("N0") + "   x" + iConf.buyCount;
			crystal_in_info.SetActive(false);
			break;
		case ItemType.InstantSuper:
			item_Info.text_Accessor = "ENLARGES YOUR SIZE AND \nINCREASES MAXIMUM HP \nFOR 15 SECONDS.";
			item_cost.text_Accessor = "\n   " + iConf.price.ToString("N0") + "   x" + iConf.buyCount;
			crystal_in_info.SetActive(true);
			break;
		case ItemType.InstantStealth:
			item_Info.text_Accessor = "TURN INVISIBLE FOR \n20 SECONDS. (INEFFECTIVE \nAGAINST ZOMBIES)";
			item_cost.text_Accessor = "\n   " + iConf.price.ToString("N0") + "   x" + iConf.buyCount;
			crystal_in_info.SetActive(true);
			break;
		}
		RefreshOwnedCount();
	}

	private void RefreshOwnedCount()
	{
		int ownedCount = item.GetComponent<ItemItemData>().GetItem().OwnedCount;
		item_owned_count.text_Accessor = "OWN:" + ownedCount;
		if (ownedCount + item.GetComponent<ItemItemData>().GetItem().iConf.buyCount > Item.MaxOwnedCount)
		{
			buy_button.SetActive(false);
			select_button.SetActive(true);
		}
		else if (ownedCount == 0)
		{
			buy_button.SetActive(true);
			select_button.SetActive(false);
		}
		else
		{
			buy_button.SetActive(true);
			select_button.SetActive(true);
		}
	}

	private void UpdateSlider()
	{
		float num = -76f + scroll.position.y / scroll.rangeYMax * 182f;
		Slider.transform.position = new Vector3(Slider.transform.position.x, 0f - num + Slider.transform.parent.position.y, Slider.transform.position.z);
	}

	private void AdaptChosenFrameFirstDisplay(GameObject current_button)
	{
		if ((double)current_button.gameObject.transform.localPosition.y < -72.5)
		{
			float y = 0f - current_button.gameObject.transform.localPosition.y - 72.5f;
			scroll.position = new Vector2(0f, y);
			GameObject.Find("ScrollObjects").transform.localPosition = new Vector3(0f, y, 0f);
			UpdateSlider();
		}
	}
}
