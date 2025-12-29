using UnityEngine;

namespace Zombie3D
{
	public class PlayerIdleState : PlayerState
	{
		public PlayerIdleState()
		{
			state_type = PlayerStateType.Idle;
		}

		public override void EnterState(Player player)
		{
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			player.ResetSawAnimation();
			player.Animate("Idle01" + player.WeaponNameEnd, WrapMode.Loop);
			Weapon weapon = player.GetWeapon();
			if (!player.InputController.InputInfo.fire)
			{
				player.ZoomOut(deltaTime);
				if (player.InputController.InputInfo.IsMoving)
				{
					player.SetState(PlayerStateType.Run);
				}
			}
			else if (weapon.GetWeaponType() != WeaponType.SuicideGun)
			{
				if (!player.InputController.InputInfo.IsMoving)
				{
					player.SetState(PlayerStateType.Shoot);
				}
				else
				{
					player.SetState(PlayerStateType.RunShoot);
				}
			}
		}

		public override void ExitState(Player player)
		{
		}
	}
}
