using UnityEngine;

namespace Zombie3D
{
	internal class Crossbow : Weapon
	{
		public Crossbow()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = false;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.Crossbow;
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
			base.WeaponBulletName = "Bullet_Crossbow";
		}

		public override void Init()
		{
			base.Init();
			mAttributes.hitForce = 30f;
			base.WeaponBulletObject = rConf.itemCrossbow;
			fire_ori = gun.transform.Find("fire_ori").gameObject;
			gun.GetComponent<AudioSource>().Stop();
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
			GameObject gameObject = Object.Instantiate(rConf.crossbowBullet, fire_ori.transform.position, Quaternion.LookRotation(vector2)) as GameObject;
			ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
			component.dir = vector2;
			component.flySpeed = base.WConf.flySpeed;
			component.explodeRadius = mAttributes.range;
			component.hitObjectEffect = GameApp.GetInstance().GetGameResourceConfig().crossbowHit;
			component.hitForce = mAttributes.hitForce;
			component.life = 5f;
			component.damage = player.Damage;
			component.GunType = WeaponType.Crossbow;
			component.player = player;
			gun.GetComponent<AudioSource>().Play();
			gun.GetComponent<AudioSource>().mute = !GameApp.GetInstance().GetGameState().SoundOn;
			lastShootTime = Time.time;
			ConsumeBullet(1);
		}

		public override void ConsumeBullet(int count)
		{
			sbulletCount -= count;
			sbulletCount = Mathf.Clamp(sbulletCount, 0, maxCapacity);
		}

		public override void StopFire()
		{
		}
	}
}
