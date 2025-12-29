using UnityEngine;

namespace Zombie3D
{
	public class WeaponFactory
	{
		protected static WeaponFactory instance;

		public static WeaponFactory GetInstance()
		{
			if (instance == null)
			{
				instance = new WeaponFactory();
			}
			return instance;
		}

		public Weapon CreateWeapon(WeaponType wType, bool isMultiWeapon)
		{
			Weapon result = null;
			if (isMultiWeapon)
			{
				switch (wType)
				{
				case WeaponType.AssaultRifle:
					result = new MultiAssaultRifle();
					break;
				case WeaponType.ShotGun:
					result = new MultiShotGun();
					break;
				case WeaponType.RocketLauncher:
					result = new MultiRocketLauncher();
					break;
				case WeaponType.ElectricGun:
					result = new MultiElectricGun();
					break;
				case WeaponType.MachineGun:
					result = new MultiMachineGun();
					break;
				case WeaponType.LaserGun:
					result = new MultiLaserGun();
					break;
				case WeaponType.Sniper:
					result = new MultiSniper();
					break;
				case WeaponType.Saw:
					result = new MultiSaw();
					break;
				case WeaponType.Sword:
					result = new MultiSword();
					break;
				case WeaponType.M32:
					result = new MultiGrenadeRifle();
					break;
				case WeaponType.FireGun:
					result = new MultiFireGun();
					break;
				case WeaponType.SuicideGun:
					result = new MultiSuicideGun();
					break;
				case WeaponType.Crossbow:
					result = new MultiCrossbow();
					break;
				}
			}
			else
			{
				switch (wType)
				{
				case WeaponType.AssaultRifle:
					result = new AssaultRifle();
					break;
				case WeaponType.ShotGun:
					result = new ShotGun();
					break;
				case WeaponType.RocketLauncher:
					result = new RocketLauncher();
					break;
				case WeaponType.ElectricGun:
					result = new ElectricGun();
					break;
				case WeaponType.MachineGun:
					result = new MachineGun();
					break;
				case WeaponType.LaserGun:
					result = new LaserGun();
					break;
				case WeaponType.Sniper:
					result = new Sniper();
					break;
				case WeaponType.Saw:
					result = new Saw();
					break;
				case WeaponType.Sword:
					result = new Sword();
					break;
				case WeaponType.M32:
					result = new GrenadeRifle();
					break;
				case WeaponType.FireGun:
					result = new FireGun();
					break;
				case WeaponType.SuicideGun:
					result = new SuicideGun();
					break;
				case WeaponType.Crossbow:
					result = new Crossbow();
					break;
				}
			}
			return result;
		}

		public GameObject CreateWeaponModel(string weaponName, Vector3 pos, Quaternion rotation)
		{
			string path = "Prefabs/Weapon/" + weaponName;
			GameObject gameObject = Object.Instantiate(Resources.Load(path)) as GameObject;
			GunConfigScript component = gameObject.GetComponent<GunConfigScript>();
			GameObject result = (GameObject)Object.Instantiate(component.Gun_Instance, pos, rotation);
			Object.Destroy(gameObject);
			return result;
		}
	}
}
