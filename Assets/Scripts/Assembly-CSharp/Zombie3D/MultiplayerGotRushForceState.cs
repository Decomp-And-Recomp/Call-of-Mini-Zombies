namespace Zombie3D
{
	public class MultiplayerGotRushForceState : PlayerState
	{
		public MultiplayerGotRushForceState()
		{
			state_type = PlayerStateType.GotRushHit;
		}

		public override void EnterState(Player player)
		{
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
		}

		public override void ExitState(Player player)
		{
		}
	}
}
