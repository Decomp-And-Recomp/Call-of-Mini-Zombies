using UnityEngine;

namespace Zombie3D
{
	public class GrenadeRifle : Weapon
	{
		protected const float shootLastingTime = 0.5f;

		protected float rocketFlySpeed;

		private ParticleEmitter m32Spark;

		private ParticleSystem longinusSpark;

		public GrenadeRifle()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = false;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.M32;
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
			rocketFlySpeed = base.WConf.flySpeed;
			mAttributes.hitForce = 30f;
			if (base.Name == "M32")
			{
				base.WeaponBulletName = "Bullet_M32";
			}
			else if (base.Name == "Longinus-Silver")
			{
				base.WeaponBulletName = "Bullet_ElectricGun";
			}
		}

		public override void Init()
		{
			base.Init();
			fire_ori = gun.transform.Find("fire_ori").gameObject;
			if (base.Name == "M32")
			{
				base.WeaponBulletObject = rConf.itemM32;
				m32Spark = gun.transform.Find("GunSpark").GetComponent<ParticleEmitter>();
				m32Spark.emit = false;
			}
			else if (base.Name == "Longinus-Silver")
			{
				base.WeaponBulletObject = rConf.itemElectricGun;
				longinusSpark = gun.transform.Find("GunSpark").GetComponent<ParticleSystem>();
				longinusSpark.Stop(true);
			}
		}

		public override void Fire(float deltaTime)
		{
			Vector3 vector = cameraComponent.ScreenToWorldPoint(new Vector3(gameCamera.ReticlePosition.x, (float)Screen.height - gameCamera.ReticlePosition.y, 50f));
			Ray ray = new Ray(cameraTransform.position, vector - cameraTransform.position);
			RaycastHit hitInfo;
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				if (Physics.Raycast(ray, out hitInfo, 1000f, 35072))
				{
					aimTarget = hitInfo.point;
				}
				else
				{
					aimTarget = cameraTransform.TransformPoint(0f, 0f, 1000f);
				}
			}
			else if (Physics.Raycast(ray, out hitInfo, 1000f, 35328))
			{
				aimTarget = hitInfo.point;
			}
			else
			{
				aimTarget = cameraTransform.TransformPoint(0f, 0f, 1000f);
			}
			float num = Vector3.Dot(cameraTransform.position - aimTarget, fire_ori.transform.position - aimTarget);
			Vector3 vector2 = ((!(num <= 0f)) ? (aimTarget - fire_ori.transform.position).normalized : (vector - fire_ori.transform.position).normalized);
			if (base.Name == "M32")
			{
				m32Spark.emit = true;
			}
			else if (base.Name == "Longinus-Silver")
			{
				longinusSpark.Play(true);
			}
			GameObject gameObject = Object.Instantiate((!(base.Name == "M32")) ? rConf.longinusSilverBullet : rConf.m32tile, fire_ori.transform.position, Quaternion.LookRotation(vector2)) as GameObject;
			ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
			component.dir = vector2;
			component.flySpeed = rocketFlySpeed;
			component.explodeRadius = mAttributes.range;
			component.hitObjectEffect = ((!(base.Name == "M32")) ? rConf.longinusSilverExplosion : rConf.rocketExlposion);
			component.hitForce = mAttributes.hitForce;
			component.life = 8f;
			component.damage = player.Damage;
			component.GunType = WeaponType.M32;
			component.player = player;
			component.targetPos = new Ray(fire_ori.transform.position, vector2).GetPoint(100f);
			TPSSimpleCameraScript tPSSimpleCameraScript = (TPSSimpleCameraScript)gameCamera;
			float num2 = (tPSSimpleCameraScript.angelV - tPSSimpleCameraScript.minAngelV) / (tPSSimpleCameraScript.maxAngelV - tPSSimpleCameraScript.minAngelV);
			component.initAngel = 10f + num2 * 30f;
			component.targetPos.y = 10000f;
			component.angelDelta = 80f;
			component.GetComponent<AudioSource>().mute = !GameApp.GetInstance().GetGameState().SoundOn;
			lastShootTime = Time.time;
			ConsumeBullet(1);
		}

		public override void StopFire()
		{
			if (m32Spark != null)
			{
				m32Spark.emit = false;
			}
			if (longinusSpark != null)
			{
				longinusSpark.Stop(true);
				longinusSpark.Clear(true);
			}
		}

		public override void ConsumeBullet(int count)
		{
			sbulletCount -= count;
			sbulletCount = Mathf.Clamp(sbulletCount, 0, maxCapacity);
		}
	}
}
