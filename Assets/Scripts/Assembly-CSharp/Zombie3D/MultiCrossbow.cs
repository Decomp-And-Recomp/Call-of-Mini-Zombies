using UnityEngine;

namespace Zombie3D
{
	internal class MultiCrossbow : Weapon
	{
		public MultiCrossbow()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = true;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.Crossbow;
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
		}

		public override void Init()
		{
			base.MultiInit();
			mAttributes.hitForce = 30f;
			fire_ori = gun.transform.Find("fire_ori").gameObject;
			gun.GetComponent<AudioSource>().Stop();
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
			Vector3 normalized = (aimTarget - fire_ori.transform.position).normalized;
			GameObject gameObject = Object.Instantiate(rConf.crossbowBullet, fire_ori.transform.position, Quaternion.LookRotation(normalized)) as GameObject;
			ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
			component.dir = normalized;
			component.flySpeed = base.WConf.flySpeed;
			component.explodeRadius = mAttributes.range;
			component.hitObjectEffect = GameApp.GetInstance().GetGameResourceConfig().crossbowHit;
			component.hitForce = mAttributes.hitForce;
			component.life = 5f;
			component.damage = 0f;
			component.GunType = WeaponType.Crossbow;
			component.player = player;
			gun.GetComponent<AudioSource>().Play();
			gun.GetComponent<AudioSource>().mute = !GameApp.GetInstance().GetGameState().SoundOn;
			lastShootTime = Time.time;
		}

		public override void StopFire()
		{
		}
	}
}
