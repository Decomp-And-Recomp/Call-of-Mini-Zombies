using System.Collections.Generic;
using TNetSdk;
using UnityEngine;
using Zombie3D;

public class VSHallTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	private TNetObject tnetObj;

	public string domain = string.Empty;

	public static int groupIdMin;

	public static int groupIdMax;

	public static bool isPixelVS;

	private JoinRoomType joinRoomType;

	private int auto_match_count;

	private int searchRoomID;

	private LogoutDestination logoutDestination;

	public TUIMeshText label_money;

	public TUIMeshText label_crystal;

	protected GameObject avatar;

	public GameObject avatar_mover;

	public GameObject SearchPanel;

	public GameObject IndicatorPanel;

	public GameObject MsgBox;

	public GameObject JoinRoomPwdPanel;

	public GameObject ModelSelectPanel;

	public Camera FrameCamera;

	private AudioPlayer audioPlayer = new AudioPlayer();

	private float lastMotionTime;

	private List<int> randomSceneList;

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

	private void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFadeEx>()
			.FadeIn();
		audioPlayer.AddAudio(base.transform, "Button", true);
		audioPlayer.AddAudio(base.transform, "Back", true);
		IndicatorPanel.GetComponent<IndicatorTUI>().automaticClose = OnIndicatorClose;
		label_money.text_Accessor = "$" + GameApp.GetInstance().GetGameState().GetCash()
			.ToString("N0");
		label_crystal.text_Accessor = GameApp.GetInstance().GetGameState().GetCrystal()
			.ToString("N0");
		if (GameApp.GetInstance().GetGameState().last_scene == "MainMapTUI" || GameApp.GetInstance().GetGameState().last_scene == "NickNameTUI")
		{
			ModelSelectPanel.transform.localPosition = new Vector3(0f, 0f, ModelSelectPanel.transform.localPosition.z);
		}
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
		if (TNetConnection.IsInitialized)
		{
			tnetObj = TNetConnection.Connection;
		}
		else
		{
			tnetObj = new TNetObject();
		}
		TNetConnection.is_server = false;
		tnetObj.AddEventListener(TNetEventSystem.CONNECTION, OnConnection);
		tnetObj.AddEventListener(TNetEventSystem.DISCONNECT, OnDisConnection);
		tnetObj.AddEventListener(TNetEventSystem.CONNECTION_KILLED, OnConnectionLost);
		tnetObj.AddEventListener(TNetEventSystem.CONNECTION_TIMEOUT, OnConnectionTimeOut);
		tnetObj.AddEventListener(TNetEventSystem.LOGIN, OnLogin);
		tnetObj.AddEventListener(TNetEventRoom.GET_ROOM_LIST, OnRoomList);
		tnetObj.AddEventListener(TNetEventRoom.ROOM_JOIN, OnJoinRoom);
		tnetObj.AddEventListener(TNetEventSystem.REVERSE_HEART_RENEW, OnReverseHearRenew);
		tnetObj.AddEventListener(TNetEventSystem.REVERSE_HEART_TIMEOUT, OnReverseHearTimeout);
		tnetObj.AddEventListener(TNetEventSystem.REVERSE_HEART_WAITING, OnReverseHearWaiting);
		tnetObj.AddEventListener(TNetEventSystem.GetHostAddresses_Error, OnGetHostAddressError);
		CheckServerVersion();
		auto_match_count = 0;
		lastMotionTime = Time.time;
		if (GameApp.GetInstance().GetGameState().tutorialTriggers[3])
		{
			GameApp.GetInstance().GetGameState().tutorialTriggers[3] = false;
			GameApp.GetInstance().PlayerPrefsSave();
		}
		OpenClikPlugin.Hide();
		Resources.UnloadUnusedAssets();
	}

	private List<int> RandomSortSceneList()
	{
		int num = ((!isPixelVS) ? 9 : 10);
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		for (int i = 1; i <= num; i++)
		{
			list.Add((!isPixelVS) ? i : (i + 9));
		}
		for (int j = 0; j < num; j++)
		{
			int index = Random.Range(0, list.Count);
			list2.Add(list[index]);
			list.RemoveAt(index);
		}
		return list2;
	}

	private void FixedUpdate()
	{
		tnetObj.Update(Time.fixedDeltaTime);
	}

	private void Update()
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

	private void ConnectServerFailed()
	{
		TNetConnection.UnregisterSFSSceneCallbacks();
		TNetConnection.Disconnect();
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
		if (MsgBox != null)
		{
			FrameCamera.depth = -5f;
			MsgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.netUnableConnect, MsgBoxType.ContectingTimeout);
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Back");
			FrameCamera.depth = -5f;
			logoutDestination = LogoutDestination.MainMap;
			tnetObj.Close();
			return;
		}
		if (control.name == "Achi_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			logoutDestination = LogoutDestination.Achievement;
			tnetObj.Close();
			return;
		}
		if (control.name == "Shop_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			logoutDestination = LogoutDestination.Shop;
			tnetObj.Close();
			return;
		}
		if (control.name == "Money_getMore" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			logoutDestination = LogoutDestination.IapShop;
			tnetObj.Close();
			return;
		}
		if (control.name == "Option_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			logoutDestination = LogoutDestination.Option;
			tnetObj.Close();
			return;
		}
		if (control.name == "Button_Create" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			SceneName.FadeOutLevel("CreateRoomTUI");
			return;
		}
		if (control.name == "Button_Quickmatch" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			QuickMatch();
			return;
		}
		if (control.name == "Button_Search" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			SearchPanel.transform.localPosition = new Vector3(0f, 0f, SearchPanel.transform.localPosition.z);
			SearchPanel.GetComponent<TUITextField>().ResetText();
			return;
		}
		if (control.name == "Search_OK_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			SearchPanel.transform.localPosition = new Vector3(0f, 1000f, SearchPanel.transform.localPosition.z);
			try
			{
				searchRoomID = int.Parse(SearchPanel.GetComponent<TUITextField>().GetText());
				tnetObj.Send(new JoinRoomRequest(searchRoomID, string.Empty));
				joinRoomType = JoinRoomType.SearchWithoutPassword;
				IndicatorPanel.GetComponent<IndicatorTUI>().SetContent("SEARCHING...");
				IndicatorPanel.GetComponent<IndicatorTUI>().Show(IndicatorTUI.IndicatorType.RoomSearch);
				return;
			}
			catch
			{
				if (MsgBox != null)
				{
					MsgBox.GetComponent<MsgBoxDelegate>().Show("Invalid ID. Please try again.", MsgBoxType.WrongTextFormat);
				}
				return;
			}
		}
		if (control.name == "Search_Close_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = 1f;
			SearchPanel.transform.localPosition = new Vector3(0f, 1000f, SearchPanel.transform.localPosition.z);
		}
		else if (control.name == "Join_Password_OK_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			JoinRoomPwdPanel.transform.localPosition = new Vector3(0f, -1000f, SearchPanel.transform.localPosition.z);
			tnetObj.Send(new JoinRoomRequest(searchRoomID, JoinRoomPwdPanel.GetComponent<TUITextField>().GetText()));
			joinRoomType = JoinRoomType.SearchWithPassword;
			IndicatorPanel.GetComponent<IndicatorTUI>().SetContent(DialogString.netConnecting);
			IndicatorPanel.GetComponent<IndicatorTUI>().Show(IndicatorTUI.IndicatorType.RoomJoin);
		}
		else if (control.name == "Join_Password_Close_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			JoinRoomPwdPanel.transform.localPosition = new Vector3(0f, -1000f, JoinRoomPwdPanel.transform.localPosition.z);
			SearchPanel.transform.localPosition = new Vector3(0f, 0f, SearchPanel.transform.localPosition.z);
		}
		else if (control.name == "NormalVS" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			ModelSelectPanel.transform.localPosition = new Vector3(0f, -2000f, ModelSelectPanel.transform.localPosition.z);
			ModelSelectPanel.SetActive(false);
			FrameCamera.depth = 1f;
			isPixelVS = false;
			GameObject.Find("TUI/TUIControl/Coop-VS/text").GetComponent<TUIMeshText>().text_Accessor = "VS-NORMAL";
			randomSceneList = RandomSortSceneList();
		}
		else if (control.name == "PixelVS" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			ModelSelectPanel.transform.localPosition = new Vector3(0f, -2000f, ModelSelectPanel.transform.localPosition.z);
			ModelSelectPanel.SetActive(false);
			FrameCamera.depth = 1f;
			isPixelVS = true;
			GameObject.Find("TUI/TUIControl/Coop-VS/text").GetComponent<TUIMeshText>().text_Accessor = "VS-PIXEL";
			randomSceneList = RandomSortSceneList();
		}
		else if (control.name == "Msg_OK_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			MsgBox.GetComponent<MsgBoxDelegate>().Hide();
			FrameCamera.depth = 1f;
			switch (MsgBox.GetComponent<MsgBoxDelegate>().m_type)
			{
			case MsgBoxType.WrongTextFormat:
				FrameCamera.depth = -5f;
				SearchPanel.transform.localPosition = new Vector3(0f, 0f, SearchPanel.transform.localPosition.z);
				SearchPanel.GetComponent<TUITextField>().ResetText();
				break;
			case MsgBoxType.ContectingTimeout:
				FrameCamera.depth = -5f;
				SceneName.FadeOutLevel("MainMapTUI");
				FlurryStatistics.ServerDisconnectEvent("VS");
				break;
			case MsgBoxType.VersionError:
				FrameCamera.depth = -5f;
				logoutDestination = LogoutDestination.MainMap;
				tnetObj.Close();
				Application.OpenURL("market://details?id=com.trinitigame.callofminiandroid");
				break;
			}
		}
	}

	private void OnConnection(TNetEventData evt)
	{
		TNetConnection.Connection = tnetObj;
		tnetObj.Send(new LoginRequest(GameApp.GetInstance().GetGameState().nick_name, string.Empty, string.Empty));
		Debug.Log("OnConnecting...");
	}

	private void OnConnectionTimeOut(TNetEventData evt)
	{
		ConnectServerFailed();
	}

	private void OnDisConnection(TNetEventData evt)
	{
		TNetConnection.UnregisterSFSSceneCallbacks();
		TNetConnection.Disconnect();
		switch (logoutDestination)
		{
		case LogoutDestination.Shop:
			SceneName.FadeOutLevel("ShopMenuTUI");
			ShopMenuTUI.last_map_to_shop = "VSHallTUI";
			ShopMenuTUI.chosen_shop = ShopMenu.AvatarShop;
			break;
		case LogoutDestination.Achievement:
			SceneName.FadeOutLevel("AchievementTUI");
			break;
		case LogoutDestination.MainMap:
			SceneName.FadeOutLevel("MainMapTUI");
			break;
		case LogoutDestination.IapShop:
			//SceneName.FadeOutLevel("IapShopTUI");
			//IapShopTUI.last_scene_to_iap = "VSHallTUI";
			break;
		case LogoutDestination.Option:
			SceneName.FadeOutLevel("OptionTUI");
			break;
		case LogoutDestination.SurvivalMap:
		case LogoutDestination.VsMap:
			break;
		}
	}

	private void OnConnectionLost(TNetEventData evt)
	{
		ConnectServerFailed();
	}

	private void OnLogin(TNetEventData evt)
	{
		Debug.Log(string.Concat("OnLogin:", (SysLoginResCmd.Result)(int)evt.data["result"], " id:", tnetObj.Myself.Id));
		if ((int)evt.data["result"] == 0)
		{
			if (IndicatorPanel != null)
			{
				IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
				if (ModelSelectPanel.transform.localPosition.y != 0f)
				{
					if (isPixelVS)
					{
						GameObject.Find("TUI/TUIControl/Coop-VS/text").GetComponent<TUIMeshText>().text_Accessor = "VS-PIXEL";
					}
					else
					{
						GameObject.Find("TUI/TUIControl/Coop-VS/text").GetComponent<TUIMeshText>().text_Accessor = "VS-NORMAL";
					}
					randomSceneList = RandomSortSceneList();
					FrameCamera.depth = 1f;
				}
			}
			TutorialMainMap.CheckShopButton();
		}
		else
		{
			Debug.Log("Login Failed.");
			ConnectServerFailed();
		}
	}

	private void QuickMatch()
	{
		joinRoomType = JoinRoomType.AutoMatch;
		auto_match_count++;
		for (int i = 0; i < randomSceneList.Count; i++)
		{
			int num = groupIdMin + randomSceneList[i] - 1;
			if (num <= groupIdMax)
			{
				tnetObj.Send(new GetRoomListRequest(num, 0, 10, RoomDragListCmd.ListType.not_full));
			}
		}
		IndicatorPanel.GetComponent<IndicatorTUI>().SetContent("SEARCHING...");
		IndicatorPanel.GetComponent<IndicatorTUI>().Show(IndicatorTUI.IndicatorType.QuickMatch);
	}

	private void OnRoomList(TNetEventData evt)
	{
		List<TNetRoom> list = (List<TNetRoom>)evt.data["roomList"];
		if (list.Count <= 0 || joinRoomType != JoinRoomType.AutoMatch)
		{
			return;
		}
		foreach (TNetRoom item in list)
		{
			if (!item.IsPasswordProtected && CoopHallTUI.LevelSelectRoom(GameApp.GetInstance().GetGameState().LevelNum, int.Parse(item.Commnet), auto_match_count))
			{
				Debug.Log("Randomly join room:" + item.Name + "  room level: " + item.Commnet);
				tnetObj.Send(new JoinRoomRequest(item.Id, string.Empty));
				break;
			}
		}
	}

	private void OnJoinRoom(TNetEventData evt)
	{
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
		if (joinRoomType == JoinRoomType.None)
		{
			return;
		}
		switch ((RoomJoinResCmd.Result)(int)evt.data["result"])
		{
		case RoomJoinResCmd.Result.ok:
		{
			Debug.Log("Room joined successfully.");
			FrameCamera.depth = -5f;
			TNetRoom tNetRoom = (TNetRoom)evt.data["room"];
			GameApp.GetInstance().GetGameState().cur_net_map = SceneName.GetNetMapName(tNetRoom.GroupId - groupIdMin + 1);
			SceneName.FadeOutLevel("RoomTUI");
			return;
		}
		case RoomJoinResCmd.Result.pwd_error:
			if (joinRoomType == JoinRoomType.SearchWithPassword)
			{
				FrameCamera.depth = -5f;
				MsgBox.GetComponent<MsgBoxDelegate>().Show("Incorrect password!".ToUpper(), MsgBoxType.JoinRommFailed);
			}
			else if (joinRoomType == JoinRoomType.SearchWithoutPassword)
			{
				JoinRoomPwdPanel.transform.localPosition = new Vector3(0f, 0f, JoinRoomPwdPanel.transform.localPosition.z);
				JoinRoomPwdPanel.GetComponent<TUITextField>().ResetText();
			}
			break;
		case RoomJoinResCmd.Result.full:
			if (joinRoomType == JoinRoomType.AutoMatch)
			{
				FrameCamera.depth = -5f;
				MsgBox.GetComponent<MsgBoxDelegate>().Show("No match found! Please \ntry again!", MsgBoxType.JoinRommFailed);
			}
			else if (joinRoomType == JoinRoomType.SearchWithoutPassword || joinRoomType == JoinRoomType.SearchWithPassword)
			{
				FrameCamera.depth = -5f;
				MsgBox.GetComponent<MsgBoxDelegate>().Show("Room full!", MsgBoxType.JoinRommFailed);
			}
			break;
		case RoomJoinResCmd.Result.no_exist:
			FrameCamera.depth = -5f;
			MsgBox.GetComponent<MsgBoxDelegate>().Show("No match found! Please \ntry again!", MsgBoxType.JoinRommFailed);
			break;
		}
		joinRoomType = JoinRoomType.None;
	}

	private void OnIndicatorClose()
	{
		switch (IndicatorPanel.GetComponent<IndicatorTUI>().type)
		{
		case IndicatorTUI.IndicatorType.ServerConnect:
		case IndicatorTUI.IndicatorType.HeartWaiting:
			TNetConnection.UnregisterSFSSceneCallbacks();
			TNetConnection.Disconnect();
			if (MsgBox != null)
			{
				FrameCamera.depth = -5f;
				MsgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.netUnableConnect, MsgBoxType.ContectingTimeout);
			}
			break;
		case IndicatorTUI.IndicatorType.QuickMatch:
			joinRoomType = JoinRoomType.None;
			FrameCamera.depth = -5f;
			MsgBox.GetComponent<MsgBoxDelegate>().Show("No match found! Please \ntry again.", MsgBoxType.JoinRommFailed);
			Debug.Log("quick match time out!");
			break;
		case IndicatorTUI.IndicatorType.RoomSearch:
			searchRoomID = -1;
			joinRoomType = JoinRoomType.None;
			FrameCamera.depth = -5f;
			MsgBox.GetComponent<MsgBoxDelegate>().Show("No match found! Please \ntry again!", MsgBoxType.JoinRommFailed);
			Debug.Log("search room failed.");
			break;
		case IndicatorTUI.IndicatorType.RoomJoin:
			joinRoomType = JoinRoomType.None;
			FrameCamera.depth = -5f;
			MsgBox.GetComponent<MsgBoxDelegate>().Show("Room unavailable, try \nanother one!", MsgBoxType.JoinRommFailed);
			Debug.Log("join room failed.");
			break;
		}
	}

	private void OnReverseHearWaiting(TNetEventData evt)
	{
		if (IndicatorPanel != null)
		{
			FrameCamera.depth = -5f;
			IndicatorPanel.GetComponent<IndicatorTUI>().SetContent("WAITING FOR SERVER.");
			IndicatorPanel.GetComponent<IndicatorTUI>().Show(IndicatorTUI.IndicatorType.HeartWaiting);
		}
	}

	private void OnReverseHearRenew(TNetEventData evt)
	{
		if (ModelSelectPanel.transform.localPosition.y != 0f && SearchPanel.transform.position.y != 0f && JoinRoomPwdPanel.transform.position.y != 0f)
		{
			FrameCamera.depth = 1f;
		}
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
	}

	private void OnReverseHearTimeout(TNetEventData evt)
	{
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
		if (MsgBox != null)
		{
			FrameCamera.depth = -5f;
			MsgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.netDisconnected, MsgBoxType.ContectingTimeout);
		}
		TNetConnection.UnregisterSFSSceneCallbacks();
		TNetConnection.Disconnect();
		SFSServerVersion.stateVs = SFSServerVersion.State.Failed;
	}

	private void CheckServerVersion()
	{
		IndicatorPanel.GetComponent<IndicatorTUI>().SetContent(DialogString.netConnecting);
		IndicatorPanel.GetComponent<IndicatorTUI>().Show(IndicatorTUI.IndicatorType.ServerConnect);
		base.gameObject.AddComponent<SFSServerVersion>();
		base.gameObject.GetComponent<SFSServerVersion>().callback = OnServerVersion;
		base.gameObject.GetComponent<SFSServerVersion>().callback_error = OnServerVersionError;
	}

	private void OnServerVersion(bool status)
	{
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
		if (status)
		{
            ConnectToSFS(SFSServerVersion.VsDomain, SFSServerVersion.VsServerPort);
            /*string empty = string.Empty;
			int num = 0;
			domain = base.gameObject.GetComponent<SFSServerVersion>().VsDomain;
			groupIdMin = base.gameObject.GetComponent<SFSServerVersion>().VsGroupIdMin;
			groupIdMax = base.gameObject.GetComponent<SFSServerVersion>().VsGroupIdMax;
			num = base.gameObject.GetComponent<SFSServerVersion>().VsServerPort;
			empty = tnetObj.GetHostAddresses(domain);
			if (empty != string.Empty)
			{
				Debug.Log("Success!!domain: " + domain + "\nport: " + num + "\nip: " + empty + "\ngroupid: " + groupIdMin + "~" + groupIdMax);
				ConnectToSFS(empty, num);
			}*/
        }
		else if (MsgBox != null)
		{
			FrameCamera.depth = -5f;
			MsgBox.GetComponent<MsgBoxDelegate>().Show("Game Version Error!", MsgBoxType.VersionError);
		}
	}

	private void OnServerVersionError()
	{
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
		if (MsgBox != null)
		{
			FrameCamera.depth = -5f;
			MsgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.netDisconnected, MsgBoxType.ContectingTimeout);
		}
		TNetConnection.UnregisterSFSSceneCallbacks();
		TNetConnection.Disconnect();
	}

	private void OnGetHostAddressError(TNetEventData evt)
	{
		Debug.Log("get host address error!!!!");
		string vsStandbyServerIP = base.gameObject.GetComponent<SFSServerVersion>().VsStandbyServerIP;
		int vsServerPort = SFSServerVersion.VsServerPort;
		Debug.Log("Standby Server IP!!domain: " + domain + "\nport: " + vsServerPort + "\nip: " + vsStandbyServerIP + "\ngroupid: " + groupIdMin + "~" + groupIdMax);
		ConnectToSFS(vsStandbyServerIP, vsServerPort);
	}

	private void ConnectToSFS(string server_ip, int server_port)
	{
#if UNITY_EDITOR
		Debug.Log(server_ip + "  " + server_port);
#else
		Debug.Log("Connecting to server..");
#endif
        if (!tnetObj.IsContected())
		{
			tnetObj.Connect(server_ip, server_port);
			IndicatorPanel.GetComponent<IndicatorTUI>().SetContent(DialogString.netConnecting);
			IndicatorPanel.GetComponent<IndicatorTUI>().Show(IndicatorTUI.IndicatorType.ServerConnect);
			FrameCamera.depth = -5f;
		}
		else
		{
			TNetConnection.Connection = tnetObj;
			tnetObj.Send(new LoginRequest(GameApp.GetInstance().GetGameState().nick_name, string.Empty, string.Empty));
		}
		FlurryStatistics.ConnectServerEvent("VS");
	}

	private void OnDestroy()
	{
		TNetConnection.UnregisterSFSSceneCallbacks();
		Debug.Log("unregister callbacks...");
	}
}
