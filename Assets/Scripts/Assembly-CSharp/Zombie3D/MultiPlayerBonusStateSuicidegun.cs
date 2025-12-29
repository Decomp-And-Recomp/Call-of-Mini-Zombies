using UnityEngine;

namespace Zombie3D
{
	public class MultiPlayerBonusStateSuicidegun : PlayerBonusState
	{
		private Weapon previousWeapon;

		private MultiSuicideGun suicide;

		public MultiPlayerBonusStateSuicidegun()
		{
			stateType = PlayerBonusStateType.Suicidegun;
		}

		public override void EnterState(Player player)
		{
			if (!CheckPlayerInDeadState(player))
			{
				player.GetWeapon().StopFire();
				previousWeapon = player.GetWeapon();
				suicide = WeaponFactory.GetInstance().CreateWeapon(WeaponType.SuicideGun, true) as MultiSuicideGun;
				suicide.LoadConfig();
				suicide.WeaponPlayer = player;
				suicide.Init();
				suicide.VSReset();
				suicide.IsSelectedForBattle = true;
				suicide.BulletCount = 1;
				player.ChangeWeapon(suicide);
			}
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
		}

		public override void ExitState(Player player)
		{
			if (player.GetWeapon().GetWeaponType() == WeaponType.SuicideGun)
			{
				player.ChangeWeapon(previousWeapon);
				Object.Destroy(suicide.gun);
				suicide = null;
			}
		}

		protected override bool CheckPlayerInDeadState(Player player)
		{
			if (player.GetPlayerState() != null && player.GetPlayerState().GetStateType() == PlayerStateType.Dead)
			{
				return true;
			}
			return false;
		}
	}
}
