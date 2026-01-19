using UnityEngine;

namespace Zombie3D
{
	public class MultiFireGun : Weapon
	{
		protected float flySpeed;

		protected Vector3 laserStartScale;

		protected float lastLaserHitInitiatTime;

		protected ParticleSystem FireDream;

		protected ParticleSystem FireHeart1;

		protected ParticleSystem FireHeart2;

		public MultiFireGun()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = true;
		}

		public override void Init()
		{
			base.MultiInit();
			mAttributes.hitForce = 0f;
			gunfire = gun.transform.Find("gun_fire_new").gameObject;
			GameObject gameObject = gun.transform.Find("gun_fire_new/hellfire/hellfire_01").gameObject;
			gameObject.GetComponent<HellFireProjectileScript>().SetPlayer(player);
			FireDream = gameObject.GetComponent<ParticleSystem>();
			gameObject = gun.transform.Find("gun_fire_new/hellfire/hellfire_02").gameObject;
			FireHeart1 = gameObject.GetComponent<ParticleSystem>();
			gameObject = gun.transform.Find("gun_fire_new/hellfire/hellfire_03").gameObject;
			FireHeart2 = gameObject.GetComponent<ParticleSystem>();
			fire_ori = gun.transform.Find("fire_ori").gameObject;
			EnableFire(false);
		}

		public void EnableFire(bool status)
		{
			if (FireDream != null)
			{
				if (status) FireDream.Play(true);
				else FireDream.Stop(true);
			}
			if (FireHeart1 != null)
			{
				if (status) FireHeart1.Play(true);
				else FireHeart1.Stop(true);
			}
			if (FireHeart2 != null)
			{
				if (status) FireHeart2.Play(true);
				else FireHeart2.Stop(true);
			}
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
			flySpeed = base.WConf.flySpeed;
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

		public override void FireUpdate(float deltaTime)
		{
			if (!FireDream.isPlaying)
			{
				return;
			}
			Vector3 worldPosition = fire_ori.transform.TransformPoint(0f, 0f, 5f);
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
	}
}
