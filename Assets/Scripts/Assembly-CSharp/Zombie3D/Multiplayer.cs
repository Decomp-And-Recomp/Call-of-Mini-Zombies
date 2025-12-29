using System.Collections.Generic;
using TNetSdk;
using UnityEngine;

namespace Zombie3D
{
	public class Multiplayer : Player
	{
		protected const float lerp_limit = 0.5f;

		public bool multiplayer_inited;

		protected Quaternion rot_to;

		protected Vector3 pos_to;

		public bool m_is_lerp_position;

		protected Vector3 tem_pos_to;

		protected float total_offset_val;

		protected float cur_offset_val;

		public string nick_name = string.Empty;

		protected GameObject RebirthTimerEff;

		protected GameObject Multi_NickName;

		protected float rebirth_time = 10f;

		public bool is_rebirth_msg;

		public GameObject NickNameLabel
		{
			get
			{
				return Multi_NickName;
			}
			set
			{
				Multi_NickName = value;
			}
		}

		public Vector3 GetPosTo()
		{
			return pos_to;
		}

		public Multiplayer()
		{
			multiplayer_inited = false;
			m_is_lerp_position = false;
		}

		public void InitAvatar(AvatarType type, uint birth_index)
		{
			avatarType = type;
			birth_point_index = birth_index;
		}

		public void InitWeaponList(int weapon1, int weapon2, int weapon3)
		{
			weaponList = new List<Weapon>();
			if (weapon1 != 9999)
			{
				weaponList.Add(GameApp.GetInstance().GetGameState().InitMultiWeapon(weapon1));
			}
			if (weapon2 != 9999)
			{
				weaponList.Add(GameApp.GetInstance().GetGameState().InitMultiWeapon(weapon2));
			}
			if (weapon3 != 9999)
			{
				weaponList.Add(GameApp.GetInstance().GetGameState().InitMultiWeapon(weapon3));
			}
		}

		public void InitWeaponList(int weapon1, float AttackFrequency1, int weapon2, float AttackFrequency2, int weapon3, float AttackFrequency3)
		{
			weaponList = new List<Weapon>();
			Weapon weapon4 = null;
			if (weapon1 != 9999)
			{
				weapon4 = GameApp.GetInstance().GetGameState().InitMultiWeapon(weapon1);
				weapon4.AttackFrequency = AttackFrequency1;
				weaponList.Add(weapon4);
			}
			if (weapon2 != 9999)
			{
				weapon4 = GameApp.GetInstance().GetGameState().InitMultiWeapon(weapon2);
				weapon4.AttackFrequency = AttackFrequency2;
				weaponList.Add(weapon4);
			}
			if (weapon3 != 9999)
			{
				weapon4 = GameApp.GetInstance().GetGameState().InitMultiWeapon(weapon3);
				weapon4.AttackFrequency = AttackFrequency3;
				weaponList.Add(weapon4);
			}
		}

		public override void Init()
		{
			if (multiplayer_inited)
			{
				return;
			}
            net_com = GameApp.GetInstance().GetGameState().net_com;
			points = GameObject.FindGameObjectsWithTag("WayPoint");
			GameObject[] array = GameObject.FindGameObjectsWithTag("Respawn");
			GameObject gameObject = array[birth_point_index];
			respawnTrans = gameObject.transform;
			playerObject = AvatarFactory.GetInstance().CreateAvatar(avatarType);
			playerObject.transform.position = gameObject.transform.position;
			playerObject.transform.rotation = gameObject.transform.rotation;
			playerObject.name = "Multiplayer" + m_multi_id;
			playerTransform = playerObject.transform;
			aimedTransform = playerTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head");
			AvatarAttributes avatarByType = GameApp.GetInstance().GetGameState().GetAvatarByType(avatarType);
			maxHp = avatarByType.maxHp;
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs && avatarType >= AvatarType.HumanPixel && SceneName.GetNetMapIndex(Application.loadedLevelName) > 9)
			{
				maxHp = avatarByType.maxHp * 1.1f;
			}
			hp = maxHp;
			moveSpeed = avatarByType.moveSpeed;
			damage = avatarByType.damage;
			isPixel = avatarByType.aConf.isPixel;
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				GameUIScriptNew.GetGameUIScript().AddMultiplayerHpBar(this);
			}
			charController = playerObject.GetComponent<CharacterController>();
			animation = playerObject.GetComponent<Animation>();
			collider = playerObject.GetComponent<Collider>();
			audioPlayer = new AudioPlayer();
			Transform folderTrans = playerTransform.Find("Audio");
			audioPlayer.AddAudio(folderTrans, "Dead", true);
			foreach (Weapon weapon in weaponList)
			{
				weapon.WeaponPlayer = this;
				weapon.Init();
				weapon.VSReset();
			}
			ChangeWeapon(weaponList[0]);
			playerState = null;
			playerBonusState = null;
			if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Vs)
			{
				SetState(PlayerStateType.Idle);
				SetBonusState(PlayerBonusStateType.Normal);
			}
			playerShell = playerObject.AddComponent<PlayerShell>() as PlayerShell;
			playerShell.m_player = this;
			rot_to = playerTransform.rotation;
			pos_to = playerTransform.position;
			multiplayer_inited = true;
			AddNickNameMesh(nick_name);
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				AddRebirthCom();
				GameApp.GetInstance().GetGameScene().GameGUI.AddMultiplayerMiniMapMark(this, (int)birth_point_index);
			}
			else if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				tnetObj = TNetConnection.Connection;
				net_trans = NetworkTransform.FromTransform(playerTransform, moveDirection);
				playerObject.GetComponent<NetworkTransformInterpolation>().enabled = true;
				playerObject.GetComponent<NetworkTransformInterpolation>().StartReceiving();
			}
		}

		public void AddNickNameMesh(string name)
		{
			nick_name = name;
			Multi_NickName = Object.Instantiate(Resources.Load("Prefabs/TUI/MultiNickName")) as GameObject;
			Multi_NickName.transform.parent = playerObject.transform;
			Multi_NickName.transform.localPosition = new Vector3(0f, 0f, 0f);
			Multi_NickName.transform.position = new Vector3(Multi_NickName.transform.position.x, Multi_NickName.transform.position.y + 2f, Multi_NickName.transform.position.z);
			Multi_NickName.GetComponent<TUIMeshTextFx>().text_Accessor = nick_name;
			Multi_NickName.GetComponent<TUIMeshTextFx>().color_Accessor = ColorName.GetPlayerMarkColor((int)birth_point_index);
		}

		public override void OnVSRebirth()
		{
			OnRebirth();
			playerObject.GetComponent<NetworkTransformInterpolation>().StartReceiving();
		}

		public override bool AddVSWeapon(int type)
		{
			Weapon weapon = GameApp.GetInstance().GetGameState().InitMultiWeapon(type);
			weapon.WeaponPlayer = this;
			weapon.Init();
			weapon.VSReset();
			weapon.IsSelectedForBattle = true;
			weaponList.Add(weapon);
			return true;
		}

		public override void DoLogic(float deltaTime)
		{
			if (!multiplayer_inited || weaponList == null)
			{
				return;
			}
			CheckMultiTransform();
			if (playerBonusState != null)
			{
				playerBonusState.DoStateLogic(this, deltaTime);
			}
			if (playerState != null)
			{
				playerState.DoStateLogic(this, deltaTime);
			}
			foreach (Weapon weapon in weaponList)
			{
				if (weapon.IsSelectedForBattle)
				{
					weapon.DoLogic();
				}
			}
			if (is_rebirth_msg)
			{
				rebirth_time -= Time.deltaTime;
				if (rebirth_time <= 0f)
				{
					is_rebirth_msg = false;
					PlayerRealDead();
					rebirth_time = 0f;
				}
			}
		}

		public override void PlayerRealDead()
		{
			is_real_dead = true;
			(GameApp.GetInstance().GetGameScene() as GameMultiplayerScene).CheckMultiGameOver();
		}

		public void OnMultiSniperFire()
		{
			if (cur_weapon.GetWeaponType() == WeaponType.Sniper)
			{
				cur_weapon.Fire(0f);
			}
		}

		public override void SetState(PlayerStateType type)
		{
			if (playerState != null)
			{
				playerState.ExitState(this);
			}
			switch (type)
			{
			case PlayerStateType.Dead:
				playerState = new MultiplayerDeadState();
				break;
			case PlayerStateType.GotHit:
				playerState = new MultiplayerGotHitState();
				break;
			case PlayerStateType.Idle:
				playerState = new MultiplayerIdleState();
				break;
			case PlayerStateType.Run:
				playerState = new MultiplayerRunState();
				break;
			case PlayerStateType.RunShoot:
				playerState = new MultiplayerRunAndShootState();
				break;
			case PlayerStateType.Shoot:
				playerState = new MultiplayerShootState();
				break;
			case PlayerStateType.GotRushHit:
				playerState = new MultiplayerGotRushForceState();
				break;
			}
			playerState.EnterState(this);
		}

		public void SetBonusStateWithType(PlayerBonusStateType type)
		{
			if (playerBonusState != null)
			{
				playerBonusState.ExitState(this);
			}
			switch (type)
			{
			case PlayerBonusStateType.Normal:
				playerBonusState = new MultiPlayerBonusStateNormal();
				break;
			case PlayerBonusStateType.GodBuff:
				playerBonusState = new MultiPlayerBonusStateGodBuff();
				break;
			case PlayerBonusStateType.PowerUp:
				playerBonusState = new MultiPlayerBonusStatePower();
				break;
			case PlayerBonusStateType.Stealth:
				playerBonusState = new MultiPlayerBonusStateStealth();
				break;
			case PlayerBonusStateType.Super:
				playerBonusState = new MultiPlayerBonusStateSuper();
				break;
			case PlayerBonusStateType.Suicidegun:
				playerBonusState = new MultiPlayerBonusStateSuicidegun();
				break;
			case PlayerBonusStateType.Shield:
				playerBonusState = new MultiPlayerBonusStateShield();
				break;
			}
			playerBonusState.EnterState(this);
		}

        public void CheckMultiTransform()
		{
			if (m_is_lerp_position)
			{
				Vector3 motion = Vector3.MoveTowards(Vector3.zero, pos_to - playerTransform.position, Time.deltaTime * base.MoveSpeed * 50f);
				charController.Move(motion);
				m_is_lerp_position = false;
			}
            playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, rot_to, Time.deltaTime * 10f);
		}

		public void UpdateMultiTransform(Vector3 direction, Vector3 rotation, Vector3 position, float ping)
		{
			moveDirection = direction;
			if (!(playerTransform == null))
			{
				if (Vector3.Distance(playerTransform.position, position) >= base.MoveSpeed * 0.2f * 2f)
				{
					m_is_lerp_position = true;
					total_offset_val = Vector3.Distance(playerTransform.position, pos_to);
					cur_offset_val = 0f;
				}
				else
				{
					m_is_lerp_position = false;
				}
				pos_to = position;
				if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
				{
					net_ping_sum = ping + net_com.m_netUserInfo.net_ping;
				}
				else if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
				{
					net_ping_sum = ping;
				}
			rot_to = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
			}
		}

		public override void OnHit(float damage)
		{
		}

		public void OnMultiInjury(float damage, float max_hp, float cur_hp)
		{
			if (!(hp <= 0f))
			{
				maxHp = max_hp;
				hp = (int)cur_hp;
				hp = Mathf.Clamp(hp, 0f, maxHp);
			}
		}

		public override void OnDead()
		{
			audioPlayer.PlayAudio("Dead");
			cur_weapon.StopFire();
			PlayDeadEffect();
			if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Vs)
			{
				playerObject.GetComponent<Collider>().enabled = false;
				rebirth_triger.enabled = true;
				playerObject.layer = 27;
				playerObject.GetComponent<PlayerRebirth>().CancelRebirth();
				(GameApp.GetInstance().GetGameScene() as GameMultiplayerScene).OnMultiPlayerDead(this);
				is_rebirth_msg = true;
				rebirth_time = 10f;
			}
			else
			{
				playerObject.GetComponent<NetworkTransformInterpolation>().StopReceiving();
			}
		}

		public void ChangeWeaponWithindex(int index)
		{
			ChangeWeapon(weaponList[index]);
		}

		public override void ChangeWeapon(Weapon w)
		{
			if (w.IsSelectedForBattle)
			{
				if (cur_weapon != null)
				{
					cur_weapon.GunOff();
				}
				cur_weapon = w;
				cur_weapon.GunOn();
				weaponNameEnd = Weapon.GetWeaponNameEnd(cur_weapon.GetWeaponType(), cur_weapon.Name);
			}
		}

		public override void OnRebirth()
		{
			Debug.Log("Multipalyer OnRebirth...");
			playerObject.GetComponent<Collider>().enabled = true;
			if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Vs)
			{
				rebirth_triger.enabled = false;
			}
			playerObject.layer = 8;
			GetHealed((int)maxHp);
			PlayRebirthEffect();
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				GameMultiplayerScene gameMultiplayerScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
				gameMultiplayerScene.m_multi_player_arr.Add(this);
				gameMultiplayerScene.ResetEnemyTarget();
				is_real_dead = false;
				is_rebirth_msg = false;
			}
			if (RebirthTimerEff != null)
			{
				Object.Destroy(RebirthTimerEff);
				RebirthTimerEff = null;
			}
		}

		public override void OnRebirthStart()
		{
			Debug.Log("Multiplayer OnRebirthStart id:" + m_multi_id);
			RebirthTimerEff = Object.Instantiate(Resources.Load("Prefabs/TUI/RebirthTimerEff")) as GameObject;
			RebirthTimerEff.transform.parent = playerObject.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Weapon_Dummy");
			RebirthTimerEff.transform.localPosition = new Vector3(0f, 0f, 0f);
			RebirthTimerEff.transform.position = new Vector3(RebirthTimerEff.transform.position.x, RebirthTimerEff.transform.position.y + 2f, RebirthTimerEff.transform.position.z);
			RebirthTimerEff.GetComponent<ClipMeshEffScript>().StartClip();
		}

		public override void OnRebirthStay(float time)
		{
			if (RebirthTimerEff != null)
			{
				RebirthTimerEff.GetComponent<ClipMeshEffScript>().UpdateMesh(time);
			}
		}

		public override void OnRebirthExit()
		{
			if (RebirthTimerEff != null)
			{
				Object.Destroy(RebirthTimerEff);
				RebirthTimerEff = null;
			}
		}

		public override void OnRebirthFinish()
		{
			if (RebirthTimerEff != null)
			{
				Object.Destroy(RebirthTimerEff);
				RebirthTimerEff = null;
			}
		}

		public override void UpdateNetworkTrans()
		{
			playerObject.GetComponent<NetworkTransformInterpolation>().ReceivedTransform(NetworkTransform.Clone(net_trans));
		}

		public override void OnMultiInjured(float damage)
		{
			Packet packet = CGMultiplayerInjuryPacket.MakePacket((uint)m_multi_id, (long)(damage * 1000f));
			GameApp.GetInstance().GetGameState().net_com.Send(packet);
		}

		public override void OnVsInjured(TNetUser sender, float damage, int weapon_type)
		{
			if (weapon_type == 10)
			{
				GameVSScene gameVSScene = GameApp.GetInstance().GetGameScene() as GameVSScene;
				Vector3 vector = gameVSScene.SFS_Player_Arr[sender].GetTransform().position - GetTransform().position;
				vector.Normalize();
				Object.Instantiate(GameApp.GetInstance().GetResourceConfig().swordAttack, GetTransform().position + Vector3.up * 1.2f + vector * 0.5f, Quaternion.identity);
			}
			Object.Instantiate(GameApp.GetInstance().GetResourceConfig().hitBlood, GetTransform().position + Vector3.up * 1f, Quaternion.identity);
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutFloat("damageVal", damage);
			sFSObject.PutInt("weaponType", weapon_type);
			SFSObject sFSObject2 = new SFSObject();
			sFSObject2.PutSFSObject("damage", sFSObject);
			tnetObj.Send(new ObjectMessageRequest(tnet_user.Id, sFSObject2));
		}

		public void MultiplayerSniperFire(Vector3 target)
		{
			MultiSniper multiSniper = GetWeapon() as MultiSniper;
			if (multiSniper != null)
			{
				multiSniper.AddMultiTarget(target);
				OnMultiSniperFire();
			}
		}
	}
}
