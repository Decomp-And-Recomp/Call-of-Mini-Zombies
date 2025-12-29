using UnityEngine;
using Zombie3D;

public class NetworkObj : TcpClient
{
	public enum TCP_CONTECTED_STATUS
	{
		None = 0,
		GameServer = 1
	}

	public OnConnectDelegate connect_delegate;

	public OnCloseDelegate close_delegate;

	public OnPacketDelegate packet_delegate;

	public OnJoinRoomDelegate join_room_delegate;

	public OnJoinRoomErrorDelegate join_room_error_delegate;

	public OnLeaveRoomDelegate leave_room_delegate;

	public OnJoinRoomNotifyDelegate join_room_notity_delegate;

	public OnLeaveRoomNotifyDelegate leave_room_notity_delegate;

	public OnKickPlayerDelegate kick_player_delegate;

	public OnKickPlayerNotifyDelegate kick_player_notify_delegate;

	public OnDestroyDelegate destroy_delegate;

	public OnDestroyNotifyDelegate destroy_notify_delegate;

	public OnBeKickedDelegate be_kicked_delegate;

	public OnSomeoneBirthDelegate someone_birth_delegate;

	public OnKillDelegate kill_degegate;

	public OnReverseHearTimeout reverse_heart_timeout_delegate;

	public OnReverseHearWaiting reverse_heart_waiting_delegate;

	public OnReverseHearRenew reverse_heart_renew_delegate;

	public OnContectingTimeout contecting_timeout;

	public OnContectingLost contecting_lost;

	public NetUserInfo m_netUserInfo = new NetUserInfo();

	public NetUserInfo[] netUserInfo_array = new NetUserInfo[4];

	protected float m_last_heart_time;

	protected float m_reverse_heart_time;

	protected bool net_work_status;

	protected bool is_wait_server;

	protected float temp_time;

	public static TCP_CONTECTED_STATUS tcp_contected_status;

	protected float connect_gameServer_time;

	protected bool connect_gameServer;

	private void Start()
	{
		ClearNetUserArr();
		m_reverse_heart_time = (m_last_heart_time = Time.time);
		temp_time = Time.time;
	}

	public void ClearNetUserArr()
	{
		for (int i = 0; i < 4; i++)
		{
			netUserInfo_array[i] = null;
		}
	}

	public void UnregisterCallbacks()
	{
		connect_delegate = null;
		close_delegate = null;
		packet_delegate = null;
		join_room_delegate = null;
		join_room_error_delegate = null;
		leave_room_delegate = null;
		join_room_notity_delegate = null;
		leave_room_notity_delegate = null;
		kick_player_delegate = null;
		kick_player_notify_delegate = null;
		destroy_delegate = null;
		destroy_notify_delegate = null;
		be_kicked_delegate = null;
		someone_birth_delegate = null;
		kill_degegate = null;
		reverse_heart_timeout_delegate = null;
		reverse_heart_waiting_delegate = null;
		reverse_heart_renew_delegate = null;
		contecting_timeout = null;
		contecting_lost = null;
	}

	public void GoGameServer(string ip, int port)
	{
		Debug.Log("connecting Game Server...ip: " + ip + "   port:" + port);
		Connect(ip, port);
		connect_gameServer = true;
		connect_gameServer_time = Time.time;
	}

	public override void Update()
	{
		base.Update();
		if (tcp_contected_status == TCP_CONTECTED_STATUS.None)
		{
			if (connect_gameServer && Time.time - connect_gameServer_time > 10f)
			{
				Debug.Log("Can't conect to server...");
				tcp_contected_status = TCP_CONTECTED_STATUS.None;
				connect_gameServer = false;
				connect_gameServer_time = 0f;
				if (contecting_timeout != null)
				{
					contecting_timeout();
				}
			}
		}
		else
		{
			if (tcp_contected_status != TCP_CONTECTED_STATUS.GameServer)
			{
				return;
			}
			if (Time.time - m_last_heart_time >= 3f)
			{
				if (IsContected())
				{
					Packet packet = CGHeartBeatPacket.MakePacket((long)(Time.time * 1000f));
					Send(packet);
					m_last_heart_time = Time.time;
				}
				else
				{
					Debug.Log("TCP not conected...");
					tcp_contected_status = TCP_CONTECTED_STATUS.None;
					if (contecting_lost != null)
					{
						contecting_lost();
					}
				}
			}
			if (Time.time - m_reverse_heart_time > 30f && net_work_status)
			{
				Debug.Log("Server no respond...!!! TCP will shut down!...\n And Game Will Over...");
				if (reverse_heart_timeout_delegate != null)
				{
					reverse_heart_timeout_delegate();
				}
				net_work_status = false;
				tcp_contected_status = TCP_CONTECTED_STATUS.None;
			}
			else if (Time.time - m_reverse_heart_time > 10f && !is_wait_server)
			{
				Debug.Log("Waiting Server respond...");
				is_wait_server = true;
				if (reverse_heart_waiting_delegate != null)
				{
					reverse_heart_waiting_delegate();
				}
			}
		}
	}

	public override void OnConnected()
	{
		base.OnConnected();
		if (tcp_contected_status == TCP_CONTECTED_STATUS.None)
		{
			tcp_contected_status = TCP_CONTECTED_STATUS.GameServer;
			connect_gameServer = true;
			connect_gameServer_time = 0f;
		}
		if (connect_delegate != null)
		{
			connect_delegate();
		}
		net_work_status = true;
	}

	public override void OnPacket(Packet packet)
	{
		base.OnPacket(packet);
		uint val = 0u;
		if (packet.WatchUInt32(ref val, 4))
		{
			switch ((PROTOCOLS)val)
			{
			case PROTOCOLS.GC_HEARTBEAT:
				OnHeartProess(packet);
				break;
			case PROTOCOLS.GC_JOINROOM:
				JoinRoom(packet);
				break;
			case PROTOCOLS.GC_JOINROOM_NOTIFY:
				PlayerJoinNotify(packet);
				break;
			case PROTOCOLS.GC_LEAVEROOM:
				LeaveRoom(packet);
				break;
			case PROTOCOLS.GC_LEAVEROOM_NOTIFY:
				SomeoneLeaveRoom(packet);
				break;
			case PROTOCOLS.GC_KICKUSER:
				KickPlayer(packet);
				break;
			case PROTOCOLS.GC_BYKICKED_NOTIFY:
				UserBeKicked(packet);
				break;
			case PROTOCOLS.GC_KICKUSER_NOTIFY:
				SomeoneKickerRoom(packet);
				break;
			case PROTOCOLS.GC_DESTROYROOM:
				DestroyRoom(packet);
				break;
			case PROTOCOLS.GC_DESTROYROOM_NOTIFY:
				DestroyRoomNotify(packet);
				break;
			case PROTOCOLS.GC_USERRBIRTH_NOTIFY:
				SomeoneBirthNotify(packet);
				break;
			case PROTOCOLS.GC_STARTGAME_NOTIFY:
				OnStartNotifyGame(packet);
				break;
			}
		}
		if (packet_delegate != null)
		{
			packet_delegate(packet);
		}
	}

	public override void OnClosed()
	{
		base.OnClosed();
		Debug.Log("OnClosed...");
		if (close_delegate != null)
		{
			close_delegate();
		}
	}

	public override void OnKilled()
	{
		base.OnKilled();
		if (kill_degegate != null)
		{
			kill_degegate();
		}
	}

	public override void OnTimeout()
	{
		base.OnTimeout();
		Debug.Log("OnTimeout...");
		if (contecting_timeout != null)
		{
			contecting_timeout();
		}
	}

	public void OnHeartProess(Packet packet)
	{
		GCHeartBeatPacket gCHeartBeatPacket = new GCHeartBeatPacket();
		if (!gCHeartBeatPacket.ParserPacket(packet))
		{
			return;
		}
		if (m_netUserInfo != null)
		{
			m_netUserInfo.net_ping = (Time.time - (float)gCHeartBeatPacket.m_lLocalTime / 1000f) / 2f;
			if (m_netUserInfo.net_ping > 0.4f)
			{
				Debug.Log("net is slow!!! ping:" + m_netUserInfo.net_ping);
			}
		}
		m_reverse_heart_time = Time.time;
		if (is_wait_server)
		{
			is_wait_server = false;
			if (reverse_heart_renew_delegate != null)
			{
				reverse_heart_renew_delegate();
			}
		}
	}

	public void PlayerJoinNotify(Packet packet)
	{
		GCJoinRoomNotifyPacket gCJoinRoomNotifyPacket = new GCJoinRoomNotifyPacket();
		if (gCJoinRoomNotifyPacket.ParserPacket(packet))
		{
			Debug.Log("PlayerJoinNotify" + gCJoinRoomNotifyPacket.m_room_index);
			bool master = ((gCJoinRoomNotifyPacket.m_room_index == 0) ? true : false);
			netUserInfo_array[gCJoinRoomNotifyPacket.m_room_index] = new NetUserInfo();
			netUserInfo_array[gCJoinRoomNotifyPacket.m_room_index].SetNetUserInfo(master, gCJoinRoomNotifyPacket.m_strNickname, 0, (int)gCJoinRoomNotifyPacket.m_iUserId, 0f, 0, (int)gCJoinRoomNotifyPacket.m_room_index, (int)gCJoinRoomNotifyPacket.m_iAvatarType, (int)gCJoinRoomNotifyPacket.m_iLevel);
			if (join_room_notity_delegate != null)
			{
				join_room_notity_delegate(netUserInfo_array[gCJoinRoomNotifyPacket.m_room_index]);
			}
		}
	}

	public void JoinRoom(Packet packet)
	{
		GCJoinRoomPacket gCJoinRoomPacket = new GCJoinRoomPacket();
		if (!gCJoinRoomPacket.ParserPacket(packet))
		{
			return;
		}
		if (gCJoinRoomPacket.m_iResult == 0)
		{
			float num = (Time.time - (float)gCJoinRoomPacket.m_lLocalTime / 1000f) / 2f;
			m_netUserInfo.SetNetUserInfo(false, GameApp.GetInstance().GetGameState().nick_name, (int)gCJoinRoomPacket.m_iRoomId, (int)gCJoinRoomPacket.m_iUserId, num, (int)gCJoinRoomPacket.m_map_id, (int)gCJoinRoomPacket.m_room_index, (int)GameApp.GetInstance().GetGameState().Avatar, GameApp.GetInstance().GetGameState().LevelNum);
			Debug.Log("join ping : " + num + "user_id:" + gCJoinRoomPacket.m_iUserId);
			netUserInfo_array[gCJoinRoomPacket.m_room_index] = m_netUserInfo;
			if (join_room_delegate != null)
			{
				join_room_delegate(gCJoinRoomPacket.m_map_id);
			}
		}
		else
		{
			Debug.Log("result error : " + gCJoinRoomPacket.m_iResult);
			if (join_room_error_delegate != null)
			{
				join_room_error_delegate(gCJoinRoomPacket.m_iResult);
			}
		}
	}

	public void LeaveRoom(Packet packet)
	{
		GCLeaveRoomPacket gCLeaveRoomPacket = new GCLeaveRoomPacket();
		if (!gCLeaveRoomPacket.ParserPacket(packet))
		{
			return;
		}
		if (gCLeaveRoomPacket.m_iResult == 0)
		{
			for (int i = 0; i < 4; i++)
			{
				netUserInfo_array[i] = null;
			}
			m_netUserInfo.is_master = false;
			m_netUserInfo.cur_room_id = -1;
			m_netUserInfo.room_index = -1;
			if (leave_room_delegate != null)
			{
				leave_room_delegate();
			}
		}
		else
		{
			Debug.Log("result error : " + gCLeaveRoomPacket.m_iResult);
		}
	}

	public void KickPlayer(Packet packet)
	{
		GCKickUserPacket gCKickUserPacket = new GCKickUserPacket();
		if (!gCKickUserPacket.ParserPacket(packet))
		{
			return;
		}
		if (gCKickUserPacket.m_iResult == 0)
		{
			int num = -1;
			for (int i = 0; i < 4; i++)
			{
				if (netUserInfo_array[i] != null && netUserInfo_array[i].user_id == gCKickUserPacket.m_iUserId)
				{
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				netUserInfo_array[num] = null;
				if (kick_player_delegate != null)
				{
					kick_player_delegate((int)gCKickUserPacket.m_iUserId);
				}
			}
		}
		else
		{
			Debug.Log("result error : " + gCKickUserPacket.m_iResult);
		}
	}

	public void SomeoneKickerRoom(Packet packet)
	{
		GCKickUserNotifyPacket gCKickUserNotifyPacket = new GCKickUserNotifyPacket();
		if (!gCKickUserNotifyPacket.ParserPacket(packet))
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < 4; i++)
		{
			if (netUserInfo_array[i] != null && netUserInfo_array[i].user_id == gCKickUserNotifyPacket.m_iUserId)
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			netUserInfo_array[num] = null;
			if (kick_player_notify_delegate != null)
			{
				kick_player_notify_delegate((int)gCKickUserNotifyPacket.m_iUserId);
			}
		}
	}

	public void SomeoneLeaveRoom(Packet packet)
	{
		GCLeaveRoomNotifyPacket gCLeaveRoomNotifyPacket = new GCLeaveRoomNotifyPacket();
		if (!gCLeaveRoomNotifyPacket.ParserPacket(packet))
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < 4; i++)
		{
			if (netUserInfo_array[i] != null && netUserInfo_array[i].user_id == gCLeaveRoomNotifyPacket.m_iUserId)
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			netUserInfo_array[num] = null;
			if (leave_room_notity_delegate != null)
			{
				leave_room_notity_delegate((int)gCLeaveRoomNotifyPacket.m_iUserId);
			}
		}
	}

	public void DestroyRoom(Packet packet)
	{
		GCDestroyRoomPacket gCDestroyRoomPacket = new GCDestroyRoomPacket();
		if (!gCDestroyRoomPacket.ParserPacket(packet))
		{
			return;
		}
		if (gCDestroyRoomPacket.m_iResult == 0)
		{
			for (int i = 0; i < 4; i++)
			{
				netUserInfo_array[i] = null;
			}
			m_netUserInfo.is_master = false;
			m_netUserInfo.cur_room_id = -1;
			m_netUserInfo.room_index = -1;
			if (destroy_delegate != null)
			{
				destroy_delegate();
			}
		}
		else
		{
			Debug.Log("result error : " + gCDestroyRoomPacket.m_iResult);
		}
	}

	public void DestroyRoomNotify(Packet packet)
	{
		GCDestroyRoomNotifyPacket gCDestroyRoomNotifyPacket = new GCDestroyRoomNotifyPacket();
		if (gCDestroyRoomNotifyPacket.ParserPacket(packet))
		{
			for (int i = 0; i < 4; i++)
			{
				netUserInfo_array[i] = null;
			}
			m_netUserInfo.is_master = false;
			m_netUserInfo.cur_room_id = -1;
			m_netUserInfo.room_index = -1;
			if (destroy_notify_delegate != null)
			{
				destroy_notify_delegate();
			}
		}
	}

	public void UserBeKicked(Packet packet)
	{
		GUserKickedPacket gUserKickedPacket = new GUserKickedPacket();
		if (gUserKickedPacket.ParserPacket(packet))
		{
			for (int i = 0; i < 4; i++)
			{
				netUserInfo_array[i] = null;
			}
			m_netUserInfo.is_master = false;
			m_netUserInfo.cur_room_id = -1;
			m_netUserInfo.room_index = -1;
			if (be_kicked_delegate != null)
			{
				be_kicked_delegate();
			}
		}
	}

	public void SomeoneBirthNotify(Packet packet)
	{
		GCUserBirthNotifyPacket gCUserBirthNotifyPacket = new GCUserBirthNotifyPacket();
		if (!gCUserBirthNotifyPacket.ParserPacket(packet))
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			if (netUserInfo_array[i] != null && netUserInfo_array[i].user_id == gCUserBirthNotifyPacket.m_iUserId)
			{
				Multiplayer multiplayer = new Multiplayer();
				multiplayer.InitAvatar((AvatarType)netUserInfo_array[i].avatarType, gCUserBirthNotifyPacket.m_iBirthPointIndex);
				multiplayer.InitWeaponList((int)gCUserBirthNotifyPacket.m_iWeaponIndex1, (int)gCUserBirthNotifyPacket.m_iWeaponIndex2, (int)gCUserBirthNotifyPacket.m_iWeaponIndex3);
				multiplayer.m_multi_id = (int)gCUserBirthNotifyPacket.m_iUserId;
				netUserInfo_array[i].multiplayer = multiplayer;
				multiplayer.nick_name = netUserInfo_array[i].nick_name;
				if (someone_birth_delegate != null)
				{
					someone_birth_delegate(multiplayer);
				}
				break;
			}
		}
	}

	public void OnStartNotifyGame(Packet packet)
	{
		GCStartGameNotifyPacket gCStartGameNotifyPacket = new GCStartGameNotifyPacket();
		if (gCStartGameNotifyPacket.ParserPacket(packet))
		{
			Resources.UnloadUnusedAssets();
			GameApp.GetInstance().GetGameState().gameMode = GameMode.Coop;
			SceneName.FadeOutLevel(GameApp.GetInstance().GetGameState().cur_net_map);
			Debug.Log("Start Game Now...");
		}
	}

	public static void CheckNetWorkCom()
	{
		if (GameObject.Find("NetworkObj") == null)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/NetworkObj")) as GameObject;
			gameObject.name = "NetworkObj";
			Object.DontDestroyOnLoad(gameObject);
			GameApp.GetInstance().GetGameState().net_com = gameObject.GetComponent<NetworkObj>();
			Debug.Log("CheckNetWorkCom, Create NetworkObj...");
		}
	}

	public static void DestroyNetCom()
	{
		if (GameObject.Find("NetworkObj") != null)
		{
			GameObject obj = GameObject.Find("NetworkObj");
			Object.Destroy(obj);
			Debug.Log("DestroyNetCom NetworkObj...");
		}
		tcp_contected_status = TCP_CONTECTED_STATUS.None;
	}
}
