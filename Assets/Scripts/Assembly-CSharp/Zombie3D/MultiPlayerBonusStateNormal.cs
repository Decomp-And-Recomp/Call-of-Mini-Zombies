namespace Zombie3D
{
	public class MultiPlayerBonusStateNormal : PlayerBonusState
	{
		public MultiPlayerBonusStateNormal()
		{
			stateType = PlayerBonusStateType.Normal;
		}

		public override void EnterState(Player player)
		{
			player.PowerBuff = 1f;
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
		}

		public override void ExitState(Player player)
		{
		}
	}
}
