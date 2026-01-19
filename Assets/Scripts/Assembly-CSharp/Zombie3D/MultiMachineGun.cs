using UnityEngine;

namespace Zombie3D
{
	public class MultiMachineGun : Weapon
	{
		protected ObjectPool bulletsObjectPool;

		protected ObjectPool firelineObjectPool;

		protected ObjectPool sparksObjectPool;

		public static Rect lockAreaRect = AutoRect.AutoPos(new Rect(230f, 200f, 500f, 250f));

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

		public MultiMachineGun()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = true;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.MachineGun;
		}

		public override void Init()
		{
			base.MultiInit();
			maxCapacity = 9999;
			mAttributes.hitForce = 20f;
			maxDeflection = 4f;
			gunfire = gun.transform.Find("gun_fire_new").gameObject;
			fire_ori = gun.transform.Find("fire_ori").gameObject;
			bulletsObjectPool = new ObjectPool();
			firelineObjectPool = new ObjectPool();
			sparksObjectPool = new ObjectPool();
			bulletsObjectPool.Init("MultiBullets", rConf.bullets, 6, 1f);
			firelineObjectPool.Init("MultiFirelines", rConf.fireline, 20, 0.5f);
			sparksObjectPool.Init("MultiSparks", rConf.hitparticles, 3, 0.22f);
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
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
			ray = new Ray(fire_ori.transform.position, fire_ori.transform.TransformDirection(Vector3.forward));
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 1000f, 559616))
			{
				aimTarget = hitInfo.point;
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = player.GetTransform().InverseTransformPoint(aimTarget);
				if (vector2.z > 2f)
				{
					for (int i = -2; i <= 2; i++)
					{
						Vector3 zero = Vector3.zero;
						zero = ((i % 2 != 0) ? fire_ori.transform.TransformPoint(Vector3.up * i * 1.5f) : fire_ori.transform.TransformPoint(Vector3.left * i * 1.5f));
						vector = (aimTarget - fire_ori.transform.position).normalized;
						GameObject gameObject = firelineObjectPool.CreateObject(zero + vector * (Mathf.Abs(i) + 2), vector);
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
				bulletsObjectPool.CreateObject(rightGun.position, vector);
				GameObject gameObject2 = hitInfo.collider.gameObject;
				if (gameObject2.name.StartsWith("E_"))
				{
					Enemy enemyByID = gameScene.GetEnemyByID(gameObject2.name);
					if (enemyByID.GetState() != Enemy.DEAD_STATE)
					{
						if (vector2.z > 2f)
						{
							sparksObjectPool.CreateObject(hitInfo.point, -ray.direction);
						}
						DamageProperty damageProperty = new DamageProperty();
						damageProperty.hitForce = ray.direction * mAttributes.hitForce;
						damageProperty.damage = 0f;
						bool criticalAttack = false;
						int num = Random.Range(0, 100);
						if (num < 70)
						{
							criticalAttack = true;
						}
						float sqrMagnitude = (enemyByID.GetPosition() - player.GetTransform().position).sqrMagnitude;
						float num2 = mAttributes.range * mAttributes.range;
						if (sqrMagnitude < num2)
						{
							enemyByID.OnHit(damageProperty, GetWeaponType(), criticalAttack, player);
						}
						else if ((float)num < mAttributes.accuracy)
						{
							enemyByID.OnHit(damageProperty, GetWeaponType(), criticalAttack, player);
						}
					}
				}
				else
				{
					if (vector2.z > 2f)
					{
						sparksObjectPool.CreateObject(hitInfo.point, -ray.direction);
					}
					if (gameObject2.layer == 19)
					{
						WoodBoxScript component2 = gameObject2.GetComponent<WoodBoxScript>();
						component2.OnHit(0f);
					}
				}
				gun.GetComponent<Animation>().GetComponent<Animation>().Play();
				AudioPlayer.PlayAudio(shootAudio, true);
				lastShootTime = Time.time;
			}
			else
			{
				aimTarget = fire_ori.transform.TransformPoint(0f, 0f, 1000f);
			}
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
	}
}
