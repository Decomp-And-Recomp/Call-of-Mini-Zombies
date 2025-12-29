using UnityEngine;

namespace Zombie3D
{
	public class PlayerRunState : PlayerState
	{
		public PlayerRunState()
		{
			state_type = PlayerStateType.Run;
		}

		public override void EnterState(Player player)
		{
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			player.Move(deltaTime);
			player.ResetSawAnimation();
			player.Animate("Run01" + player.WeaponNameEnd, WrapMode.Loop);
			Weapon weapon = player.GetWeapon();
			if (!player.InputController.InputInfo.fire)
			{
				player.ZoomOut(deltaTime);
				if (!player.InputController.InputInfo.IsMoving)
				{
					player.SetState(PlayerStateType.Idle);
				}
			}
			else if (weapon.GetWeaponType() != WeaponType.SuicideGun)
			{
				if (player.InputController.InputInfo.IsMoving)
				{
					player.SetState(PlayerStateType.RunShoot);
				}
				else
				{
					player.SetState(PlayerStateType.Shoot);
				}
			}
		}

		public override void ExitState(Player player)
		{
		}
	}
}
