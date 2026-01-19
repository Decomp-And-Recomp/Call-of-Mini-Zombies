using System.Collections;
using UnityEngine;

namespace Zombie3D
{
	public class Sword : Weapon
	{
		protected float lightFlySpeed;

		protected GameObject _swordlight;

		protected float _createtime;

		public Sword()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = false;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.Sword;
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
			base.WeaponBulletName = string.Empty;
			lightFlySpeed = base.WConf.flySpeed;
		}

		public override void Init()
		{
			base.Init();
		}

		public override void DoLogic()
		{
		}

		public override void FireUpdate(float deltaTime)
		{
		}

		public override bool HaveBullets(bool isStopFire)
		{
			return true;
		}

		public override void Fire(float deltaTime)
		{
			if (shootAudio != null && !shootAudio.isPlaying)
			{
				AudioPlayer.PlayAudio(shootAudio, true);
			}
			if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Vs)
			{
				Hashtable enemies = gameScene.GetEnemies();
				foreach (Enemy value in enemies.Values)
				{
					Collider collider = value.GetCollider();
					if (gun.GetComponent<Collider>().bounds.Intersects(collider.bounds))
					{
						DamageProperty damageProperty = new DamageProperty();
						damageProperty.damage = player.Damage;
						value.OnHit(damageProperty, WeaponType.Sword, true, player);
					}
				}
				GameObject[] woodBoxes = gameScene.GetWoodBoxes();
				GameObject[] array = woodBoxes;
				foreach (GameObject gameObject in array)
				{
					if (gameObject != null)
					{
						Collider collider2 = gameObject.GetComponent<Collider>();
						if (gun.GetComponent<Collider>().bounds.Intersects(collider2.bounds))
						{
							WoodBoxScript component = gameObject.GetComponent<WoodBoxScript>();
							component.OnHit(player.Damage);
						}
					}
				}
			}
			else
			{
				GameVSScene gameVSScene = GameApp.GetInstance().GetGameScene() as GameVSScene;
				foreach (Player value2 in gameVSScene.SFS_Player_Arr.Values)
				{
					if (value2 != null && value2 != player && value2.GetPlayerState() != null && value2.GetPlayerState().GetStateType() != PlayerStateType.Dead)
					{
						Collider collider3 = value2.Collider;
						if (gun.GetComponent<Collider>().bounds.Intersects(collider3.bounds))
						{
							value2.OnVsInjured(player.tnet_user, player.Damage, 10);
						}
					}
				}
			}
			lastShootTime = Time.time;
		}

		public override void AutoAim(float deltaTime)
		{
		}

		public override void GunOn()
		{
			GameObject gameObject = gun.transform.Find("GuangJian_01").gameObject;
			GameObject gameObject2 = gun.transform.Find("GuangJian_02").gameObject;
			if (gameObject.GetComponent<Renderer>() != null)
			{
				gameObject.GetComponent<Renderer>().enabled = true;
			}
			if (gameObject2.GetComponent<Renderer>() != null)
			{
				gameObject2.GetComponent<Renderer>().enabled = true;
			}
		}

		public override void GunOff()
		{
			GameObject gameObject = gun.transform.Find("GuangJian_01").gameObject;
			GameObject gameObject2 = gun.transform.Find("GuangJian_02").gameObject;
			if (gameObject.GetComponent<Renderer>() != null)
			{
				gameObject.GetComponent<Renderer>().enabled = false;
			}
			if (gameObject2.GetComponent<Renderer>() != null)
			{
				gameObject2.GetComponent<Renderer>().enabled = false;
			}
			StopFire();
			EffectTrailSwitch(false);
		}

		public override void StopFire()
		{
			if (shootAudio != null)
			{
			}
			if (gunfire != null)
			{
				if (gunfire.TryGetComponent(out Renderer gunfireRenderer)) gunfireRenderer.enabled = false;
				else if (gunfire.TryGetComponent(out ParticleSystem gunfireParticles)) gunfireParticles.Stop();
			}
		}

		public void EffectTrailSwitch(bool on)
		{
			if (on)
			{
				SwordEffectTrail swordEffectTrail = gun.GetComponent("SwordEffectTrail") as SwordEffectTrail;
				if (null != swordEffectTrail)
				{
					swordEffectTrail.ShowTrail(true);
				}
			}
			else
			{
				SwordEffectTrail swordEffectTrail2 = gun.GetComponent("SwordEffectTrail") as SwordEffectTrail;
				if (null != swordEffectTrail2)
				{
					swordEffectTrail2.ShowTrail(false);
				}
			}
		}
	}
}
