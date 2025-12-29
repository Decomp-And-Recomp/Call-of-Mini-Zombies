using System.Collections;
using TNetSdk;
using UnityEngine;
using Zombie3D;

public class VSRoomTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	private TUIInput[] input;

	private AudioPlayer audioPlayer = new AudioPlayer();

	private TNetObject tnetObj;

	public GameObject MsgBox;

	public GameObject IndicatorPanel;

	public GameObject StartButton;

	public VSRoomOwnerPanel CellPanel;

	public TUIMeshText MapName;

	public TUIMeshText RoomID;

	private void Awake()
	{
		if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Vs)
		{
			base.enabled = false;
			return;
		}
		base.enabled = true;
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
		if (TNetConnection.IsInitialized)
		{
			tnetObj = TNetConnection.Connection;
			tnetObj.AddEventListener(TNetEventSystem.CONNECTION_KILLED, OnConnectionLost);
			tnetObj.AddEventListener(TNetEventSystem.REVERSE_HEART_RENEW, OnReverseHearRenew);
			tnetObj.AddEventListener(TNetEventSystem.REVERSE_HEART_TIMEOUT, OnReverseHearTimeout);
			tnetObj.AddEventListener(TNetEventSystem.REVERSE_HEART_WAITING, OnReverseHearWaiting);
			tnetObj.AddEventListener(TNetEventRoom.USER_ENTER_ROOM, OnUserEnterRoom);
			tnetObj.AddEventListener(TNetEventRoom.USER_EXIT_ROOM, OnUserExitRoom);
			tnetObj.AddEventListener(TNetEventRoom.ROOM_REMOVE, OnDestroyRoom);
			tnetObj.AddEventListener(TNetEventRoom.ROOM_REMOVE_RES, OnDestroyRoomRes);
			tnetObj.AddEventListener(TNetEventRoom.ROOM_VARIABLES_UPDATE, OnRoomVarsUpdate);
			tnetObj.AddEventListener(TNetEventRoom.USER_VARIABLES_UPDATE, OnUserVarsUpdate);
			tnetObj.AddEventListener(TNetEventRoom.ROOM_NAME_CHANGE, OnRoomNameChange);
			tnetObj.AddEventListener(TNetEventRoom.USER_BE_KICKED, OnUserKicked);
			tnetObj.AddEventListener(TNetEventRoom.ROOM_START, OnRoomStart);
			tnetObj.AddEventListener(TNetEventRoom.ROOM_MASTER_CHANGE, OnMasterChange);
		}
		else
		{
			Debug.Log("tnetObj init error!");
		}
	}

	private IEnumerator Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		audioPlayer.AddAudio(base.transform, "Button", true);
		audioPlayer.AddAudio(base.transform, "Back", true);
		IndicatorPanel.GetComponent<IndicatorTUI>().automaticClose = OnConnectingTimeOut;
		CellPanel.RefrashClientCellShow();
		if (TNetConnection.is_server && tnetObj.TimeManager.IsSynchronized())
		{
			StartButton.SetActive(true);
		}
		else
		{
			StartButton.SetActive(false);
		}
		MapName.text_Accessor = SceneName.GetTrueMapName(GameApp.GetInstance().GetGameState().cur_net_map);
		RoomID.text_Accessor = "ROOM " + TNetConnection.Connection.CurRoom.Id;
		OpenClikPlugin.Hide();
		Resources.UnloadUnusedAssets();
		SFSObject dataObj = new SFSObject();
		dataObj.PutInt("avatarType", (int)GameApp.GetInstance().GetGameState().Avatar);
		dataObj.PutInt("avatarLevel", GameApp.GetInstance().GetGameState().LevelNum);
		dataObj.PutUtfString("nickname", GameApp.GetInstance().GetGameState().nick_name);
		dataObj.PutBool("InRoom", true);
		SFSObject userVars = new SFSObject();
		userVars.PutSFSObject("data", dataObj);
		tnetObj.Send(new SetUserVariableRequest(TNetUserVarType.roomState, userVars));
		yield return 1;
		if (TNetConnection.is_server)
		{
			int cnt = SceneName.GetBonusNumberOfScene(GameApp.GetInstance().GetGameState().cur_net_map);
			SFSArray bonusInfoArray = new SFSArray();
			for (int i = 0; i < cnt; i++)
			{
				SFSObject info = new SFSObject();
				info.PutInt("sceneIdx", -1);
				info.PutInt("lockIdx", -1);
				info.PutInt("type", 37);
				bonusInfoArray.AddSFSObject(info);
			}
			SFSObject var = new SFSObject();
			var.PutSFSArray("data", bonusInfoArray);
			tnetObj.Send(new SetRoomVariableRequest(TNetRoomVarType.BonusInfo, var));
			yield break;
		}
		if (tnetObj.CurRoom.ContainsVariable(TNetRoomVarType.GameStarted))
		{
			LoadGameLevel(GameApp.GetInstance().GetGameState().cur_net_map);
			yield break;
		}
		yield return 1;
		if (tnetObj.CurRoom.ContainsVariable(TNetRoomVarType.GameStarted))
		{
			LoadGameLevel(GameApp.GetInstance().GetGameState().cur_net_map);
			yield break;
		}
		yield return 1;
		if (tnetObj.CurRoom.ContainsVariable(TNetRoomVarType.GameStarted))
		{
			LoadGameLevel(GameApp.GetInstance().GetGameState().cur_net_map);
		}
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
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Back");
			tnetObj.Send(new LeaveRoomRequest());
		}
		else if (control.name == "Start_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			if ((bool)CellPanel)
			{
				if (tnetObj.CurRoom.UserCount > 1)
				{
					string[] array = tnetObj.CurRoom.Name.Split('|');
					tnetObj.Send(new ChangeRoomNameRequest(array[0] + "|close"));
				}
				else if (MsgBox != null)
				{
					MsgBox.GetComponent<MsgBoxDelegate>().Show("You need at least 1 party \nmember to start the game.".ToUpper(), MsgBoxType.NotEnoughUser);
				}
			}
		}
		else if (control.name == "kick" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			tnetObj.Send(new KickUserRequest(control.transform.parent.GetComponent<RoomCellData>().sfs_user.Id));
		}
		else if (control.name == "Msg_OK_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			MsgBoxType type = MsgBox.GetComponent<MsgBoxDelegate>().m_type;
			MsgBox.GetComponent<MsgBoxDelegate>().Hide();
			switch (type)
			{
			case MsgBoxType.BeKicked:
				SceneName.FadeOutLevel("VSHallTUI");
				break;
			case MsgBoxType.ContectingTimeout:
				SceneName.FadeOutLevel("VSHallTUI");
				FlurryStatistics.ServerDisconnectEvent("VS");
				break;
			case MsgBoxType.RoomDestroyed:
				TNetConnection.UnregisterSFSSceneCallbacks();
				SceneName.FadeOutLevel("VSHallTUI");
				break;
			case MsgBoxType.OnClosed:
				break;
			}
		}
	}

	private void OnConnectingTimeOut()
	{
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
		if (MsgBox != null)
		{
			MsgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.netDisconnected, MsgBoxType.ContectingTimeout);
		}
		TNetConnection.UnregisterSFSSceneCallbacks();
		TNetConnection.Disconnect();
	}

	private void OnConnectionLost(TNetEventData evt)
	{
		Debug.Log("Connection was lost.");
		TNetConnection.UnregisterSFSSceneCallbacks();
		TNetConnection.Disconnect();
		if (MsgBox != null)
		{
			MsgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.netUnableConnect, MsgBoxType.ContectingTimeout);
		}
	}

	private void OnUserEnterRoom(TNetEventData evt)
	{
		TNetUser tNetUser = (TNetUser)evt.data["user"];
		Debug.Log("User: " + tNetUser.Name + " has just joined Room.");
		if ((bool)CellPanel)
		{
			CellPanel.RefrashClientCellShow();
		}
	}

	private void OnUserExitRoom(TNetEventData evt)
	{
		TNetUser tNetUser = (TNetUser)evt.data["user"];
		Debug.Log("User: " + tNetUser.Name + " has just left Room.");
		if (tNetUser.Id == tnetObj.Myself.Id)
		{
			TNetConnection.UnregisterSFSSceneCallbacks();
			SceneName.FadeOutLevel("VSHallTUI");
		}
		else if ((bool)CellPanel)
		{
			CellPanel.RefrashClientCellShow();
		}
	}

	private void OnDestroyRoomRes(TNetEventData evt)
	{
		if ((int)evt.data["result"] == 0)
		{
			TNetConnection.UnregisterSFSSceneCallbacks();
			SceneName.FadeOutLevel("VSHallTUI");
		}
	}

	private void OnDestroyRoom(TNetEventData evt)
	{
		if ((bool)MsgBox)
		{
			MsgBox.GetComponent<MsgBoxDelegate>().Show("Room closed, \ntry another one!", MsgBoxType.RoomDestroyed);
		}
	}

	private void OnRoomVarsUpdate(TNetEventData evt)
	{
		Debug.Log("On room variable update...");
		if ((int)evt.data["key"] == 0)
		{
			TNetUser tNetUser = (TNetUser)evt.data["user"];
			if (TNetConnection.is_server && tNetUser.Id == tnetObj.Myself.Id)
			{
				tnetObj.Send(new RoomStartRequest());
				Debug.Log("Send Game Start, Start Time:" + tnetObj.CurRoom.GetVariable(TNetRoomVarType.GameStarted).GetDouble("GameStartTime"));
			}
		}
	}

	private void OnRoomStart(TNetEventData evt)
	{
		Debug.Log("On Room Start!");
		LoadGameLevel(GameApp.GetInstance().GetGameState().cur_net_map);
	}

	private void OnMasterChange(TNetEventData evt)
	{
		TNetUser tNetUser = (TNetUser)evt.data["user"];
		if (tNetUser == null)
		{
			return;
		}
		Debug.Log("OnMasterChange..");
		if (tNetUser.Id == tnetObj.Myself.Id)
		{
			Debug.Log("I become master!");
			TNetConnection.is_server = true;
			if ((bool)CellPanel)
			{
				CellPanel.RefrashClientCellShow();
			}
			if (tnetObj.TimeManager.IsSynchronized())
			{
				StartButton.SetActive(true);
			}
			else
			{
				StartButton.SetActive(false);
			}
		}
		else if ((bool)CellPanel)
		{
			CellPanel.RefrashClientCellShow();
		}
	}

	private void LoadGameLevel(string level)
	{
		GameApp.GetInstance().GetGameState().gameMode = GameMode.Vs;
		SceneName.FadeOutLevel(level);
		Debug.Log("current game map: " + level);
	}

	private void OnRoomNameChange(TNetEventData evt)
	{
		if ((ushort)evt.data["userId"] == tnetObj.Myself.Id && TNetConnection.is_server)
		{
			Debug.Log("Room was renamed successfully " + evt.data["roomName"]);
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutDouble("GameStartTime", tnetObj.TimeManager.NetworkTime);
			tnetObj.Send(new SetRoomVariableRequest(TNetRoomVarType.GameStarted, sFSObject));
			SFSObject sFSObject2 = new SFSObject();
			sFSObject2.PutBool("FirstBlood", false);
			sFSObject2.PutUtfString("NickName", string.Empty);
			tnetObj.Send(new SetRoomVariableRequest(TNetRoomVarType.firstBlood, sFSObject2));
		}
	}

	private void OnUserKicked(TNetEventData evt)
	{
		TNetUser tNetUser = (TNetUser)evt.data["user"];
		Debug.Log("user:" + tNetUser.Id + "be kicked.");
		if (tNetUser.Id == tnetObj.Myself.Id)
		{
			TNetConnection.UnregisterSFSSceneCallbacks();
			if ((bool)MsgBox)
			{
				MsgBox.GetComponent<MsgBoxDelegate>().Show("YOU WERE REMOVED FROM \nTHE ROOM.", MsgBoxType.BeKicked);
			}
		}
		else if ((bool)CellPanel)
		{
			CellPanel.RefrashClientCellShow();
		}
	}

	private void OnUserVarsUpdate(TNetEventData evt)
	{
		if ((bool)CellPanel)
		{
			CellPanel.RefrashClientCellShow();
		}
	}

	private void OnReverseHearWaiting(TNetEventData evt)
	{
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().SetContent("WAITING FOR SERVER.");
			IndicatorPanel.GetComponent<IndicatorTUI>().Show(IndicatorTUI.IndicatorType.HeartWaiting);
		}
	}

	private void OnReverseHearRenew(TNetEventData evt)
	{
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
			MsgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.netDisconnected, MsgBoxType.ContectingTimeout);
		}
		TNetConnection.UnregisterSFSSceneCallbacks();
		TNetConnection.Disconnect();
	}

	private void OnDestroy()
	{
		TNetConnection.UnregisterSFSSceneCallbacks();
		Debug.Log("unregister callbacks...");
	}
}
