using UnityEngine;

namespace Zombie3D
{
	public class PlayerBonusStateSuicidegun : PlayerBonusState
	{
		private Weapon previousWeapon;

		private SuicideGun suicide;

		private float holdOnAfterFinishShoot = 10f;

		private bool enableShoot;

		public PlayerBonusStateSuicidegun()
		{
			stateType = PlayerBonusStateType.Suicidegun;
		}

		public override void EnterState(Player player)
		{
			if (!CheckPlayerInDeadState(player))
			{
				player.GetWeapon().StopFire();
				player.SetState(PlayerStateType.Idle);
				previousWeapon = player.GetWeapon();
				suicide = WeaponFactory.GetInstance().CreateWeapon(WeaponType.SuicideGun, false) as SuicideGun;
				suicide.LoadConfig();
				suicide.Init();
				suicide.IsSelectedForBattle = true;
				suicide.BulletCount = 1;
				player.ChangeWeapon(suicide);
				holdOnAfterFinishShoot = 10f;
				enableShoot = true;
			}
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			if (CheckPlayerInDeadState(player))
			{
				return;
			}
			Weapon weapon = player.GetWeapon();
			if (weapon.enableShoot && enableShoot)
			{
				player.SetState(PlayerStateType.Shoot);
				weapon.enableShoot = false;
				enableShoot = false;
			}
			if (!player.GetWeapon().HaveBullets(false))
			{
				holdOnAfterFinishShoot -= deltaTime;
				if (holdOnAfterFinishShoot <= 0f)
				{
					player.SetBonusState(PlayerBonusStateType.Normal);
				}
			}
		}

		public override void ExitState(Player player)
		{
			if (player.GetWeapon().GetWeaponType() == WeaponType.SuicideGun)
			{
				player.ChangeWeaponBackFromSuicideGun(previousWeapon);
				Object.Destroy(suicide.gun);
				suicide = null;
			}
		}

		protected override bool CheckPlayerInDeadState(Player player)
		{
			if (player.GetPlayerState() != null && player.GetPlayerState().GetStateType() == PlayerStateType.Dead)
			{
				if (player.GetWeapon().GetWeaponType() == WeaponType.SuicideGun)
				{
					player.GetWeapon().enableShoot = false;
				}
				return true;
			}
			return false;
		}
	}
}
