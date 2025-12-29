namespace Zombie3D
{
	public class MultiplayerGotHitState : PlayerState
	{
		public MultiplayerGotHitState()
		{
			state_type = PlayerStateType.GotHit;
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
