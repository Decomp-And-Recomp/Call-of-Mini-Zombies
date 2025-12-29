using UnityEngine;

namespace Zombie3D
{
	public class FireGun : Weapon
	{
		protected float flySpeed;

		protected Vector3 laserStartScale;

		protected float lastLaserHitInitiatTime;

		protected ParticleEmitter FireDream;

		protected ParticleEmitter FireHeart1;

		protected ParticleEmitter FireHeart2;

		protected float temp_consume;

		public FireGun()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = false;
		}

		public override void Init()
		{
			base.Init();
			mAttributes.hitForce = 0f;
			base.WeaponBulletObject = rConf.itemFire;
			gunfire = gun.transform.Find("gun_fire_new").gameObject;
			GameObject gameObject = gun.transform.Find("gun_fire_new/hellfire/hellfire_01").gameObject;
			gameObject.GetComponent<HellFireProjectileScript>().SetPlayer(player);
			FireDream = gameObject.GetComponent<ParticleEmitter>();
			gameObject = gun.transform.Find("gun_fire_new/hellfire/hellfire_02").gameObject;
			FireHeart1 = gameObject.GetComponent<ParticleEmitter>();
			gameObject = gun.transform.Find("gun_fire_new/hellfire/hellfire_03").gameObject;
			FireHeart2 = gameObject.GetComponent<ParticleEmitter>();
			EnableFire(false);
		}

		public void EnableFire(bool status)
		{
			if (FireDream != null)
			{
				FireDream.emit = status;
			}
			if (FireHeart1 != null)
			{
				FireHeart1.emit = status;
			}
			if (FireHeart2 != null)
			{
				FireHeart2.emit = status;
			}
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
			flySpeed = base.WConf.flySpeed;
			base.WeaponBulletName = "Bullet_FireGun";
		}

		public void PlayShootAudio()
		{
			if (shootAudio != null)
			{
				AudioPlayer.PlayAudio(shootAudio, true);
			}
		}

		public void SetShootTimeNow()
		{
			lastShootTime = Time.time;
		}

		public void ConsumeBulletTime(float count)
		{
			temp_consume += count;
			if (temp_consume >= 1f)
			{
				sbulletCount--;
				sbulletCount = Mathf.Clamp(sbulletCount, 0, maxCapacity);
				temp_consume = 0f;
			}
		}

		public override void FireUpdate(float deltaTime)
		{
			if (!FireDream.emit)
			{
				return;
			}
			ConsumeBulletTime(20f * deltaTime);
			Vector3 worldPosition = cameraComponent.ScreenToWorldPoint(new Vector3(gameCamera.ReticlePosition.x, (float)Screen.height - gameCamera.ReticlePosition.y, 10f));
			FireDream.gameObject.transform.LookAt(worldPosition);
			if (CouldMakeNextShoot())
			{
				if (shootAudio != null && !shootAudio.isPlaying)
				{
					AudioPlayer.PlayAudio(shootAudio, true);
				}
				lastShootTime = Time.time;
			}
		}

		public override void Fire(float deltaTime)
		{
			EnableFire(true);
		}

		public override void StopFire()
		{
			EnableFire(false);
			if (shootAudio != null)
			{
				shootAudio.Stop();
			}
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.FireGun;
		}

		public override void ConsumeBullet(int count)
		{
			sbulletCount -= count;
			sbulletCount = Mathf.Clamp(sbulletCount, 0, maxCapacity);
		}
	}
}
