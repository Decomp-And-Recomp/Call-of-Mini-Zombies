using System.Collections;
using UnityEngine;

namespace Zombie3D
{
	public class MultiSword : Weapon
	{
		protected float lightFlySpeed;

		protected GameObject _swordlight;

		protected float _createtime;

		public MultiSword()
		{
			base.IsSelectedForBattle = true;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.Sword;
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
			lightFlySpeed = base.WConf.flySpeed;
		}

		public override void Init()
		{
			base.MultiInit();
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
						damageProperty.damage = 0f;
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
							component.OnHit(0f);
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
				gunfire.GetComponent<Renderer>().enabled = false;
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
