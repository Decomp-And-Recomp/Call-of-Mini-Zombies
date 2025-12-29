using UnityEngine;

namespace Zombie3D
{
	public class PlayerGotHitState : PlayerState
	{
		public PlayerGotHitState()
		{
			state_type = PlayerStateType.GotHit;
		}

		public override void EnterState(Player player)
		{
			if (player.GetWeapon().GetWeaponType() != WeaponType.ElectricGun)
			{
				player.StopFire();
			}
			player.Animate("Damage01", WrapMode.Once);
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			if (!player.IsPlayingAnimation("Damage01"))
			{
				player.SetState(PlayerStateType.Idle);
			}
		}

		public override void ExitState(Player player)
		{
		}
	}
}
