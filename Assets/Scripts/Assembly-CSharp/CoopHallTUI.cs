using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Zombie3D;

public class CoopHallTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	private TUIInput[] input;

	private NetworkObj net_com;

	public static int mapIdMin;

	private bool net_com_inited;

	private JoinRoomType joinRoomType;

	private int auto_match_count;

	private int searchRoomID;

	private string search_room_password = string.Empty;

	public TUIMeshText label_money;

	public TUIMeshText label_crystal;

	public GameObject IndicatorPanel;

	public GameObject MsgBox;

	public TUITextField SearchPanel;

	public TUITextField JoinRoomPwdPanel;

	private GameObject avatar;

	public GameObject avatar_mover;

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
		auto_match_count = 0;
		lastMotionTime = Time.time;
		if (GameApp.GetInstance().GetGameState().tutorialTriggers[2])
		{
			GameApp.GetInstance().GetGameState().tutorialTriggers[2] = false;
			GameApp.GetInstance().PlayerPrefsSave();
		}
		OpenClikPlugin.Hide();
		Resources.UnloadUnusedAssets();
		CheckServerVersion();
		randomSceneList = RandomSortSceneList();
	}

	public void CheckNetWorkCom(string m_serverIP, int m_serverPort)
	{
		if (NetworkObj.tcp_contected_status == NetworkObj.TCP_CONTECTED_STATUS.None)
		{
			Debug.Log("NetWorkCom init...");
			NetworkObj.CheckNetWorkCom();
			net_com_inited = false;
		}
		if (!net_com_inited)
		{
			net_com = GameApp.GetInstance().GetGameState().net_com;
			net_com.connect_delegate = OnConnected;
			net_com.packet_delegate = OnPacket;
			net_com.close_delegate = OnClosed;
			net_com.contecting_timeout = OnContectingTimeout;
			net_com.contecting_lost = OnConnectionLost;
			net_com.join_room_delegate = OnJoinRoom;
			net_com.join_room_error_delegate = OnJoinRoomError;
			net_com.reverse_heart_timeout_delegate = OnReverseHearTimeout;
			net_com.reverse_heart_renew_delegate = OnReverseHearRenew;
			net_com.reverse_heart_waiting_delegate = OnReverseHearWaiting;
			net_com_inited = true;
		}
		net_com.ClearNetUserArr();
		if (NetworkObj.tcp_contected_status == NetworkObj.TCP_CONTECTED_STATUS.None)
		{
			net_com.GoGameServer(m_serverIP, m_serverPort);
			FlurryStatistics.ConnectServerEvent("COOP");
		}
		else if (NetworkObj.tcp_contected_status == NetworkObj.TCP_CONTECTED_STATUS.GameServer)
		{
			if (IndicatorPanel != null)
			{
				IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
			}
			FrameCamera.depth = 1f;
		}
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

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Back");
			FrameCamera.depth = -5f;
			NetworkObj.DestroyNetCom();
			SceneName.FadeOutLevel("MainMapTUI");
			return;
		}
		if (control.name == "Achi_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			NetworkObj.DestroyNetCom();
			SceneName.FadeOutLevel("AchievementTUI");
			return;
		}
		if (control.name == "Shop_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			NetworkObj.DestroyNetCom();
			SceneName.FadeOutLevel("ShopMenuTUI");
			ShopMenuTUI.last_map_to_shop = "CoopHallTUI";
			ShopMenuTUI.chosen_shop = ShopMenu.AvatarShop;
			return;
		}
		if (control.name == "Money_getMore" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			//FrameCamera.depth = -5f;
			//NetworkObj.DestroyNetCom();
			//SceneName.FadeOutLevel("IapShopTUI");
			//IapShopTUI.last_scene_to_iap = "CoopHallTUI";
			return;
		}
		if (control.name == "Option_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			FrameCamera.depth = -5f;
			NetworkObj.DestroyNetCom();
			SceneName.FadeOutLevel("OptionTUI");
			return;
		}
		if (control.name == "NoExpPanel_OK_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			UnityEngine.Object.Destroy(GameObject.Find("NoExpPanel"));
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
				int room_id = int.Parse(SearchPanel.GetComponent<TUITextField>().GetText());
				net_com.Send(CGRoomInfoPacket.MakePacket((uint)room_id));
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
		else if (control.name == "Join_Password_Close_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			JoinRoomPwdPanel.transform.localPosition = new Vector3(0f, -1000f, JoinRoomPwdPanel.transform.localPosition.z);
			SearchPanel.transform.localPosition = new Vector3(0f, 0f, SearchPanel.transform.localPosition.z);
		}
		else if (control.name == "Join_Password_OK_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			JoinRoomPwdPanel.transform.localPosition = new Vector3(0f, -1000f, JoinRoomPwdPanel.transform.localPosition.z);
			if (search_room_password == JoinRoomPwdPanel.GetText())
			{
				Packet packet = CGJoinRoomPacket.MakePacket((uint)searchRoomID, (long)(Time.time * 1000f), GameApp.GetInstance().GetGameState().nick_name, (uint)GameApp.GetInstance().GetGameState().Avatar, (uint)GameApp.GetInstance().GetGameState().LevelNum);
				net_com.Send(packet);
				joinRoomType = JoinRoomType.SearchWithPassword;
				IndicatorPanel.GetComponent<IndicatorTUI>().SetContent(DialogString.netConnecting);
				IndicatorPanel.GetComponent<IndicatorTUI>().Show(IndicatorTUI.IndicatorType.RoomJoin);
			}
			else
			{
				MsgBox.GetComponent<MsgBoxDelegate>().Show("Incorrect password!".ToUpper(), MsgBoxType.JoinRommFailed);
			}
		}
		else if (control.name == "Msg_OK_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			MsgBoxType type = MsgBox.GetComponent<MsgBoxDelegate>().m_type;
			MsgBox.GetComponent<MsgBoxDelegate>().Hide();
			FrameCamera.depth = 1f;
			switch (type)
			{
			case MsgBoxType.OnClosed:
				FrameCamera.depth = -5f;
				SceneName.FadeOutLevel("MainMapTUI");
				break;
			case MsgBoxType.ContectingTimeout:
			case MsgBoxType.ContectingLost:
				FrameCamera.depth = -5f;
				SceneName.FadeOutLevel("MainMapTUI");
				FlurryStatistics.ServerDisconnectEvent("COOP");
				break;
			case MsgBoxType.VersionError:
				FrameCamera.depth = -5f;
				NetworkObj.DestroyNetCom();
				SceneName.FadeOutLevel("MainMapTUI");
				Application.OpenURL("market://details?id=com.trinitigame.callofminiandroid");
				break;
			}
		}
	}

	private void OnConnected()
	{
		if (NetworkObj.tcp_contected_status == NetworkObj.TCP_CONTECTED_STATUS.GameServer)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
			Debug.Log("Game Server Connected...");
			FrameCamera.depth = 1f;
			TutorialMainMap.CheckShopButton();
		}
	}

	private void OnPacket(Packet packet)
	{
		uint val = 0u;
		if (packet.WatchUInt32(ref val, 4) && NetworkObj.tcp_contected_status == NetworkObj.TCP_CONTECTED_STATUS.GameServer)
		{
			switch ((PROTOCOLS)val)
			{
			case PROTOCOLS.CG_HEARTBEAT:
				Debug.Log("CG_HEARTBEAT revceived!!");
				break;
			case PROTOCOLS.GC_QUICK_ROOMLIST:
				OnQuickRoomList(packet);
				break;
			case PROTOCOLS.GC_ROOMINFO:
				OnRoomInfo(packet);
				break;
			}
		}
	}

	private void OnClosed()
	{
		NetworkObj.DestroyNetCom();
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
		if (MsgBox != null)
		{
			FrameCamera.depth = -5f;
			MsgBox.GetComponent<MsgBoxDelegate>().Show("YOU WERE DISCONNECTED.", MsgBoxType.OnClosed);
		}
	}

	private void OnConnectionLost()
	{
		NetworkObj.DestroyNetCom();
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
		if (MsgBox != null)
		{
			FrameCamera.depth = -5f;
			MsgBox.GetComponent<MsgBoxDelegate>().Show("YOU WERE DISCONNECTED.", MsgBoxType.ContectingLost);
		}
	}

	private void OnContectingTimeout()
	{
		Debug.Log("Server Connecting Timeount...");
		NetworkObj.DestroyNetCom();
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
		if (MsgBox != null)
		{
			MsgBox.GetComponent<MsgBoxDelegate>().Show("UNABLE TO CONNECT.", MsgBoxType.ContectingTimeout);
		}
	}

	private void OnRoomInfo(Packet packet)
	{
		GCRoomInfoPacket gCRoomInfoPacket = new GCRoomInfoPacket();
		if (!gCRoomInfoPacket.ParserPacket(packet))
		{
			Debug.Log("GCRoomInfoPacket ParserPacket error!!");
		}
		else if (gCRoomInfoPacket.m_iResult == 0)
		{
			searchRoomID = (int)gCRoomInfoPacket.m_roomInfo.m_iRoomId;
			if (gCRoomInfoPacket.m_roomInfo.m_password != string.Empty)
			{
				IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
				search_room_password = gCRoomInfoPacket.m_roomInfo.m_password;
				JoinRoomPwdPanel.transform.localPosition = new Vector3(0f, 0f, JoinRoomPwdPanel.transform.localPosition.z);
				JoinRoomPwdPanel.ResetText();
			}
			else
			{
				Packet packet2 = CGJoinRoomPacket.MakePacket(gCRoomInfoPacket.m_roomInfo.m_iRoomId, (long)(Time.time * 1000f), GameApp.GetInstance().GetGameState().nick_name, (uint)GameApp.GetInstance().GetGameState().Avatar, (uint)GameApp.GetInstance().GetGameState().LevelNum);
				net_com.Send(packet2);
				IndicatorPanel.GetComponent<IndicatorTUI>().SetContent(DialogString.netConnecting);
			}
		}
		else
		{
			searchRoomID = -1;
			joinRoomType = JoinRoomType.None;
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
			MsgBox.GetComponent<MsgBoxDelegate>().Show("No match found! Please \ntry again!", MsgBoxType.JoinRommFailed);
		}
	}

	private void QuickMatch()
	{
		joinRoomType = JoinRoomType.AutoMatch;
		auto_match_count++;
		for (int i = 0; i < randomSceneList.Count; i++)
		{
			Packet packet = CGQuickRoomListPacket.MakePacket(0u, (uint)(mapIdMin + randomSceneList[i] - 1));
			net_com.Send(packet);
		}
		FrameCamera.depth = -5f;
		IndicatorPanel.GetComponent<IndicatorTUI>().SetContent("SEARCHING...");
		IndicatorPanel.GetComponent<IndicatorTUI>().Show(IndicatorTUI.IndicatorType.QuickMatch);
	}

	private List<int> RandomSortSceneList()
	{
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		for (int i = 1; i <= 9; i++)
		{
			list.Add(i);
		}
		for (int j = 0; j < 9; j++)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			list2.Add(list[index]);
			list.RemoveAt(index);
		}
		return list2;
	}

	public static bool LevelSelectRoom(int myLevel, int roomLevel, int autoMatchCount)
	{
		if (myLevel < 30)
		{
			if (Mathf.Abs(myLevel - roomLevel) < 20 + (autoMatchCount - 1) * 10)
			{
				return true;
			}
		}
		else
		{
			if (myLevel >= 100)
			{
				return true;
			}
			if (Mathf.Abs(myLevel - roomLevel) < 10 + (autoMatchCount - 1) * 10)
			{
				return true;
			}
		}
		return false;
	}

	private void OnQuickRoomList(Packet packet)
	{
		GCQuickRoomListPacket gCQuickRoomListPacket = new GCQuickRoomListPacket();
		if (!gCQuickRoomListPacket.ParserPacket(packet))
		{
			Debug.Log("GCQuickRoomListPacket ParserPacket error!!");
			return;
		}
		Debug.Log("on quick search room list, room list count: " + gCQuickRoomListPacket.m_vRoomList.Count);
		if (gCQuickRoomListPacket.m_vRoomList.Count <= 0 || joinRoomType != JoinRoomType.AutoMatch)
		{
			return;
		}
		foreach (GCRoomListPacket.RoomInfo vRoom in gCQuickRoomListPacket.m_vRoomList)
		{
			if (LevelSelectRoom(GameApp.GetInstance().GetGameState().LevelNum, (int)vRoom.m_Creater_level, auto_match_count))
			{
				Debug.Log("Randomly join room:" + vRoom.m_iRoomId);
				Packet packet2 = CGJoinRoomPacket.MakePacket(vRoom.m_iRoomId, (long)(Time.time * 1000f), GameApp.GetInstance().GetGameState().nick_name, (uint)GameApp.GetInstance().GetGameState().Avatar, (uint)GameApp.GetInstance().GetGameState().LevelNum);
				net_com.Send(packet2);
				break;
			}
		}
	}

	private void OnJoinRoom(uint map_id)
	{
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
		if (joinRoomType != 0)
		{
			Debug.Log("join room now...");
			FrameCamera.depth = -5f;
			GameApp.GetInstance().GetGameState().cur_net_map = SceneName.GetNetMapName((int)map_id - mapIdMin + 1);
			SceneName.FadeOutLevel("RoomTUI");
		}
	}

	private void OnJoinRoomError(uint error_info)
	{
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
		if (joinRoomType == JoinRoomType.None)
		{
			return;
		}
		if (joinRoomType == JoinRoomType.AutoMatch)
		{
			FrameCamera.depth = -5f;
			MsgBox.GetComponent<MsgBoxDelegate>().Show("No match found! Please \ntry again!", MsgBoxType.JoinRommFailed);
		}
		else if (joinRoomType == JoinRoomType.SearchWithoutPassword || joinRoomType == JoinRoomType.SearchWithPassword)
		{
			string content = string.Empty;
			switch (error_info)
			{
			case 1u:
				content = "Room full or closed, \ntry another one!";
				break;
			case 2u:
				content = "Room full or closed, \ntry another one!";
				break;
			case 3u:
				content = "Room unavailable, try \nanother one!";
				break;
			}
			FrameCamera.depth = -5f;
			MsgBox.GetComponent<MsgBoxDelegate>().Show(content, MsgBoxType.JoinRommFailed);
		}
		joinRoomType = JoinRoomType.None;
	}

	private void OnIndicatorClose()
	{
		switch (IndicatorPanel.GetComponent<IndicatorTUI>().type)
		{
		case IndicatorTUI.IndicatorType.ServerConnect:
		case IndicatorTUI.IndicatorType.HeartWaiting:
			Debug.Log("Server Connecting Timeount...");
			NetworkObj.DestroyNetCom();
			if (MsgBox != null)
			{
				FrameCamera.depth = -5f;
				MsgBox.GetComponent<MsgBoxDelegate>().Show("UNABLE TO CONNECT.", MsgBoxType.ContectingTimeout);
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
			searchRoomID = -1;
			joinRoomType = JoinRoomType.None;
			FrameCamera.depth = -5f;
			MsgBox.GetComponent<MsgBoxDelegate>().Show("Room unavailable, try \nanother one!", MsgBoxType.JoinRommFailed);
			Debug.Log("join room failed.");
			break;
		}
	}

	private void CancelSearchRoom(bool withPassword)
	{
		joinRoomType = JoinRoomType.None;
		IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		if (withPassword)
		{
			JoinRoomPwdPanel.transform.localPosition = new Vector3(0f, 0f, JoinRoomPwdPanel.transform.localPosition.z);
			JoinRoomPwdPanel.GetComponent<TUITextField>().ResetText();
		}
		else
		{
			SearchPanel.transform.localPosition = new Vector3(0f, 0f, SearchPanel.transform.localPosition.z);
			SearchPanel.GetComponent<TUITextField>().ResetText();
		}
	}

	private void OnReverseHearWaiting()
	{
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().SetContent("WAITING FOR SERVER.");
			IndicatorPanel.GetComponent<IndicatorTUI>().Show(IndicatorTUI.IndicatorType.HeartWaiting);
			FrameCamera.depth = -5f;
		}
	}

	private void OnReverseHearRenew()
	{
		if (SearchPanel.transform.position.y != 0f && JoinRoomPwdPanel.transform.position.y != 0f)
		{
			FrameCamera.depth = 1f;
		}
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
	}

	private void OnReverseHearTimeout()
	{
		NetworkObj.DestroyNetCom();
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
		if (MsgBox != null)
		{
			MsgBox.GetComponent<MsgBoxDelegate>().Show("YOU WERE DISCONNECTED.", MsgBoxType.ContectingTimeout);
			FrameCamera.depth = -5f;
		}
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
		if (status)
		{
			string empty = SFSServerVersion.CoopDomain;
            int coopServerPort = SFSServerVersion.CoopServerPort;
            /*string empty = string.Empty;
			string coopDomain = base.gameObject.GetComponent<SFSServerVersion>().CoopDomain;
			int coopServerPort = base.gameObject.GetComponent<SFSServerVersion>().CoopServerPort;
			mapIdMin = base.gameObject.GetComponent<SFSServerVersion>().CoopMapIdMin;
			try
			{
				IPAddress[] hostAddresses = Dns.GetHostAddresses(coopDomain);
				empty = hostAddresses[0].ToString();
				Debug.Log("Success!!domain: " + coopDomain + "\nport: " + coopServerPort + "\nip: " + empty);
			}
			catch (Exception)
			{
				empty = base.gameObject.GetComponent<SFSServerVersion>().CoopStandbyServerIP;
				Debug.Log("Standby Server IP!!domain: " + coopDomain + "\nport: " + coopServerPort + "\nip: " + empty);
			}*/
            CheckNetWorkCom(empty, coopServerPort);
		}
		else if (MsgBox != null)
		{
			if (IndicatorPanel != null)
			{
				IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
			}
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
			MsgBox.GetComponent<MsgBoxDelegate>().Show("YOU WERE DISCONNECTED.", MsgBoxType.ContectingTimeout);
		}
		NetworkObj.DestroyNetCom();
	}

	private void OnDestroy()
	{
		if (net_com != null)
		{
			net_com.UnregisterCallbacks();
			Debug.Log("unregister callbacks...");
		}
	}
}
