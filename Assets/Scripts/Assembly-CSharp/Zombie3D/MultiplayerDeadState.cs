namespace Zombie3D
{
	public class MultiplayerDeadState : PlayerState
	{
		public MultiplayerDeadState()
		{
			state_type = PlayerStateType.Dead;
		}

		public override void EnterState(Player player)
		{
			player.StopFire();
			player.OnDead();
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
		}

		public override void ExitState(Player player)
		{
		}
	}
}
