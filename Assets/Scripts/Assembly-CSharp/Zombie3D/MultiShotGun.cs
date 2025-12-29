using System;
using UnityEngine;

namespace Zombie3D
{
	public class MultiShotGun : Weapon
	{
		protected Timer shotgunFireTimer;

		public MultiShotGun()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = true;
			shotgunFireTimer = new Timer();
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.ShotGun;
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
		}

		public override void Init()
		{
			base.MultiInit();
			mAttributes.hitForce = 20f;
			gunfire = gun.transform.Find("gun_fire_new").gameObject;
		}

		public void PlayPumpAnimation()
		{
		}

		public override void FireUpdate(float deltaTime)
		{
			base.FireUpdate(deltaTime);
			if (shotgunFireTimer.Ready())
			{
				if (gunfire != null)
				{
					gunfire.GetComponent<Renderer>().enabled = false;
				}
				shotgunFireTimer.Do();
			}
		}

		public override void Fire(float deltaTime)
		{
			AudioPlayer.PlayAudio(shootAudio, true);
			if (base.Name == "Winchester-1200")
			{
				gun.GetComponent<Animation>()["Reload"].wrapMode = WrapMode.Once;
				gun.GetComponent<Animation>().Play("Reload");
			}
			gunfire.GetComponent<Renderer>().enabled = true;
			shotgunFireTimer.SetTimer(0.2f, false);
			UnityEngine.Object.Instantiate(rConf.shotgunBullet, rightGun.position, player.GetTransform().rotation);
			GameObject gameObject = UnityEngine.Object.Instantiate(rConf.shotgunfire, gunfire.transform.position, player.GetTransform().rotation) as GameObject;
			gameObject.transform.parent = player.GetTransform();
			float num = Mathf.Tan((float)Math.PI / 3f);
			foreach (Enemy value in gameScene.GetEnemies().Values)
			{
				if (value.GetState() == Enemy.DEAD_STATE)
				{
					continue;
				}
				Vector3 vector = player.GetTransform().InverseTransformPoint(value.GetPosition());
				float sqrMagnitude = (value.GetPosition() - player.GetTransform().position).sqrMagnitude;
				float num2 = mAttributes.range * mAttributes.range;
				if (!(vector.z > 0f) || !(Mathf.Abs(vector.z / vector.x) > num))
				{
					continue;
				}
				DamageProperty damageProperty = new DamageProperty();
				damageProperty.damage = 0f;
				if (sqrMagnitude < num2)
				{
					value.OnHit(damageProperty, GetWeaponType(), true, player);
				}
				else if (sqrMagnitude < num2 * 2f * 2f)
				{
					int num3 = UnityEngine.Random.Range(0, 100);
					if ((float)num3 < mAttributes.accuracy)
					{
						value.OnHit(damageProperty, GetWeaponType(), true, player);
					}
				}
				else if (sqrMagnitude < num2 * 3f * 3f)
				{
					int num4 = UnityEngine.Random.Range(0, 100);
					if ((float)num4 < mAttributes.accuracy / 2f)
					{
						value.OnHit(damageProperty, GetWeaponType(), true, player);
					}
				}
				else if (sqrMagnitude < num2 * 4f * 4f)
				{
					int num5 = UnityEngine.Random.Range(0, 100);
					if ((float)num5 < mAttributes.accuracy / 4f)
					{
						value.OnHit(damageProperty, GetWeaponType(), true, player);
					}
				}
			}
			GameObject[] woodBoxes = gameScene.GetWoodBoxes();
			GameObject[] array = woodBoxes;
			foreach (GameObject gameObject2 in array)
			{
				if (gameObject2 != null)
				{
					Vector3 vector2 = player.GetTransform().InverseTransformPoint(gameObject2.transform.position);
					float sqrMagnitude2 = (gameObject2.transform.position - player.GetTransform().position).sqrMagnitude;
					float num6 = mAttributes.range * mAttributes.range;
					if (sqrMagnitude2 < num6 * 2f * 2f && vector2.z > 0f)
					{
						WoodBoxScript component = gameObject2.GetComponent<WoodBoxScript>();
						component.OnHit(0f);
					}
				}
			}
			lastShootTime = Time.time;
		}

		public override void StopFire()
		{
			if (gunfire != null)
			{
				gunfire.GetComponent<Renderer>().enabled = false;
			}
		}

		public override void GunOff()
		{
			base.GunOff();
		}
	}
}
