using UnityEngine;

namespace Zombie3D
{
	public class MultiplayerRunState : PlayerState
	{
		public MultiplayerRunState()
		{
			state_type = PlayerStateType.Run;
		}

		public override void EnterState(Player player)
		{
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			Multiplayer multiplayer = player as Multiplayer;
			if (multiplayer.m_is_lerp_position)
			{
				player.Move(Mathf.Lerp(deltaTime + player.net_ping_sum, deltaTime, deltaTime));
			}
			else
			{
				player.Move(deltaTime);
			}
			player.StopFire();
			player.ResetSawAnimation();
			player.Animate("Run01" + player.WeaponNameEnd, WrapMode.Loop);
		}

		public override void ExitState(Player player)
		{
		}
	}
}
