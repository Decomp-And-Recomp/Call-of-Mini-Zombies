using UnityEngine;

namespace Zombie3D
{
	public class MultiplayerIdleState : PlayerState
	{
		Vector3 _ref;

		public MultiplayerIdleState()
		{
			state_type = PlayerStateType.Idle;
		}

		public override void EnterState(Player player)
		{
			player.StopFire();
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			//player.Move(deltaTime);
			Transform t = player.GetTransform();
			t.position = Vector3.SmoothDamp(t.position, (player as Multiplayer).GetPosTo(), ref _ref, 0.2f);
			player.ResetSawAnimation();
			player.Animate("Idle01" + player.WeaponNameEnd, WrapMode.Loop);
		}

		public override void ExitState(Player player)
		{
		}
	}
}
