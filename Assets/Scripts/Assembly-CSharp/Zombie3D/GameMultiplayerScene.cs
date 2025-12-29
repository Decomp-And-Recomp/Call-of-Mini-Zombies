using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zombie3D
{
	public class GameMultiplayerScene : GameScene
	{
		protected int winnerId;

		protected NetworkObj net_com;

		protected CallbackFunc game_over_call_back;

		protected float game_over_check_time;

		public bool is_game_over;

		public override bool GetGameExcute()
		{
			return is_game_excute;
		}

		public override void Init(int index)
		{
			net_com = GameApp.GetInstance().GetGameState().net_com;
			net_com.packet_delegate = OnPacket;
			net_com.someone_birth_delegate = OnSomeoneBirth;
			net_com.leave_room_notity_delegate = OnSomeoneLeave;
			net_com.leave_room_delegate = OnLeaveRoom;
			game_over_call_back = OnGameOver;
			is_game_excute = true;
			m_multi_player_arr = new List<Player>();
			m_player_set = new List<Player>();
			base.Init(index);
			m_multi_player_arr.Add(player);
			m_player_set.Add(player);
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null)
				{
					net_com.netUserInfo_array[i].multiplayer.nick_name = net_com.netUserInfo_array[i].nick_name;
					net_com.netUserInfo_array[i].multiplayer.Init();
					m_multi_player_arr.Add(net_com.netUserInfo_array[i].multiplayer);
					m_player_set.Add(net_com.netUserInfo_array[i].multiplayer);
				}
			}
		}

		public override void DoLogic(float deltaTime)
		{
			if (!is_game_excute)
			{
				return;
			}
			player.DoLogic(deltaTime);
			if (!is_game_over)
			{
				object[] array = new object[enemyList.Count];
				enemyList.Keys.CopyTo(array, 0);
				for (int i = 0; i < array.Length; i++)
				{
					Enemy enemy = enemyList[array[i]] as Enemy;
					enemy.DoLogic(deltaTime);
				}
			}
			for (int j = 0; j < 4; j++)
			{
				if (net_com.netUserInfo_array[j] != null && net_com.netUserInfo_array[j].multiplayer != null)
				{
					net_com.netUserInfo_array[j].multiplayer.DoLogic(deltaTime);
				}
			}
			game_over_check_time += deltaTime;
			if (game_over_check_time >= 5f)
			{
				game_over_check_time = 0f;
				CheckMultiGameOver();
			}
		}

		public void ResetEnemyTarget()
		{
			if (m_multi_player_arr.Count > 1)
			{
				return;
			}
			foreach (DictionaryEntry enemy in enemyList)
			{
				((Enemy)enemy.Value).SetTargetWithMultiplayer();
			}
		}

		public void OnPacket(Packet packet)
		{
			if (!is_game_excute)
			{
				return;
			}
			uint val = 0u;
			if (packet.WatchUInt32(ref val, 4))
			{
				switch ((PROTOCOLS)val)
				{
				case PROTOCOLS.GC_USERACTION_NOTIFY:
					OnUserActionNotify(packet);
					break;
				case PROTOCOLS.GC_USER_BONUS_ACTION_NOTIFY:
					OnUserBonusActionNotify(packet);
					break;
				case PROTOCOLS.GC_USERSTATUS_NOTIFY:
					OnUserStatusNotify(packet);
					break;
				case PROTOCOLS.GC_USERCHANGEWEAPON_NOTIFY:
					OnUserChangeWeaponNotify(packet);
					break;
				case PROTOCOLS.GC_USER_SNIPER_FIRE_NOTIFY:
					OnUserSniperFireNotify(packet);
					break;
				case PROTOCOLS.GC_ENEMY_BIRTH_NOTIFY:
					OnEnemyBirthNotify(packet);
					break;
				case PROTOCOLS.GC_ENEMYSTATUS_NOTIFY:
					OnEnemyStatusNotify(packet);
					break;
				case PROTOCOLS.GC_ENEMY_GOTHIT_NOTIFY:
					OnEnemyGotHitNotify(packet);
					break;
				case PROTOCOLS.GC_ENEMY_DEAD_NOTIFY:
					OnEnemyDeadNotify(packet);
					break;
				case PROTOCOLS.GC_ENEMY_REMOVE_NOTIFY:
					OnEnemyRemoveNotify(packet);
					break;
				case PROTOCOLS.GC_ENEMY_LOOT_NEW_NOTIFY:
					OnEnemyLootNotify(packet);
					break;
				case PROTOCOLS.GC_ITEM_PICK_NOTIFY:
					OnPickItemNotify(packet);
					break;
				case PROTOCOLS.GC_USER_INJURY_NOTIFY:
					OnUserInjuryed(packet);
					break;
				case PROTOCOLS.GC_ENEMY_CHANGE_TARGET_NOTIFY:
					OnEnemyChangeTarget(packet);
					break;
				case PROTOCOLS.GC_USER_REBIRTH_NOTIFY:
					OnUserRebirth(packet);
					break;
				case PROTOCOLS.GC_MASTER_CHANGE:
					OnMasterChange(packet);
					break;
				case PROTOCOLS.GC_USER_DO_REBIRTH:
					OnUserDoRebirth(packet);
					break;
				case PROTOCOLS.GC_USER_DO_REBIRTH_NOTIFY:
					OnUserDoRebirthNotity(packet);
					break;
				case PROTOCOLS.GC_GAME_OVER_NOTIFY:
					OnUserRealDead(packet);
					break;
				case PROTOCOLS.GC_COOP_WINNER_NOTIFY:
					OnGameWinNotify(packet);
					break;
				}
			}
		}

		public void OnSomeoneBirth(Player player)
		{
			player.Init();
			m_multi_player_arr.Add(player);
			m_player_set.Add(player);
		}

		public void OnUserStatusNotify(Packet packet)
		{
			GCUserStatusNotifyPacket gCUserStatusNotifyPacket = new GCUserStatusNotifyPacket();
			if (!gCUserStatusNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnUserStatusNotify ParserPacket Error!!!");
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null && net_com.netUserInfo_array[i].user_id == gCUserStatusNotifyPacket.m_iUserId)
				{
					float ping = (float)gCUserStatusNotifyPacket.m_iPingTime / 1000f;
					net_com.netUserInfo_array[i].multiplayer.UpdateMultiTransform(gCUserStatusNotifyPacket.m_direct, gCUserStatusNotifyPacket.m_Rotation, gCUserStatusNotifyPacket.m_Position, ping);
					break;
				}
			}
		}

		public void OnUserChangeWeaponNotify(Packet packet)
		{
			GCUserChangeWeaponNotifyPacket gCUserChangeWeaponNotifyPacket = new GCUserChangeWeaponNotifyPacket();
			if (!gCUserChangeWeaponNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnUserChangeWeaponNotify ParserPacket Error!!!");
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null && net_com.netUserInfo_array[i].user_id == gCUserChangeWeaponNotifyPacket.m_iUserId)
				{
					net_com.netUserInfo_array[i].multiplayer.ChangeWeaponWithindex((int)gCUserChangeWeaponNotifyPacket.m_iWeaponIndex);
					break;
				}
			}
		}

		public void OnUserSniperFireNotify(Packet packet)
		{
			GCUserSniperFireNotifyPacket gCUserSniperFireNotifyPacket = new GCUserSniperFireNotifyPacket();
			if (!gCUserSniperFireNotifyPacket.ParserPacket(packet))
			{
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null && net_com.netUserInfo_array[i].user_id == gCUserSniperFireNotifyPacket.m_iUserId)
				{
					if (net_com.netUserInfo_array[i].multiplayer.GetWeapon().GetWeaponType() == WeaponType.Sniper)
					{
						MultiSniper multiSniper = net_com.netUserInfo_array[i].multiplayer.GetWeapon() as MultiSniper;
						multiSniper.AddMultiTarget(gCUserSniperFireNotifyPacket.m_Position);
						net_com.netUserInfo_array[i].multiplayer.OnMultiSniperFire();
					}
					break;
				}
			}
		}

		public void OnUserActionNotify(Packet packet)
		{
			GCUserActionNotifyPacket gCUserActionNotifyPacket = new GCUserActionNotifyPacket();
			if (!gCUserActionNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnUserActionNotify ParserPacket Error!!!");
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null && net_com.netUserInfo_array[i].user_id == gCUserActionNotifyPacket.m_iUserId)
				{
					net_com.netUserInfo_array[i].multiplayer.SetState((PlayerStateType)gCUserActionNotifyPacket.m_iAction);
					break;
				}
			}
		}

		public void OnUserBonusActionNotify(Packet packet)
		{
			GCUserAuxiliaryActionNotifyPacket gCUserAuxiliaryActionNotifyPacket = new GCUserAuxiliaryActionNotifyPacket();
			if (!gCUserAuxiliaryActionNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnUserAuxiliaryActionNotify ParserPacekt Error!");
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null && net_com.netUserInfo_array[i].user_id == gCUserAuxiliaryActionNotifyPacket.m_iUserId)
				{
					net_com.netUserInfo_array[i].multiplayer.SetBonusStateWithType((PlayerBonusStateType)gCUserAuxiliaryActionNotifyPacket.m_iBonusAction);
					break;
				}
			}
		}

		public void OnEnemyBirthNotify(Packet packet)
		{
			GCEnemyBirthNotifyPacket gCEnemyBirthNotifyPacket = new GCEnemyBirthNotifyPacket();
			if (!gCEnemyBirthNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnEnemyBirthNotify ParserPacket Error!!!");
				return;
			}
			bool isElite = gCEnemyBirthNotifyPacket.m_isElite == 1;
			bool isSuperBoss = gCEnemyBirthNotifyPacket.m_isSuperBoss == 1;
			bool isGrave = gCEnemyBirthNotifyPacket.m_isGrave == 1;
			ArenaTriggerBossScript.SpawnMultiEnemy((EnemyType)gCEnemyBirthNotifyPacket.m_enemy_type, (int)gCEnemyBirthNotifyPacket.m_enemy_Id, isElite, isSuperBoss, gCEnemyBirthNotifyPacket.m_Position, isGrave, (int)gCEnemyBirthNotifyPacket.m_target_id);
		}

		public void OnEnemyStatusNotify(Packet packet)
		{
			GCEnemyStatusNotifyPacket gCEnemyStatusNotifyPacket = new GCEnemyStatusNotifyPacket();
			if (!gCEnemyStatusNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnEnemyStatusNotify ParserPacket Error!!!");
				return;
			}
			Enemy enemyByID = GetEnemyByID(gCEnemyStatusNotifyPacket.m_enemyID);
			if (enemyByID != null)
			{
				enemyByID.SetNetEnemyStatus(gCEnemyStatusNotifyPacket.m_Direction, gCEnemyStatusNotifyPacket.m_Rotation, gCEnemyStatusNotifyPacket.m_Position);
			}
		}

		public void OnEnemyGotHitNotify(Packet packet)
		{
			GCEnemyGotHitNotifyPacket gCEnemyGotHitNotifyPacket = new GCEnemyGotHitNotifyPacket();
			if (!gCEnemyGotHitNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnEnemyGotHitNotify ParserPacket Error!!!");
				return;
			}
			Enemy enemyByID = GetEnemyByID(gCEnemyGotHitNotifyPacket.m_enemyID);
			if (enemyByID != null)
			{
				enemyByID.OnMultiHit((double)gCEnemyGotHitNotifyPacket.m_iDamage / 1000.0, (WeaponType)gCEnemyGotHitNotifyPacket.m_weapon_type, (int)gCEnemyGotHitNotifyPacket.m_critical_attack);
			}
		}

		public void OnEnemyDeadNotify(Packet packet)
		{
			GCEnemyDeadNotifyPacket gCEnemyDeadNotifyPacket = new GCEnemyDeadNotifyPacket();
			if (!gCEnemyDeadNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnEnemyDeadNotify ParserPacket Error!!!");
				return;
			}
			Enemy enemyByID = GetEnemyByID(gCEnemyDeadNotifyPacket.enemy_id);
			if (enemyByID != null && enemyByID.GetState() != Enemy.DEAD_STATE)
			{
				enemyByID.OnDead();
				enemyByID.SetState(Enemy.DEAD_STATE);
			}
		}

		public void OnEnemyRemoveNotify(Packet packet)
		{
			GCEnemyRemoveNotifyPacket gCEnemyRemoveNotifyPacket = new GCEnemyRemoveNotifyPacket();
			if (!gCEnemyRemoveNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnEnemyRemoveNotify ParserPacket Error!!!");
				return;
			}
			Enemy enemyByID = GetEnemyByID(gCEnemyRemoveNotifyPacket.m_enemyID);
			if (enemyByID != null && enemyByID.GetState() != Enemy.DEAD_STATE)
			{
				enemyByID.OnDead();
				enemyByID.SetState(Enemy.DEAD_STATE);
			}
		}

		public void OnGameWinNotify(Packet packet)
		{
			GCCoopWinnerNotifyPacket gCCoopWinnerNotifyPacket = new GCCoopWinnerNotifyPacket();
			if (!gCCoopWinnerNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnGameWinNotify ParserPacket Error!!!");
				return;
			}
			winnerId = (int)gCCoopWinnerNotifyPacket.iWinnerId;
			if (winnerId != player.m_multi_id)
			{
				GameApp.GetInstance().GetGameState().AddCash(SceneName.GetRewardFromMap(GameApp.GetInstance().GetGameState().cur_net_map));
			}
			foreach (DictionaryEntry enemy2 in enemyList)
			{
				Enemy enemy = (Enemy)enemy2.Value;
				if (!enemy.IsSuperBoss)
				{
					DamageProperty damageProperty = new DamageProperty();
					damageProperty.damage = (float)(enemy.Attributes.Hp + 10.0);
					enemy.OnHit(damageProperty, WeaponType.NoGun, false, null);
				}
			}
			foreach (Player item in m_multi_player_arr)
			{
				if (item.m_multi_id == gCCoopWinnerNotifyPacket.iWinnerId)
				{
					player.PlayerObject.GetComponent<PlayerShell>().OnAvatarShowCameraChange(false, item);
				}
			}
			is_game_over = true;
			GameGUI.HideRebirthMsgBox();
			SaveDataReport();
			QuitGameForDisconnect(8f);
			VSReportUITemp.nextScene = "MultiReportTUI";
		}

		public void OnEnemyLootNotify(Packet packet)
		{
			GCEnemyLootNewNotifyPacket gCEnemyLootNewNotifyPacket = new GCEnemyLootNewNotifyPacket();
			if (!gCEnemyLootNewNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnEnemyLootNotify ParserPacket Error!!!");
				return;
			}
			Vector2 a = new Vector2(gCEnemyLootNewNotifyPacket.m_Position.x, gCEnemyLootNewNotifyPacket.m_Position.z);
			GameObject[] array = woodboxList;
			foreach (GameObject gameObject in array)
			{
				if (!(gameObject != null))
				{
					continue;
				}
				Vector2 b = new Vector2(gameObject.transform.position.x, gameObject.transform.position.z);
				if (Vector2.Distance(a, b) < 1f)
				{
					GameObject gameObject2 = Object.Instantiate(GameApp.GetInstance().GetResourceConfig().woodExplode, gameObject.transform.position, Quaternion.identity) as GameObject;
					AudioSource component = gameObject2.GetComponent<AudioSource>();
					if (component != null)
					{
						component.mute = !GameApp.GetInstance().GetGameState().SoundOn;
					}
					Object.Destroy(gameObject);
					break;
				}
			}
			LootManagerScript.LootSpawnItem((ItemType)gCEnemyLootNewNotifyPacket.item_type, gCEnemyLootNewNotifyPacket.m_Position, (int)gCEnemyLootNewNotifyPacket.id);
		}

		public void OnPickItemNotify(Packet packet)
		{
			GCPickItemNotifyPacket gCPickItemNotifyPacket = new GCPickItemNotifyPacket();
			if (!gCPickItemNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnPickItemNotify ParserPacket Error!!!");
				return;
			}
			foreach (GameObject item in itemList)
			{
				if (item != null && item.GetComponent<ItemScript>().GameItemID == (int)gCPickItemNotifyPacket.id)
				{
					itemList.Remove(item);
					Object.Destroy(item);
					break;
				}
			}
		}

		public void OnUserInjuryed(Packet packet)
		{
			GCUserInjuryNotifyPacket gCUserInjuryNotifyPacket = new GCUserInjuryNotifyPacket();
			if (!gCUserInjuryNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnUserInjuryed ParserPacket Error!!!");
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null && net_com.netUserInfo_array[i].user_id == gCUserInjuryNotifyPacket.m_iUserId)
				{
					net_com.netUserInfo_array[i].multiplayer.OnMultiInjury((float)gCUserInjuryNotifyPacket.m_iInjury_val / 1000f, (float)gCUserInjuryNotifyPacket.m_total_hp_val / 1000f, (float)gCUserInjuryNotifyPacket.m_cur_hp_val / 1000f);
					break;
				}
			}
		}

		public void OnMultiplayerInjury(Packet packet)
		{
			GCMultiplayerInjuryNotifyPacket gCMultiplayerInjuryNotifyPacket = new GCMultiplayerInjuryNotifyPacket();
			if (!gCMultiplayerInjuryNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnMultiplayerInjury ParserPacket Error!!!");
			}
			else if (gCMultiplayerInjuryNotifyPacket.m_playerId == player.m_multi_id)
			{
				player.OnHit((float)gCMultiplayerInjuryNotifyPacket.m_damage / 1000f);
			}
		}

		public void OnEnemyChangeTarget(Packet packet)
		{
			GCEnemyChangeTargetNotifyPacket gCEnemyChangeTargetNotifyPacket = new GCEnemyChangeTargetNotifyPacket();
			if (!gCEnemyChangeTargetNotifyPacket.ParserPacket(packet))
			{
				Debug.Log("OnEnemyChangeTarget ParserPacket Error!!!");
				return;
			}
			Enemy enemyByID = GetEnemyByID(gCEnemyChangeTargetNotifyPacket.m_enemyID);
			if (enemyByID == null)
			{
				return;
			}
			foreach (Player item in m_multi_player_arr)
			{
				if (item.m_multi_id == gCEnemyChangeTargetNotifyPacket.target_id)
				{
					enemyByID.TargetPlayer = item;
					break;
				}
			}
		}

		public void OnUserRebirth(Packet packet)
		{
			GCUserRebirthNotifyPacket gCUserRebirthNotifyPacket = new GCUserRebirthNotifyPacket();
			if (!gCUserRebirthNotifyPacket.ParserPacket(packet))
			{
				return;
			}
			Debug.Log("GCUserRebirthNotifyPacket ***" + gCUserRebirthNotifyPacket.m_iUserId);
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null && net_com.netUserInfo_array[i].user_id == gCUserRebirthNotifyPacket.m_iUserId)
				{
					net_com.netUserInfo_array[i].multiplayer.OnRebirth();
					GameObject gameObject = Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().itemFullReviveEffect, Vector3.zero, Quaternion.identity) as GameObject;
					gameObject.transform.parent = net_com.netUserInfo_array[i].multiplayer.GetTransform();
					gameObject.transform.localPosition = Vector3.zero;
					break;
				}
			}
		}

		public void OnMasterChange(Packet packet)
		{
			GCMasterChangePacket gCMasterChangePacket = new GCMasterChangePacket();
			if (!gCMasterChangePacket.ParserPacket(packet))
			{
				Debug.Log("OnMasterChange ParserPacket Error!!!");
				return;
			}
			net_com.m_netUserInfo.is_master = true;
			Enemy enemy = null;
			foreach (DictionaryEntry enemy2 in enemyList)
			{
				enemy = (Enemy)enemy2.Value;
				enemy.SetTargetWithMultiplayer();
			}
			base.EnemyID += 50;
			ArenaTrigger_Boss.CheckSpawnBoss();
			Debug.Log("OnMasterChange!!!");
		}

		public void OnUserDoRebirth(Packet packet)
		{
			GCUserDoRebirthPacket gCUserDoRebirthPacket = new GCUserDoRebirthPacket();
			if (gCUserDoRebirthPacket.ParserPacket(packet))
			{
				Debug.Log("OnUserDoRebirth..." + gCUserDoRebirthPacket.rebirth_user_id);
				if (gCUserDoRebirthPacket.m_iResult == 0)
				{
					GameGUI.playerInfo.SetMedpackCount();
				}
				else
				{
					GameApp.GetInstance().GetGameState().Medpack++;
				}
			}
		}

		public void OnUserDoRebirthNotity(Packet packet)
		{
			Debug.Log("OnUserDoRebirthNotity...");
			GCUserDoRebirthNotifyPacket gCUserDoRebirthNotifyPacket = new GCUserDoRebirthNotifyPacket();
			if (!gCUserDoRebirthNotifyPacket.ParserPacket(packet))
			{
				return;
			}
			if (gCUserDoRebirthNotifyPacket.rebirth_user_id == player.m_multi_id)
			{
				player.OnRebirth();
				GameObject gameObject = Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().itemMedpackEffect, Vector3.zero, Quaternion.identity) as GameObject;
				gameObject.transform.parent = player.GetTransform();
				gameObject.transform.localPosition = Vector3.zero;
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].multiplayer != null && net_com.netUserInfo_array[i].user_id == gCUserDoRebirthNotifyPacket.rebirth_user_id)
				{
					net_com.netUserInfo_array[i].multiplayer.OnRebirth();
					GameObject gameObject2 = Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().itemMedpackEffect, Vector3.zero, Quaternion.identity) as GameObject;
					gameObject2.transform.parent = net_com.netUserInfo_array[i].multiplayer.GetTransform();
					gameObject2.transform.localPosition = Vector3.zero;
					break;
				}
			}
		}

		public void OnUserRealDead(Packet packet)
		{
			GCGameOverNotifyPacket gCGameOverNotifyPacket = new GCGameOverNotifyPacket();
			if (!gCGameOverNotifyPacket.ParserPacket(packet))
			{
				return;
			}
			Debug.Log("OnUserRealDead...");
			foreach (Player item in m_player_set)
			{
				if (item.m_multi_id == gCGameOverNotifyPacket.m_iUserId)
				{
					item.PlayerRealDead();
					break;
				}
			}
		}

		public void OnSomeoneLeave(int user_id)
		{
			if (!is_game_excute)
			{
				return;
			}
			Debug.Log("Mutiplayer leave room : " + user_id);
			GameGUI.RemoveMultiplayerMiniMapMark(user_id);
			Player player = null;
			foreach (Player item in m_multi_player_arr)
			{
				if (item.m_multi_id == user_id)
				{
					player = item;
					break;
				}
			}
			if (player != null)
			{
				m_multi_player_arr.Remove(player);
				m_player_set.Remove(player);
			}
			else
			{
				GameObject gameObject = GameObject.Find("Multiplayer" + user_id);
				if (gameObject != null)
				{
					player = gameObject.GetComponent<PlayerShell>().m_player;
				}
			}
			if (player != null)
			{
				Enemy enemy = null;
				foreach (DictionaryEntry enemy2 in enemyList)
				{
					enemy = (Enemy)enemy2.Value;
					if (enemy.TargetPlayer != null && enemy.TargetPlayer.m_multi_id == user_id)
					{
						enemy.SetTargetWithMultiplayer();
						enemy.SetState(Enemy.IDLE_STATE);
					}
				}
				player.is_real_dead = true;
				Object.Destroy(player.PlayerObject);
				player = null;
			}
			OnMultiPlayerDead(null);
			CheckMultiGameOver();
		}

		public void OnLeaveRoom()
		{
			Debug.Log("Scene OnLeaveRoom...");
		}

		public void OnMultiPlayerDead(Player mPlayer)
		{
			m_multi_player_arr.Remove(mPlayer);
		}

		public void OnGameOver(object param, object attach, bool bFinish)
		{
			SceneName.LoadLevel("VSReportTUITemp");
		}

		public void OnGameWin()
		{
			winnerId = player.m_multi_id;
			GameApp.GetInstance().GetGameState().AddCash(2 * SceneName.GetRewardFromMap(GameApp.GetInstance().GetGameState().cur_net_map));
			Packet packet = CGCoopWinnerPacket.MakePacket((uint)player.m_multi_id);
			net_com.Send(packet);
			foreach (DictionaryEntry enemy2 in enemyList)
			{
				Enemy enemy = (Enemy)enemy2.Value;
				if (!enemy.IsSuperBoss)
				{
					DamageProperty damageProperty = new DamageProperty();
					damageProperty.damage = (float)(enemy.Attributes.Hp + 10.0);
					enemy.OnHit(damageProperty, WeaponType.NoGun, false, null);
				}
			}
			player.PlayerObject.GetComponent<PlayerShell>().OnAvatarShowCameraChange(true, player);
			is_game_over = true;
			SaveDataReport();
			QuitGameForDisconnect(8f);
			VSReportUITemp.nextScene = "MultiReportTUI";
		}

		public override void SaveDataReport()
		{
			Debug.Log("Scene OnGameOver...");
			GameApp.GetInstance().GetGameState().Achievement.LoseGame();
			GameObject gameObject = new GameObject("MultiReportData");
			Object.DontDestroyOnLoad(gameObject);
			MultiReportData multiReportData = gameObject.AddComponent<MultiReportData>();
			multiReportData.play_time = Time.time - GameStartTime;
			multiReportData.isMVP = winnerId == player.m_multi_id;
			multiReportData.map = GameApp.GetInstance().GetGameState().cur_net_map;
			multiReportData.avatar = player.AvatarType;
			multiReportData.weapons = new List<string>();
			foreach (Weapon weapon in player.weaponList)
			{
				multiReportData.weapons.Add(weapon.Name);
			}
			int rewardFromMap = SceneName.GetRewardFromMap(GameApp.GetInstance().GetGameState().cur_net_map);
			for (int i = 0; i < 4; i++)
			{
				if (net_com.netUserInfo_array[i] == null)
				{
					continue;
				}
				if (winnerId == net_com.netUserInfo_array[i].user_id)
				{
					multiReportData.userReport.Add(i + net_com.netUserInfo_array[i].nick_name, rewardFromMap * 2);
				}
				else if (net_com.netUserInfo_array[i].multiplayer != null)
				{
					if (net_com.netUserInfo_array[i].multiplayer.PlayerObject.layer != 8)
					{
						multiReportData.userReport.Add(i + net_com.netUserInfo_array[i].nick_name, rewardFromMap / 2);
					}
					else
					{
						multiReportData.userReport.Add(i + net_com.netUserInfo_array[i].nick_name, rewardFromMap);
					}
				}
				else if (player.PlayerObject.layer != 8)
				{
					multiReportData.userReport.Add(i + net_com.netUserInfo_array[i].nick_name, rewardFromMap / 2);
				}
				else
				{
					multiReportData.userReport.Add(i + net_com.netUserInfo_array[i].nick_name, rewardFromMap);
				}
			}
			GameGUI.gameOverPanel.GetComponent<GameOverTUI>().totalDeaths = player.m_death_count;
		}

		public void CheckMultiGameOver()
		{
			if (is_game_over)
			{
				return;
			}
			int num = 0;
			foreach (Player item in m_player_set)
			{
				if (item.is_real_dead && item.GetPlayerState() != null && item.GetPlayerState().GetStateType() == PlayerStateType.Dead)
				{
					num++;
				}
			}
			if ((num == m_player_set.Count) ? true : false)
			{
				is_game_over = true;
				QuitGameForDisconnect(5f);
				GameGUI.ShowGameOverPanel(GameOverType.coopLose);
			}
		}

		public override void TimeGameOver(float time)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/GameSceneMono")) as GameObject;
			gameObject.GetComponent<GameSceneMono>().TimerTask("GameOver", time);
		}

		public override void QuitGameForDisconnect(float time)
		{
			is_game_excute = false;
			if (net_com != null)
			{
				Packet packet = CGLeaveRoomPacket.MakePacket();
				GameApp.GetInstance().GetGameState().net_com.Send(packet);
				net_com.UnregisterCallbacks();
			}
			NetworkObj.DestroyNetCom();
			TimeGameOver(time);
			VSReportUITemp.nextScene = "CoopHallTUI";
		}
	}
}
