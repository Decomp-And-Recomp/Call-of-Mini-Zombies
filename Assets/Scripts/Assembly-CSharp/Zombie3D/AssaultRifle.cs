using UnityEngine;

namespace Zombie3D
{
	public class AssaultRifle : Weapon
	{
		protected ObjectPool bulletsObjectPool;

		protected ObjectPool firelineObjectPool;

		protected ObjectPool sparksObjectPool;

		protected float lastPlayAudioTime;

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

		public AssaultRifle()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = false;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.AssaultRifle;
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
			base.WeaponBulletName = "Bullet_AssaultRifle";
		}

		public override void Init()
		{
			base.Init();
			lockAreaRect = AutoRect.AutoPos(new Rect(460f, 300f, 40f, 40f));
			mAttributes.hitForce = 20f;
			base.WeaponBulletObject = rConf.itemAssaultGun;
			gunfire = gun.transform.Find("gun_fire_new").gameObject;
			fire_ori = gun.transform.Find("fire_ori").gameObject;
			bulletsObjectPool = new ObjectPool();
			firelineObjectPool = new ObjectPool();
			sparksObjectPool = new ObjectPool();
			bulletsObjectPool.Init("Bullets", rConf.bullets, 6, 1f);
			firelineObjectPool.Init("Firelines", rConf.fireline, 4, 0.5f);
			sparksObjectPool.Init("Sparks", rConf.hitparticles, 3, 0.22f);
			TimerManager.GetInstance().SetTimer(4, base.AttackFrequency, false);
		}

		public override void DoLogic()
		{
			bulletsObjectPool.AutoDestruct();
			firelineObjectPool.AutoDestruct();
			sparksObjectPool.AutoDestruct();
		}

		public override void Fire(float deltaTime)
		{
			if (gunfire.TryGetComponent(out Renderer gunfireRenderer)) gunfireRenderer.enabled = true;
			else if (gunfire.TryGetComponent(out ParticleSystem gunfireParticles)) gunfireParticles.Play();

			Ray ray = default(Ray);
			Vector3 vector = cameraComponent.ScreenToWorldPoint(new Vector3(gameCamera.ReticlePosition.x, (float)Screen.height - gameCamera.ReticlePosition.y, 0.1f));
			ray = new Ray(cameraTransform.position, vector - cameraTransform.position);
			RaycastHit raycastHit = default(RaycastHit);
			RaycastHit[] array = ((GameApp.GetInstance().GetGameState().gameMode != GameMode.Vs) ? Physics.RaycastAll(ray, 1000f, 559616) : Physics.RaycastAll(ray, 1000f, 559360));
			float num = float.MaxValue;
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 zero = Vector3.zero;
				if (((array[i].collider.gameObject.layer == 9) ? gunfire.transform.InverseTransformPoint(array[i].collider.transform.position) : ((array[i].collider.gameObject.layer != 8) ? gunfire.transform.InverseTransformPoint(array[i].point) : gunfire.transform.InverseTransformPoint(array[i].collider.transform.position))).z < 1f)
				{
					float sqrMagnitude = (array[i].point - gunfire.transform.position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						raycastHit = array[i];
						num = sqrMagnitude;
					}
				}
			}
			if (raycastHit.collider != null)
			{
				aimTarget = raycastHit.point;
				Vector3 normalized = (aimTarget - fire_ori.transform.position).normalized;
				Vector3 vector2 = base.player.GetTransform().InverseTransformPoint(aimTarget);
				if (vector2.z > 2f)
				{
					GameObject gameObject = firelineObjectPool.CreateObject(fire_ori.transform.position + normalized * 2f, normalized);
					gameObject.transform.Rotate(180f, 0f, 0f);
					if (gameObject == null)
					{
						Debug.Log("fire line obj null");
					}
					else
					{
						FireLineScript component = gameObject.GetComponent<FireLineScript>();
						component.transform.Rotate(90f, 0f, 0f);
						component.beginPos = rightGun.position;
						component.endPos = raycastHit.point;
						component.Reset();
					}
				}
				bulletsObjectPool.CreateObject(rightGun.position, normalized);
				GameObject gameObject2 = raycastHit.collider.gameObject;
				if (gameObject2.name.StartsWith("E_"))
				{
					Enemy enemyByID = gameScene.GetEnemyByID(gameObject2.name);
					if (enemyByID.GetState() != Enemy.DEAD_STATE)
					{
						if (vector2.z > 2f)
						{
							sparksObjectPool.CreateObject(raycastHit.point, -ray.direction);
						}
						DamageProperty damageProperty = new DamageProperty();
						damageProperty.hitForce = ray.direction * mAttributes.hitForce;
						damageProperty.damage = base.player.Damage;
						bool criticalAttack = false;
						int num2 = Random.Range(0, 100);
						if (num2 < 70)
						{
							criticalAttack = true;
						}
						float sqrMagnitude2 = (enemyByID.GetPosition() - base.player.GetTransform().position).sqrMagnitude;
						float num3 = mAttributes.range * mAttributes.range;
						if (sqrMagnitude2 < num3)
						{
							enemyByID.OnHit(damageProperty, GetWeaponType(), criticalAttack, base.player);
						}
						else if ((float)num2 < mAttributes.accuracy)
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
					if (vector2.z > 2f)
					{
						sparksObjectPool.CreateObject(raycastHit.point, -ray.direction);
					}
					if (gameObject2.layer == 19)
					{
						WoodBoxScript component2 = gameObject2.GetComponent<WoodBoxScript>();
						if (component2.OnHit(base.player.Damage))
						{
							curEnemyInfo = null;
						}
					}
					else if (gameObject2.layer == 15 && Application.loadedLevelName == "Zombie3D_Village")
					{
						Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().snow_explosion_eff, raycastHit.point + Vector3.up * 1f, Quaternion.identity);
					}
				}
			}
			else
			{
				aimTarget = cameraTransform.TransformPoint(0f, 0f, 1000f);
				Vector3 normalized2 = (aimTarget - fire_ori.transform.position).normalized;
				bulletsObjectPool.CreateObject(rightGun.position, normalized2);
				GameObject gameObject3 = firelineObjectPool.CreateObject(fire_ori.transform.position + normalized2 * 2f, normalized2);
				if (gameObject3 == null)
				{
					Debug.Log("fire line obj null");
				}
				else
				{
					FireLineScript component3 = gameObject3.GetComponent<FireLineScript>();
					component3.transform.Rotate(90f, 0f, 0f);
					component3.beginPos = rightGun.position;
					component3.endPos = raycastHit.point;
					component3.Reset();
				}
			}
			if (!shootAudio.isPlaying)
			{
				AudioPlayer.PlayAudio(shootAudio, true);
			}
			ConsumeBullet(1);
			lastShootTime = Time.time;
		}

		public override void ConsumeBullet(int count)
		{
			sbulletCount -= count;
			sbulletCount = Mathf.Clamp(sbulletCount, 0, maxCapacity);
		}

		public override void StopFire()
		{
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
			if (!TimerManager.GetInstance().Ready(4))
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
						}
					}
				}
				if (curEnemyInfo != null && shootAudio != null)
				{
					AudioPlayer.PlayAudio(shootAudio, true);
				}
			}
			TimerManager.GetInstance().Do(4);
		}
	}
}
