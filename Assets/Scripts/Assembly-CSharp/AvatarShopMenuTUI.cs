using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class AvatarShopMenuTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	public TUIMeshText label_day;

	public TUIMeshText label_money;

	public TUIMeshText label_crystal;

	public GameObject avatar_mover;

	public GameObject scroll_obj;

	public TUIScroll scroll;

	public TUIMeshText Avatar_Info;

	public TUIMeshText Avatar_level_info;

	public GameObject crystal_in_info;

	public TUIMeshText avatar_price;

	public GameObject chosen_frame;

	public TUIMeshText hp_buy_price;

	public GameObject hp_buy_crystal;

	public GameObject hp_buy_button;

	public GameObject action_button;

	public GameObject unlock_button;

	public TUIMeshText unlock_price;

	public GameObject unlock_crystal;

	public GameObject Slider;

	public GetMoneyPanel get_money_panel;

	public GetMoneyPanel maxlevel_dialog_panel;

	public MsgBoxDelegate msgBox;

	public Camera FrameCamera;

	protected AudioPlayer audioPlayer = new AudioPlayer();

	protected float lastMotionTime;

	protected Quaternion cur_rotation = Quaternion.identity;

	protected Vector3 availably_postion = new Vector3(0f, -1000f, 0f);

	protected Vector3 invalid_postion = new Vector3(0f, -2000f, 0f);

	protected List<GameObject> avatar_set = new List<GameObject>();

	protected GameObject weapon;

	protected GameObject avatar;

	protected TUIButtonSelect[] avatar_buttons;

	protected AvatarType avatarUsedBeforeEnteringShop;

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
			GameApp.GetInstance().GetGameState().InitAvatars();
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
		for (int i = 0; i < 19; i++)
		{
			GameObject gameObject = AvatarFactory.GetInstance().CreateAvatar((AvatarType)i);
			gameObject.transform.position = invalid_postion;
			gameObject.transform.rotation = Quaternion.Euler(0f, 150f, 0f);
			gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
			gameObject.name = "AvatarType_" + i;
			avatar_set.Add(gameObject);
		}
		Weapon weapon = GameApp.GetInstance().GetGameState().GetBattleWeapons()[0];
		this.weapon = WeaponFactory.GetInstance().CreateWeaponModel(weapon.Name, invalid_postion, Quaternion.identity);
		this.weapon.transform.localScale *= 0.6f;
		this.weapon.name = weapon.Name;
		avatar_buttons = GameObject.Find("ScrollObjects").GetComponentsInChildren<TUIButtonSelect>();
		for (int j = 0; j < avatar_buttons.Length; j++)
		{
			for (int k = j; k < avatar_buttons.Length; k++)
			{
				if (int.Parse(avatar_buttons[k].gameObject.name.Substring("Avatar_".Length)) == j)
				{
					if (j != k)
					{
						TUIButtonSelect tUIButtonSelect = avatar_buttons[j];
						avatar_buttons[j] = avatar_buttons[k];
						avatar_buttons[k] = tUIButtonSelect;
					}
					break;
				}
			}
		}
		avatarUsedBeforeEnteringShop = GameApp.GetInstance().GetGameState().Avatar;
		InitDisplayAndButtons();
		lastMotionTime = Time.time;
		OpenClikPlugin.Hide();
		Resources.UnloadUnusedAssets();
	}

	private void InitDisplayAndButtons()
	{
		for (int i = 0; i < avatar_buttons.Length; i++)
		{
			AvatarType avatarType = (AvatarType)int.Parse(avatar_buttons[i].gameObject.name.Substring("Avatar_".Length));
			if (avatarType == avatarUsedBeforeEnteringShop)
			{
				ChangeAvatar(avatarUsedBeforeEnteringShop, avatar_buttons[i].gameObject);
				AdaptChosenFrameFirstDisplay(avatar_buttons[i].gameObject);
				avatar_buttons[i].transform.Find("tip").gameObject.GetComponent<TUIMeshSprite>().frameName_Accessor = "logo_equipped";
			}
			else if (GameApp.GetInstance().GetGameState().GetAvatarByType(avatarType)
				.existState == AvatarState.Locked)
			{
				avatar_buttons[i].transform.Find("tip").gameObject.GetComponent<TUIMeshSprite>().frameName_Accessor = "logo_locked";
			}
		}
		for (int j = 0; j < GameApp.GetInstance().GetGameState().unlockNewAvatar.Count; j++)
		{
			AvatarType avatarType2 = GameApp.GetInstance().GetGameState().unlockNewAvatar[j];
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/New_Label"), Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.name = "NewLabel";
			gameObject.transform.parent = avatar_buttons[(int)avatarType2].transform;
			gameObject.transform.localPosition = new Vector3(-33.6f, 21.3f, -0.5f);
		}
		GameApp.GetInstance().GetGameState().unlockNewAvatar.Clear();
	}

	private void OnSceneOut()
	{
	}

	private void OnSceneIn()
	{
		FrameCamera.depth = 1f;
	}

	private void ChangeAvatar(AvatarType type, GameObject control)
	{
		bool flag = false;
		if (avatar != null)
		{
			cur_rotation = avatar.transform.rotation;
			flag = true;
		}
		foreach (GameObject item in avatar_set)
		{
			if (int.Parse(item.name.Substring("AvatarType_".Length)) == (int)type)
			{
				item.transform.position = availably_postion;
				if (flag)
				{
					item.transform.rotation = cur_rotation;
				}
				avatar = item;
				if (avatar_mover != null)
				{
					avatar_mover.GetComponent<ShopUIAvatarMover>().slider_obj = avatar;
				}
				if (this.weapon != null)
				{
					Weapon weapon = GameApp.GetInstance().GetGameState().GetBattleWeapons()[0];
					string weaponNameEnd = Weapon.GetWeaponNameEnd(weapon.GetWeaponType(), weapon.Name);
					avatar.GetComponent<Animation>()["Idle01" + weaponNameEnd].wrapMode = WrapMode.Loop;
					avatar.GetComponent<Animation>().Play("Idle01" + weaponNameEnd);
					this.weapon.transform.parent = avatar.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Weapon_Dummy");
					this.weapon.transform.localPosition = new Vector3(0f, 0f, 0f);
					this.weapon.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
				}
				if (control.transform.Find("NewLabel") != null)
				{
					Object.Destroy(control.transform.Find("NewLabel").gameObject);
				}
			}
			else
			{
				item.transform.position = invalid_postion;
			}
		}
		RefreshDescription(type);
		chosen_frame.GetComponent<ChosenFrameManager>().ChosenObject = control;
	}

	public void Update()
	{
		input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
		UpdateAnimation();
		UpdateSlider();
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
			SceneName.FadeOutLevel("ShopMenuTUI");
		}
		else if (control.name == "Money_getMore" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			//FrameCamera.depth = -5f;
			//SceneName.FadeOutLevel("IapShopTUI");
			//IapShopTUI.last_scene_to_iap = "AvatarShopMenuTUI";
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
		else if (control.name == "Action_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			if (control.gameObject.transform.Find("text_p").GetComponent<TUIMeshText>().text_Accessor == "SELECT")
			{
				AvatarType avatarType = GameApp.GetInstance().GetGameState().Avatar;
				avatar_buttons[(int)avatarType].transform.Find("tip").GetComponent<TUIMeshSprite>().frameName_Accessor = string.Empty;
				avatarType = (AvatarType)int.Parse(avatar.name.Substring("AvatarType_".Length));
				GameApp.GetInstance().GetGameState().Avatar = avatarType;
				avatar_buttons[(int)avatarType].transform.Find("tip").GetComponent<TUIMeshSprite>().frameName_Accessor = "logo_equipped";
				GameApp.GetInstance().Save();
				return;
			}
			if (control.gameObject.transform.Find("text_p").GetComponent<TUIMeshText>().text_Accessor == "GET")
			{
				Application.OpenURL("https://itunes.apple.com/us/app/call-of-mini-zombies-2/id605681399?ls=1&mt=8");
				return;
			}
			AvatarType avatarType2 = (AvatarType)int.Parse(avatar.name.Substring("AvatarType_".Length));
			switch (GameApp.GetInstance().GetGameState().BuyAvatar(avatarType2))
			{
			case BuyStatus.Succeed:
			{
				Debug.Log("avatar buy success.");
				RefreshCashLebel();
				RefreshDescription(avatarType2);
				AvatarType avatarType3 = GameApp.GetInstance().GetGameState().Avatar;
				avatar_buttons[(int)avatarType3].transform.Find("tip").GetComponent<TUIMeshSprite>().frameName_Accessor = string.Empty;
				GameApp.GetInstance().GetGameState().Avatar = avatarType2;
				avatar_buttons[(int)avatarType2].transform.Find("tip").GetComponent<TUIMeshSprite>().frameName_Accessor = "logo_equipped";
				GameApp.GetInstance().Save();
				FlurryStatistics.BuyAvatarEvent(avatarType2, GameApp.GetInstance().GetGameState().GetAvatarByType(avatarType2)
					.aConf.isCrystalBuy);
				break;
			}
			case BuyStatus.NotEnoughCash:
				get_money_panel.SetContent(DialogString.lack_of_money);
				//get_money_panel.Show();
				//FrameCamera.depth = -5f;
				break;
			default:
				get_money_panel.SetContent(DialogString.lack_of_crystal);
				//get_money_panel.Show();
				//FrameCamera.depth = -5f;
				break;
			}
		}
		else if (control.name == "Unlock_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			AvatarType avatarType4 = (AvatarType)int.Parse(avatar.name.Substring("AvatarType_".Length));
			AvatarAttributes avatarByType = GameApp.GetInstance().GetGameState().GetAvatarByType(avatarType4);
			int unlockCrystalNeeded = GetUnlockCrystalNeeded(avatarByType.aConf.unlockDay);
			if (GameApp.GetInstance().GetGameState().GetCrystal() >= unlockCrystalNeeded)
			{
				GameApp.GetInstance().GetGameState().LoseCrystal(unlockCrystalNeeded);
				avatarByType.existState = AvatarState.ToBuy;
				RefreshCashLebel();
				RefreshDescription(avatarType4);
				avatar_buttons[(int)avatarType4].transform.Find("tip").GetComponent<TUIMeshSprite>().frameName_Accessor = string.Empty;
				GameApp.GetInstance().Save();
			}
			else
			{
				get_money_panel.SetContent(DialogString.lack_of_crystal);
				//get_money_panel.Show();
				//FrameCamera.depth = -5f;
			}
		}
		else if (control.name == "HpBuy_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			AvatarType avatarType5 = (AvatarType)int.Parse(avatar.name.Substring("AvatarType_".Length));
			switch (GameApp.GetInstance().GetGameState().BuyAvatarHp(avatarType5))
			{
			case UpgradeStatus.Succeed:
				RefreshCashLebel();
				RefreshDescription(avatarType5);
				GameApp.GetInstance().Save();
				break;
			case UpgradeStatus.MaxLevel:
				maxlevel_dialog_panel.SetContent(DialogString.avatar_max_hp_buy);
				maxlevel_dialog_panel.Show();
				FrameCamera.depth = -5f;
				break;
			case UpgradeStatus.NotEnoughCash:
				get_money_panel.SetContent(DialogString.lack_of_money);
				//get_money_panel.Show();
				//FrameCamera.depth = -5f;
				break;
			}
		}
		else if (control.name == "Iap_yes" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			//FrameCamera.depth = -5f;
			//SceneName.FadeOutLevel("IapShopTUI");
			//IapShopTUI.last_scene_to_iap = "AvatarShopMenuTUI";
		}
		else if (control.name == "Iap_no" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			get_money_panel.Hide();
			FrameCamera.depth = 1f;
		}
		else if (control.name == "maxlevel_ok" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			maxlevel_dialog_panel.Hide();
			FrameCamera.depth = 1f;
		}
		else if (control.name.StartsWith("Avatar_") && eventType == 1)
		{
			AvatarType type = (AvatarType)int.Parse(control.name.Substring("Avatar_".Length));
			ChangeAvatar(type, control.gameObject);
		}
		else if (control.name == "Msg_OK_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = 1f;
			msgBox.Hide();
		}
	}

	private void RefreshCashLebel()
	{
		label_money.text_Accessor = "$" + GameApp.GetInstance().GetGameState().GetCash()
			.ToString("N0");
		label_crystal.text_Accessor = GameApp.GetInstance().GetGameState().GetCrystal()
			.ToString("N0");
	}

	private void RefreshDescription(AvatarType type)
	{
		AvatarAttributes avatarByType = GameApp.GetInstance().GetGameState().GetAvatarByType(type);
		AvatarConfig aConf = avatarByType.aConf;
		Avatar_level_info.text_Accessor = "LV: " + RoundForDisplay(avatarByType.level);
		if (avatarByType.existState == AvatarState.Avaliable)
		{
			Avatar_Info.text_Accessor = "HP: " + RoundForDisplay(avatarByType.maxHp) + "\n";
			TUIMeshText avatar_Info = Avatar_Info;
			string text_Accessor = avatar_Info.text_Accessor;
            avatar_Info.text_Accessor = text_Accessor + "DAMAGE: " + RoundForDisplay(avatarByType.damage) + "\n";
            TUIMeshText avatar_Info2 = Avatar_Info;
			avatar_Info2.text_Accessor = avatar_Info2.text_Accessor + "MOVEMENT SPEED: " + RoundForDisplay(avatarByType.moveSpeed);
		}
		else
		{
			Avatar_Info.text_Accessor = "HP: " + RoundForDisplay(avatarByType.maxHp) + " -> " + avatarByType.GetLastMaxHp() + "\n";
			TUIMeshText avatar_Info3 = Avatar_Info;
			string text_Accessor = avatar_Info3.text_Accessor;
            avatar_Info3.text_Accessor = text_Accessor + "DAMAGE: " + RoundForDisplay(avatarByType.damage) + "\n";
            TUIMeshText avatar_Info4 = Avatar_Info;
			avatar_Info4.text_Accessor = avatar_Info4.text_Accessor + "MOVEMENT SPEED: " + RoundForDisplay(avatarByType.moveSpeed);
		}
		if (avatarByType.existState == AvatarState.Locked)
		{
			crystal_in_info.SetActive(false);
			avatar_price.gameObject.SetActive(true);
			avatar_price.text_Accessor = "COMPLETE DAY " + aConf.unlockDay + " TO UNLOCK";
			avatar_price.fontName_Accessor = "CAI-10";
			action_button.SetActive(false);
			unlock_button.SetActive(true);
			unlock_crystal.SetActive(true);
			unlock_price.gameObject.SetActive(true);
			unlock_price.text_Accessor = GetUnlockCrystalNeeded(aConf.unlockDay).ToString("N0");
			hp_buy_price.gameObject.SetActive(false);
			hp_buy_button.SetActive(false);
			hp_buy_crystal.SetActive(false);
			return;
		}
		if (avatarByType.existState == AvatarState.ToBuy)
		{
			avatar_price.gameObject.SetActive(true);
			if (aConf.isCrystalBuy)
			{
				crystal_in_info.SetActive(true);
				avatar_price.text_Accessor = "   " + aConf.price;
			}
			else
			{
				crystal_in_info.SetActive(false);
				avatar_price.text_Accessor = "$" + aConf.price;
			}
			avatar_price.fontName_Accessor = "CAI-12";
			action_button.SetActive(true);
			action_button.transform.Find("text_n").GetComponent<TUIMeshText>().text_Accessor = "BUY";
			action_button.transform.Find("text_p").GetComponent<TUIMeshText>().text_Accessor = "BUY";
			unlock_button.SetActive(false);
			unlock_crystal.SetActive(false);
			unlock_price.gameObject.SetActive(false);
			hp_buy_price.gameObject.SetActive(false);
			hp_buy_button.SetActive(false);
			hp_buy_crystal.SetActive(false);
			return;
		}
		crystal_in_info.SetActive(false);
		if (avatarByType.level < UpgradeParas.AvatarMaxLevel)
		{
			avatar_price.gameObject.SetActive(true);
			avatar_price.text_Accessor = "EXP: " + avatarByType.GetExpPercent() + "%";
			avatar_price.fontName_Accessor = "CAI-12";
		}
		else
		{
			avatar_price.gameObject.SetActive(false);
		}
		action_button.SetActive(true);
		action_button.transform.Find("text_n").GetComponent<TUIMeshText>().text_Accessor = "SELECT";
		action_button.transform.Find("text_p").GetComponent<TUIMeshText>().text_Accessor = "SELECT";
		unlock_button.SetActive(false);
		unlock_crystal.SetActive(false);
		unlock_price.gameObject.SetActive(false);
		hp_buy_button.SetActive(true);
		if (avatarByType.hpBuyCount < UpgradeParas.AvatarHpMaxLevel)
		{
			hp_buy_crystal.SetActive(true);
			hp_buy_price.gameObject.SetActive(true);
			hp_buy_price.text_Accessor = avatarByType.hpBuyPrice.ToString();
			hp_buy_button.transform.Find("text_n").GetComponent<TUIMeshText>().text_Accessor = avatarByType.hpBuyNext + "HP";
			hp_buy_button.transform.Find("text_p").GetComponent<TUIMeshText>().text_Accessor = avatarByType.hpBuyNext + "HP";
		}
		else
		{
			hp_buy_crystal.SetActive(false);
			hp_buy_price.gameObject.SetActive(false);
			hp_buy_button.transform.Find("text_n").GetComponent<TUIMeshText>().text_Accessor = "MAX HP";
			hp_buy_button.transform.Find("text_p").GetComponent<TUIMeshText>().text_Accessor = "MAX HP";
		}
	}

	private void UpdateSlider()
	{
		float num = -76f + scroll.position.y / scroll.rangeYMax * 182f;
		Slider.transform.position = new Vector3(Slider.transform.position.x, 0f - num + Slider.transform.parent.position.y, Slider.transform.position.z);
	}

	private void AdaptChosenFrameFirstDisplay(GameObject current_button)
	{
		if (GameApp.GetInstance().GetGameState().show_avatar_update)
		{
			for (int i = 0; i < 1; i++)
			{
				GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/New_Label"), Vector3.zero, Quaternion.identity) as GameObject;
				gameObject.name = "NewLabel";
				gameObject.transform.parent = avatar_buttons[19 - i - 1].transform;
				gameObject.transform.localPosition = new Vector3(-33.6f, 21.3f, -1.5f);
			}
			float y = 0f - avatar_buttons[18].transform.localPosition.y - 72.5f;
			scroll.position = new Vector2(0f, y);
			GameObject.Find("ScrollObjects").transform.localPosition = new Vector3(0f, y, 0f);
			UpdateSlider();
			GameApp.GetInstance().GetGameState().show_avatar_update = false;
		}
		else if ((double)current_button.transform.localPosition.y < -72.5)
		{
			float y2 = 0f - current_button.gameObject.transform.localPosition.y - 72.5f;
			scroll.position = new Vector2(0f, y2);
			GameObject.Find("ScrollObjects").transform.localPosition = new Vector3(0f, y2, 0f);
			UpdateSlider();
		}
	}

	public static int GetUnlockCrystalNeeded(int unlockDay)
	{
		return unlockDay - GameApp.GetInstance().GetGameState().LevelNum;
	}

    private string RoundForDisplay(float val, int decimals = 0)
    {
        float factor = Mathf.Pow(10, decimals);
        return (Mathf.Round(val * factor) / factor).ToString();
    }
}
