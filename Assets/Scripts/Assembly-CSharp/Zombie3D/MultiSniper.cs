using System.Collections.Generic;
using UnityEngine;

namespace Zombie3D
{
	public class MultiSniper : Weapon
	{
		protected float trimWidth = 25f;

		protected float trimHeight = 25f;

		protected List<NearestEnemyInfo> nearestEnemyInfoList;

		protected int maxLocks = 5;

		protected bool locked;

		protected float flySpeed;

		public static Rect lockAreaRect = AutoRect.AutoPos(new Rect(230f, 200f, 500f, 250f));

		public int MaxLocks
		{
			get
			{
				return maxLocks;
			}
		}

		public MultiSniper()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = true;
		}

		public List<NearestEnemyInfo> GetNearestEnemyInfoList()
		{
			return nearestEnemyInfoList;
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.Sniper;
		}

		public override void Init()
		{
			base.MultiInit();
			mAttributes.hitForce = 60f;
			nearestEnemyInfoList = new List<NearestEnemyInfo>();
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
			flySpeed = base.WConf.flySpeed;
		}

		public bool AimedTarget()
		{
			if (nearestEnemyInfoList.Count == 0)
			{
				return false;
			}
			return true;
		}

		public void AddMultiTarget(Vector3 pos)
		{
			NearestEnemyInfo nearestEnemyInfo = new NearestEnemyInfo();
			nearestEnemyInfo.tar_pos = pos;
			nearestEnemyInfoList.Add(nearestEnemyInfo);
		}

		public override void Fire(float deltaTime)
		{
			foreach (NearestEnemyInfo nearestEnemyInfo in nearestEnemyInfoList)
			{
				Vector3 normalized = (nearestEnemyInfo.tar_pos - rightGun.position).normalized;
				GameObject gameObject = Object.Instantiate(rConf.isnipertile, rightGun.position + Vector3.up, Quaternion.LookRotation(-normalized)) as GameObject;
				ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
				component.dir = normalized;
				component.life = 10f;
				component.damage = 0f;
				component.flySpeed = flySpeed;
				component.hitForce = mAttributes.hitForce;
				component.targetPos = nearestEnemyInfo.tar_pos;
				component.GunType = WeaponType.Sniper;
				component.explodeRadius = mAttributes.range;
				component.hitObjectEffect = GameApp.GetInstance().GetGameResourceConfig().rocketExlposion;
				component.player = player;
			}
			nearestEnemyInfoList.Clear();
			locked = false;
		}

		public override void StopFire()
		{
			if (nearestEnemyInfoList != null)
			{
				nearestEnemyInfoList.Clear();
			}
		}
	}
}
