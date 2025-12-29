using System;
using UnityEngine;

namespace Zombie3D
{
	public abstract class Enemy
	{
		public const float net_status_rate = 0.2f;

		public static EnemyState GRAVEBORN_STATE = new GraveBornState();

		public static EnemyState PREYGONE_STATE = new PreyGoneState();

		public static EnemyState IDLE_STATE = new IdleState();

		public static EnemyState CATCHING_STATE = new CatchingState();

		public static EnemyState GOTHIT_STATE = new GotHitState();

		public static EnemyState PATROL_STATE = new PatrolState();

		public static EnemyState ATTACK_STATE = new AttackState();

		public static EnemyState DEAD_STATE = new DeadState();

		protected GameObject enemyObject;

		protected Transform enemyTransform;

		protected Animation animation;

		protected Transform aimedTransform;

		protected GameObject m_enemy_mark;

		protected ResourceConfigScript rConfig;

		protected EnemyConfigScript enemyResourceConfig;

		protected GameConfig gConfig;

		protected EnemyType enemyType;

		protected Vector3 lastTarget;

		protected GameScene gameScene;

		protected Player player;

		protected GameObject PreyTip;

		protected CharacterController controller;

		protected Vector3 moveDirection;

		protected AudioPlayer audio;

		protected IPathFinding pathFinding;

		protected string name;

		protected EnemyAttributes mAttributes;

		public bool DestroyAfterDead;

		public float last_enemy_status_time;

		protected Quaternion rot_to;

		protected Vector3 pos_to;

		protected bool m_is_lerp_position;

		protected float runSlowTime;

		protected bool is_RunSlow;

		protected bool moveWithCharacterController;

		protected EnemyState state;

		protected float lastPathFindingTime;

		protected float lastAttackTime = -100f;

		protected float lastGotHitTime = -100f;

		protected int nextPoint = -1;

		protected string runAnimationName = "Run";

		protected GameObject targetObj;

		protected bool criticalAttacked;

		protected bool attacked;

		protected Quaternion deadRotation;

		protected Vector3 deadPosition;

		protected float m_tip_height;

		public bool IsSuperBoss;

		public bool IsPrey;

		public Ray ray;

		public RaycastHit rayhit;

		protected bool is_multi_dead;

		protected bool is_multi_dead_killed;

		public float lastFireDamagedTime;

		protected Vector3 last_pos = Vector3.zero;

		protected Vector3 last_rot = Vector3.zero;

		protected Vector3 last_dir = Vector3.zero;

		public bool IsElite { get; set; }

		public AudioPlayer Audio
		{
			get
			{
				return audio;
			}
		}

		public string RunAnimationName
		{
			get
			{
				return runAnimationName;
			}
		}

		public bool MoveWithCharacterController
		{
			get
			{
				return moveWithCharacterController;
			}
			set
			{
				moveWithCharacterController = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public EnemyAttributes Attributes
		{
			get
			{
				return mAttributes;
			}
		}

		public EnemyType EnemyType
		{
			get
			{
				return enemyType;
			}
			set
			{
				enemyType = value;
			}
		}

		public Vector3 LastTarget
		{
			get
			{
				return lastTarget;
			}
		}

		public Player TargetPlayer
		{
			get
			{
				return player;
			}
			set
			{
				player = value;
				if (player != null)
				{
					lastTarget = player.GetTransform().position;
					Debug.Log("get enemy:" + name + "change target plyer:" + player.m_multi_id);
				}
			}
		}

		public float SqrDistanceFromPlayer
		{
			get
			{
				if (player != null && player.GetTransform() != null)
				{
					return (player.GetTransform().position - enemyTransform.position).sqrMagnitude;
				}
				return float.MaxValue;
			}
		}

		public GameObject GetEnemyObject()
		{
			return enemyObject;
		}

		public Transform GetTransform()
		{
			return enemyTransform;
		}

		public bool CouldPlayHitAnimation()
		{
			if (Time.time - lastGotHitTime >= 2f)
			{
				return true;
			}
			return false;
		}

		public bool CouldMakeNextAttack()
		{
			if (Time.time - lastAttackTime >= mAttributes.AttackSpeed)
			{
				return true;
			}
			return false;
		}

		public virtual bool CouldEnterAttackState()
		{
			if (SqrDistanceFromPlayer < mAttributes.attackRange * mAttributes.attackRange)
			{
				return true;
			}
			return false;
		}

		public virtual bool AttackAnimationEnds()
		{
			if (Time.time - lastAttackTime > animation["Attack01"].clip.length)
			{
				return true;
			}
			return false;
		}

		public bool IsAnimationPlayedPercentage(string aniName, float percentage)
		{
			if (animation[aniName].time >= animation[aniName].clip.length * percentage)
			{
				return true;
			}
			return false;
		}

		public virtual void Animate(string animationName, WrapMode wrapMode)
		{
			animation[animationName].wrapMode = wrapMode;
			if (!animation.IsPlaying("Damage"))
			{
				if (wrapMode == WrapMode.Loop || (!animation.IsPlaying(animationName) && animationName != "Damage"))
				{
					animation.CrossFade(animationName);
					return;
				}
				animation.Stop();
				animation.Play(animationName);
			}
		}

		public void SetInGrave(bool inGrave)
		{
			if (inGrave)
			{
				SetState(GRAVEBORN_STATE);
				enemyObject.layer = 13;
				if (enemyObject.GetComponent<Rigidbody>() != null)
				{
					enemyTransform.Translate(Vector3.down * enemyTransform.GetComponent<Collider>().bounds.size.y * 1.2f, Space.World);
					UnityEngine.Object.DestroyImmediate(enemyObject.GetComponent<Rigidbody>());
					enemyObject.AddComponent<Rigidbody>();
					enemyObject.GetComponent<Rigidbody>().useGravity = false;
					enemyObject.GetComponent<Rigidbody>().freezeRotation = true;
				}
				else
				{
					enemyTransform.Translate(Vector3.down * controller.height * 1.2f, Space.World);
				}
			}
			else
			{
				enemyObject.layer = 9;
				if (enemyObject.GetComponent<Rigidbody>() != null)
				{
					enemyObject.GetComponent<Rigidbody>().useGravity = true;
				}
			}
		}

		public void SetInPreyGone(bool state)
		{
			if (state)
			{
				SetState(PREYGONE_STATE);
				enemyObject.layer = 13;
				if (enemyObject.GetComponent<Rigidbody>() != null)
				{
					enemyObject.GetComponent<Rigidbody>().freezeRotation = true;
					enemyObject.GetComponent<Rigidbody>().useGravity = false;
				}
			}
			else
			{
				RemoveDeadbodyCheck();
			}
		}

		public bool MoveFromGrave(float deltaTime)
		{
			if (enemyTransform.position.y > 10000.1f)
			{
				return true;
			}
			enemyTransform.Translate(Vector3.up * deltaTime * 2f, Space.World);
			return false;
		}

		public bool MoveToMucilage(float deltaTime)
		{
			float num = 0f;
			num = ((!(enemyObject.GetComponent<Rigidbody>() != null)) ? (controller.height * 1.2f) : (enemyTransform.GetComponent<Collider>().bounds.size.y * 1.2f));
			if (enemyTransform.position.y <= 10000.1f - num)
			{
				return true;
			}
			enemyTransform.Translate(Vector3.down * deltaTime * 2f, Space.World);
			return false;
		}

		public void SetTargetWithMultiplayer()
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop && GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master)
			{
				player = null;
				if (GameApp.GetInstance().GetGameScene().m_multi_player_arr.Count > 0)
				{
					int index = UnityEngine.Random.Range(0, GameApp.GetInstance().GetGameScene().m_multi_player_arr.Count);
					player = GameApp.GetInstance().GetGameScene().m_multi_player_arr[index];
				}
				else
				{
					player = gameScene.GetPlayer();
				}
			}
		}

		protected void ComputeAttributes(MonsterConfig mConf)
		{
			mAttributes.Hp = Mathf.Round(mConf.hpWeight * gameScene.GetEnemyAttributesComputingFactor("hp"));
			mAttributes.AttackDamage = mConf.damageWeight * mConf.attackFrequency * gameScene.GetEnemyAttributesComputingFactor("damage");
			mAttributes.AttackDamage = (float)Math.Round(mAttributes.AttackDamage, 1);
			mAttributes.Loot = Mathf.RoundToInt(mConf.lootWeight * gameScene.GetEnemyAttributesComputingFactor("loot"));
			mAttributes.ExpLoot = mAttributes.Loot;
			mAttributes.AttackSpeed = mConf.attackFrequency;
			mAttributes.MoveSpeed = mConf.walkSpeed;
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				if (IsSuperBoss)
				{
					CoopBossConfig coopBossConfig = gConfig.GetCoopBossConfig(GameApp.GetInstance().GetGameState().cur_net_map);
					mAttributes.Hp = coopBossConfig.bossHp;
					mAttributes.AttackDamage = coopBossConfig.bossDamage;
					mAttributes.Loot = 0;
					Debug.Log("hp: " + mAttributes.Hp + " damage: " + mAttributes.AttackDamage);
				}
				return;
			}
			if (IsElite)
			{
				mAttributes.Hp *= 2.0;
				mAttributes.AttackDamage *= 1.5f;
			}
			if (IsSuperBoss)
			{
				mAttributes.Hp *= 3.0;
			}
			if (IsPrey)
			{
				mAttributes.Hp *= MonsterParametersConfig.huntingParameters["hp"];
				mAttributes.AttackDamage *= MonsterParametersConfig.huntingParameters["damage"];
			}
			mAttributes.ExpLoot = mAttributes.Loot;
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Instance)
			{
				mAttributes.Loot = Mathf.RoundToInt((float)mAttributes.Loot * InstanceModeConfig.CashAdjust);
				mAttributes.ExpLoot = Mathf.RoundToInt((float)mAttributes.ExpLoot * InstanceModeConfig.ExpAdjust);
			}
		}

		public virtual void Init(GameObject gObject)
		{
			gameScene = GameApp.GetInstance().GetGameScene();
			player = gameScene.GetPlayer();
			lastTarget = Vector3.zero;
			enemyObject = gObject;
			enemyTransform = enemyObject.transform;
			animation = enemyObject.GetComponent<Animation>();
			aimedTransform = enemyTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head");
			rConfig = GameApp.GetInstance().GetResourceConfig();
			enemyResourceConfig = GameApp.GetInstance().GetEnemyResourceConfig();
			gConfig = GameApp.GetInstance().GetGameConfig();
			mAttributes = new EnemyAttributes();
			mAttributes.detectionRange = 150f;
			mAttributes.attackRange = 1.5f;
			criticalAttacked = false;
			SetTargetWithMultiplayer();
			audio = new AudioPlayer();
			Transform folderTrans = enemyTransform.Find("Audio");
			audio.AddAudio(folderTrans, "Attack", true);
			audio.AddAudio(folderTrans, "Dead", true);
			audio.AddAudio(folderTrans, "Special", true);
			audio.AddAudio(folderTrans, "Shout", true);
			pathFinding = new GraphPathFinding();
			pathFinding.InitPath(gameScene.scene_points);
			animation.wrapMode = WrapMode.Loop;
			animation.Play("Idle01");
			SetState(IDLE_STATE);
			lastPathFindingTime = Time.time;
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				LootManagerScript component = enemyObject.GetComponent<LootManagerScript>();
				component.dropRate *= 0.5f;
			}
			rot_to = Quaternion.Euler(enemyTransform.rotation.x, enemyTransform.rotation.y, enemyTransform.rotation.z);
			CreatePreyTip();
			InitEnemyMark();
		}

		public void InitEnemyMark()
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				m_enemy_mark = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/TUI/EnemyMark")) as GameObject;
				GameMultiplayerScene gameMultiplayerScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
				m_enemy_mark.transform.parent = gameMultiplayerScene.GameGUI.mapShow.transform;
				m_enemy_mark.transform.localPosition = new Vector3(0f, 0f, -1f);
				m_enemy_mark.GetComponent<EnemyMark>().m_enemy = this;
			}
		}

		public void RemoveEnemyMark()
		{
			if (m_enemy_mark != null)
			{
				UnityEngine.Object.Destroy(m_enemy_mark);
			}
		}

		public void SetState(EnemyState newState)
		{
			state = newState;
			state.startTime = Time.time;
		}

		public EnemyState GetState()
		{
			return state;
		}

		public virtual void CheckHit()
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop && GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master && player.GetPlayerState() != null && player.GetPlayerState().GetStateType() == PlayerStateType.Dead)
			{
				SetTargetWithMultiplayer();
			}
		}

		public Transform GetAimedTransform()
		{
			return aimedTransform;
		}

		public Vector3 GetPosition()
		{
			return enemyTransform.position;
		}

		public Collider GetCollider()
		{
			return enemyTransform.GetComponent<Collider>();
		}

		public virtual void OnHit(DamageProperty dp, WeaponType weaponType, bool criticalAttack, Player mPlayer)
		{
			if (state == GRAVEBORN_STATE)
			{
				return;
			}
			if (mPlayer != null)
			{
				if (mPlayer.AvatarType == AvatarType.Pastor)
				{
					OnPastorAffect();
				}
				if (weaponType == WeaponType.Sword)
				{
					Vector3 vector = mPlayer.GetTransform().position - enemyTransform.position;
					vector.Normalize();
					UnityEngine.Object.Instantiate(rConfig.swordAttack, enemyTransform.position + Vector3.up * 1.2f + vector * 0.5f, Quaternion.identity);
				}
			}
			UnityEngine.Object.Instantiate(rConfig.hitBlood, enemyTransform.position + Vector3.up * 1f, Quaternion.identity);
			mAttributes.Hp -= dp.damage;
			criticalAttacked = criticalAttack;
			if (IsSuperBoss && CouldPlayHitAnimation())
			{
				state.OnHit(this);
				lastGotHitTime = Time.time;
			}
			if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Coop || !(dp.damage > 0f))
			{
				return;
			}
			int criticalAttack2 = (criticalAttack ? 1 : 0);
			Packet packet = CGEnemyGotHitPacket.MakePacket(name, (long)(dp.damage * 1000f), (uint)weaponType, (uint)criticalAttack2);
			GameApp.GetInstance().GetGameState().net_com.Send(packet);
			if (mAttributes.Hp <= 0.0 && !is_multi_dead)
			{
				is_multi_dead = true;
				if (mPlayer != null)
				{
					int bElite = (IsElite ? 1 : 0);
					Packet packet2 = CGEnemyDeadPacket.MakePacket((uint)mPlayer.m_multi_id, name, (uint)enemyType, (uint)bElite, (uint)weaponType);
					GameApp.GetInstance().GetGameState().net_com.Send(packet2);
					is_multi_dead_killed = true;
				}
				else
				{
					Packet packet3 = CGEnemyRemovePacket.MakePacket(name);
					GameApp.GetInstance().GetGameState().net_com.Send(packet3);
					is_multi_dead_killed = false;
				}
			}
		}

		public virtual void OnMultiHit(double dp, WeaponType weaponType, int criticalAttack)
		{
			if (state != GRAVEBORN_STATE)
			{
				UnityEngine.Object.Instantiate(rConfig.hitBlood, enemyTransform.position + Vector3.up * 1f, Quaternion.identity);
				mAttributes.Hp -= dp;
				criticalAttacked = criticalAttack == 1;
				state.OnHit(this);
			}
		}

		public void OnPastorAffect()
		{
			if (!is_RunSlow)
			{
				mAttributes.MoveSpeed *= 0.5f;
				animation[runAnimationName].speed *= 0.5f;
			}
			is_RunSlow = true;
			runSlowTime = Time.time;
		}

		public virtual void OnAttack()
		{
			audio.PlayAudio("Attack");
		}

		protected void PlayDeadEffects()
		{
			if ((bool)enemyObject && enemyObject.activeInHierarchy)
			{
				PlayBloodEffect();
				Quaternion rotation = Quaternion.Euler(enemyTransform.rotation.eulerAngles.x, UnityEngine.Random.Range(0, 360), enemyTransform.rotation.eulerAngles.z);
				GameObject gameObject = UnityEngine.Object.Instantiate(enemyResourceConfig.GetDeadbody(enemyType), enemyTransform.position + new Vector3(0f, 0.2f, 0f), rotation) as GameObject;
				gameObject.transform.rotation = deadRotation * gameObject.transform.rotation;
				AudioSource component = gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.mute = !GameApp.GetInstance().GetGameState().SoundOn;
				}
				GameObject gameObject2 = UnityEngine.Object.Instantiate(enemyResourceConfig.GetDeadHead(enemyType), enemyTransform.position + new Vector3(0f, 2f, 0f), enemyTransform.rotation) as GameObject;
				gameObject2.GetComponent<Rigidbody>().AddForce(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 0), UnityEngine.Random.Range(-5, 5), ForceMode.Impulse);
				RemoveDeadbodyCheck();
			}
		}

		protected void PlayBloodEffect()
		{
			if ((bool)enemyObject && enemyObject.activeInHierarchy)
			{
				GameObject deadBlood = rConfig.deadBlood;
				int num = UnityEngine.Random.Range(0, 100);
				float y = 10000.119f;
				GameObject original;
				if (num > 50)
				{
					original = rConfig.deadFoorblood;
				}
				else
				{
					original = rConfig.deadFoorblood2;
					y = 10000.109f;
				}
				UnityEngine.Object.Instantiate(deadBlood, enemyTransform.position + new Vector3(0f, 0.5f, 0f), Quaternion.Euler(0f, 0f, 0f));
				GameObject gameObject = UnityEngine.Object.Instantiate(original, new Vector3(enemyTransform.position.x, y, enemyTransform.position.z), Quaternion.Euler(270f, 0f, 0f)) as GameObject;
				gameObject.transform.rotation = deadRotation * gameObject.transform.rotation;
				gameObject.transform.position = deadPosition;
			}
		}

		public virtual void FindPath()
		{
			if ((GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop && !GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master) || player == null || !(Time.time - lastPathFindingTime > 0.5f))
			{
				return;
			}
			lastPathFindingTime = Time.time;
			if (lastTarget == Vector3.zero)
			{
				lastTarget = player.GetTransform().position;
			}
			ray = new Ray(enemyTransform.position + new Vector3(0f, 0.5f, 0f), player.GetTransform().position - enemyTransform.position);
			float magnitude = (player.GetTransform().position - enemyTransform.position).magnitude;
			if (magnitude < mAttributes.detectionRange && !Physics.Raycast(ray, out rayhit, magnitude, 67209216))
			{
				pathFinding.ClearPath();
				lastTarget = player.GetTransform().position;
			}
			else if (!MultiplayerDistanceChack())
			{
				if (!pathFinding.HavePath())
				{
					Transform nextWayPoint = pathFinding.GetNextWayPoint(enemyTransform.position, player.GetTransform().position);
					if (nextWayPoint != null)
					{
						lastTarget = nextWayPoint.position;
					}
				}
				else if ((enemyTransform.position - lastTarget).magnitude < 1.5f)
				{
					pathFinding.PopNode();
					Transform nextWayPoint2 = pathFinding.GetNextWayPoint(enemyTransform.position, player.GetTransform().position);
					if (nextWayPoint2 != null)
					{
						lastTarget = nextWayPoint2.position;
					}
				}
			}
			enemyTransform.LookAt(new Vector3(lastTarget.x, enemyTransform.position.y, lastTarget.z));
			moveDirection = (lastTarget - enemyTransform.position).normalized;
		}

		public bool MultiplayerDistanceChack()
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop && GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master)
			{
				float magnitude = (this.player.GetTransform().position - enemyTransform.position).magnitude;
				Player player = null;
				foreach (Player item in GameApp.GetInstance().GetGameScene().m_multi_player_arr)
				{
					float magnitude2 = (item.GetTransform().position - enemyTransform.position).magnitude;
					if (magnitude2 < mAttributes.detectionRange && magnitude2 < magnitude && item.m_multi_id != this.player.m_multi_id)
					{
						Ray ray = new Ray(enemyTransform.position + Vector3.up * 0.5f, item.GetTransform().position - enemyTransform.position);
						RaycastHit hitInfo;
						if (!Physics.Raycast(ray, out hitInfo, magnitude2, 67209216))
						{
							magnitude = (item.GetTransform().position - enemyTransform.position).magnitude;
							player = item;
						}
					}
				}
				if (player != null)
				{
					this.player = player;
					lastTarget = this.player.GetTransform().position;
					pathFinding.ClearPath();
					Packet packet = CGEnemyChangeTargetPacket.MakePacket(name, (uint)this.player.m_multi_id);
					GameApp.GetInstance().GetGameState().net_com.Send(packet);
					return true;
				}
			}
			return false;
		}

		public void ResetRunSpeedTimer()
		{
			if (is_RunSlow && Time.time - runSlowTime > 3f)
			{
				mAttributes.MoveSpeed /= 0.5f;
				animation[runAnimationName].speed /= 0.5f;
				is_RunSlow = false;
			}
		}

		public void RemoveDeadbodyCheck()
		{
			gameScene.GetEnemies().Remove(enemyObject.name);
			if (DestroyAfterDead)
			{
				if (enemyObject != null)
				{
					UnityEngine.Object.Destroy(enemyObject);
				}
				return;
			}
			if (PreyTip != null)
			{
				UnityEngine.Object.Destroy(PreyTip);
			}
			enemyObject.SetActive(false);
		}

		public void RemovePreyEnemy()
		{
			GameObject mucilage_M = GameApp.GetInstance().GetGameResourceConfig().mucilage_M;
			UnityEngine.Object.Instantiate(mucilage_M, new Vector3(enemyTransform.position.x, enemyTransform.position.y + 0.1f, enemyTransform.position.z), Quaternion.identity);
			SetInPreyGone(true);
		}

		public virtual void OnDead()
		{
			gameScene.EnemyKills++;
			GameApp.GetInstance().GetGameState().Achievement.KillEnemy();
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				if (is_multi_dead_killed)
				{
					if (IsSuperBoss)
					{
						(gameScene as GameMultiplayerScene).OnGameWin();
					}
					else
					{
						enemyObject.SendMessage("OnLoot", IsPrey);
					}
					GameApp.GetInstance().GetGameState().AddAvatarExp(mAttributes.ExpLoot);
					GameApp.GetInstance().GetGameState().AddCashForRecord(mAttributes.Loot);
				}
			}
			else
			{
				enemyObject.SendMessage("OnLoot", IsPrey);
				if (IsSuperBoss && GameApp.GetInstance().GetGameState().gameMode == GameMode.SoloBoss)
				{
					enemyObject.SendMessage("OnSuperBossLoot");
				}
				GameApp.GetInstance().GetGameState().AddAvatarExp(mAttributes.ExpLoot);
				GameApp.GetInstance().GetGameState().AddCashForRecord(mAttributes.Loot);
			}
			if (enemyType == EnemyType.E_DOG || enemyType == EnemyType.E_HELL_FIRER || enemyType == EnemyType.E_POLICE || enemyType == EnemyType.E_VELOCI || enemyType == EnemyType.E_DILO)
			{
				criticalAttacked = false;
			}
			else if (enemyType == EnemyType.E_BOOMER)
			{
				criticalAttacked = true;
			}
			else if (IsElite)
			{
				criticalAttacked = false;
			}
			deadRotation = Quaternion.identity;
			deadPosition = enemyTransform.position;
			deadPosition.y = 10000.119f;
			if (enemyTransform.position.y > 10000.6f)
			{
				Ray ray = new Ray(enemyTransform.position + Vector3.up * 0.5f, -Vector3.up);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, 50f, 32768))
				{
					deadRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
					deadPosition = hitInfo.point + Vector3.up * 0.01f;
				}
			}
			if (criticalAttacked)
			{
				PlayDeadEffects();
			}
			else
			{
				if ((bool)animation)
				{
					animation["Death01"].wrapMode = WrapMode.ClampForever;
					animation["Death01"].speed = 1f;
					if (enemyType == EnemyType.E_DOG || enemyType == EnemyType.E_POLICE)
					{
						animation.Stop();
						animation.Play("Death01");
					}
					else
					{
						animation.CrossFade("Death01");
					}
				}
				if ((bool)enemyObject && enemyObject.activeInHierarchy)
				{
					enemyTransform.rotation = deadRotation * enemyTransform.rotation;
					enemyObject.layer = 18;
				}
				PlayBloodEffect();
			}
			CheckPreyEnemyDeath();
			RemoveEnemyMark();
		}

		public void CheckPreyEnemyDeath()
		{
			if (IsPrey)
			{
				enemyObject.SendMessage("OnSuperBossLoot");
				GameApp.GetInstance().GetGameState().Hunting_val = 1;
				GameApp.GetInstance().PlayerPrefsSave();
				UnityEngine.Object.Destroy(GameApp.GetInstance().GetGameScene().GameGUI.huntingTimer);
			}
		}

		public void CreatePreyTip()
		{
			if (IsPrey || IsSuperBoss)
			{
				PreyTip = UnityEngine.Object.Instantiate(GameApp.GetInstance().GetResourceConfig().PreyTip, enemyTransform.TransformPoint(Vector3.up * m_tip_height), Quaternion.identity) as GameObject;
				PreyTip.transform.parent = enemyTransform;
			}
		}

		public virtual EnemyState EnterSpecialState(float deltaTime)
		{
			return null;
		}

		public virtual void DoMove(float deltaTime)
		{
			enemyTransform.Translate(moveDirection * mAttributes.MoveSpeed * deltaTime, Space.World);
		}

		public virtual void DoLogic(float deltaTime)
		{
			if (player != null && !(player.GetTransform() == null))
			{
				CheckMultiRot();
				state.NextState(this, deltaTime, player);
				RemoveExceptionPositionEnemy();
				ResetRunSpeedTimer();
				if (Time.time - last_enemy_status_time >= 0.2f)
				{
					SendNetEnemyStatusMsg();
					last_enemy_status_time = Time.time;
				}
			}
		}

		public void SendNetEnemyStatusMsg()
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop && GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master && (last_pos != enemyTransform.position || last_rot != enemyTransform.rotation.eulerAngles || last_dir != moveDirection))
			{
				Packet packet = CGEnemyStatusPacket.MakePacket(name, enemyTransform.position, enemyTransform.rotation.eulerAngles, moveDirection);
				GameApp.GetInstance().GetGameState().net_com.Send(packet);
				last_pos = enemyTransform.position;
				last_rot = enemyTransform.rotation.eulerAngles;
				last_dir = moveDirection;
			}
		}

		public void SetNetEnemyStatus(Vector3 direction, Vector3 rotation, Vector3 position)
		{
			moveDirection = direction;
			if (Vector3.Distance(enemyTransform.position, position) >= mAttributes.MoveSpeed * 0.2f * 2f)
			{
				pos_to = position;
				m_is_lerp_position = true;
			}
			else
			{
				m_is_lerp_position = false;
			}
			rot_to = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
			CheckMultiStatus();
		}

		public void CheckMultiRot()
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop && !GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master)
			{
				enemyTransform.rotation = Quaternion.Lerp(enemyTransform.rotation, rot_to, Time.deltaTime * 10f);
			}
		}

		public void CheckMultiStatus()
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop && !GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master)
			{
				enemyTransform.rotation = Quaternion.Lerp(enemyTransform.rotation, rot_to, Time.deltaTime * 10f);
				if (m_is_lerp_position)
				{
					Vector3 translation = pos_to - enemyTransform.position;
					enemyTransform.Translate(translation, Space.World);
					m_is_lerp_position = false;
				}
			}
		}

		protected void RemoveExceptionPositionEnemy()
		{
			if (enemyTransform.position.y < 9980.1f)
			{
				DamageProperty damageProperty = new DamageProperty();
				damageProperty.damage = (float)(mAttributes.Hp + 10.0);
				OnHit(damageProperty, WeaponType.NoGun, false, null);
			}
		}
	}
}
