using UnityEngine;

namespace Zombie3D
{
	public class SuperDinosaur : Enemy
	{
		public enum SpecialState
		{
			None = 0,
			Rush = 1,
			Dash = 2
		}

		private const double hpFactor1 = 0.6;

		private const double hpFactor2 = 0.3;

		public static EnemyState RUSHINGSTART_STATE = new RushingStartState();

		public static EnemyState RUSHING_STATE = new RushingState();

		public static EnemyState RUSHINGATTACK_STATE = new RushingAttackState();

		public static EnemyState ROAR_SATE = new RoarState();

		private Collider headCollider;

		private GameObject rushEffect;

		private float rushSpeed;

		private float rushDamage;

		private float dashSpeed;

		private float dashDamage;

		private float dashRange;

		private float rushRange;

		private float lastRushTime;

		private double maxHp;

		private string attackAnimation = string.Empty;

		public SpecialState specialState;

		private Vector3 rushingTarget;

		private bool startAttacking;

		private bool startRush;

		private int tinyDinoCalled;

		public override void Init(GameObject gObject)
		{
			m_tip_height = 4.5f;
			base.Init(gObject);
			headCollider = enemyTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Neck/Bip01 Neck1/Bip01 Head/Bip01 HeadNub").gameObject.GetComponent<Collider>();
			controller = enemyTransform.GetComponent<Collider>() as CharacterController;
			aimedTransform = enemyTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Neck/Bip01 Neck1/Bip01 Head");
			MonsterConfig monsterConfig = gConfig.GetMonsterConfig("SuperDino");
			ComputeAttributes(monsterConfig);
			rushSpeed = monsterConfig.rushSpeed;
			rushDamage = monsterConfig.rushDamage * gameScene.GetEnemyAttributesComputingFactor("damage");
			dashSpeed = monsterConfig.dashSpeed;
			dashDamage = monsterConfig.dashDamage;
			mAttributes.attackRange = 2f;
			dashRange = 10f;
			rushRange = 15f;
			lastRushTime = 0f;
			specialState = SpecialState.None;
			runAnimationName = "Run";
			maxHp = mAttributes.Hp;
			rushEffect = Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().superDinoRushEffect, Vector3.zero, Quaternion.identity) as GameObject;
			rushEffect.transform.parent = enemyTransform;
			rushEffect.transform.localPosition = new Vector3(0f, 1.2f, 6f);
			rushEffect.transform.localRotation = Quaternion.identity;
			rushEffect.SetActive(false);
			audio.AddAudio(enemyTransform.Find("Audio"), "Dash", true);
		}

		public override void DoMove(float deltaTime)
		{
			Vector3 vector = enemyTransform.TransformDirection(Vector3.forward);
			vector += Physics.gravity * 0.5f;
			controller.Move(vector * mAttributes.MoveSpeed * deltaTime);
		}

		public override void OnHit(DamageProperty dp, WeaponType weaponType, bool criticalAttack, Player mPlayer)
		{
			if (state == Enemy.GRAVEBORN_STATE)
			{
				return;
			}
			if (mPlayer != null && mPlayer.AvatarType == AvatarType.Pastor)
			{
				OnPastorAffect();
			}
			Object.Instantiate(rConfig.hitBlood, enemyTransform.position + Vector3.up * 1.5f, Quaternion.identity);
			if ((mAttributes.Hp >= maxHp * 0.6 && mAttributes.Hp - (double)dp.damage < maxHp * 0.6) || (mAttributes.Hp >= maxHp * 0.3 && mAttributes.Hp - (double)dp.damage < maxHp * 0.3))
			{
				state.OnHit(this);
			}
			mAttributes.Hp -= dp.damage;
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
					int bElite = (base.IsElite ? 1 : 0);
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

		public override void OnMultiHit(double dp, WeaponType weaponType, int criticalAttack)
		{
			if (state != Enemy.GRAVEBORN_STATE)
			{
				Object.Instantiate(rConfig.hitBlood, enemyTransform.position + Vector3.up * 1.5f, Quaternion.identity);
				criticalAttacked = criticalAttack == 1;
				if ((mAttributes.Hp >= maxHp * 0.6 && mAttributes.Hp - dp < maxHp * 0.6) || (mAttributes.Hp >= maxHp * 0.3 && mAttributes.Hp - dp < maxHp * 0.3))
				{
					state.OnHit(this);
				}
				mAttributes.Hp -= dp;
			}
		}

		public override bool AttackAnimationEnds()
		{
			if (Time.time - lastAttackTime > animation[attackAnimation].clip.length)
			{
				return true;
			}
			return false;
		}

		public override void OnAttack()
		{
			base.OnAttack();
			attackAnimation = ((Random.Range(0, 100) >= 50) ? "Attack02" : "Attack01");
			Animate(attackAnimation, WrapMode.ClampForever);
			lastAttackTime = Time.time;
			startAttacking = true;
		}

		public override void CheckHit()
		{
			if (startAttacking && animation[attackAnimation].time > animation[attackAnimation].clip.length * 0.6f)
			{
				if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Coop)
				{
					Collider collider = player.Collider;
					if (collider != null && headCollider.bounds.Intersects(collider.bounds))
					{
						player.OnHit(mAttributes.AttackDamage);
					}
				}
				else
				{
					foreach (Player item in GameApp.GetInstance().GetGameScene().m_multi_player_arr)
					{
						Collider collider2 = item.Collider;
						if (collider2 != null && headCollider.bounds.Intersects(collider2.bounds))
						{
							item.OnHit(mAttributes.AttackDamage);
							break;
						}
					}
				}
				startAttacking = false;
			}
			base.CheckHit();
		}

		public override void OnDead()
		{
			audio.PlayAudio("Dead");
			PlayBloodEffect();
			animation["Death01"].wrapMode = WrapMode.ClampForever;
			animation.CrossFade("Death01");
			enemyObject.layer = 18;
			gameScene.EnemyKills++;
			GameApp.GetInstance().GetGameState().Achievement.KillEnemy();
			GameApp.GetInstance().GetGameState().Achievement.CheckAchievemnet_BraveHeart();
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				if (is_multi_dead_killed)
				{
					(gameScene as GameMultiplayerScene).OnGameWin();
					GameApp.GetInstance().GetGameState().AddAvatarExp(mAttributes.ExpLoot);
					GameApp.GetInstance().GetGameState().AddCashForRecord(mAttributes.Loot);
				}
			}
			else
			{
				enemyObject.SendMessage("OnLoot", false);
				enemyObject.SendMessage("OnSuperBossLoot");
				GameApp.GetInstance().GetGameState().AddAvatarExp(mAttributes.ExpLoot);
				GameApp.GetInstance().GetGameState().AddCashForRecord(mAttributes.Loot);
			}
			RemoveEnemyMark();
		}

		public bool Rush(float deltaTime)
		{
			if (specialState == SpecialState.Dash)
			{
				enemyTransform.Translate(moveDirection * dashSpeed * deltaTime, Space.World);
				if (player.Collider != null && CouldMakeNextAttack() && headCollider.bounds.Intersects(player.Collider.bounds))
				{
					player.OnHit(dashDamage);
					lastAttackTime = Time.time;
				}
				if ((enemyTransform.position - rushingTarget).sqrMagnitude < 8f || Time.time - state.startTime > 2f)
				{
					Animate("Rush03", WrapMode.ClampForever);
					SetState(RUSHINGATTACK_STATE);
					return true;
				}
			}
			else if (specialState == SpecialState.Rush)
			{
				rushingTarget = new Vector3(player.GetTransform().position.x, enemyTransform.position.y, player.GetTransform().position.z);
				enemyTransform.LookAt(rushingTarget);
				moveDirection = (rushingTarget - enemyTransform.position).normalized;
				enemyTransform.Translate(moveDirection * rushSpeed * deltaTime, Space.World);
				if (startRush)
				{
					startRush = false;
					rushEffect.SetActive(true);
				}
				if (player.Collider != null)
				{
					if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Coop)
					{
						if (headCollider.bounds.Intersects(player.Collider.bounds))
						{
							if (player.GetWeapon().GetWeaponType() != WeaponType.SuicideGun || (player.GetWeapon().GetWeaponType() == WeaponType.SuicideGun && !player.IsPlayingAnimation("Shoot01_SuicideGun")))
							{
								player.SetState(PlayerStateType.GotRushHit);
								(player.GetPlayerState() as PlayerGotRushForceState).force = new Vector2(moveDirection.x, moveDirection.z);
							}
							player.OnHit(rushDamage);
							lastAttackTime = Time.time;
							SetState(Enemy.IDLE_STATE);
							rushEffect.SetActive(false);
							animation.Stop();
							return true;
						}
					}
					else if (CouldMakeNextAttack() && headCollider.bounds.Intersects(player.Collider.bounds))
					{
						player.OnHit(dashDamage);
						lastAttackTime = Time.time;
					}
				}
				if ((enemyTransform.position - rushingTarget).sqrMagnitude < 8f || Time.time - state.startTime > 2f)
				{
					SetState(Enemy.IDLE_STATE);
					rushEffect.SetActive(false);
					animation.Stop();
					return true;
				}
			}
			return false;
		}

		public bool RushAttack(float deltaTime)
		{
			if (player.Collider != null && CouldMakeNextAttack() && IsAnimationPlayedPercentage("Rush03", 0.3f) && headCollider.bounds.Intersects(player.Collider.bounds))
			{
				player.OnHit(dashDamage);
				lastAttackTime = Time.time;
			}
			if (IsAnimationPlayedPercentage("Rush03", 1f))
			{
				return true;
			}
			return false;
		}

		public bool EnterRoarState()
		{
			if (mAttributes.Hp < maxHp * 0.6)
			{
				Debug.Log("enter roar state");
				tinyDinoCalled = 0;
				return true;
			}
			return false;
		}

		public void CallTinyDino()
		{
			if (tinyDinoCalled < 5)
			{
				Vector3 vector = enemyTransform.position - moveDirection.normalized * 5f;
				float x = Random.Range(vector.x - 5f, vector.x + 5f);
				float z = Random.Range(vector.z - 5f, vector.z + 5f);
				Vector3 position = new Vector3(x, Constant.FloorHeight_Real(enemyTransform.position), z);
				bool isElite = Random.Range(0, 100) >= 75;
				EnemyType enermyType = ((Random.Range(0, 10) >= 5) ? EnemyType.E_DILO : EnemyType.E_VELOCI);
				EnermyFactory.SpawnEnemy(gameScene.GetNextEnemyID(), isElite, false, false, enermyType, SpawnFromType.Grave, position);
				tinyDinoCalled++;
			}
		}

		public override EnemyState EnterSpecialState(float deltaTime)
		{
			if (base.SqrDistanceFromPlayer < mAttributes.attackRange * mAttributes.attackRange)
			{
				attackAnimation = ((Random.Range(0, 100) >= 50) ? "Attack02" : "Attack01");
				return Enemy.ATTACK_STATE;
			}
			if (base.SqrDistanceFromPlayer < dashRange * dashRange)
			{
				if (mAttributes.Hp >= maxHp * 0.6)
				{
					return (Random.Range(0, 100) >= 80) ? null : StartDash();
				}
				return (Random.Range(0, 100) >= 50) ? StartRush() : StartDash();
			}
			if (base.SqrDistanceFromPlayer < rushRange * rushRange)
			{
				if (mAttributes.Hp < maxHp * 0.3)
				{
					return StartRush();
				}
				if (mAttributes.Hp < maxHp * 0.6)
				{
					return (Random.Range(0, 100) >= 50) ? null : StartDash();
				}
			}
			return null;
		}

		private EnemyState StartDash()
		{
			rushingTarget = new Vector3(player.GetTransform().position.x, enemyTransform.position.y, player.GetTransform().position.z);
			Vector3 normalized = (rushingTarget - enemyTransform.position).normalized;
			float magnitude = (rushingTarget - enemyTransform.position).magnitude;
			Ray ray = new Ray(enemyTransform.position + new Vector3(0f, 0.5f, 0f), normalized);
			RaycastHit hitInfo;
			if (!Physics.Raycast(ray, out hitInfo, magnitude, 4261888))
			{
				enemyTransform.LookAt(rushingTarget);
				moveDirection = (rushingTarget - enemyTransform.position).normalized;
				specialState = SpecialState.Dash;
				Animate("Rush01", WrapMode.ClampForever);
				return RUSHINGSTART_STATE;
			}
			return null;
		}

		private EnemyState StartRush()
		{
			if (Time.time - lastRushTime > 5f)
			{
				rushingTarget = new Vector3(player.GetTransform().position.x, enemyTransform.position.y, player.GetTransform().position.z);
				Vector3 normalized = (rushingTarget - enemyTransform.position).normalized;
				float magnitude = (rushingTarget - enemyTransform.position).magnitude;
				Ray ray = new Ray(enemyTransform.position + new Vector3(0f, 0.5f, 0f), normalized);
				RaycastHit hitInfo;
				if (!Physics.Raycast(ray, out hitInfo, magnitude, 4261888))
				{
					enemyTransform.LookAt(rushingTarget);
					specialState = SpecialState.Rush;
					Animate("Ready", WrapMode.ClampForever);
					startRush = true;
					lastRushTime = Time.time;
					return RUSHINGSTART_STATE;
				}
			}
			return null;
		}

		public override void Animate(string animationName, WrapMode wrapMode)
		{
			animation[animationName].wrapMode = wrapMode;
			if (animation.IsPlaying("Damage") && animationName == "Roar")
			{
				animation.CrossFade(animationName);
			}
			else if (!animation.IsPlaying("Damage") && !animation.IsPlaying("Roar"))
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
	}
}
