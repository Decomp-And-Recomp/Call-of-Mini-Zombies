using UnityEngine;

namespace Zombie3D
{
	public class MachineGun : Weapon
	{
		protected ObjectPool bulletsObjectPool;

		protected ObjectPool firelineObjectPool;

		protected ObjectPool sparksObjectPool;

		private Rect lockAreaRect;

		protected AssaultRifleNearestEnemyInfo _curEnemyInfo;

		public AssaultRifleNearestEnemyInfo curEnemyInfo
		{
			get
			{
				return _curEnemyInfo;
			}
			set
			{
				_curEnemyInfo = value;
			}
		}

		public MachineGun()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = false;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.MachineGun;
		}

		public override void Init()
		{
			base.Init();
			lockAreaRect = AutoRect.AutoPos(new Rect(460f, 300f, 40f, 40f));
			mAttributes.hitForce = 20f;
			maxDeflection = 4f;
			base.WeaponBulletObject = rConf.itemGatlin;
			gunfire = gun.transform.Find("gun_fire_new").gameObject;
			fire_ori = gun.transform.Find("fire_ori").gameObject;
			bulletsObjectPool = new ObjectPool();
			firelineObjectPool = new ObjectPool();
			sparksObjectPool = new ObjectPool();
			bulletsObjectPool.Init("Bullets", rConf.bullets, 6, 1f);
			firelineObjectPool.Init("Firelines", rConf.fireline, 20, 0.5f);
			sparksObjectPool.Init("Sparks", rConf.hitparticles, 3, 0.22f);
			TimerManager.GetInstance().SetTimer(5, base.AttackFrequency, false);
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
			base.WeaponBulletName = "Bullet_MachineGun";
		}

		public override void FireUpdate(float deltaTime)
		{
			deflection.x += Random.Range(-0.5f, 0.5f);
			deflection.y += Random.Range(-0.5f, 0.5f);
			deflection.x = Mathf.Clamp(deflection.x, 0f - maxDeflection, maxDeflection);
			deflection.y = Mathf.Clamp(deflection.y, 0f - maxDeflection, maxDeflection);
		}

		public override void Fire(float deltaTime)
		{
			if (gunfire.TryGetComponent(out Renderer gunfireRenderer)) gunfireRenderer.enabled = true;
			else if (gunfire.TryGetComponent(out ParticleSystem gunfireParticles)) gunfireParticles.Play();

			Ray ray = default(Ray);
			Vector3 vector = cameraComponent.ScreenToWorldPoint(new Vector3(gameCamera.ReticlePosition.x, (float)Screen.height - gameCamera.ReticlePosition.y, 50f));
			ray = new Ray(cameraTransform.position, vector - cameraTransform.position);
			bool flag = false;
			RaycastHit hitInfo;
			if ((GameApp.GetInstance().GetGameState().gameMode != GameMode.Vs) ? Physics.Raycast(ray, out hitInfo, 1000f, 559616) : Physics.Raycast(ray, out hitInfo, 1000f, 559360))
			{
				aimTarget = hitInfo.point;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = base.player.GetTransform().InverseTransformPoint(aimTarget);
				if (vector3.z > 2f)
				{
					for (int i = -2; i <= 2; i++)
					{
						Vector3 zero = Vector3.zero;
						zero = ((i % 2 != 0) ? fire_ori.transform.TransformPoint(Vector3.up * i * 1.5f) : fire_ori.transform.TransformPoint(Vector3.left * i * 1.5f));
						vector2 = (aimTarget - fire_ori.transform.position).normalized;
						GameObject gameObject = firelineObjectPool.CreateObject(zero + vector2 * (Mathf.Abs(i) + 2), vector2);
						gameObject.transform.Rotate(180f, 0f, 0f);
						if (gameObject == null)
						{
							Debug.Log("fire line obj null");
							continue;
						}
						FireLineScript component = gameObject.GetComponent<FireLineScript>();
						component.transform.Rotate(90f, 0f, 0f);
						component.beginPos = rightGun.position;
						component.endPos = hitInfo.point;
						component.Reset();
					}
				}
				bulletsObjectPool.CreateObject(rightGun.position, vector2);
				GameObject gameObject2 = hitInfo.collider.gameObject;
				if (gameObject2.name.StartsWith("E_"))
				{
					Enemy enemyByID = gameScene.GetEnemyByID(gameObject2.name);
					if (enemyByID.GetState() != Enemy.DEAD_STATE)
					{
						if (vector3.z > 2f)
						{
							sparksObjectPool.CreateObject(hitInfo.point, -ray.direction);
						}
						DamageProperty damageProperty = new DamageProperty();
						damageProperty.hitForce = ray.direction * mAttributes.hitForce;
						damageProperty.damage = base.player.Damage;
						bool criticalAttack = false;
						int num = Random.Range(0, 100);
						if (num < 70)
						{
							criticalAttack = true;
						}
						float sqrMagnitude = (enemyByID.GetPosition() - base.player.GetTransform().position).sqrMagnitude;
						float num2 = mAttributes.range * mAttributes.range;
						if (sqrMagnitude < num2)
						{
							enemyByID.OnHit(damageProperty, GetWeaponType(), criticalAttack, base.player);
						}
						else if ((float)num < mAttributes.accuracy)
						{
							enemyByID.OnHit(damageProperty, GetWeaponType(), criticalAttack, base.player);
						}
					}
				}
				else if (gameObject2.layer == 8)
				{
					Player player = gameObject2.GetComponent<PlayerShell>().m_player;
					if (player.GetPlayerState() != null && player.GetPlayerState().GetStateType() != PlayerStateType.Dead)
					{
						player.OnVsInjured(base.player.tnet_user, base.player.Damage, (int)GetWeaponType());
					}
				}
				else
				{
					if (vector3.z > 2f)
					{
						sparksObjectPool.CreateObject(hitInfo.point, -ray.direction);
					}
					if (gameObject2.layer == 19)
					{
						WoodBoxScript component2 = gameObject2.GetComponent<WoodBoxScript>();
						component2.OnHit(base.player.Damage);
					}
					else if (gameObject2.layer == 15 && Application.loadedLevelName == "Zombie3D_Village")
					{
						Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().snow_explosion_eff, hitInfo.point + Vector3.up * 1f, Quaternion.identity);
					}
				}
				gun.GetComponent<Animation>().GetComponent<Animation>().Play();
				AudioPlayer.PlayAudio(shootAudio, true);
				ConsumeBullet(1);
				lastShootTime = Time.time;
			}
			else
			{
				aimTarget = cameraTransform.TransformPoint(0f, 0f, 1000f);
			}
		}

		public override void ConsumeBullet(int count)
		{
			sbulletCount -= count;
			sbulletCount = Mathf.Clamp(sbulletCount, 0, maxCapacity);
		}

		public override void DoLogic()
		{
			bulletsObjectPool.AutoDestruct();
			firelineObjectPool.AutoDestruct();
			sparksObjectPool.AutoDestruct();
		}

		public override void StopFire()
		{
			deflection = Vector2.zero;
			if (shootAudio != null)
			{
				shootAudio.Stop();
			}
			if (gunfire != null)
			{
				if (gunfire.TryGetComponent(out Renderer gunfireRenderer)) gunfireRenderer.enabled = false;
				else if (gunfire.TryGetComponent(out ParticleSystem gunfireParticles)) gunfireParticles.Stop();
			}
			if (curEnemyInfo != null)
			{
				curEnemyInfo = null;
			}
			gun.GetComponent<Animation>().GetComponent<Animation>().Stop();
		}

		public override void GunOff()
		{
			base.GunOff();
		}

		public override void AutoAim(float deltaTime)
		{
			if (curEnemyInfo != null && curEnemyInfo.curEnemy != null && curEnemyInfo._type == NearestEnemyType.NearestEnemyType_Enemy && curEnemyInfo.curEnemy.GetState() == Enemy.DEAD_STATE)
			{
				curEnemyInfo = null;
			}
			if (curEnemyInfo != null && curEnemyInfo.transform != null)
			{
				Vector2 vector = cameraComponent.WorldToScreenPoint(curEnemyInfo.transform.position);
				curEnemyInfo.screenPos = new Vector2(vector.x, (float)Screen.height - vector.y);
				curEnemyInfo.currentScreenPos = curEnemyInfo.screenPos;
			}
			if (!TimerManager.GetInstance().Ready(5))
			{
				return;
			}
			if (curEnemyInfo != null && curEnemyInfo.transform != null)
			{
				Vector3 vector2 = cameraComponent.WorldToScreenPoint(curEnemyInfo.transform.position);
				if (vector2.z < 0f || vector2.x < lockAreaRect.xMin || vector2.x > lockAreaRect.xMax || vector2.y < lockAreaRect.yMin || vector2.y > lockAreaRect.yMax)
				{
					if (curEnemyInfo.transform.gameObject.layer == 23)
					{
						Object.Destroy(curEnemyInfo.transform.gameObject);
					}
					curEnemyInfo = null;
				}
			}
			if (curEnemyInfo != null)
			{
				return;
			}
			float num = 99999f;
			foreach (Enemy value in gameScene.GetEnemies().Values)
			{
				if (value.GetState() == Enemy.DEAD_STATE)
				{
					continue;
				}
				Transform aimedTransform = value.GetAimedTransform();
				Vector3 vector3 = cameraComponent.WorldToScreenPoint(aimedTransform.position);
				if (vector3.z < 0f || !(vector3.x > lockAreaRect.xMin) || !(vector3.x < lockAreaRect.xMax) || !(vector3.y > lockAreaRect.yMin) || !(vector3.y < lockAreaRect.yMax))
				{
					continue;
				}
				Ray ray = new Ray(rightGun.position, aimedTransform.position - rightGun.position);
				RaycastHit hitInfo;
				if (!Physics.Raycast(ray, out hitInfo, 1000f, 35328) || hitInfo.collider.gameObject.name.StartsWith("E_"))
				{
					float sqrMagnitude = (rightGun.position - aimedTransform.position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						curEnemyInfo = new AssaultRifleNearestEnemyInfo();
						curEnemyInfo.transform = aimedTransform;
						curEnemyInfo.screenPos = new Vector2(vector3.x, (float)Screen.height - vector3.y);
						curEnemyInfo.distance = sqrMagnitude;
						curEnemyInfo.currentScreenPos = gameScene.GetCamera().ReticlePosition;
						curEnemyInfo.curEnemy = value;
						curEnemyInfo._type = NearestEnemyType.NearestEnemyType_Enemy;
						num = sqrMagnitude;
					}
				}
			}
			if (curEnemyInfo != null)
			{
				if (shootAudio != null)
				{
					AudioPlayer.PlayAudio(shootAudio, true);
				}
			}
			else
			{
				GameObject[] woodBoxes = gameScene.GetWoodBoxes();
				num = 99999f;
				GameObject[] array = woodBoxes;
				foreach (GameObject gameObject in array)
				{
					if (!(gameObject != null))
					{
						continue;
					}
					Transform transform = gameObject.transform;
					Vector3 vector4 = cameraComponent.WorldToScreenPoint(transform.position + Vector3.up * 0.6f);
					if (vector4.z < 0f || !(vector4.x > lockAreaRect.xMin) || !(vector4.x < lockAreaRect.xMax) || !(vector4.y > lockAreaRect.yMin) || !(vector4.y < lockAreaRect.yMax))
					{
						continue;
					}
					Ray ray2 = new Ray(rightGun.position, transform.position + Vector3.up * 0.6f - rightGun.position);
					RaycastHit hitInfo2;
					if (!Physics.Raycast(ray2, out hitInfo2, 1000f, 559104) || hitInfo2.collider.gameObject.layer == 19)
					{
						float sqrMagnitude2 = (rightGun.position - transform.position).sqrMagnitude;
						if (sqrMagnitude2 < num)
						{
							num = sqrMagnitude2;
							curEnemyInfo = new AssaultRifleNearestEnemyInfo();
							GameObject gameObject2 = new GameObject();
							gameObject2.transform.position = transform.position + Vector3.up * 0.6f;
							gameObject2.transform.parent = transform;
							gameObject2.layer = 23;
							curEnemyInfo.transform = gameObject2.transform;
							curEnemyInfo.screenPos = new Vector2(vector4.x, (float)Screen.height - vector4.y);
							curEnemyInfo.distance = sqrMagnitude2;
							curEnemyInfo.currentScreenPos = gameScene.GetCamera().ReticlePosition;
							curEnemyInfo._type = NearestEnemyType.NearestEnemyType_Box;
							curEnemyInfo.curEnemy = null;
							Debug.Log("woodbox" + curEnemyInfo.transform.position);
						}
					}
				}
				if (curEnemyInfo != null && shootAudio != null)
				{
					AudioPlayer.PlayAudio(shootAudio, true);
				}
			}
			TimerManager.GetInstance().Do(5);
		}
	}
}
