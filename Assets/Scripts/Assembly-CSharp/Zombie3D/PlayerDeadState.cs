namespace Zombie3D
{
	public class PlayerDeadState : PlayerState
	{
		private float vs_rebirth_time = 6f;

		public PlayerDeadState()
		{
			state_type = PlayerStateType.Dead;
		}

		public override void EnterState(Player player)
		{
			vs_rebirth_time = 5f;
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			player.ZoomOut(deltaTime);
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				vs_rebirth_time -= deltaTime;
				if (vs_rebirth_time <= 0f)
				{
					player.OnVSRebirth();
				}
			}
		}

		public override void ExitState(Player player)
		{
		}

		public override void OnHit(Player player)
		{
		}
	}
}
