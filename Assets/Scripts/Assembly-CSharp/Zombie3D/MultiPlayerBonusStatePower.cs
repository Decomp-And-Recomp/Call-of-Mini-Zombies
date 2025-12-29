using UnityEngine;

namespace Zombie3D
{
	public class MultiPlayerBonusStatePower : PlayerBonusState
	{
		private GameObject powerEffect;

		public MultiPlayerBonusStatePower()
		{
			stateType = PlayerBonusStateType.PowerUp;
		}

		public override void EnterState(Player player)
		{
			player.PowerBuff = 2f;
			powerEffect = Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().powerEffect, player.GetTransform().position, Quaternion.identity) as GameObject;
			powerEffect.transform.parent = player.GetTransform();
			powerEffect.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
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
