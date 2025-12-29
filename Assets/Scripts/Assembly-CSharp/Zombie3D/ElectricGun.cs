using UnityEngine;

namespace Zombie3D
{
	internal class ElectricGun : Weapon
	{
		protected float rocketFlySpeed;

		protected ParticleSystem gunSpark;

		protected GameObject gunBullet;

		protected AudioSource audio;

		private Vector3 shootDirection = default(Vector3);

		public ElectricGun()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = false;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.ElectricGun;
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
			rocketFlySpeed = base.WConf.flySpeed;
			base.WeaponBulletName = "Bullet_ElectricGun";
		}

		public override void Init()
		{
			base.Init();
			mAttributes.hitForce = 30f;
			base.WeaponBulletObject = rConf.itemElectricGun;
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
			if (num <= 0f)
			{
				shootDirection = (vector - fire_ori.transform.position).normalized;
			}
			else
			{
				shootDirection = (aimTarget - fire_ori.transform.position).normalized;
			}
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
				component.damage = player.Damage;
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
				component2.hitObjectEffect = ((!(base.Name == "Pixel-Cannon")) ? rConf.airGunExplosion : rConf.pixelAirGunExplosion);
				component2.hitForce = mAttributes.hitForce;
				component2.life = 6f;
				component2.damage = player.Damage;
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
			ConsumeBullet(1);
		}

		public override void ConsumeBullet(int count)
		{
			sbulletCount -= count;
			sbulletCount = Mathf.Clamp(sbulletCount, 0, maxCapacity);
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
