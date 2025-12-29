using System;
using UnityEngine;

namespace Zombie3D
{
	public abstract class Weapon
	{
		protected Camera cameraComponent;

		protected Transform cameraTransform;

		protected Transform rightGun;

		protected BaseCameraScript gameCamera;

		protected GameObject gunfire;

		protected GameObject fire_ori;

		public GameObject gun;

		protected Transform weaponBoneTrans;

		protected ResourceConfigScript rConf;

		protected AudioSource shootAudio;

		protected GameScene gameScene;

		protected Player player;

		protected Vector3 aimTarget;

		protected WeaponAttributes mAttributes;

		public int weapon_index;

		private SafeFloat vs_damage;

		private SafeFloat vs_frequency;

		protected float lastShootTime;

		protected SafeInteger maxCapacity;

		protected SafeInteger capacity;

		protected float maxDeflection;

		protected Vector2 deflection;

		protected SafeInteger sbulletCount;

		public string Name { get; set; }

		public SafeInteger Level { get; set; }

		public WeaponExistState Exist { get; set; }

		public WeaponConfig WConf { get; set; }

		public float AttackDamage
		{
			get
			{
				if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs && Application.loadedLevelName.StartsWith("Zombie3D_"))
				{
					return vs_damage;
				}
				return mAttributes.damage;
			}
		}

		public float AttackFrequency
		{
			get
			{
				if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs && Application.loadedLevelName.StartsWith("Zombie3D_"))
				{
					return vs_frequency;
				}
				return mAttributes.attackFrenquency;
			}
			set
			{
				if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs && Application.loadedLevelName.StartsWith("Zombie3D_"))
				{
					vs_frequency = value;
				}
				else
				{
					mAttributes.attackFrenquency = value;
				}
			}
		}

		public float AttackAccuracy
		{
			get
			{
				return mAttributes.accuracy;
			}
		}

		public float SpeedDrag
		{
			get
			{
				return mAttributes.speedDrag;
			}
		}

		public int UpgradePrice
		{
			get
			{
				return mAttributes.upgradePrice;
			}
		}

		public int MaxCapacity
		{
			get
			{
				return maxCapacity;
			}
		}

		public Vector2 Deflection
		{
			get
			{
				return deflection;
			}
		}

		public int BulletCount
		{
			get
			{
				return sbulletCount;
			}
			set
			{
				sbulletCount = value;
			}
		}

		public bool IsSelectedForBattle { get; set; }

		public bool enableShoot { get; set; }

		public Player WeaponPlayer
		{
			get
			{
				return player;
			}
			set
			{
				player = value;
			}
		}

		public GameObject WeaponBulletObject { get; set; }

		public string WeaponBulletName { get; set; }

		public Weapon()
		{
		}

		public abstract WeaponType GetWeaponType();

		public float GetLastShootTime()
		{
			return lastShootTime;
		}

		public abstract void Fire(float deltaTime);

		public abstract void StopFire();

		public GameObject GetWeaponObject()
		{
			return gun;
		}

		public virtual void DoLogic()
		{
		}

		public virtual void LoadConfig()
		{
			mAttributes = new WeaponAttributes();
			ComputeAttributes();
			mAttributes.range = WConf.range;
			mAttributes.speedDrag = WConf.moveSpeedDrag;
			sbulletCount = WConf.initBullet;
		}

		public void ComputeAttributes()
		{
			if (Level < UpgradeParas.WeaponMidLevel)
			{
				mAttributes.damage = WConf.damageInitial + (WConf.damageFinal - WConf.damageInitial) / (float)(UpgradeParas.WeaponMidLevel - 1) * (float)(Level - 1);
				mAttributes.attackFrenquency = WConf.frequencyInitial + (WConf.frequencyFinal - WConf.frequencyInitial) / (float)(UpgradeParas.WeaponMidLevel - 1) * (float)(Level - 1);
				mAttributes.accuracy = WConf.accuracyInitial + (WConf.accuracyFinal - WConf.accuracyInitial) / (float)(UpgradeParas.WeaponMidLevel - 1) * (float)(Level - 1);
				mAttributes.upgradePrice = (int)((UpgradeParas.WeaponParas["a"] * (float)Level * (float)Level + UpgradeParas.WeaponParas["b"] * (float)Level + UpgradeParas.WeaponParas["c"]) * WConf.upgradePriceWeight);
			}
			else
			{
				mAttributes.damage = WConf.damageFinal * Mathf.Pow(UpgradeParas.WeaponParas["q"], Level - UpgradeParas.WeaponMidLevel);
				mAttributes.attackFrenquency = WConf.frequencyFinal;
				mAttributes.accuracy = WConf.accuracyFinal;
				mAttributes.upgradePrice = (int)((UpgradeParas.WeaponParas["a"] * (float)Level * (float)Level + UpgradeParas.WeaponParas["b"] * (float)Level + UpgradeParas.WeaponParas["c"]) * WConf.upgradePriceWeight * UpgradeParas.WeaponParas["d"]);
			}
			mAttributes.damage = (float)Math.Round(mAttributes.damage, 1);
			mAttributes.attackFrenquency = (float)Math.Round(mAttributes.attackFrenquency, 2);
			mAttributes.accuracy = (float)Math.Round(mAttributes.accuracy, 1);
		}

		public static string GetWeaponNameEnd(WeaponType wType, string wName)
		{
			string empty = string.Empty;
			switch (wType)
			{
			case WeaponType.RocketLauncher:
			case WeaponType.Sniper:
				return "_RPG";
			case WeaponType.ShotGun:
			case WeaponType.M32:
			case WeaponType.Crossbow:
				return "_Shotgun";
			case WeaponType.Saw:
			case WeaponType.Sword:
				return "_Saw";
			case WeaponType.ElectricGun:
				if (wName == "Dragon-Breath" || wName == "Pixel-Cannon")
				{
					return string.Empty;
				}
				return "_Shotgun";
			default:
				return string.Empty;
			}
		}

		public void Upgrade()
		{
			Level++;
			if (Level == 10)
			{
				GameApp.GetInstance().GetGameState().Achievement.UpgradeTenTimes();
			}
			ComputeAttributes();
		}

		public virtual void Init()
		{
			gameScene = GameApp.GetInstance().GetGameScene();
			rConf = GameApp.GetInstance().GetResourceConfig();
			gameCamera = gameScene.GetCamera();
			cameraComponent = gameCamera.GetComponent<Camera>();
			cameraTransform = gameCamera.CameraTransform;
			player = gameScene.GetPlayer();
			aimTarget = default(Vector3);
			weaponBoneTrans = player.GetTransform().Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Weapon_Dummy");
			CreateGunModel();
			shootAudio = gun.GetComponent<AudioSource>();
			GunOff();
			enableShoot = true;
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				VSReset();
				SetVsParameter();
			}
		}

		public virtual void MultiInit()
		{
			gameScene = GameApp.GetInstance().GetGameScene();
			rConf = GameApp.GetInstance().GetResourceConfig();
			aimTarget = default(Vector3);
			IsSelectedForBattle = true;
			weaponBoneTrans = player.GetTransform().Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Weapon_Dummy");
			CreateGunModel();
			shootAudio = gun.GetComponent<AudioSource>();
			GunOff();
			enableShoot = true;
		}

		protected void CreateGunModel()
		{
			gun = WeaponFactory.GetInstance().CreateWeaponModel(Name, player.GetTransform().position, player.GetTransform().rotation);
			gun.transform.parent = weaponBoneTrans;
			rightGun = gun.transform;
		}

		public virtual void FireUpdate(float deltaTime)
		{
		}

		public virtual void AutoAim(float deltaTime)
		{
		}

		public virtual void ReleaseBullet()
		{
		}

		public void AddBullets(int num)
		{
			BulletCount += num;
			BulletCount = Mathf.Clamp(BulletCount, 0, 9999);
		}

		public virtual void GunOn()
		{
			GameObject gameObject = gun.transform.Find("Bone01").gameObject;
			if (gameObject.GetComponent<Renderer>() == null)
			{
				gameObject = gameObject.transform.Find("Bone02").gameObject;
			}
			gameObject.GetComponent<Renderer>().enabled = true;
		}

		public virtual void ConsumeBullet(int count)
		{
		}

		public virtual bool HaveBullets(bool isStopFire)
		{
			if (BulletCount == 0)
			{
				if (isStopFire)
				{
					StopFire();
				}
				return false;
			}
			return true;
		}

		public virtual bool CouldMakeNextShoot()
		{
			if (Time.time - lastShootTime > AttackFrequency)
			{
				return true;
			}
			return false;
		}

		public virtual void GunOff()
		{
			GameObject gameObject = gun.transform.Find("Bone01").gameObject;
			if (gameObject.GetComponent<Renderer>() == null)
			{
				gameObject = gameObject.transform.Find("Bone02").gameObject;
			}
			gameObject.GetComponent<Renderer>().enabled = false;
			StopFire();
		}

		public void DestroyGun()
		{
			UnityEngine.Object.Destroy(gun);
		}

		public virtual void VSReset()
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				gun.transform.localPosition = Vector3.zero;
				gun.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
			}
		}

		public void SetVsParameter()
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				if (GetWeaponType() == WeaponType.SuicideGun)
				{
					vs_damage = mAttributes.damage;
					vs_frequency = mAttributes.attackFrenquency;
				}
				else if (Level < UpgradeParas.WeaponMidLevel)
				{
					vs_damage = WConf.vsDamageBase + (float)(Level - 1) * WConf.vsDamageDelta;
					vs_frequency = WConf.vsFrequencyBase + (float)(Level - 1) * WConf.vsFrequencyDelta;
				}
				else
				{
					vs_damage = WConf.vsDamageBase + (float)(UpgradeParas.WeaponMidLevel - 1) * WConf.vsDamageDelta + (float)(Level - UpgradeParas.WeaponMidLevel) * WConf.vsDamageDelta2;
					vs_frequency = WConf.vsFrequencyBase + (float)(UpgradeParas.WeaponMidLevel - 1) * WConf.vsFrequencyDelta;
				}
			}
		}
	}
}
