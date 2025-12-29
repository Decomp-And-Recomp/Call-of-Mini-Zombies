using UnityEngine;
using Zombie3D;

public class ShopMenuTUI : MonoBehaviour, TUIHandler
{
	public static string last_map_to_shop = string.Empty;

	public static ShopMenu chosen_shop;

	private TUI m_tui;

	protected TUIInput[] input;

	public TUIMeshText label_day;

	public TUIMeshText label_money;

	public TUIMeshText label_crystal;

	public GameObject chosen_frame;

	public GameObject button_avatar;

	public GameObject[] button_weapons;

	public GameObject[] button_items;

	public TUIMeshText[] count_items;

	protected GameObject avatar;

	public GameObject avatar_mover;

	public GameObject Msg_Box;

	public Camera FrameCamera;

	protected AudioPlayer audioPlayer = new AudioPlayer();

	protected float lastMotionTime;

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
		label_money.text_Accessor = "$" + GameApp.GetInstance().GetGameState().GetCash()
			.ToString("N0");
		label_crystal.text_Accessor = GameApp.GetInstance().GetGameState().GetCrystal()
			.ToString("N0");
		label_day.text_Accessor = "DAY " + GameApp.GetInstance().GetGameState().LevelNum;
		GameObject gameObject = AvatarFactory.GetInstance().CreateAvatar(GameApp.GetInstance().GetGameState().Avatar);
		gameObject.transform.position = new Vector3(0f, -1000f, 0f);
		gameObject.transform.rotation = Quaternion.Euler(0f, 150f, 0f);
		avatar = gameObject;
		if (avatar_mover != null)
		{
			avatar_mover.GetComponent<ShopUIAvatarMover>().slider_obj = gameObject;
		}
		Weapon weapon = GameApp.GetInstance().GetGameState().GetBattleWeapons()[0];
		string weaponNameEnd = Weapon.GetWeaponNameEnd(weapon.GetWeaponType(), weapon.Name);
		GameObject gameObject2 = WeaponFactory.GetInstance().CreateWeaponModel(weapon.Name, gameObject.transform.position, gameObject.transform.rotation);
		Transform parent = gameObject.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Weapon_Dummy");
		gameObject2.transform.parent = parent;
		gameObject.GetComponent<Animation>()["Idle01" + weaponNameEnd].wrapMode = WrapMode.Loop;
		gameObject.GetComponent<Animation>().Play("Idle01" + weaponNameEnd);
		SetAvatarButtonFrame();
		SetWeaponButtonFrame();
		SetItemButtonFrame();
		LocateChosenFrame();
		lastMotionTime = Time.time;
		FlurryStatistics.EnterShopEvent();
		OpenClikPlugin.Hide();
		Resources.UnloadUnusedAssets();
		NetworkObj.DestroyNetCom();
	}

	private void LocateChosenFrame()
	{
		switch (chosen_shop)
		{
		case ShopMenu.AvatarShop:
			chosen_frame.GetComponent<ChosenFrameManager>().ChosenObject = button_avatar;
			break;
		case ShopMenu.WeaponShop1:
			chosen_frame.GetComponent<ChosenFrameManager>().ChosenObject = button_weapons[0];
			break;
		case ShopMenu.WeaponShop2:
			chosen_frame.GetComponent<ChosenFrameManager>().ChosenObject = button_weapons[1];
			break;
		case ShopMenu.WeaponShop3:
			chosen_frame.GetComponent<ChosenFrameManager>().ChosenObject = button_weapons[2];
			break;
		case ShopMenu.ItemShop1:
			chosen_frame.GetComponent<ChosenFrameManager>().ChosenObject = button_items[0];
			break;
		case ShopMenu.ItemShop2:
			chosen_frame.GetComponent<ChosenFrameManager>().ChosenObject = button_items[1];
			break;
		}
	}

	private void SetAvatarButtonFrame()
	{
		button_avatar.transform.Find("button_n").GetComponent<TUIMeshSprite>().frameName_Accessor = "Avatar_" + (int)GameApp.GetInstance().GetGameState().Avatar;
		if (GameApp.GetInstance().GetGameState().show_avatar_update || GameApp.GetInstance().GetGameState().unlockNewAvatar.Count > 0)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/New_Label"), Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.name = "NewLabel";
			gameObject.transform.parent = button_avatar.transform;
			gameObject.transform.localPosition = new Vector3(-33.6f, 21.3f, -0.5f);
		}
	}

	private void SetWeaponButtonFrame()
	{
		int count = GameApp.GetInstance().GetGameState().GetBattleWeapons()
			.Count;
		for (int i = 0; i < 3; i++)
		{
			if (i < count)
			{
				button_weapons[i].transform.Find("button_n").GetComponent<TUIMeshSprite>().frameName_Accessor = "weapon_" + GameApp.GetInstance().GetGameState().GetBattleWeapons()[i].Name;
			}
			else
			{
				button_weapons[i].transform.Find("button_n").GetComponent<TUIMeshSprite>().frameName_Accessor = "empty_button";
			}
		}
		if (GameApp.GetInstance().GetGameState().show_weapon_update || GameApp.GetInstance().GetGameState().unlockNewWeapon.Count > 0)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/New_Label"), Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.name = "NewLabel";
			gameObject.transform.parent = button_weapons[0].transform;
			gameObject.transform.localPosition = new Vector3(-33.6f, 21.3f, -0.5f);
		}
	}

	private void SetItemButtonFrame()
	{
		for (int i = 0; i < 2; i++)
		{
			Item item = GameApp.GetInstance().GetGameState().GetItemsCarried()[i];
			button_items[i].transform.Find("button_n").GetComponent<TUIMeshSprite>().frameName_Accessor = "item_" + item.iType;
			count_items[i].text_Accessor = "x" + item.GetCarryInGameCount();
		}
	}

	public void Update()
	{
		input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
		if (avatar != null)
		{
			UpdateAnimation();
		}
	}

	private void OnSceneIn()
	{
		FrameCamera.depth = 1f;
		if (GameApp.GetInstance().GetGameState().AlreadyReviewCountered)
		{
			return;
		}
		if (GameApp.GetInstance().GetGameState().review_count < int.MaxValue)
		{
			GameApp.GetInstance().GetGameState().review_count++;
			if (GameApp.GetInstance().GetGameState().review_count == 3)
			{
				FrameCamera.depth = -5f;
				Msg_Box.GetComponent<MsgBoxDelegate>().Show("Having fun? Rate this app!", MsgBoxType.None);
			}
			GameApp.GetInstance().PlayerPrefsSave();
		}
		GameApp.GetInstance().GetGameState().AlreadyReviewCountered = true;
	}

	public void UpdateAnimation()
	{
		Weapon weapon = GameApp.GetInstance().GetGameState().GetBattleWeapons()[0];
		string weaponNameEnd = Weapon.GetWeaponNameEnd(weapon.GetWeaponType(), weapon.Name);
		if (!(avatar != null))
		{
			return;
		}
		if (weapon.GetWeaponType() == WeaponType.RocketLauncher || weapon.GetWeaponType() == WeaponType.Sniper)
		{
			if (Time.time - lastMotionTime > 7f)
			{
				string empty = string.Empty;
				empty = ((!avatar.GetComponent<Animation>().IsPlaying("Run01" + weaponNameEnd)) ? ("Run01" + weaponNameEnd) : ("Idle01" + weaponNameEnd));
				avatar.GetComponent<Animation>()[empty].wrapMode = WrapMode.Loop;
				avatar.GetComponent<Animation>().CrossFade(empty);
				lastMotionTime = Time.time;
			}
			return;
		}
		if (weapon.GetWeaponType() == WeaponType.Saw || weapon.GetWeaponType() == WeaponType.Sword)
		{
			if (Time.time - lastMotionTime > 7f)
			{
				avatar.GetComponent<Animation>()["Shoot01_Saw2"].wrapMode = WrapMode.ClampForever;
				avatar.GetComponent<Animation>().CrossFade("Shoot01_Saw");
				avatar.GetComponent<Animation>().CrossFadeQueued("Shoot01_Saw2");
				lastMotionTime = Time.time;
			}
			if (avatar.GetComponent<Animation>().IsPlaying("Shoot01_Saw2") && Time.time - lastMotionTime > avatar.GetComponent<Animation>()["Shoot01_Saw2"].clip.length * 2f)
			{
				avatar.GetComponent<Animation>().CrossFade("Idle01" + weaponNameEnd);
			}
			return;
		}
		string text = "Standby03";
		if (weaponNameEnd == "_Shotgun")
		{
			text += weaponNameEnd;
		}
		if (Time.time - lastMotionTime > 7f)
		{
			avatar.GetComponent<Animation>()[text].wrapMode = WrapMode.ClampForever;
			avatar.GetComponent<Animation>().CrossFade(text);
			lastMotionTime = Time.time;
		}
		if (avatar.GetComponent<Animation>()[text].time > avatar.GetComponent<Animation>()[text].clip.length)
		{
			avatar.GetComponent<Animation>().CrossFade("Idle01" + weaponNameEnd);
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Back");
			FrameCamera.depth = -5f;
			string text = last_map_to_shop;
			if (text != string.Empty)
			{
				SceneName.FadeOutLevel(text);
			}
		}
		else if (control.name == "Money_getMore" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			//FrameCamera.depth = -5f;
			//SceneName.FadeOutLevel("IapShopTUI");
			//IapShopTUI.last_scene_to_iap = "ShopMenuTUI";
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
		else if (control.name == "AvatarShop" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			chosen_shop = ShopMenu.AvatarShop;
			LocateChosenFrame();
			if (control.transform.Find("NewLabel") != null)
			{
				Object.Destroy(control.transform.Find("NewLabel").gameObject);
			}
			SceneName.FadeOutLevel("AvatarShopMenuTUI");
		}
		else if (control.name == "WeaponShop_1" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			chosen_shop = ShopMenu.WeaponShop1;
			LocateChosenFrame();
			if (control.transform.Find("NewLabel") != null)
			{
				Object.Destroy(control.transform.Find("NewLabel").gameObject);
			}
			WeaponShopMenuTUI.weaponShopIndex = 0;
			SceneName.FadeOutLevel("WeaponShopMenuTUI");
		}
		else if (control.name == "WeaponShop_2" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			chosen_shop = ShopMenu.WeaponShop2;
			LocateChosenFrame();
			WeaponShopMenuTUI.weaponShopIndex = 1;
			SceneName.FadeOutLevel("WeaponShopMenuTUI");
		}
		else if (control.name == "WeaponShop_3" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			chosen_shop = ShopMenu.WeaponShop3;
			LocateChosenFrame();
			WeaponShopMenuTUI.weaponShopIndex = 2;
			SceneName.FadeOutLevel("WeaponShopMenuTUI");
		}
		else if (control.name == "ItemShop_1" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			chosen_shop = ShopMenu.ItemShop1;
			LocateChosenFrame();
			ItemShopMenuTUI.ItemShopIndex = 0;
			SceneName.FadeOutLevel("ItemShopMenuTUI");
		}
		else if (control.name == "ItemShop_2" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			chosen_shop = ShopMenu.ItemShop2;
			LocateChosenFrame();
			ItemShopMenuTUI.ItemShopIndex = 1;
			SceneName.FadeOutLevel("ItemShopMenuTUI");
		}
		else if (control.name == "Msg_Yes_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = 1f;
			Msg_Box.GetComponent<MsgBoxDelegate>().Hide();
			GameApp.GetInstance().GetGameState().AddCash(500);
			GameApp.GetInstance().Save();
			Application.OpenURL("http://itunes.apple.com/us/app/call-of-mini-zombies/id431213733?mt=8");
		}
		else if (control.name == "Msg_No_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = 1f;
			Msg_Box.GetComponent<MsgBoxDelegate>().Hide();
			GameApp.GetInstance().GetGameState().review_count = 0;
			GameApp.GetInstance().PlayerPrefsSave();
		}
	}
}
