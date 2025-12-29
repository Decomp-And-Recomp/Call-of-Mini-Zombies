using UnityEngine;

namespace Zombie3D
{
	public class PlayerBonusStatePower : PlayerBonusState
	{
		protected float lastTime;

		private GameObject powerEffect;

		public PlayerBonusStatePower()
		{
			stateType = PlayerBonusStateType.PowerUp;
		}

		public override void EnterState(Player player)
		{
			lastTime = GameApp.GetInstance().GetGameState().GetItemByType(ItemType.Power)
				.iConf.lastDuration;
			player.PowerBuff = 2f;
			powerEffect = Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().powerEffect, player.GetTransform().position, Quaternion.identity) as GameObject;
			powerEffect.transform.parent = player.GetTransform();
			powerEffect.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			if (!CheckPlayerInDeadState(player))
			{
				lastTime -= deltaTime;
				if (lastTime < 0f)
				{
					player.SetBonusState(PlayerBonusStateType.Normal);
				}
			}
		}

		public override void ExitState(Player player)
		{
			player.PowerBuff = 1f;
			if (powerEffect != null)
			{
				Object.Destroy(powerEffect);
			}
		}
	}
}
