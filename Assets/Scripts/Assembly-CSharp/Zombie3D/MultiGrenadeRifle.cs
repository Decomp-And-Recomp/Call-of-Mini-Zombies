using UnityEngine;

namespace Zombie3D
{
	public class MultiGrenadeRifle : Weapon
	{
		protected const float shootLastingTime = 0.5f;

		protected float rocketFlySpeed;

		private ParticleEmitter m32Spark;

		private ParticleSystem longinusSpark;

		public MultiGrenadeRifle()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = true;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.M32;
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
			rocketFlySpeed = base.WConf.flySpeed;
		}

		public override void Init()
		{
			base.MultiInit();
			mAttributes.hitForce = 30f;
			fire_ori = gun.transform.Find("fire_ori").gameObject;
			if (base.Name == "M32")
			{
				m32Spark = gun.transform.Find("GunSpark").GetComponent<ParticleEmitter>();
				m32Spark.emit = false;
			}
			else if (base.Name == "Longinus-Silver")
			{
				longinusSpark = gun.transform.Find("GunSpark").GetComponent<ParticleSystem>();
				longinusSpark.Stop(true);
			}
		}

		public override void Fire(float deltaTime)
		{
			Ray ray = default(Ray);
			ray = new Ray(player.PlayerObject.transform.position + Vector3.up, player.PlayerObject.transform.forward);
			RaycastHit hitInfo;
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				if (Physics.Raycast(ray, out hitInfo, 1000f, 35072))
				{
					aimTarget = hitInfo.point;
				}
				else
				{
					aimTarget = player.PlayerObject.transform.TransformPoint(0f, 0f, 1000f);
				}
			}
			else if (Physics.Raycast(ray, out hitInfo, 1000f, 35328))
			{
				aimTarget = hitInfo.point;
			}
			else
			{
				aimTarget = player.PlayerObject.transform.TransformPoint(0f, 0f, 1000f);
			}
			if (base.Name == "M32")
			{
				m32Spark.emit = true;
			}
			else if (base.Name == "Longinus-Silver")
			{
				longinusSpark.Play(true);
			}
			Vector3 normalized = (aimTarget - fire_ori.transform.position).normalized;
			GameObject gameObject = Object.Instantiate((!(base.Name == "M32")) ? rConf.longinusSilverBullet : rConf.m32tile, fire_ori.transform.position, Quaternion.LookRotation(normalized)) as GameObject;
			ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
			component.dir = normalized;
			component.flySpeed = rocketFlySpeed;
			component.explodeRadius = mAttributes.range;
			component.hitObjectEffect = ((!(base.Name == "M32")) ? rConf.longinusSilverExplosion : rConf.rocketExlposion);
			component.hitForce = mAttributes.hitForce;
			component.life = 8f;
			component.damage = 0f;
			component.GunType = WeaponType.M32;
			component.player = player;
			component.targetPos = new Ray(fire_ori.transform.position, normalized).GetPoint(100f);
			component.initAngel = 25f;
			component.targetPos.y = 10000f;
			component.angelDelta = 80f;
			component.GetComponent<AudioSource>().mute = !GameApp.GetInstance().GetGameState().SoundOn;
			lastShootTime = Time.time;
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
	}
}
