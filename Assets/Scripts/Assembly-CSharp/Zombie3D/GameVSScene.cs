using System;
using System.Collections;
using System.Collections.Generic;
using TNetSdk;
using UnityEngine;

namespace Zombie3D
{
	public class GameVSScene : GameScene
	{
		public Dictionary<TNetUser, Player> SFS_Player_Arr = new Dictionary<TNetUser, Player>();

		public Dictionary<int, VSPlayerReport> SFS_Player_Report = new Dictionary<int, VSPlayerReport>();

		public float TimeToNextBonusSpawn = 15f;

		private TNetObject tnetObj;

		private float freshman_time;

		private bool is_freshman = true;

		public override void Init(int index)
		{
			if (TNetConnection.IsInitialized)
			{
				tnetObj = TNetConnection.Connection;
				tnetObj.AddEventListener(TNetEventSystem.CONNECTION_KILLED, OnConnectionLost);
				tnetObj.AddEventListener(TNetEventSystem.DISCONNECT, OnClosed);
				tnetObj.AddEventListener(TNetEventRoom.USER_ENTER_ROOM, OnUserEnterRoom);
				tnetObj.AddEventListener(TNetEventRoom.USER_EXIT_ROOM, OnUserExitRoom);
				tnetObj.AddEventListener(TNetEventRoom.OBJECT_MESSAGE, OnObjectMessage);
				tnetObj.AddEventListener(TNetEventRoom.ROOM_VARIABLES_UPDATE, OnRoomVarsUpdate);
				tnetObj.AddEventListener(TNetEventRoom.USER_VARIABLES_UPDATE, OnUserVarsUpdate);
				tnetObj.AddEventListener(TNetEventRoom.LOCK_STH, OnLockSth);
				tnetObj.AddEventListener(TNetEventRoom.ROOM_MASTER_CHANGE, OnMasterChange);
			}
			else
			{
				Debug.LogError("TNetConnection init error!");
			}
			GameApp.GetInstance().GetGameState().loot_cash = 0;
			is_game_excute = true;
			is_freshman = true;
			camera = GameObject.Find("Main Camera").GetComponent<TPSSimpleCameraScript>();
			CreateSceneBorderData();
			base.UnlockAvatar = AvatarType.None;
			base.UnlockWeapon = null;
			player = new Player();
			player.Init();
			camera.Init();
			playingState = PlayingState.GamePlaying;
			Color[] array = new Color[8]
			{
				Color.white,
				Color.red,
				Color.blue,
				Color.yellow,
				Color.magenta,
				Color.gray,
				Color.grey,
				Color.cyan
			};
			int num = UnityEngine.Random.Range(0, array.Length);
			RenderSettings.ambientLight = array[num];
			SFS_Player_Arr[tnetObj.Myself] = player;
			SFS_Player_Report[tnetObj.Myself.Id] = new VSPlayerReport(GameApp.GetInstance().GetGameState().nick_name, true);
			foreach (TNetUser user in tnetObj.CurRoom.UserList)
			{
				if (user.Id != tnetObj.Myself.Id)
				{
					OnSFSPlayerBirth(user);
				}
			}
			bonusList = GameObject.FindGameObjectsWithTag("Bonus");
			OnSFSBonusUpdate(tnetObj.CurRoom);
			enemyList = new Hashtable();
			woodboxList = new GameObject[0];
			GameStartTime = Time.time;
			GC.Collect();
		}

		public override void DoLogic(float deltaTime)
		{
			if (!is_game_excute)
			{
				return;
			}
			if (tnetObj != null)
			{
				tnetObj.Update(deltaTime);
			}
			foreach (Player value in SFS_Player_Arr.Values)
			{
				value.DoLogic(deltaTime);
			}
			PeriodicallyRespawnBonus();
		}

		public void PeriodicallyRespawnBonus()
		{
			if (tnetObj.CurRoom.RoomMasterID == tnetObj.Myself.Id)
			{
				TimeToNextBonusSpawn -= Time.deltaTime;
				if (TimeToNextBonusSpawn <= 0f)
				{
					TimeToNextBonusSpawn = 30f;
					player.UpdateAndBroadcastBonusInfo(true, "-1");
				}
			}
		}

		public void GetLastMasterKiller()
		{
			float num = -9999f;
			int num2 = -1;
			foreach (int key in SFS_Player_Report.Keys)
			{
				if (SFS_Player_Report[key].kill_cout != 0 || SFS_Player_Report[key].death_count != 0)
				{
					float num3 = (float)SFS_Player_Report[key].kill_cout - 0.6f * (float)SFS_Player_Report[key].death_count + 1.5f * (float)SFS_Player_Report[key].combo_kill;
					if (num3 > num)
					{
						num = num3;
						num2 = key;
					}
				}
			}
			if (num2 == tnetObj.Myself.Id)
			{
				GameGUI.gameOverPanel.GetComponent<GameOverTUI>().vsChampion = true;
				player.PlayerObject.GetComponent<PlayerShell>().OnAvatarShowCameraChange(true, player);
				player.pickupItemsPacket = new Dictionary<ItemType, int>();
				player.pickupItemsPacket.Add(ItemType.Crystal, 1);
				GameGUI.ShowItemsReportPanel();
				Time.timeScale = 0f;
				return;
			}
			foreach (TNetUser key2 in SFS_Player_Arr.Keys)
			{
				if (key2.Id == num2)
				{
					player.PlayerObject.GetComponent<PlayerShell>().OnAvatarShowCameraChange(false, SFS_Player_Arr[key2]);
				}
			}
		}

		public override void SaveDataReport()
		{
			GameObject gameObject = new GameObject("VSReprotObj");
			gameObject.AddComponent<VSReportData>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.GetComponent<VSReportData>().player_reports = new List<VSPlayerReport>();
			foreach (VSPlayerReport value in SFS_Player_Report.Values)
			{
				gameObject.GetComponent<VSReportData>().player_reports.Add(value);
			}
			gameObject.GetComponent<VSReportData>().combo_kill_count = SFS_Player_Report[tnetObj.Myself.Id].combo_kill;
			gameObject.GetComponent<VSReportData>().total_kill_count = SFS_Player_Report[tnetObj.Myself.Id].kill_cout;
			gameObject.GetComponent<VSReportData>().freshman_time = freshman_time;
			GameGUI.gameOverPanel.GetComponent<GameOverTUI>().totalKills = SFS_Player_Report[tnetObj.Myself.Id].kill_cout;
			GameGUI.gameOverPanel.GetComponent<GameOverTUI>().totalDeaths = SFS_Player_Report[tnetObj.Myself.Id].death_count;
		}

		public override void QuitGameForDisconnect(float time)
		{
			is_game_excute = false;
			GameGUI.vsLabelMissionTime.GetComponent<VSGameMissionTimer>().isMissionOver = true;
			SaveDataReport();
			if (tnetObj != null)
			{
				tnetObj.Send(new LeaveRoomRequest());
				TNetConnection.UnregisterSFSSceneCallbacks();
			}
			DestroyNetConnection();
			TimeGameOver(time);
		}

		public TNetUser GetSFSUserFromArray(int id)
		{
			foreach (TNetUser key in SFS_Player_Arr.Keys)
			{
				if (key.Id == id)
				{
					return key;
				}
			}
			return null;
		}

		private void DestroyNetConnection()
		{
			TNetConnection.UnregisterSFSSceneCallbacks();
			TNetConnection.Disconnect();
			tnetObj = null;
		}

		private void OnClosed(TNetEventData evt)
		{
			TNetConnection.UnregisterSFSSceneCallbacks();
			GameGUI.OnClosed();
		}

		private void OnConnectionLost(TNetEventData evt)
		{
			TNetConnection.UnregisterSFSSceneCallbacks();
			GameGUI.OnConnectingLost();
		}

		private void OnUserEnterRoom(TNetEventData evt)
		{
			TNetUser tNetUser = (TNetUser)evt.data["user"];
			Debug.Log("User: " + tNetUser.Name + " has just joined Room: ");
			GameGUI.vsMessagePanel.AddSFSRoom(tNetUser.Name + " JOINED THE GAME");
		}

		private void OnUserExitRoom(TNetEventData evt)
		{
			TNetUser tNetUser = (TNetUser)evt.data["user"];
			Debug.Log("User: " + tNetUser.Name + " has just left Room: ");
			if (tNetUser == tnetObj.Myself)
			{
				Debug.Log("user leave room..");
				return;
			}
			Player player = null;
			if (SFS_Player_Arr.ContainsKey(tNetUser))
			{
				player = SFS_Player_Arr[tNetUser];
				SFS_Player_Arr.Remove(tNetUser);
				SFS_Player_Report.Remove(tNetUser.Id);
				GameGUI.vsMessagePanel.AddSFSRoom(tNetUser.Name + " LEFT THE GAME");
				GameGUI.vsSeatState.RefrashSeatList(SFS_Player_Arr.Count);
			}
			if (player != null)
			{
				GameApp.GetInstance().GetGameScene().GetCamera()
					.player = base.player;
				UnityEngine.Object.Destroy(player.PlayerObject);
				player = null;
			}
		}

		private void OnObjectMessage(TNetEventData evt)
		{
			SFSObject sFSObject = (SFSObject)evt.data["message"];
			TNetUser tNetUser = (TNetUser)evt.data["user"];
			if (!SFS_Player_Arr.ContainsKey(tNetUser))
			{
				return;
			}
			if (sFSObject.ContainsKey("msg"))
			{
				GameGUI.vsMessagePanel.AddSFSRoom(sFSObject.GetUtfString("msg"));
			}
			else if (sFSObject.ContainsKey("comboCount"))
			{
				int @int = sFSObject.GetInt("comboCount");
				GameGUI.SetComboCountLabel(tNetUser.Name, @int);
				if (@int >= 8)
				{
					UnityEngine.Object.Instantiate(Resources.Load("Prefabs/narratage/8"), new Vector3(0f, 10000.1f, 0f), Quaternion.identity);
				}
				else
				{
					UnityEngine.Object.Instantiate(Resources.Load("Prefabs/narratage/" + @int), new Vector3(0f, 10000.1f, 0f), Quaternion.identity);
				}
			}
			if (tNetUser == tnetObj.Myself)
			{
				return;
			}
			if (sFSObject.ContainsKey("trans"))
			{
				SFSObject data = sFSObject.GetSFSObject("trans") as SFSObject;
				if (tNetUser != null && tNetUser.Id != tnetObj.Myself.Id)
				{
					NetworkTransform ntransform = NetworkTransform.FromSFSObject(data);
					SFS_Player_Arr[tNetUser].networkTransform.Load(ntransform);
					SFS_Player_Arr[tNetUser].UpdateNetworkTrans();
				}
			}
			else if (sFSObject.ContainsKey("damage"))
			{
				SFSObject sFSObject2 = sFSObject.GetSFSObject("damage") as SFSObject;
				float @float = sFSObject2.GetFloat("damageVal");
				int int2 = sFSObject2.GetInt("weaponType");
				SFS_Player_Arr[player.tnet_user].OnInjuredWithUser(tNetUser, @float, int2);
			}
			else if (sFSObject.ContainsKey("killed"))
			{
				SFS_Player_Arr[player.tnet_user].PlusVsKillCount();
				if (is_freshman)
				{
					is_freshman = false;
					freshman_time = Time.time - GameStartTime;
				}
			}
			else if (sFSObject.ContainsKey("deaded"))
			{
				SFS_Player_Arr[tNetUser].OnDead();
				SFS_Player_Arr[tNetUser].SetState(PlayerStateType.Dead);
			}
			else if (sFSObject.ContainsKey("rebirth"))
			{
				SFS_Player_Arr[tNetUser].OnVSRebirth();
				NetworkTransform ntransform2 = NetworkTransform.FromSFSObject(sFSObject.GetSFSObject("rebirth"));
				SFS_Player_Arr[tNetUser].networkTransform.Load(ntransform2);
				SFS_Player_Arr[tNetUser].UpdateNetworkTrans();
			}
			else if (sFSObject.ContainsKey("pgmFire"))
			{
				ISFSObject sFSObject3 = sFSObject.GetSFSObject("pgmFire");
				float float2 = sFSObject3.GetFloat("pgm_x");
				float float3 = sFSObject3.GetFloat("pgm_y");
				float float4 = sFSObject3.GetFloat("pgm_z");
				Multiplayer multiplayer = SFS_Player_Arr[tNetUser] as Multiplayer;
				multiplayer.MultiplayerSniperFire(new Vector3(float2, float3, float4));
			}
		}

		private void OnRoomVarsUpdate(TNetEventData evt)
		{
			switch ((TNetRoomVarType)(int)evt.data["key"])
			{
			case TNetRoomVarType.BonusInfo:
				OnSFSBonusUpdate(tnetObj.CurRoom);
				break;
			case TNetRoomVarType.firstBlood:
				if (tnetObj.CurRoom.GetVariable(TNetRoomVarType.firstBlood).GetBool("FirstBlood"))
				{
					string utfString = tnetObj.CurRoom.GetVariable(TNetRoomVarType.firstBlood).GetUtfString("NickName");
					GameGUI.SetComboCountLabel(utfString, 1);
					if (!GameObject.Find("FirstBloodSound"))
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/narratage/1"), new Vector3(0f, 10000.1f, 0f), Quaternion.identity) as GameObject;
						gameObject.name = "FirstBloodSound";
					}
				}
				break;
			}
		}

		private void OnUserVarsUpdate(TNetEventData evt)
		{
			TNetUserVarType tNetUserVarType = (TNetUserVarType)(int)evt.data["key"];
			TNetUser tNetUser = (TNetUser)evt.data["user"];
			if (tNetUser == tnetObj.Myself && tNetUserVarType != TNetUserVarType.userStatistics)
			{
				return;
			}
			if (!SFS_Player_Arr.ContainsKey(tNetUser) && tNetUserVarType == TNetUserVarType.avatarData && tNetUser.Id != tnetObj.Myself.Id)
			{
				OnSFSPlayerBirth(tNetUser);
				return;
			}
			if (tNetUserVarType == TNetUserVarType.CurWeapon && SFS_Player_Arr.ContainsKey(tNetUser))
			{
				((Multiplayer)SFS_Player_Arr[tNetUser]).ChangeWeaponWithindex(tNetUser.GetVariable(TNetUserVarType.CurWeapon).GetInt("data"));
			}
			if (tNetUserVarType == TNetUserVarType.PlayerState && SFS_Player_Arr.ContainsKey(tNetUser))
			{
				SFS_Player_Arr[tNetUser].SetState((PlayerStateType)tNetUser.GetVariable(TNetUserVarType.PlayerState).GetInt("data"));
			}
			if (tNetUserVarType == TNetUserVarType.PlayerBonusState && SFS_Player_Arr.ContainsKey(tNetUser))
			{
				((Multiplayer)SFS_Player_Arr[tNetUser]).SetBonusStateWithType((PlayerBonusStateType)tNetUser.GetVariable(TNetUserVarType.PlayerBonusState).GetInt("data"));
			}
			if (tNetUserVarType == TNetUserVarType.userStatistics && SFS_Player_Arr.ContainsKey(tNetUser))
			{
				OnSFSPlayerStatisticUpdate(tNetUser);
			}
		}

		private void OnSFSPlayerBirth(TNetUser user)
		{
			if (!SFS_Player_Arr.ContainsKey(user) && user.ContainsVariable(TNetUserVarType.avatarData))
			{
				Debug.Log("OnSFSPlayerBirth name:" + user.ToString());
				SFSObject variable = user.GetVariable(TNetUserVarType.avatarData);
				Multiplayer multiplayer = new Multiplayer();
				multiplayer.nick_name = variable.GetUtfString("NickName");
				multiplayer.InitAvatar((AvatarType)variable.GetInt("avatarType"), 0u);
				multiplayer.InitWeaponList(variable.GetInt("weapon1"), variable.GetFloat("weaponPara1"), variable.GetInt("weapon2"), variable.GetFloat("weaponPara2"), variable.GetInt("weapon3"), variable.GetFloat("weaponPara3"));
				multiplayer.birth_point_index = (uint)variable.GetInt("birthPoint");
				multiplayer.Init();
				multiplayer.tnet_user = user;
				if (user.ContainsVariable(TNetUserVarType.CurWeapon))
				{
					multiplayer.ChangeWeaponWithindex(user.GetVariable(TNetUserVarType.CurWeapon).GetInt("data"));
				}
				else
				{
					multiplayer.ChangeWeaponWithindex(0);
				}
				if (user.ContainsVariable(TNetUserVarType.PlayerState))
				{
					multiplayer.SetState((PlayerStateType)user.GetVariable(TNetUserVarType.PlayerState).GetInt("data"));
				}
				else
				{
					multiplayer.SetState(PlayerStateType.Idle);
				}
				if (user.ContainsVariable(TNetUserVarType.PlayerBonusState))
				{
					multiplayer.SetBonusStateWithType((PlayerBonusStateType)user.GetVariable(TNetUserVarType.PlayerBonusState).GetInt("data"));
				}
				else
				{
					multiplayer.SetBonusStateWithType(PlayerBonusStateType.Normal);
				}
				SFS_Player_Arr[user] = multiplayer;
				if (!SFS_Player_Report.ContainsKey(user.Id))
				{
					SFS_Player_Report[user.Id] = new VSPlayerReport(multiplayer.nick_name, false);
				}
				GameUIScriptNew.GetGameUIScript().vsSeatState.RefrashSeatList(SFS_Player_Arr.Count);
			}
		}

		private void OnSFSBonusUpdate(TNetRoom room)
		{
			if (!room.ContainsVariable(TNetRoomVarType.BonusInfo))
			{
				return;
			}
			ISFSArray sFSArray = room.GetVariable(TNetRoomVarType.BonusInfo).GetSFSArray("data");
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				ISFSObject sFSObject = sFSArray.GetSFSObject(i);
				int @int = sFSObject.GetInt("sceneIdx");
				if (@int == -1)
				{
					break;
				}
				int int2 = sFSObject.GetInt("lockIdx");
				ItemType int3 = (ItemType)sFSObject.GetInt("type");
				BonusManager component = GetBonusItemFromSceneIndex(@int).GetComponent<BonusManager>();
				if (int3 != ItemType.NONE)
				{
					if (component.GetCurrentBonusType() != int3)
					{
						component.InitBonusObjectWithTypeAndId(int3, int2);
					}
				}
				else if (component.GetCurrentBonusType() != ItemType.NONE)
				{
					UnityEngine.Object.Destroy(component.bonus);
					component.bonus = null;
				}
			}
		}

		private GameObject GetBonusItemFromLockIndex(string idx)
		{
			for (int i = 0; i < bonusList.Length; i++)
			{
				if (bonusList[i].GetComponent<BonusManager>().ID.ToString() == idx)
				{
					return bonusList[i];
				}
			}
			return null;
		}

		private GameObject GetBonusItemFromSceneIndex(int idx)
		{
			for (int i = 0; i < bonusList.Length; i++)
			{
				if (bonusList[i].GetComponent<BonusManager>().bonusSceneIndex == idx)
				{
					return bonusList[i];
				}
			}
			return null;
		}

		private void OnSFSPlayerStatisticUpdate(TNetUser user)
		{
			if (!SFS_Player_Report.ContainsKey(user.Id))
			{
				Debug.LogError("OnSFSPlayerStatisticUpdate not contain key:" + user.Id);
				return;
			}
			ISFSObject sFSObject = user.GetVariable(TNetUserVarType.userStatistics).GetSFSObject("data");
			SFS_Player_Report[user.Id].kill_cout = sFSObject.GetInt("killCount");
			SFS_Player_Report[user.Id].death_count = sFSObject.GetInt("deathCount");
			SFS_Player_Report[user.Id].loot_cash = sFSObject.GetInt("cashLoot");
			SFS_Player_Report[user.Id].combo_kill = sFSObject.GetInt("vsCombo");
			CheckFirstBlood();
			RefrashMasterKiller();
		}

		private void CheckFirstBlood()
		{
			if (tnetObj.CurRoom.GetVariable(TNetRoomVarType.firstBlood).GetBool("FirstBlood"))
			{
				return;
			}
			string val = string.Empty;
			int num = 0;
			foreach (VSPlayerReport value in SFS_Player_Report.Values)
			{
				num += value.kill_cout;
				if (value.kill_cout == 1)
				{
					val = value.nick_name;
				}
			}
			if (num == 1)
			{
				SFSObject sFSObject = new SFSObject();
				sFSObject.PutBool("FirstBlood", true);
				sFSObject.PutUtfString("NickName", val);
				tnetObj.Send(new SetRoomVariableRequest(TNetRoomVarType.firstBlood, sFSObject));
			}
		}

		private void RefrashMasterKiller()
		{
			List<int> list = new List<int>();
			int num = -1;
			foreach (int key in SFS_Player_Report.Keys)
			{
				if (SFS_Player_Report[key].kill_cout > num)
				{
					num = SFS_Player_Report[key].kill_cout;
				}
			}
			foreach (int key2 in SFS_Player_Report.Keys)
			{
				if (SFS_Player_Report[key2].kill_cout >= num)
				{
					list.Add(key2);
				}
			}
			foreach (TNetUser key3 in SFS_Player_Arr.Keys)
			{
				if (key3.Id != tnetObj.Myself.Id)
				{
					if (list.Contains(key3.Id))
					{
						(SFS_Player_Arr[key3] as Multiplayer).NickNameLabel.GetComponent<TUIMeshTextFx>().color_Accessor = Color.red;
					}
					else
					{
						(SFS_Player_Arr[key3] as Multiplayer).NickNameLabel.GetComponent<TUIMeshTextFx>().color_Accessor = ColorName.GetPlayerMarkColor((int)SFS_Player_Arr[key3].birth_point_index);
					}
				}
			}
		}

		private void OnLockSth(TNetEventData evt)
		{
			if ((int)evt.data["result"] != 0)
			{
				return;
			}
			string text = (string)evt.data["key"];
			if (GetBonusItemFromLockIndex(text) != null)
			{
				BonusManager component = GetBonusItemFromLockIndex(text).GetComponent<BonusManager>();
				if (component != null && component.bonus != null && player.OnPickUp(component.GetCurrentBonusType()))
				{
					player.UpdateAndBroadcastBonusInfo(false, text);
				}
			}
		}

		private void OnMasterChange(TNetEventData evt)
		{
			TNetUser tNetUser = (TNetUser)evt.data["user"];
			if (tNetUser != null)
			{
				Debug.Log("OnMasterChange...");
				if (tNetUser.Id == tnetObj.Myself.Id)
				{
					TimeToNextBonusSpawn = 30f;
				}
			}
		}

		public override void TimeGameOver(float time)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GameSceneMono")) as GameObject;
			gameObject.GetComponent<GameSceneMono>().TimerTask("VSGameOver", time);
		}

		public void OnGameOver(object param, object attach, bool bFinish)
		{
			VSReportUITemp.nextScene = "VSReportTUI";
			SceneName.LoadLevel("VSReportTUITemp");
		}
	}
}
