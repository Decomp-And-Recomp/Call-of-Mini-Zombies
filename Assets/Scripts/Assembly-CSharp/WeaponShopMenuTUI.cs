using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class WeaponShopMenuTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	private string weaponChosenBeforeEntering;

	public static int weaponShopIndex;

	public TUIMeshText label_day;

	public TUIMeshText label_money;

	public TUIMeshText label_crystal;

	public GameObject weapon_mover;

	public GameObject scroll_obj;

	public TUIScroll scroll;

	public TUIMeshText weapon_Info;

	public TUIMeshText ammo_Info;

	public TUIMeshText weapon_level;

	public TUIMeshText weapon_price;

	public TUIMeshText ammo_price;

	public GameObject crystal_in_info;

	public GameObject chosen_frame;

	public TUIMeshText upgrade_price;

	public GameObject bullet_logo;

	public TUIMeshText bullet_count;

	public GameObject[] alignCameras;

	public GameObject buy_ammo_button;

	public GameObject upgrade_button;

	public GameObject action_button;

	public GameObject Slider;

	public GetMoneyPanel get_money_panel;

	public GetMoneyPanel maxlevel_dialog_panel;

	public Camera FrameCamera;

	protected AudioPlayer audioPlayer = new AudioPlayer();

	protected Vector3 availably_postion = new Vector3(0f, -1000f, 0f);

	protected Vector3 invalid_postion = new Vector3(0f, -2000f, 0f);

	protected List<GameObject> weapon_set = new List<GameObject>();

	protected GameObject weapon;

	protected GameObject avatar;

	protected float lastMotionTime;

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
		audioPlayer.AddAudio(folderTrans, "Upgrade", true);
		audioPlayer.AddAudio(folderTrans, "Bullet", true);
		audioPlayer.AddAudio(folderTrans, "Back", true);
		RefreshCashLebel();
		label_day.text_Accessor = "DAY " + GameApp.GetInstance().GetGameState().LevelNum;
		avatar = AvatarFactory.GetInstance().CreateAvatar(GameApp.GetInstance().GetGameState().Avatar);
		avatar.transform.position = availably_postion;
		avatar.transform.rotation = Quaternion.Euler(0f, 140f, 0f);
		avatar.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		if (weapon_mover != null)
		{
			weapon_mover.GetComponent<ShopUIAvatarMover>().slider_obj = avatar;
		}
		if (GameApp.GetInstance().GetGameState().RectToWeaponMap[weaponShopIndex] == -1)
		{
			weaponChosenBeforeEntering = string.Empty;
		}
		else
		{
			weaponChosenBeforeEntering = GameApp.GetInstance().GetGameState().GetBattleWeapons()[weaponShopIndex].Name;
		}
		InitDisplayAndButtons();
		lastMotionTime = Time.time;
		OpenClikPlugin.Hide();
		Resources.UnloadUnusedAssets();
	}

	private void InitDisplayAndButtons()
	{
		List<Weapon> weapons = GameApp.GetInstance().GetGameState().GetWeapons();
		for (int i = 0; i < weapons.Count; i++)
		{
			GameObject gameObject = WeaponFactory.GetInstance().CreateWeaponModel(weapons[i].Name, Vector3.zero, Quaternion.identity);
			gameObject.name = weapons[i].Name;
			gameObject.AddComponent<WeaponItemData>().SetWeapon(weapons[i]);
			gameObject.transform.parent = avatar.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Weapon_Dummy");
			gameObject.transform.localPosition = invalid_postion;
			gameObject.transform.localScale *= 0.5f;
			weapon_set.Add(gameObject);
			if (weapons[i].Name == weaponChosenBeforeEntering)
			{
				ChangeWeapon(weapons[i].Name, alignCameras[weapons[i].weapon_index]);
				AdaptChosenFrameFirstDisplay(alignCameras[weapons[i].weapon_index]);
				alignCameras[i].transform.Find("tip").gameObject.GetComponent<TUIMeshSprite>().frameName_Accessor = "logo_equipped";
			}
			else if (weapons[i].IsSelectedForBattle)
			{
				alignCameras[i].transform.Find("tip").gameObject.GetComponent<TUIMeshSprite>().frameName_Accessor = "logo_equipped";
			}
			else if (weapons[i].Exist == WeaponExistState.Locked)
			{
				alignCameras[i].transform.Find("tip").gameObject.GetComponent<TUIMeshSprite>().frameName_Accessor = "logo_locked";
			}
		}
		if (weaponChosenBeforeEntering == string.Empty)
		{
			ChangeWeapon(weapons[0].Name, alignCameras[weapons[0].weapon_index]);
			AdaptChosenFrameFirstDisplay(alignCameras[0]);
		}
		for (int j = 0; j < GameApp.GetInstance().GetGameState().unlockNewWeapon.Count; j++)
		{
			int num = GameApp.GetInstance().GetGameState().unlockNewWeapon[j];
			GameObject gameObject2 = Object.Instantiate(Resources.Load("Prefabs/TUI/New_Label"), Vector3.zero, Quaternion.identity) as GameObject;
			gameObject2.name = "NewLabel";
			gameObject2.transform.parent = alignCameras[num].transform;
			gameObject2.transform.localPosition = new Vector3(-33.6f, 21.3f, -0.5f);
		}
		GameApp.GetInstance().GetGameState().unlockNewWeapon.Clear();
	}

	private void OnSceneIn()
	{
		FrameCamera.depth = 1f;
	}

	private void ChangeWeapon(string name, GameObject control)
	{
		foreach (GameObject item in weapon_set)
		{
			if (item.name == name)
			{
				weapon = item;
				string weaponNameEnd = Weapon.GetWeaponNameEnd(weapon.GetComponent<WeaponItemData>().GetWeapon().GetWeaponType(), weapon.name);
				avatar.GetComponent<Animation>()["Idle01" + weaponNameEnd].wrapMode = WrapMode.Loop;
				avatar.GetComponent<Animation>().Play("Idle01" + weaponNameEnd);
				weapon.transform.localPosition = Vector3.zero;
				weapon.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
				if (control.transform.Find("NewLabel") != null)
				{
					Object.Destroy(control.transform.Find("NewLabel").gameObject);
				}
			}
			else
			{
				item.transform.localPosition = invalid_postion;
			}
		}
		UpdateAllActionButtons(weapon.GetComponent<WeaponItemData>().GetWeapon());
		RefreshDescription(weapon.GetComponent<WeaponItemData>().GetWeapon());
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
		Weapon weapon = this.weapon.GetComponent<WeaponItemData>().GetWeapon();
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
			//IapShopTUI.last_scene_to_iap = "WeaponShopMenuTUI";
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
		else if (control.name == "BuyAmmo_Button" && eventType == 3)
		{
			Weapon weapon = this.weapon.GetComponent<WeaponItemData>().GetWeapon();
			if (weapon.BulletCount < weapon.MaxCapacity)
			{
				if (GameApp.GetInstance().GetGameState().BuyBullets(weapon, weapon.WConf.bulletEachAdd, weapon.WConf.bulletPrice))
				{
					audioPlayer.PlayAudio("Bullet");
					RefreshDescription(weapon);
					GameApp.GetInstance().Save();
					FlurryStatistics.BuyAmmoEvent(weapon);
				}
				else
				{
					audioPlayer.PlayAudio("Button");
					get_money_panel.SetContent(DialogString.lack_of_money);
					//get_money_panel.Show();
					//FrameCamera.depth = -5f;
				}
			}
			else
			{
				audioPlayer.PlayAudio("Button");
			}
			RefreshCashLebel();
		}
		else if (control.name == "Action_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			switch (control.gameObject.transform.Find("text_p").GetComponent<TUIMeshText>().text_Accessor)
			{
			case "SELECT":
				SelectWeapon();
				break;
			case "UNLOCK":
			{
				Weapon weapon2 = this.weapon.GetComponent<WeaponItemData>().GetWeapon();
				int unlockCrystalNeeded = AvatarShopMenuTUI.GetUnlockCrystalNeeded(weapon2.WConf.UnlockLevel);
				if (GameApp.GetInstance().GetGameState().GetCrystal() >= unlockCrystalNeeded)
				{
					weapon2.Exist = WeaponExistState.Unlocked;
					GameApp.GetInstance().GetGameState().LoseCrystal(unlockCrystalNeeded);
					UpdateAllActionButtons(weapon2);
					alignCameras[weapon2.weapon_index].transform.Find("tip").GetComponent<TUIMeshSprite>().frameName_Accessor = string.Empty;
					GameApp.GetInstance().Save();
				}
				else
				{
					get_money_panel.SetContent(DialogString.lack_of_crystal);
					//get_money_panel.Show();
					//FrameCamera.depth = -5f;
				}
				break;
			}
			case "BUY":
			{
				Weapon w = this.weapon.GetComponent<WeaponItemData>().GetWeapon();
				switch (GameApp.GetInstance().GetGameState().BuyWeapon(w))
				{
				case BuyStatus.Succeed:
					UpdateAllActionButtons(w);
					SelectWeapon();
					FlurryStatistics.BuyWeaponEvent(w);
					break;
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
				}
				break;
			}
			}
			RefreshCashLebel();
		}
		else if (control.name == "Upgrade_Button" && eventType == 3)
		{
			Weapon w2 = this.weapon.GetComponent<WeaponItemData>().GetWeapon();
			switch (GameApp.GetInstance().GetGameState().UpgradeWeapon(w2))
			{
			case UpgradeStatus.Succeed:
				audioPlayer.PlayAudio("Upgrade");
				RefreshDescription(w2);
				UpdateAllActionButtons(w2);
				RefreshCashLebel();
				GameApp.GetInstance().Save();
				FlurryStatistics.UpgradeWeaponEvent(w2);
				break;
			case UpgradeStatus.NotEnoughCash:
				audioPlayer.PlayAudio("Button");
				get_money_panel.SetContent(DialogString.lack_of_money);
				//get_money_panel.Show();
				//FrameCamera.depth = -5f;
				break;
			case UpgradeStatus.MaxLevel:
				audioPlayer.PlayAudio("Button");
				maxlevel_dialog_panel.SetContent(DialogString.weapon_already_maxLevel);
				//maxlevel_dialog_panel.Show();
				//FrameCamera.depth = -5f;
				break;
			}
		}
		else if (control.name == "Iap_yes" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			//FrameCamera.depth = -5f;
			//SceneName.FadeOutLevel("IapShopTUI");
			//IapShopTUI.last_scene_to_iap = "WeaponShopMenuTUI";
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
		else if (control.name.StartsWith("weapon_") && eventType == 1)
		{
			audioPlayer.PlayAudio("Button");
			string frameName_Accessor = control.GetComponent<TUIMeshSprite>().frameName_Accessor;
			ChangeWeapon(frameName_Accessor.Substring("weapon_".Length), control.gameObject);
		}
	}

	private void SelectWeapon()
	{
		int weapon_index = weapon.GetComponent<WeaponItemData>().GetWeapon().weapon_index;
		int num = GameApp.GetInstance().GetGameState().RectToWeaponMap[weaponShopIndex];
		if (weapon_index == num)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < GameApp.GetInstance().GetGameState().RectToWeaponMap.Length; i++)
		{
			if (weapon_index == GameApp.GetInstance().GetGameState().RectToWeaponMap[i])
			{
				if (num != -1)
				{
					GameApp.GetInstance().GetGameState().RectToWeaponMap[i] = num;
					GameApp.GetInstance().GetGameState().RectToWeaponMap[weaponShopIndex] = weapon_index;
				}
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			weapon.GetComponent<WeaponItemData>().GetWeapon().IsSelectedForBattle = true;
			if (num != -1)
			{
				GameApp.GetInstance().GetGameState().GetWeapons()[num].IsSelectedForBattle = false;
				alignCameras[num].transform.Find("tip").GetComponent<TUIMeshSprite>().frameName_Accessor = string.Empty;
			}
			GameApp.GetInstance().GetGameState().RectToWeaponMap[weaponShopIndex] = weapon_index;
			alignCameras[weapon_index].transform.Find("tip").GetComponent<TUIMeshSprite>().frameName_Accessor = "logo_equipped";
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

	private void RefreshDescription(Weapon w)
	{
        weapon_level.text_Accessor = "LV: " + RoundForDisplay(w.Level);
        if (w.Name == "Crossbow")
        {
            weapon_Info.text_Accessor = "DAMAGE: " + RoundForDisplay(w.AttackDamage, 2) + " (x2)";
        }
        else
        {
            weapon_Info.text_Accessor = "DAMAGE: " + RoundForDisplay(w.AttackDamage, 2);
        }

        weapon_Info.text_Accessor += "\nFIRE RATE: " + RoundForDisplay(w.AttackFrequency, 2);
        weapon_Info.text_Accessor += "\nACCURACY: " + RoundForDisplay(w.AttackAccuracy) + "%";

        ammo_Info.gameObject.SetActive(true);
        ammo_Info.text_Accessor = "AMMO: " + w.BulletCount;
    }

	private void UpdateAllActionButtons(Weapon w)
	{
		if (w.Exist == WeaponExistState.Owned)
		{
			upgrade_button.SetActive(true);
			action_button.SetActive(true);
			action_button.transform.Find("text_n").GetComponent<TUIMeshText>().text_Accessor = "SELECT";
			action_button.transform.Find("text_p").GetComponent<TUIMeshText>().text_Accessor = "SELECT";
			crystal_in_info.SetActive(false);
			ammo_price.gameObject.SetActive(true);
			weapon_price.gameObject.SetActive(false);
			ammo_price.fontName_Accessor = "CAI-12";
			if (w.GetWeaponType() == WeaponType.Saw || w.GetWeaponType() == WeaponType.Sword)
			{
				ammo_price.text_Accessor = string.Empty;
				bullet_logo.SetActive(false);
				bullet_count.gameObject.SetActive(false);
				buy_ammo_button.SetActive(false);
			}
			else
			{
				ammo_price.text_Accessor = "$" + w.WConf.bulletPrice.ToString("N0");
				bullet_logo.SetActive(true);
				bullet_logo.GetComponent<TUIMeshSprite>().frameName_Accessor = w.WeaponBulletName;
				bullet_count.gameObject.SetActive(true);
				bullet_count.text_Accessor = "+" + w.WConf.bulletEachAdd.ToString("N0");
				buy_ammo_button.SetActive(true);
			}
			if (w.Level >= UpgradeParas.WeaponMaxLevel)
			{
				upgrade_price.gameObject.SetActive(false);
				return;
			}
			upgrade_price.gameObject.SetActive(true);
			upgrade_price.text_Accessor = "$" + w.UpgradePrice.ToString("N0");
		}
		else if (w.Exist == WeaponExistState.Unlocked)
		{
			buy_ammo_button.SetActive(false);
			ammo_price.gameObject.SetActive(false);
			weapon_price.gameObject.SetActive(true);
			upgrade_button.SetActive(false);
			action_button.SetActive(true);
			action_button.transform.Find("text_n").GetComponent<TUIMeshText>().text_Accessor = "BUY";
			action_button.transform.Find("text_p").GetComponent<TUIMeshText>().text_Accessor = "BUY";
			if (w.WConf.isCrystalBuy)
			{
				crystal_in_info.SetActive(true);
				weapon_price.text_Accessor = "     " + w.WConf.price.ToString("N0");
			}
			else
			{
				crystal_in_info.SetActive(false);
				weapon_price.text_Accessor = "$" + w.WConf.price.ToString("N0");
			}
			upgrade_price.gameObject.SetActive(false);
			bullet_logo.SetActive(false);
			bullet_count.gameObject.SetActive(false);
		}
		else
		{
			buy_ammo_button.SetActive(false);
			upgrade_button.SetActive(false);
			action_button.SetActive(true);
			action_button.transform.Find("text_n").GetComponent<TUIMeshText>().text_Accessor = "UNLOCK";
			action_button.transform.Find("text_p").GetComponent<TUIMeshText>().text_Accessor = "UNLOCK";
			weapon_price.gameObject.SetActive(true);
			weapon_price.text_Accessor = "     " + AvatarShopMenuTUI.GetUnlockCrystalNeeded(w.WConf.UnlockLevel).ToString("N0");
			crystal_in_info.SetActive(true);
			ammo_price.gameObject.SetActive(true);
			ammo_price.text_Accessor = "  COMPLETE DAY " + w.WConf.UnlockLevel + " TO UNLOCK";
			ammo_price.fontName_Accessor = "CAI-10";
			upgrade_price.gameObject.SetActive(false);
			bullet_logo.SetActive(false);
			bullet_count.gameObject.SetActive(false);
		}
	}

	private void UpdateSlider()
	{
		float num = -76f + scroll.position.y / scroll.rangeYMax * 182f;
		Slider.transform.position = new Vector3(Slider.transform.position.x, 0f - num + Slider.transform.parent.position.y, Slider.transform.position.z);
	}

	private void AdaptChosenFrameFirstDisplay(GameObject current_button)
	{
		if (GameApp.GetInstance().GetGameState().show_weapon_update)
		{
			for (int i = 0; i < 0; i++)
			{
				GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/New_Label"), Vector3.zero, Quaternion.identity) as GameObject;
				gameObject.name = "NewLabel";
				gameObject.transform.parent = alignCameras[alignCameras.Length - i - 1].transform;
				gameObject.transform.localPosition = new Vector3(-33.6f, 21.3f, -1.5f);
			}
			float y = 0f - alignCameras[alignCameras.Length - 1].transform.localPosition.y - 72.5f;
			scroll.position = new Vector2(0f, y);
			GameObject.Find("ScrollObjects").transform.localPosition = new Vector3(0f, y, 0f);
			UpdateSlider();
			GameApp.GetInstance().GetGameState().show_weapon_update = false;
		}
		else if ((double)current_button.gameObject.transform.localPosition.y < -72.5)
		{
			float y2 = 0f - current_button.transform.localPosition.y - 72.5f;
			scroll.position = new Vector2(0f, y2);
			GameObject.Find("ScrollObjects").transform.localPosition = new Vector3(0f, y2, 0f);
			UpdateSlider();
		}
	}

    private string RoundForDisplay(float val, int decimals = 0)
    {
        float factor = Mathf.Pow(10, decimals);
        return (Mathf.Round(val * factor) / factor).ToString();
    }
}
