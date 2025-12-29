using UnityEngine;

namespace Zombie3D
{
	internal class MultiElectricGun : Weapon
	{
		protected float rocketFlySpeed;

		protected ParticleSystem gunSpark;

		protected GameObject gunBullet;

		protected AudioSource audio;

		private Vector3 shootDirection = default(Vector3);

		public MultiElectricGun()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = true;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.ElectricGun;
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
			gunSpark = gun.transform.Find("GunSpark").GetComponent<ParticleSystem>();
			gunSpark.Stop(true);
			if (gunSpark.GetComponent<DelayShootBullet>() != null)
			{
				gunSpark.GetComponent<DelayShootBullet>().weapon = this;
				gunSpark.GetComponent<DelayShootBullet>().enabled = false;
			}
			audio = gunSpark.gameObject.GetComponent<AudioSource>();
			audio.Stop();
		}

		public override void Fire(float deltaTime)
		{
			gunSpark.Play();
			if (gunSpark.GetComponent<DelayShootBullet>() != null)
			{
				gunSpark.GetComponent<DelayShootBullet>().enabled = true;
				gunSpark.GetComponent<DelayShootBullet>().startTime = Time.time;
			}
			else
			{
				ReleaseBullet();
			}
			audio.Play();
			audio.mute = !GameApp.GetInstance().GetGameState().SoundOn;
			if (base.Name == "Dragon-Breath")
			{
				gun.GetComponent<Animation>().GetComponent<Animation>().Play();
			}
			lastShootTime = Time.time;
		}

		public override void ReleaseBullet()
		{
			Ray ray = default(Ray);
			ray = new Ray(player.PlayerObject.transform.position + Vector3.up, player.PlayerObject.transform.forward);
			RaycastHit hitInfo;
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				if (Physics.Raycast(ray, out hitInfo, 1000f, 2304))
				{
					aimTarget = hitInfo.point;
				}
				else
				{
					aimTarget = player.PlayerObject.transform.TransformPoint(0f, 0f, 1000f);
				}
			}
			else if (Physics.Raycast(ray, out hitInfo, 1000f, 2560))
			{
				aimTarget = hitInfo.point;
			}
			else
			{
				aimTarget = player.PlayerObject.transform.TransformPoint(0f, 0f, 1000f);
			}
			shootDirection = (aimTarget - fire_ori.transform.position).normalized;
			if (base.Name == "Ion-Cannon")
			{
				gunBullet = Object.Instantiate(rConf.electricGunBullet, fire_ori.transform.position, Quaternion.LookRotation(shootDirection)) as GameObject;
				ProjectileScript component = gunBullet.GetComponent<ProjectileScript>();
				component.dir = shootDirection;
				component.flySpeed = rocketFlySpeed;
				component.explodeRadius = mAttributes.range;
				component.hitObjectEffect = GameApp.GetInstance().GetGameResourceConfig().electricGunExplosion;
				component.hitForce = mAttributes.hitForce;
				component.life = 8f;
				component.damage = 0f;
				component.GunType = WeaponType.ElectricGun;
				component.player = player;
			}
			else if (base.Name == "Dragon-Breath" || base.Name == "Pixel-Cannon")
			{
				gunBullet = Object.Instantiate((!(base.Name == "Pixel-Cannon")) ? rConf.airGunBullet : rConf.pixelAirGunBullet, fire_ori.transform.position, Quaternion.LookRotation(shootDirection)) as GameObject;
				AirGunBulletScript component2 = gunBullet.GetComponent<AirGunBulletScript>();
				component2.dir = shootDirection;
				component2.flySpeed = rocketFlySpeed;
				component2.explodeRadius = mAttributes.range;
				component2.hitForce = mAttributes.hitForce;
				component2.hitObjectEffect = ((!(base.Name == "Pixel-Cannon")) ? rConf.airGunExplosion : rConf.pixelAirGunExplosion);
				component2.life = 6f;
				component2.damage = 0f;
				component2.player = player;
				gunBullet.GetComponent<AudioSource>().mute = !GameApp.GetInstance().GetGameState().SoundOn;
			}
			else if (base.Name == "Longinus-Gold")
			{
				gunBullet = Object.Instantiate(rConf.longinusGoldBullet, fire_ori.transform.position, Quaternion.LookRotation(shootDirection)) as GameObject;
				LonginusBulletScript component3 = gunBullet.GetComponent<LonginusBulletScript>();
				component3.dir = shootDirection;
				component3.flySpeed = rocketFlySpeed;
				component3.explodeRadius = mAttributes.range;
				component3.hitForce = mAttributes.hitForce;
				component3.damage = player.Damage;
				component3.player = player;
			}
		}

		public override void StopFire()
		{
			if (base.Name == "Longinus-Gold")
			{
				if (gunSpark != null && gunSpark.isPlaying)
				{
					gunSpark.Stop();
					gunSpark.Clear(true);
				}
				if (audio != null)
				{
					audio.Stop();
				}
			}
			else if (player.GetPlayerState() != null && player.GetPlayerState().GetStateType() != PlayerStateType.Shoot && player.GetPlayerState().GetStateType() != PlayerStateType.RunShoot)
			{
				if (gunSpark != null && gunSpark.isPlaying)
				{
					gunSpark.Stop();
					gunSpark.Clear(true);
					gunSpark.GetComponent<DelayShootBullet>().enabled = false;
				}
				if (audio != null)
				{
					audio.Stop();
				}
			}
		}
	}
}
