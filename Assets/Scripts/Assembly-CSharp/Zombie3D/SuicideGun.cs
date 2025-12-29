using UnityEngine;

namespace Zombie3D
{
	internal class SuicideGun : Weapon
	{
		protected const float shootLastingTime = 0.5f;

		protected ParticleSystem gunSpark;

		protected AudioSource audio;

		public SuicideGun()
		{
			maxCapacity = 1;
			base.IsSelectedForBattle = false;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.SuicideGun;
		}

		public override void LoadConfig()
		{
			base.Name = "SuicideGun";
			mAttributes = new WeaponAttributes();
			mAttributes.damage = 5000f;
			mAttributes.attackFrenquency = 2f;
			mAttributes.accuracy = 100f;
			mAttributes.upgradePrice = 0;
			mAttributes.range = 15f;
			mAttributes.speedDrag = 3f;
			sbulletCount = 1;
			base.WeaponBulletName = "Bullet_SuicideGun";
		}

		public override void Init()
		{
			base.Init();
			mAttributes.hitForce = 30f;
			fire_ori = gun.transform.Find("fire_ori").gameObject;
			GameObject gameObject = Object.Instantiate(rConf.suicideGunSpark, fire_ori.transform.position, Quaternion.identity) as GameObject;
			gameObject.transform.parent = fire_ori.transform;
			gunSpark = gameObject.GetComponent<ParticleSystem>();
			gunSpark.enableEmission = false;
			gunSpark.Stop(true);
			audio = gameObject.GetComponent<AudioSource>();
			audio.Stop();
			gun.transform.localPosition = Vector3.zero;
			gun.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
			base.enableShoot = false;
		}

		public override void Fire(float deltaTime)
		{
			ConsumeBullet(1);
		}

		public override void ReleaseBullet()
		{
			gunSpark.enableEmission = true;
			gunSpark.Play();
			audio.Play();
			audio.mute = !GameApp.GetInstance().GetGameState().SoundOn;
			GameObject gameObject = Object.Instantiate(rConf.suicideGunBullet, fire_ori.transform.position, Quaternion.identity) as GameObject;
			SuicideGunBullectScript component = gameObject.GetComponent<SuicideGunBullectScript>();
			component.player = player;
			component.damage = player.Damage;
			component.explodeRadius = mAttributes.range;
			component.hitForce = mAttributes.hitForce;
		}

		public override void ConsumeBullet(int count)
		{
			sbulletCount -= count;
			sbulletCount = Mathf.Clamp(sbulletCount, 0, maxCapacity);
		}

		public override void StopFire()
		{
			if (gunSpark != null)
			{
				gunSpark.Stop();
				gunSpark.Clear(true);
			}
			if (audio != null)
			{
				audio.Stop();
			}
		}
	}
}
