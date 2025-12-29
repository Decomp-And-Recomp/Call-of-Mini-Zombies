using System;
using UnityEngine;

namespace Zombie3D
{
	public class ShotGun : Weapon
	{
		protected Timer shotgunFireTimer;

		public ShotGun()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = false;
			shotgunFireTimer = new Timer();
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.ShotGun;
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
			base.WeaponBulletName = "Bullet_ShotGun";
		}

		public override void Init()
		{
			base.Init();
			mAttributes.hitForce = 20f;
			base.WeaponBulletObject = rConf.itemShotGun;
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
			int num2 = 0;
			foreach (Enemy value in gameScene.GetEnemies().Values)
			{
				if (value.GetState() == Enemy.DEAD_STATE)
				{
					continue;
				}
				Vector3 vector = player.GetTransform().InverseTransformPoint(value.GetPosition());
				float sqrMagnitude = (value.GetPosition() - player.GetTransform().position).sqrMagnitude;
				float num3 = mAttributes.range * mAttributes.range;
				if (vector.z > 0f && Mathf.Abs(vector.z / vector.x) > num)
				{
					DamageProperty damageProperty = new DamageProperty();
					damageProperty.damage = player.Damage;
					if (sqrMagnitude < num3)
					{
						value.OnHit(damageProperty, GetWeaponType(), true, player);
					}
					else if (sqrMagnitude < num3 * 2f * 2f)
					{
						int num4 = UnityEngine.Random.Range(0, 100);
						if ((float)num4 < mAttributes.accuracy)
						{
							value.OnHit(damageProperty, GetWeaponType(), true, player);
						}
					}
					else if (sqrMagnitude < num3 * 3f * 3f)
					{
						int num5 = UnityEngine.Random.Range(0, 100);
						if ((float)num5 < mAttributes.accuracy / 2f)
						{
							value.OnHit(damageProperty, GetWeaponType(), true, player);
						}
					}
					else if (sqrMagnitude < num3 * 4f * 4f)
					{
						int num6 = UnityEngine.Random.Range(0, 100);
						if ((float)num6 < mAttributes.accuracy / 4f)
						{
							value.OnHit(damageProperty, GetWeaponType(), true, player);
						}
					}
				}
				if (value.Attributes.Hp <= 0.0)
				{
					num2++;
				}
			}
			if (num2 >= 4)
			{
				GameApp.GetInstance().GetGameState().Achievement.CheckAchievemnet_WeaponMaster();
			}
			GameObject[] woodBoxes = gameScene.GetWoodBoxes();
			GameObject[] array = woodBoxes;
			foreach (GameObject gameObject2 in array)
			{
				if (gameObject2 != null)
				{
					Vector3 vector2 = player.GetTransform().InverseTransformPoint(gameObject2.transform.position);
					float sqrMagnitude2 = (gameObject2.transform.position - player.GetTransform().position).sqrMagnitude;
					float num7 = mAttributes.range * mAttributes.range;
					if (sqrMagnitude2 < num7 * 2f * 2f && vector2.z > 0f)
					{
						WoodBoxScript component = gameObject2.GetComponent<WoodBoxScript>();
						component.OnHit(player.Damage);
					}
				}
			}
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				GameVSScene gameVSScene = GameApp.GetInstance().GetGameScene() as GameVSScene;
				foreach (Player value2 in gameVSScene.SFS_Player_Arr.Values)
				{
					if (value2 == null || value2 == player || (value2.GetPlayerState() != null && value2.GetPlayerState().GetStateType() == PlayerStateType.Dead))
					{
						continue;
					}
					Vector3 vector3 = player.GetTransform().InverseTransformPoint(value2.GetTransform().position);
					float sqrMagnitude3 = (value2.GetTransform().position - player.GetTransform().position).sqrMagnitude;
					float num8 = mAttributes.range * mAttributes.range;
					if (!(vector3.z > 0f) || !(Mathf.Abs(vector3.z / vector3.x) > num))
					{
						continue;
					}
					if (sqrMagnitude3 < num8)
					{
						value2.OnVsInjured(player.tnet_user, player.Damage, (int)GetWeaponType());
					}
					else if (sqrMagnitude3 < num8 * 2f * 2f)
					{
						int num9 = UnityEngine.Random.Range(0, 100);
						if ((float)num9 < mAttributes.accuracy)
						{
							value2.OnVsInjured(player.tnet_user, player.Damage, (int)GetWeaponType());
						}
					}
					else if (sqrMagnitude3 < num8 * 3f * 3f)
					{
						int num10 = UnityEngine.Random.Range(0, 100);
						if ((float)num10 < mAttributes.accuracy / 2f)
						{
							value2.OnVsInjured(player.tnet_user, player.Damage, (int)GetWeaponType());
						}
					}
					else if (sqrMagnitude3 < num8 * 4f * 4f)
					{
						int num11 = UnityEngine.Random.Range(0, 100);
						if ((float)num11 < mAttributes.accuracy / 4f)
						{
							value2.OnVsInjured(player.tnet_user, player.Damage, (int)GetWeaponType());
						}
					}
				}
			}
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
