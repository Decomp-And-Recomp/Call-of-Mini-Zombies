using UnityEngine;

namespace Zombie3D
{
	public class MultiSaw : Weapon
	{
		public MultiSaw()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = true;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.Saw;
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
			lastShootTime = Time.time;
		}

		public override void GunOn()
		{
			GameObject gameObject = gun.transform.Find("Saw01").gameObject;
			GameObject gameObject2 = gun.transform.Find("Saw02").gameObject;
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
			GameObject gameObject = gun.transform.Find("Saw01").gameObject;
			GameObject gameObject2 = gun.transform.Find("Saw02").gameObject;
			if (gameObject.GetComponent<Renderer>() != null)
			{
				gameObject.GetComponent<Renderer>().enabled = false;
			}
			if (gameObject2.GetComponent<Renderer>() != null)
			{
				gameObject2.GetComponent<Renderer>().enabled = false;
			}
			StopFire();
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
	}
}
