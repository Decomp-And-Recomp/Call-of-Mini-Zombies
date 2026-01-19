using System.Collections.Generic;
using TNetSdk;
using UnityEngine;

namespace Zombie3D
{
	public class Player
	{
		public const float net_status_rate = 0.2f;

		protected GameObject playerObject;

		protected BaseCameraScript gameCamera;

		protected Transform playerTransform;

		protected CharacterController charController;

		protected Animation animation;

		protected Collider collider;

		protected GameObject powerObj;

		protected Transform respawnTrans;

		protected AudioPlayer audioPlayer;

		protected AvatarType avatarType;

		protected float hp;

		protected float maxHp;

		protected float damage;

		protected float moveSpeed;

		protected bool isPixel;

		protected Vector3 moveDirection = Vector3.zero;

		protected PlayerState playerState;

		protected PlayerBonusState playerBonusState;

		protected InputController inputController;

		protected Vector3 getHitFlySpeed;

		protected Weapon cur_weapon;

		protected int currentWeaponIndex;

		protected string weaponNameEnd;

		public List<Weapon> weaponList;

		public Dictionary<ItemType, int> carryItemsPacket;

		public Dictionary<ItemType, int> pickupItemsPacket;

		protected GameObject[] points;

		public uint birth_point_index;

		public NetworkObj net_com;

		public float last_user_status_time;

		protected PlayerShell playerShell;

		protected SphereCollider rebirth_triger;

		public float net_ping_sum;

		public int m_multi_id;

		protected float gothitEndTime;

		protected bool No_Damage = true;

		public bool is_real_dead;

		public float lastFireDamagedTime;

		public int m_death_count;

		protected Vector3 last_pos = Vector3.zero;

		protected Vector3 last_rot = Vector3.zero;

		protected Vector3 last_dir = Vector3.zero;

		protected int vs_kill_count;

		protected TNetObject tnetObj;

		protected NetworkTransform net_trans;

		public TNetUser tnet_user;

		public int vs_combo_val;

		public int vs_combo_val_temp;

		protected Transform aimedTransform;

		protected GameObject[] spawnSpots;

		public bool enableHit = true;

		public Collider Collider
		{
			get
			{
				return collider;
			}
			set
			{
				collider = value;
			}
		}

		public GameObject PowerObj
		{
			get
			{
				return powerObj;
			}
			set
			{
				powerObj = value;
			}
		}

		public AvatarType AvatarType
		{
			get
			{
				return avatarType;
			}
			set
			{
				avatarType = value;
			}
		}

		public float HP
		{
			get
			{
				return hp;
			}
			set
			{
				hp = value;
			}
		}

		public float GuiHp { get; set; }

		public float MaxHp
		{
			get
			{
				return maxHp;
			}
			set
			{
				maxHp = value;
			}
		}

		public float Damage
		{
			get
			{
				float num = (cur_weapon.AttackDamage + damage * cur_weapon.AttackFrequency) * PowerBuff;
				if (cur_weapon.Name.StartsWith("Pixel") && SceneName.GetNetMapIndex(Application.loadedLevelName) > 9)
				{
					return num * 1.1f;
				}
				return num;
			}
		}

		public float MoveSpeed
		{
			get
			{
				return moveSpeed - cur_weapon.SpeedDrag;
			}
		}

		public int Level { get; set; }

		public float ExpProgress { get; set; }

		public float PowerBuff { get; set; }

		public float DamageBuff { get; set; }

		public float EXPBuff { get; set; }

		public PlayerBonusState PlayerBonusState
		{
			get
			{
				return playerBonusState;
			}
			set
			{
				playerBonusState = value;
			}
		}

		public string CurrentAnimationName { get; set; }

		public GameObject PlayerObject
		{
			get
			{
				return playerObject;
			}
			set
			{
				playerObject = value;
			}
		}

		public Vector3 HitPoint { get; set; }

		public InputController InputController
		{
			get
			{
				return inputController;
			}
			set
			{
				inputController = value;
			}
		}

		public Vector3 GetHitFlySpeed
		{
			get
			{
				return getHitFlySpeed;
			}
		}

		public NetworkTransform networkTransform
		{
			get
			{
				return net_trans;
			}
			set
			{
				net_trans = value;
			}
		}

		public string WeaponNameEnd
		{
			get
			{
				if (cur_weapon.Name == "Dragon-Breath" || cur_weapon.Name == "Pixel-Cannon")
				{
					if (playerState.GetStateType() == PlayerStateType.Run || playerState.GetStateType() == PlayerStateType.Shoot || playerState.GetStateType() == PlayerStateType.RunShoot)
					{
						return "_AirGun";
					}
				}
				else if (cur_weapon.Name == "Ion-Cannon")
				{
					if (playerState.GetStateType() == PlayerStateType.Shoot || playerState.GetStateType() == PlayerStateType.RunShoot)
					{
						return "_ElectricGun";
					}
				}
				else if (cur_weapon.Name == "SuicideGun" && playerState.GetStateType() == PlayerStateType.Shoot)
				{
					return "_SuicideGun";
				}
				return weaponNameEnd;
			}
		}

		public Transform GetTransform()
		{
			return playerTransform;
		}

		public Transform GetRespawnTransform()
		{
			return respawnTrans;
		}

		public AudioPlayer GetAudioPlayer()
		{
			return audioPlayer;
		}

		public PlayerState GetPlayerState()
		{
			return playerState;
		}

		public Weapon GetWeapon()
		{
			return cur_weapon;
		}

		public WayPointScript NearestWayPoint()
		{
			float num = float.MaxValue;
			WayPointScript result = null;
			GameObject[] array = points;
			foreach (GameObject gameObject in array)
			{
				WayPointScript component = gameObject.GetComponent<WayPointScript>();
				float magnitude = (component.transform.position - playerTransform.position).magnitude;
				if (magnitude < num)
				{
					Ray ray = new Ray(playerTransform.position + new Vector3(0f, 0.5f, 0f), component.transform.position - playerTransform.position);
					RaycastHit hitInfo;
					if (!Physics.Raycast(ray, out hitInfo, magnitude, 100352))
					{
						result = component;
						num = magnitude;
					}
				}
			}
			return result;
		}

		public Transform GetAimedTransform()
		{
			return aimedTransform;
		}

		public void RandomSwordAnimation()
		{
			if ("_Saw2" == weaponNameEnd)
			{
				weaponNameEnd = "_Saw";
			}
			else
			{
				weaponNameEnd = "_Saw2";
			}
		}

		public void RandomSawAnimation()
		{
			weaponNameEnd = ((Random.Range(0, 100) >= 50) ? "_Saw2" : "_Saw");
		}

		public void ResetSawAnimation()
		{
			if (cur_weapon.GetWeaponType() == WeaponType.Saw || cur_weapon.GetWeaponType() == WeaponType.Sword)
			{
				weaponNameEnd = "_Saw";
			}
		}

		public virtual void CreateScreenBlood()
		{
			if (gameCamera != null)
			{
				gameCamera.CreateScreenBlood(1f);
			}
		}

		public void SetMoveDirection()
		{
			moveDirection = inputController.InputInfo.moveDirection;
		}

		public virtual void Move(float m_delta)
		{
			if (charController != null)
			{
				Vector3 motion = moveDirection * (MoveSpeed * m_delta);
				charController.Move(motion);
			}
		}

		public void Move(float m_delta, Vector3 dir)
		{
			if (charController != null)
			{
				Vector3 motion = dir * (MoveSpeed * m_delta);
				charController.Move(motion);
			}
		}

		public void PlusVsKillCount()
		{
			vs_kill_count++;
			GameVSScene gameVSScene = GameApp.GetInstance().GetGameScene() as GameVSScene;
			vs_combo_val_temp++;
			if (vs_combo_val_temp > vs_combo_val)
			{
				vs_combo_val = vs_combo_val_temp;
			}
			if (vs_combo_val_temp > 1)
			{
				SFSObject sFSObject = new SFSObject();
				sFSObject.PutInt("comboCount", vs_combo_val_temp);
				tnetObj.Send(new BroadcastMessageRequest(sFSObject));
			}
			gameVSScene.GameGUI.SetKillCountLabel(vs_kill_count);
			GameApp.GetInstance().GetGameState().AddCashForRecord(2000);
			UpdateVSStatistic();
		}

		public void UpdateVSStatistic()
		{
			SFSObject sFSObject = new SFSObject();
			SFSObject sFSObject2 = new SFSObject();
			sFSObject2.PutInt("killCount", vs_kill_count);
			sFSObject2.PutInt("deathCount", m_death_count);
			sFSObject2.PutInt("cashLoot", GameApp.GetInstance().GetGameState().loot_cash);
			sFSObject2.PutInt("vsCombo", vs_combo_val);
			sFSObject.PutSFSObject("data", sFSObject2);
			tnetObj.Send(new SetUserVariableRequest(TNetUserVarType.userStatistics, sFSObject));
		}

		public virtual void Init()
		{
			avatarType = GameApp.GetInstance().GetGameState().Avatar;
			birth_point_index = 0u;
			points = GameObject.FindGameObjectsWithTag("WayPoint");
			playerObject = AvatarFactory.GetInstance().CreateAvatar(avatarType);
			if (SceneName.GetNetMapIndex(Application.loadedLevelName) > 9)
			{
				playerObject.GetComponent<CharacterController>().slopeLimit = 60f;
			}
			spawnSpots = GameObject.FindGameObjectsWithTag("Respawn");
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				net_com = GameApp.GetInstance().GetGameState().net_com;
				if (GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master)
				{
					birth_point_index = 0u;
				}
				else
				{
					birth_point_index = (uint)GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.room_index;
				}
				playerObject.transform.position = spawnSpots[birth_point_index].transform.position;
				playerObject.transform.rotation = spawnSpots[birth_point_index].transform.rotation;
				GameUIScriptNew.GetGameUIScript().AddMultiplayerMiniMapMark(this, (int)birth_point_index);
				respawnTrans = playerObject.transform;
			}
			else
			{
				int num = (int)(birth_point_index = (uint)Random.Range(0, spawnSpots.Length));
				playerObject.transform.position = spawnSpots[num].transform.position;
				playerObject.transform.rotation = spawnSpots[num].transform.rotation;
				respawnTrans = playerObject.transform;
			}
			playerObject.name = "Player";
			playerTransform = playerObject.transform;
			aimedTransform = playerTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head");
			gameCamera = GameApp.GetInstance().GetGameScene().GetCamera();
			charController = playerObject.GetComponent<CharacterController>();
			animation = playerObject.GetComponent<Animation>();
			collider = playerObject.GetComponent<Collider>();
			inputController = new TPSInputController();
			inputController.Init();
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				tnetObj = TNetConnection.Connection;
				net_trans = NetworkTransform.FromTransform(playerTransform, moveDirection);
				tnet_user = tnetObj.Myself;
			}
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
			GuiHp = hp;
			Level = avatarByType.level;
			ExpProgress = avatarByType.GetExpPercent();
			EXPBuff = 1f;
			if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Vs)
			{
				if (GameApp.GetInstance().GetGameState().gameCounterForPlayerExpBuff > 0 && GameApp.GetInstance().GetGameState().gameCounterForPlayerExpBuff <= 10)
				{
					EXPBuff = 1.5f;
					GameApp.GetInstance().GetGameState().gameCounterForPlayerExpBuff--;
				}
				else if (GameApp.GetInstance().GetGameState().gameCounterForPlayerExpBuff > 100 && GameApp.GetInstance().GetGameState().gameCounterForPlayerExpBuff <= 115)
				{
					EXPBuff = 2.5f;
					GameApp.GetInstance().GetGameState().gameCounterForPlayerExpBuff--;
				}
				if (GameApp.GetInstance().GetGameState().gameCounterForPlayerExpBuff == 0 || GameApp.GetInstance().GetGameState().gameCounterForPlayerExpBuff == 100)
				{
					GameApp.GetInstance().GetGameState().gameCounterForPlayerExpBuff = -1;
				}
			}
			audioPlayer = new AudioPlayer();
			Transform folderTrans = playerTransform.Find("Audio");
			audioPlayer.AddAudio(folderTrans, "GetItem", true);
			audioPlayer.AddAudio(folderTrans, "Dead", true);
			audioPlayer.AddAudio(folderTrans, "Switch", true);
			audioPlayer.AddAudio(folderTrans, "GetHp", true);
			audioPlayer.AddAudio(folderTrans, "GetBullet", true);
			audioPlayer.AddAudio(folderTrans, "Rebirth", true);
			audioPlayer.AddAudio(folderTrans, "Stealth", true);
			audioPlayer.AddAudio(folderTrans, "Super", true);
			audioPlayer.AddAudio(folderTrans, "GetMoney", true);
			audioPlayer.AddAudio(folderTrans, "Power", true);
			audioPlayer.AddAudio(folderTrans, "Medpack", true);
			audioPlayer.AddAudio(folderTrans, "Shield", true);
			audioPlayer.AddAudio(folderTrans, "LevelUp", true);
			GameApp.GetInstance().GetGameState().InitWeapons();
			weaponList = GameApp.GetInstance().GetGameState().GetBattleWeapons();
			foreach (Weapon weapon in weaponList)
			{
				weapon.Init();
			}
			ChangeWeapon(weaponList[0]);
			carryItemsPacket = new Dictionary<ItemType, int>();
			foreach (Item item in GameApp.GetInstance().GetGameState().GetItemsCarried())
			{
				carryItemsPacket.Add(item.iType, item.GetCarryInGameCount());
			}
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				if (GameApp.GetInstance().GetGameState().Medpack < 1)
				{
					GameApp.GetInstance().GetGameState().Medpack = 1;
				}
			}
			else if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Hunting || GameApp.GetInstance().GetGameState().gameMode == GameMode.SoloBoss || GameApp.GetInstance().GetGameState().gameMode == GameMode.Instance || GameApp.GetInstance().GetGameState().gameMode == GameMode.DinoHunting)
			{
				pickupItemsPacket = new Dictionary<ItemType, int>();
			}
			playerState = null;
			playerBonusState = null;
			SetState(PlayerStateType.Idle);
			SetBonusState(PlayerBonusStateType.Normal);
			playerShell = playerObject.AddComponent<PlayerShell>() as PlayerShell;
			playerShell.m_player = this;
			last_user_status_time = Time.time;
			SendNetMsg();
		}

		public virtual void OnVSRebirth()
		{
			OnRebirth();
			int num = Random.Range(0, spawnSpots.Length);
			GameObject gameObject = spawnSpots[num];
			respawnTrans = gameObject.transform;
			playerObject.transform.position = gameObject.transform.position;
			playerObject.transform.rotation = gameObject.transform.rotation;
			SFSObject sFSObject = new SFSObject();
			net_trans = NetworkTransform.FromTransform(playerTransform, moveDirection);
			net_trans.TimeStamp = tnetObj.TimeManager.NetworkTime;
			net_trans.ToSFSObject(sFSObject);
			SFSObject sFSObject2 = new SFSObject();
			sFSObject2.PutSFSObject("rebirth", sFSObject);
			tnetObj.Send(new BroadcastMessageRequest(sFSObject2));
			gameCamera.player = this;
		}

		public void SendNetMsg()
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				m_multi_id = GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.user_id;
				Debug.Log("Player id:" + m_multi_id);
				uint[] array = new uint[3];
				for (int i = 0; i < 3; i++)
				{
					array[i] = 9999u;
				}
				int num = 0;
				foreach (Weapon weapon in weaponList)
				{
					array[num] = (uint)weapon.weapon_index;
					num++;
				}
				Packet packet = CGUserBirthPacket.MakePacket((long)(Time.time * 1000f), birth_point_index, array[0], array[1], array[2]);
				net_com.Send(packet);
				AddRebirthCom();
			}
			else
			{
				if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Vs)
				{
					return;
				}
				int[] array2 = new int[3];
				for (int j = 0; j < 3; j++)
				{
					array2[j] = 9999;
				}
				int num2 = 0;
				foreach (Weapon weapon2 in weaponList)
				{
					array2[num2] = weapon2.weapon_index;
					num2++;
				}
				float[] array3 = new float[3];
				for (int k = 0; k < 3; k++)
				{
					array3[k] = 0f;
				}
				num2 = 0;
				foreach (Weapon weapon3 in weaponList)
				{
					array3[num2] = weapon3.AttackFrequency;
					num2++;
				}
				SFSObject sFSObject = new SFSObject();
				sFSObject.PutUtfString("NickName", GameApp.GetInstance().GetGameState().nick_name);
				sFSObject.PutInt("avatarType", (int)avatarType);
				sFSObject.PutInt("weapon1", array2[0]);
				sFSObject.PutInt("weapon2", array2[1]);
				sFSObject.PutInt("weapon3", array2[2]);
				sFSObject.PutFloat("weaponPara1", array3[0]);
				sFSObject.PutFloat("weaponPara2", array3[1]);
				sFSObject.PutFloat("weaponPara3", array3[2]);
				sFSObject.PutFloat("maxHp", maxHp);
				sFSObject.PutInt("birthPoint", 0);
				tnetObj.Send(new SetUserVariableRequest(TNetUserVarType.avatarData, sFSObject));
				SFSObject sFSObject2 = new SFSObject();
				sFSObject2.PutInt("data", 0);
				tnetObj.Send(new SetUserVariableRequest(TNetUserVarType.CurWeapon, sFSObject2));
				sFSObject2 = new SFSObject();
				sFSObject2.PutInt("data", (int)playerState.GetStateType());
				tnetObj.Send(new SetUserVariableRequest(TNetUserVarType.PlayerState, sFSObject2));
				sFSObject2 = new SFSObject();
				sFSObject2.PutInt("data", (int)playerBonusState.StateType);
				tnetObj.Send(new SetUserVariableRequest(TNetUserVarType.PlayerBonusState, sFSObject2));
			}
		}

		public virtual void AddRebirthCom()
		{
			rebirth_triger = playerObject.AddComponent<SphereCollider>();
			rebirth_triger.radius = 1f;
			rebirth_triger.isTrigger = true;
			rebirth_triger.enabled = false;
			playerObject.AddComponent<PlayerRebirth>();
		}

		public virtual void SetBonusState(PlayerBonusStateType type)
		{
			if (playerBonusState != null)
			{
				playerBonusState.ExitState(this);
			}
			switch (type)
			{
			case PlayerBonusStateType.GodBuff:
				playerBonusState = new PlayerBonusStateGodBuff();
				break;
			case PlayerBonusStateType.Normal:
				playerBonusState = new PlayerBonusStateNormal();
				break;
			case PlayerBonusStateType.PowerUp:
				playerBonusState = new PlayerBonusStatePower();
				break;
			case PlayerBonusStateType.Stealth:
				playerBonusState = new PlayerBonusStateStealth();
				break;
			case PlayerBonusStateType.Suicidegun:
				playerBonusState = new PlayerBonusStateSuicidegun();
				break;
			case PlayerBonusStateType.Super:
				playerBonusState = new PlayerBonusStateSuper();
				break;
			case PlayerBonusStateType.Shield:
				playerBonusState = new PlayerBonusStateShield();
				break;
			}
			playerBonusState.EnterState(this);
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop && net_com != null)
			{
				Packet packet = CGUserAuxiliaryActionPacket.MakePacket((uint)net_com.m_netUserInfo.user_id, (uint)playerBonusState.StateType);
				net_com.Send(packet);
			}
			else if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs && tnetObj != null)
			{
				SFSObject sFSObject = new SFSObject();
				sFSObject.PutInt("data", (int)playerBonusState.StateType);
				tnetObj.Send(new SetUserVariableRequest(TNetUserVarType.PlayerBonusState, sFSObject));
			}
		}

		public virtual void SetState(PlayerStateType type)
		{
			if (playerState != null)
			{
				playerState.ExitState(this);
			}
			switch (type)
			{
			case PlayerStateType.Dead:
				playerState = new PlayerDeadState();
				break;
			case PlayerStateType.GotHit:
				playerState = new PlayerGotHitState();
				break;
			case PlayerStateType.Idle:
				playerState = new PlayerIdleState();
				break;
			case PlayerStateType.Run:
				playerState = new PlayerRunState();
				break;
			case PlayerStateType.RunShoot:
				playerState = new PlayerRunAndShootState();
				break;
			case PlayerStateType.Shoot:
				playerState = new PlayerShootState();
				break;
			case PlayerStateType.GotRushHit:
				playerState = new PlayerGotRushForceState();
				break;
			}
			playerState.EnterState(this);
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop && net_com != null)
			{
				Packet packet = CGUserActionPacket.MakePacket((uint)net_com.m_netUserInfo.user_id, (uint)playerState.GetStateType());
				net_com.Send(packet);
			}
			else if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs && tnetObj != null)
			{
				SFSObject sFSObject = new SFSObject();
				sFSObject.PutInt("data", (int)playerState.GetStateType());
				tnetObj.Send(new SetUserVariableRequest(TNetUserVarType.PlayerState, sFSObject));
			}
		}

		public void UpdateAndBroadcastBonusInfo(bool isInitialSpawn, string lockIdx)
		{
			GameObject[] vSBonus = GameApp.GetInstance().GetGameScene().GetVSBonus();
			SFSArray sFSArray = new SFSArray();
			for (int i = 0; i < vSBonus.Length; i++)
			{
				BonusManager component = vSBonus[i].GetComponent<BonusManager>();
				if (isInitialSpawn)
				{
					component.InitBonusObject();
					SFSObject sFSObject = new SFSObject();
					sFSObject.PutInt("sceneIdx", component.bonusSceneIndex);
					sFSObject.PutInt("lockIdx", component.ID);
					sFSObject.PutInt("type", (int)component.GetCurrentBonusType());
					sFSArray.AddSFSObject(sFSObject);
					continue;
				}
				SFSObject sFSObject2 = new SFSObject();
				if (component.ID.ToString() == lockIdx)
				{
					sFSObject2.PutInt("sceneIdx", component.bonusSceneIndex);
					sFSObject2.PutInt("lockIdx", component.ID);
					sFSObject2.PutInt("type", 37);
				}
				else
				{
					sFSObject2.PutInt("sceneIdx", component.bonusSceneIndex);
					sFSObject2.PutInt("lockIdx", component.ID);
					sFSObject2.PutInt("type", (int)component.GetCurrentBonusType());
				}
				sFSArray.AddSFSObject(sFSObject2);
			}
			SFSObject sFSObject3 = new SFSObject();
			sFSObject3.PutSFSArray("data", sFSArray);
			tnetObj.Send(new SetRoomVariableRequest(TNetRoomVarType.BonusInfo, sFSObject3));
		}

		public bool IsPlayingAnimation(string name)
		{
			return animation.IsPlaying(name);
		}

		public bool AnimationEnds(string name)
		{
			if (name != null)
			{
				if (animation[name].time >= animation[name].clip.length * 1f || animation[name].wrapMode == WrapMode.Loop)
				{
					return true;
				}
				return false;
			}
			return true;
		}

		public bool IsAnimationPlayedPercentage(string aniName, float percentage)
		{
			if (animation[aniName].time >= animation[aniName].clip.length * percentage)
			{
				return true;
			}
			return false;
		}

		public void PlayAnimate(string animationName, WrapMode wrapMode)
		{
			if (!(animation[animationName] != null))
			{
				return;
			}
			animation[animationName].wrapMode = wrapMode;
			if (!IsPlayingAnimation("Damage01") || animationName.StartsWith("Death0"))
			{
				if (wrapMode == WrapMode.Loop || (!animation.IsPlaying(animationName) && animationName != "Damage01"))
				{
					animation.Play(animationName);
				}
				else
				{
					animation.Stop();
					animation.Play(animationName);
				}
				CurrentAnimationName = animationName;
			}
		}

		public void StopAnimation(string animationName)
		{
			if (animation.IsPlaying(animationName))
			{
				animation.Stop();
			}
		}

		public void Animate(string animationName, WrapMode wrapMode)
		{
			if (!(animation[animationName] != null))
			{
				return;
			}
			animation[animationName].wrapMode = wrapMode;
			if (!IsPlayingAnimation("Damage01") || animationName.StartsWith("Death0"))
			{
				if (wrapMode == WrapMode.Loop || (!animation.IsPlaying(animationName) && animationName != "Damage01"))
				{
					animation.CrossFade(animationName);
				}
				else
				{
					animation.Stop();
					animation.Play(animationName);
				}
				CurrentAnimationName = animationName;
			}
		}

		public void ZoomIn(float deltaTime)
		{
			if (cur_weapon.GetWeaponType() == WeaponType.AssaultRifle || cur_weapon.GetWeaponType() == WeaponType.MachineGun)
			{
				gameCamera.ZoomIn(deltaTime);
			}
		}

		public void AutoAim(float deltaTime)
		{
			cur_weapon.AutoAim(deltaTime);
		}

		public void Fire(float deltaTime)
		{
			if (GameApp.GetInstance().GetGameScene().GamePlayingState == PlayingState.GamePlaying)
			{
				cur_weapon.Fire(deltaTime);
			}
		}

		public void ZoomOut(float deltaTime)
		{
			gameCamera.ZoomOut(deltaTime);
		}

		public void StopFire()
		{
			cur_weapon.StopFire();
		}

		private void IncreaseDoctorHp(float delta)
		{
			if ((avatarType == AvatarType.Doctor || avatarType == AvatarType.Pastor) && playerState != null && playerState.GetStateType() != PlayerStateType.Dead && GameApp.GetInstance().GetGameState().gameMode != GameMode.Vs)
			{
				hp += maxHp * 1f / 100f * delta;
				if (hp > maxHp)
				{
					hp = maxHp;
				}
			}
		}

		private void UpdateGuiHp(float delta)
		{
			if (GuiHp != hp)
			{
				float num = Mathf.Abs(GuiHp - hp);
				GuiHp = Mathf.MoveTowards(GuiHp, hp, num * 5f * delta);
			}
		}

		public virtual void DoLogic(float deltaTime)
		{
			if (playerBonusState != null)
			{
				playerBonusState.DoStateLogic(this, deltaTime);
			}
			if (playerState != null)
			{
				playerState.DoStateLogic(this, deltaTime);
			}
			UpdateGuiHp(deltaTime);
			IncreaseDoctorHp(deltaTime);
			foreach (Weapon weapon in weaponList)
			{
				if (weapon.IsSelectedForBattle)
				{
					weapon.DoLogic();
				}
			}
			if (Time.time - last_user_status_time >= 0.2f)
			{
				SendNetUserStatusMsg();
				last_user_status_time = Time.time;
			}
		}

		public void SendNetUserStatusMsg()
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				if (last_pos != playerTransform.position || last_rot != playerTransform.rotation.eulerAngles || last_dir != moveDirection)
				{
					Packet packet = CGUserStatusPacket.MakePacket((uint)net_com.m_netUserInfo.user_id, playerTransform.position, playerTransform.rotation.eulerAngles, moveDirection, (ulong)(net_com.m_netUserInfo.net_ping * 1000f));
					net_com.Send(packet);
					last_pos = playerTransform.position;
					last_rot = playerTransform.rotation.eulerAngles;
					last_dir = moveDirection;
				}
			}
			else if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				net_trans = NetworkTransform.FromTransform(playerTransform, moveDirection);
				net_trans.TimeStamp = tnetObj.TimeManager.NetworkTime;
				SFSObject sFSObject = new SFSObject();
				net_trans.ToSFSObject(sFSObject);
				SFSObject sFSObject2 = new SFSObject();
				sFSObject2.PutSFSObject("trans", sFSObject);
				tnetObj.Send(new BroadcastMessageRequest(sFSObject2));
			}
		}

		public virtual void OnHit(float damage)
		{
			if (playerBonusState.StateType == PlayerBonusStateType.GodBuff)
			{
				return;
			}
			if (enableHit)
			{
				hp -= damage * DamageBuff;
			}
			hp = (int)hp;
			hp = Mathf.Clamp(hp, 0f, maxHp);
			playerState.OnHit(this);
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				Packet packet = CGUserInjuryPacket.MakePacket((uint)GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.user_id, (long)(damage * 1000f), (long)(maxHp * 1000f), (long)(hp * 1000f));
				GameApp.GetInstance().GetGameState().net_com.Send(packet);
				if (No_Damage)
				{
					No_Damage = false;
				}
			}
		}

		public virtual void OnInjuredWithUser(TNetUser sender, float damage, int weapon_type)
		{
			if (!(HP > 0f))
			{
				return;
			}
			OnHit(damage);
			if (HP <= 0f)
			{
				GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/killplayer/kill" + Random.Range(1, 4)), playerTransform.position, Quaternion.identity) as GameObject;
				gameObject.GetComponent<AudioSource>().Play();
				gameObject.GetComponent<AudioSource>().mute = !GameApp.GetInstance().GetGameState().SoundOn;
				SFSObject sFSObject = new SFSObject();
				sFSObject.PutBool("killed", true);
				tnetObj.Send(new ObjectMessageRequest(sender.Id, sFSObject));
				SFSObject sFSObject2 = new SFSObject();
				sFSObject2.PutBool("deaded", true);
				tnetObj.Send(new BroadcastMessageRequest(sFSObject2));
				GameVSScene gameVSScene = GameApp.GetInstance().GetGameScene() as GameVSScene;
				Multiplayer multiplayer = gameVSScene.SFS_Player_Arr[sender] as Multiplayer;
				SFSObject sFSObject3 = new SFSObject();
				sFSObject3.PutUtfString("msg", multiplayer.nick_name + " FRAGGED " + GameApp.GetInstance().GetGameState().nick_name);
				tnetObj.Send(new BroadcastMessageRequest(sFSObject3));
				playerShell.OnDeadCameraChange(gameVSScene.SFS_Player_Arr[sender]);
				if (gameVSScene.SFS_Player_Arr[sender].playerBonusState.StateType == PlayerBonusStateType.Stealth)
				{
					(gameVSScene.SFS_Player_Arr[sender].playerBonusState as MultiPlayerBonusStateStealth).Exposure = true;
				}
			}
		}

		public bool CouldGetAnotherHit()
		{
			if (Time.time - gothitEndTime > 0.5f)
			{
				gothitEndTime = Time.time;
				return true;
			}
			return false;
		}

		public virtual void OnRebirth()
		{
			playerObject.GetComponent<Collider>().enabled = true;
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				rebirth_triger.enabled = false;
			}
			playerObject.layer = 8;
			Transform transform = gameCamera.gameObject.transform.Find("Screen_Blood_Dead");
			if (transform != null)
			{
				transform.gameObject.SetActive(false);
			}
			GetHealed((int)maxHp);
			GameScene gameScene = GameApp.GetInstance().GetGameScene();
			gameScene.GamePlayingState = PlayingState.GamePlaying;
			PlayRebirthEffect();
			SetState(PlayerStateType.Idle);
			SetBonusState(PlayerBonusStateType.GodBuff);
			audioPlayer.PlayAudio("Rebirth");
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				GameMultiplayerScene gameMultiplayerScene = gameScene as GameMultiplayerScene;
				gameMultiplayerScene.m_multi_player_arr.Add(this);
				gameMultiplayerScene.ResetEnemyTarget();
				GameApp.GetInstance().GetGameScene().GameGUI.HideRebirthMsgBox();
				is_real_dead = false;
			}
			else
			{
				if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Vs)
				{
					return;
				}
				GameVSScene gameVSScene = gameScene as GameVSScene;
				foreach (TNetUser key in gameVSScene.SFS_Player_Arr.Keys)
				{
					if (key.Id != tnet_user.Id && gameVSScene.SFS_Player_Arr[key].playerBonusState.StateType == PlayerBonusStateType.Stealth)
					{
						(gameVSScene.SFS_Player_Arr[key].playerBonusState as MultiPlayerBonusStateStealth).Exposure = false;
					}
				}
			}
		}

		protected void PlayRebirthEffect()
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				playerObject.transform.Find("Avatar_Suit").GetComponent<Renderer>().enabled = true;
				if (playerObject.transform.Find("Avatar_Cap") != null)
				{
					playerObject.transform.Find("Avatar_Cap").GetComponent<Renderer>().enabled = true;
				}
				if (PlayerObject.transform.Find("Avatar_LegArms") != null)
				{
					PlayerObject.transform.Find("Avatar_LegArms").GetComponent<Renderer>().enabled = true;
				}
				if (PlayerObject.transform.Find("Wonk_Eyeglass") != null)
				{
					PlayerObject.transform.Find("Wonk_Eyeglass").GetComponent<Renderer>().enabled = true;
				}
				playerObject.transform.Find("shadow").GetComponent<Renderer>().enabled = true;
				cur_weapon.GunOn();
			}
		}

		protected void PlayDeadEffect()
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().pixelDead, playerTransform.position, playerTransform.rotation);
				Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().pixelExplosion, playerTransform.position, Quaternion.identity);
				playerObject.transform.Find("Avatar_Suit").GetComponent<Renderer>().enabled = false;
				if (playerObject.transform.Find("Avatar_Cap") != null)
				{
					playerObject.transform.Find("Avatar_Cap").GetComponent<Renderer>().enabled = false;
				}
				if (PlayerObject.transform.Find("Avatar_LegArms") != null)
				{
					PlayerObject.transform.Find("Avatar_LegArms").GetComponent<Renderer>().enabled = false;
				}
				if (PlayerObject.transform.Find("Wonk_Eyeglass") != null)
				{
					PlayerObject.transform.Find("Wonk_Eyeglass").GetComponent<Renderer>().enabled = false;
				}
				playerObject.transform.Find("shadow").GetComponent<Renderer>().enabled = false;
				cur_weapon.GunOff();
			}
			else
			{
				int num = Random.Range(1, 4);
				Animate("Death0" + num, WrapMode.ClampForever);
			}
		}

		public virtual void OnDead()
		{
			audioPlayer.PlayAudio("Dead");
			cur_weapon.StopFire();
			PlayDeadEffect();
			Transform transform = gameCamera.gameObject.transform.Find("Screen_Blood_Dead");
			if (transform != null)
			{
				transform.gameObject.SetActive(true);
			}
			m_death_count++;
			playerObject.GetComponent<Collider>().enabled = false;
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				GameMultiplayerScene gameMultiplayerScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
				gameMultiplayerScene.GamePlayingState = PlayingState.GameLose;
				rebirth_triger.enabled = true;
				playerObject.layer = 27;
				gameMultiplayerScene.OnMultiPlayerDead(this);
				playerObject.GetComponent<PlayerRebirth>().CancelRebirth();
				Packet packet = CGOnUserDeadPacket.MakePacket();
				net_com.Send(packet);
				is_real_dead = false;
				gameMultiplayerScene.GameGUI.ShowRebirthMsgBox();
			}
			else if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				GameVSScene gameVSScene = GameApp.GetInstance().GetGameScene() as GameVSScene;
				gameVSScene.GameGUI.SetDeathCountLabel(m_death_count);
				gameVSScene.GamePlayingState = PlayingState.GameLose;
				UpdateVSStatistic();
				vs_combo_val_temp = 0;
			}
			else if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Instance)
			{
				GameApp.GetInstance().GetGameScene().GamePlayingState = PlayingState.GameLose;
				GameApp.GetInstance().GetGameScene().SaveDataReport();
				GameApp.GetInstance().GetGameScene().GameGUI.ShowGameOverPanel(GameOverType.instanceLose);
			}
			else
			{
				GameApp.GetInstance().GetGameScene().GamePlayingState = PlayingState.GameLose;
				GameApp.GetInstance().GetGameScene().GameGUI.ShowGameOverPanel(GameOverType.soloLose);
				Debug.Log("loose game");
				GameApp.GetInstance().GetGameState().Achievement.LoseGame();
			}
		}

		public virtual void PlayerRealDead()
		{
			Packet packet = CGGameOverPacket.MakePacket((uint)m_multi_id);
			net_com.Send(packet);
			is_real_dead = true;
			(GameApp.GetInstance().GetGameScene() as GameMultiplayerScene).CheckMultiGameOver();
		}

		public virtual void OnMultiInjured(float damage)
		{
		}

		public virtual void OnVsInjured(TNetUser sender, float damage, int weapon_type)
		{
		}

		public void GetHealed(int point)
		{
			hp += point;
			hp = Mathf.Clamp(hp, 0f, maxHp);
		}

		public void GetFullyHealed()
		{
			hp = maxHp;
		}

		public void SendNetUserChangeWeaponMsg(int index)
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				Packet packet = CGUserChangeWeaponPacket.MakePacket((uint)net_com.m_netUserInfo.user_id, (uint)index);
				net_com.Send(packet);
				Debug.Log("Player change weapon : " + index);
			}
			else if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				SFSObject sFSObject = new SFSObject();
				sFSObject.PutInt("data", index);
				tnetObj.Send(new SetUserVariableRequest(TNetUserVarType.CurWeapon, sFSObject));
			}
		}

		public void ChangeWeaponBackFromSuicideGun(Weapon weapon)
		{
			if (weapon.IsSelectedForBattle || GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				if (cur_weapon != null)
				{
					cur_weapon.GunOff();
					Animate("Idle01" + weaponNameEnd, WrapMode.Loop);
				}
				cur_weapon = weapon;
				cur_weapon.GunOn();
				audioPlayer.PlayAudio("Switch");
				weaponNameEnd = Weapon.GetWeaponNameEnd(cur_weapon.GetWeaponType(), cur_weapon.Name);
				GameUIScriptNew.GetGameUIScript().weaponInfo.SetWeaponLogo(cur_weapon.Name);
			}
		}

		public virtual void ChangeWeapon(Weapon weapon)
		{
			if (weapon.IsSelectedForBattle && (playerBonusState == null || playerBonusState.StateType != PlayerBonusStateType.Suicidegun || cur_weapon == null || cur_weapon.GetWeaponType() != WeaponType.SuicideGun))
			{
				if (cur_weapon != null)
				{
					cur_weapon.GunOff();
					Animate("Idle01" + weaponNameEnd, WrapMode.Loop);
				}
				cur_weapon = weapon;
				cur_weapon.GunOn();
				audioPlayer.PlayAudio("Switch");
				weaponNameEnd = Weapon.GetWeaponNameEnd(cur_weapon.GetWeaponType(), cur_weapon.Name);
				GameUIScriptNew.GetGameUIScript().weaponInfo.SetWeaponLogo(cur_weapon.Name);
			}
		}

		public void NextWeapon()
		{
			if (GameApp.GetInstance().GetGameScene().GamePlayingState == PlayingState.GamePlaying)
			{
				currentWeaponIndex++;
				if (currentWeaponIndex >= weaponList.Count)
				{
					currentWeaponIndex = 0;
				}
				ChangeWeaponAndSendMsg(currentWeaponIndex);
			}
		}

		public void ChangeWeaponAndSendMsg(int currentWeaponIndex)
		{
			string name = cur_weapon.Name;
			ChangeWeapon(weaponList[currentWeaponIndex]);
			if (name != cur_weapon.Name)
			{
				SendNetUserChangeWeaponMsg(currentWeaponIndex);
			}
		}

		public virtual void OnVsPickUp(ItemType type, string index)
		{
			if (playerState == null || playerState.GetStateType() != PlayerStateType.Dead)
			{
				tnetObj.Send(new LockSthRequest(index));
			}
		}

		public virtual bool OnMultiplayerPickUp(ItemType type, int id)
		{
			if (!OnPickUp(type))
			{
				return false;
			}
			net_com.Send(CGPickItemPacket.MakePacket((uint)id));
			return true;
		}

		public virtual bool OnPickUp(ItemType type)
		{
			switch (type)
			{
			case ItemType.Hp:
			{
				GetHealed(2000);
				GameObject gameObject = Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().itemBigHpEffect, Vector3.zero, Quaternion.identity) as GameObject;
				gameObject.transform.parent = playerTransform;
				gameObject.transform.localPosition = Vector3.zero;
				audioPlayer.PlayAudio("GetHp");
				break;
			}
			case ItemType.Gold:
			{
				int cashGot = (int)(300f * GameApp.GetInstance().GetGameScene().GetEnemyAttributesComputingFactor("woodboxLoot"));
				GameApp.GetInstance().GetGameState().AddCashForRecord(cashGot);
				audioPlayer.PlayAudio("GetMoney");
				break;
			}
			case ItemType.Gold_Big:
			{
				int cashGot = (int)(300f * GameApp.GetInstance().GetGameScene().GetEnemyAttributesComputingFactor("woodboxLoot") * 5f);
				GameApp.GetInstance().GetGameState().AddCashForRecord(cashGot);
				audioPlayer.PlayAudio("GetMoney");
				break;
			}
			case ItemType.Power:
			case ItemType.InstantStealth:
			case ItemType.InstantSuper:
			case ItemType.SuicideGun:
			case ItemType.Shield:
				ItemEffectOnPlayer(type);
				break;
			case ItemType.AssaultRifle:
			case ItemType.ShotGun:
			case ItemType.RocketLauncher:
			case ItemType.LaserGun:
			case ItemType.Sniper:
			case ItemType.MachineGun:
			case ItemType.Saw:
			case ItemType.Crossbow:
			case ItemType.M32:
			case ItemType.FireGun:
			case ItemType.ElectricGun:
				if (!cur_weapon.WeaponBulletObject) return false;

				audioPlayer.PlayAudio("GetBullet");
				if (cur_weapon.WeaponBulletObject.GetComponent<ItemScript>().itemType == type)
				{
					cur_weapon.AddBullets(cur_weapon.WConf.bulletEachAdd / 4);
					break;
				}
				foreach (Weapon weapon in weaponList)
				{
					if (weapon.Name != cur_weapon.Name && weapon.WeaponBulletObject.GetComponent<ItemScript>().itemType == type)
					{
						weapon.AddBullets(weapon.WConf.bulletEachAdd / 4);
						break;
					}
				}
				break;
			}
			return true;
		}

		public void OnUseCarryItem(ItemType type)
		{
			if ((playerState != null && playerState.GetStateType() == PlayerStateType.Dead) || !playerBonusState.CheckEnableEquipItem(this, type))
			{
				return;
			}
			foreach (ItemType key3 in carryItemsPacket.Keys)
			{
				if (type != key3)
				{
					continue;
				}
				if (carryItemsPacket[key3] > 0)
				{
					Dictionary<ItemType, int> dictionary;
					Dictionary<ItemType, int> dictionary2 = (dictionary = carryItemsPacket);
					ItemType key;
					ItemType key2 = (key = key3);
					int num = dictionary[key];
					dictionary2[key2] = num - 1;
					GameApp.GetInstance().GetGameScene().GameGUI.itemInfo.UpdateCarryItemPacket(key3, carryItemsPacket[key3]);
					GameApp.GetInstance().GetGameState().GetItemByType(key3)
						.OwnedCount--;
					ItemEffectOnPlayer(key3);
					if (key3 == ItemType.SuicideGun)
					{
						cur_weapon.enableShoot = true;
					}
				}
				break;
			}
		}

		private void ItemEffectOnPlayer(ItemType type)
		{
			GameObject gameObject = null;
			switch (type)
			{
			case ItemType.SmallHp:
				GetHealed(300);
				gameObject = Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().itemSmallHpEffect, Vector3.zero, Quaternion.identity) as GameObject;
				gameObject.transform.parent = playerTransform;
				gameObject.transform.localPosition = Vector3.zero;
				audioPlayer.PlayAudio("GetHp");
				break;
			case ItemType.BigHp:
				GetHealed((int)maxHp);
				gameObject = Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().itemBigHpEffect, Vector3.zero, Quaternion.identity) as GameObject;
				gameObject.transform.parent = playerTransform;
				gameObject.transform.localPosition = Vector3.zero;
				audioPlayer.PlayAudio("GetHp");
				break;
			case ItemType.Shield:
				SetBonusState(PlayerBonusStateType.Shield);
				GameApp.GetInstance().GetGameScene().GameGUI.playerInfo.AddBonusStateReminder(ItemType.Shield);
				audioPlayer.PlayAudio("Shield");
				break;
			case ItemType.InstantStealth:
				SetBonusState(PlayerBonusStateType.Stealth);
				GameApp.GetInstance().GetGameScene().GameGUI.playerInfo.AddBonusStateReminder(ItemType.InstantStealth);
				audioPlayer.PlayAudio("Stealth");
				break;
			case ItemType.InstantSuper:
				SetBonusState(PlayerBonusStateType.Super);
				GameApp.GetInstance().GetGameScene().GameGUI.playerInfo.AddBonusStateReminder(ItemType.InstantSuper);
				audioPlayer.PlayAudio("Super");
				break;
			case ItemType.Power:
				SetBonusState(PlayerBonusStateType.PowerUp);
				GameApp.GetInstance().GetGameScene().GameGUI.playerInfo.AddBonusStateReminder(ItemType.Power);
				audioPlayer.PlayAudio("Power");
				break;
			case ItemType.SuicideGun:
				SetBonusState(PlayerBonusStateType.Suicidegun);
				break;
			}
		}

		public virtual bool AddVSWeapon(int type)
		{
			if (weaponList.Count >= 3)
			{
				return false;
			}
			Weapon weapon = GameApp.GetInstance().GetGameState().GetWeapons()[type];
			weapon.Init();
			weapon.IsSelectedForBattle = true;
			weaponList.Add(weapon);
			NextWeapon();
			return true;
		}

		public virtual void OnRebirthStart()
		{
		}

		public virtual void OnRebirthStay(float time)
		{
		}

		public virtual void OnRebirthExit()
		{
		}

		public virtual void OnRebirthFinish()
		{
		}

		public virtual void UpdateNetworkTrans()
		{
		}

		public virtual void OnFireBegin()
		{
			if (playerBonusState.StateType == PlayerBonusStateType.Suicidegun && cur_weapon.GetWeaponType() == WeaponType.SuicideGun && !cur_weapon.enableShoot)
			{
				cur_weapon.enableShoot = true;
			}
		}
	}
}
