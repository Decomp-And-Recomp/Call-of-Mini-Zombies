using UnityEngine;

namespace Zombie3D
{
	public class Tank : Enemy
	{
		public static EnemyState RUSHINGSTART_STATE = new RushingStartState();

		public static EnemyState RUSHING_STATE = new RushingState();

		public static EnemyState RUSHINGATTACK_STATE = new RushingAttackState();

		protected Collider handCollider;

		protected bool startAttacking;

		protected float rushingInterval;

		protected float rushingSpeed;

		protected float rushingDamage;

		protected float rushingAttackDamage;

		protected float lastRushingTime;

		protected Vector3 rushingTarget;

		protected bool rushingCollided;

		protected bool rushingAttacked;

		protected Collider leftHandCollider;

		public override void Init(GameObject gObject)
		{
			m_tip_height = 4.5f;
			base.Init(gObject);
			enemyTransform.Translate(Vector3.up * 2f);
			handCollider = enemyTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand").GetComponent<Collider>();
			leftHandCollider = enemyTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm/Bip01 L Forearm/Bip01 L Hand").GetComponent<Collider>();
			controller = enemyTransform.GetComponent<Collider>() as CharacterController;
			lastTarget = Vector3.zero;
			mAttributes.attackRange = 2f;
			lastRushingTime = -99f;
			startAttacking = false;
			rushingCollided = false;
			rushingAttacked = false;
			MonsterConfig monsterConfig = gConfig.GetMonsterConfig("Tank");
			ComputeAttributes(monsterConfig);
			rushingInterval = monsterConfig.rushInterval;
			rushingSpeed = monsterConfig.rushSpeed;
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop && IsSuperBoss)
			{
				rushingDamage = mAttributes.AttackDamage;
				rushingAttackDamage = mAttributes.AttackDamage;
				return;
			}
			rushingDamage = monsterConfig.rushDamage * gameScene.GetEnemyAttributesComputingFactor("damage");
			rushingAttackDamage = monsterConfig.rushAttackDamage * gameScene.GetEnemyAttributesComputingFactor("damage");
			if (base.IsElite)
			{
				rushingDamage *= 1.2f;
				rushingAttackDamage *= 1.2f;
			}
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
			mAttributes.Hp -= dp.damage;
			if (Random.Range(0, 100) < 10 && Time.time - lastGotHitTime > 2f)
			{
				lastGotHitTime = Time.time;
				state.OnHit(this);
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

		public override void OnAttack()
		{
			base.OnAttack();
			Animate("Attack01", WrapMode.ClampForever);
			startAttacking = true;
			lastAttackTime = Time.time;
		}

		public override void CheckHit()
		{
			if (startAttacking && animation["Attack01"].time > animation["Attack01"].clip.length * 0.8f)
			{
				if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Coop)
				{
					Collider collider = player.Collider;
					if (collider != null && handCollider.bounds.Intersects(collider.bounds))
					{
						player.OnHit(mAttributes.AttackDamage);
					}
				}
				else
				{
					foreach (Player item in GameApp.GetInstance().GetGameScene().m_multi_player_arr)
					{
						Collider collider2 = item.Collider;
						if (collider2 != null && handCollider.bounds.Intersects(collider2.bounds))
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
				GameApp.GetInstance().GetGameState().AddAvatarExp(mAttributes.ExpLoot);
				GameApp.GetInstance().GetGameState().AddCashForRecord(mAttributes.Loot);
			}
			CheckPreyEnemyDeath();
			RemoveEnemyMark();
		}

		public bool Rush(float deltaTime)
		{
			Collider collider = player.Collider;
			enemyTransform.LookAt(rushingTarget);
			Vector3 vector = enemyTransform.TransformDirection(Vector3.forward) * rushingSpeed + Physics.gravity * 0.5f;
			controller.Move(vector * deltaTime);
			if (!rushingCollided && collider != null)
			{
				Vector3 vector2 = enemyTransform.InverseTransformPoint(player.GetTransform().position);
				if (vector2.sqrMagnitude < 25f && vector2.z > 1f)
				{
					player.OnHit(rushingDamage);
					lastAttackTime = Time.time;
					rushingCollided = true;
				}
			}
			foreach (Enemy value in gameScene.GetEnemies().Values)
			{
				if (value.GetState() != Enemy.DEAD_STATE && value.EnemyType != EnemyType.E_TANK && enemyTransform.GetComponent<Collider>().bounds.Intersects(value.GetCollider().bounds))
				{
					DamageProperty damageProperty = new DamageProperty();
					damageProperty.damage = rushingDamage;
					value.OnHit(damageProperty, WeaponType.NoGun, true, null);
				}
			}
			GameObject[] woodBoxes = gameScene.GetWoodBoxes();
			GameObject[] array = woodBoxes;
			foreach (GameObject gameObject in array)
			{
				if (gameObject != null && enemyTransform.GetComponent<Collider>().bounds.Intersects(gameObject.GetComponent<Collider>().bounds))
				{
					WoodBoxScript component = gameObject.GetComponent<WoodBoxScript>();
					component.OnHit(mAttributes.AttackDamage);
				}
			}
			if ((enemyTransform.position - rushingTarget).sqrMagnitude < 1f || (enemyTransform.position - player.GetTransform().position).sqrMagnitude < 4f || Time.time - lastRushingTime > 3f)
			{
				animation["Rush03"].wrapMode = WrapMode.ClampForever;
				animation.CrossFade("Rush03");
				return true;
			}
			return false;
		}

		public bool RushAttack(float deltaTime)
		{
			Collider collider = player.Collider;
			if (!rushingAttacked && animation["Rush03"].time >= animation["Rush03"].clip.length * 0.3f)
			{
				if (collider != null && (enemyTransform.GetComponent<Collider>().bounds.Intersects(collider.bounds) || leftHandCollider.bounds.Intersects(collider.bounds)))
				{
					player.OnHit(rushingAttackDamage);
					lastAttackTime = Time.time;
				}
				rushingAttacked = true;
			}
			if (rushingAttacked && IsAnimationPlayedPercentage("Rush03", 1f))
			{
				rushingAttacked = false;
				return true;
			}
			return false;
		}

		public override EnemyState EnterSpecialState(float deltaTime)
		{
			if (Time.time - lastRushingTime > rushingInterval && enemyTransform.position.y < 10000.3f)
			{
				rushingTarget = new Vector3(player.GetTransform().position.x, enemyTransform.position.y, player.GetTransform().position.z);
				Vector3 normalized = (rushingTarget - enemyTransform.position).normalized;
				rushingTarget += normalized * 5f;
				lastRushingTime = Time.time;
				float magnitude = (rushingTarget - enemyTransform.position).magnitude;
				Ray ray = new Ray(enemyTransform.position + new Vector3(0f, 0.5f, 0f), normalized);
				RaycastHit hitInfo;
				if (!Physics.Raycast(ray, out hitInfo, magnitude, 4261888))
				{
					enemyTransform.LookAt(rushingTarget);
					rushingCollided = false;
					animation["Rush01"].wrapMode = WrapMode.ClampForever;
					animation["Rush01"].speed = 2f;
					animation.CrossFade("Rush01");
					return RUSHINGSTART_STATE;
				}
			}
			return null;
		}

		public override void DoMove(float deltaTime)
		{
			Vector3 vector = enemyTransform.TransformDirection(Vector3.forward);
			vector += Physics.gravity * 0.5f;
			controller.Move(vector * mAttributes.MoveSpeed * deltaTime);
		}
	}
}
