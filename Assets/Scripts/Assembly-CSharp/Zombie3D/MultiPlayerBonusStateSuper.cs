using UnityEngine;

namespace Zombie3D
{
	public class MultiPlayerBonusStateSuper : PlayerBonusState
	{
		protected float maxHpBeforeChange;

		protected Vector3 scaleBeforeChange;

		protected float hpChangeRatio;

		private GameObject hens;

		public MultiPlayerBonusStateSuper()
		{
			stateType = PlayerBonusStateType.Super;
		}

		public override void EnterState(Player player)
		{
			maxHpBeforeChange = player.MaxHp;
			player.MaxHp = 50000f;
			hpChangeRatio = 50000f / maxHpBeforeChange;
			player.HP *= hpChangeRatio;
			scaleBeforeChange = new Vector3(1f, 1f, 1f);
			PlayerScaleAnimationScript component = player.PlayerObject.GetComponent<PlayerScaleAnimationScript>();
			if (component != null)
			{
				component.smallToBig = true;
				component.targetScale = scaleBeforeChange * 2f;
			}
			else
			{
				component = player.PlayerObject.AddComponent<PlayerScaleAnimationScript>();
				component.enabled = true;
				component.scaleSpeed = 1f;
				component.smallToBig = true;
				component.targetScale = scaleBeforeChange * 2f;
			}
			hens = Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().superHens, player.GetTransform().position + player.GetTransform().forward, Quaternion.LookRotation(Vector3.up)) as GameObject;
			hens.transform.parent = player.GetTransform();
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
		}

		public override void ExitState(Player player)
		{
			player.MaxHp = maxHpBeforeChange;
			player.HP /= hpChangeRatio;
			PlayerScaleAnimationScript component = player.PlayerObject.GetComponent<PlayerScaleAnimationScript>();
			if (component != null)
			{
				component.enabled = true;
				component.scaleSpeed = 1f;
				component.smallToBig = false;
				component.targetScale = scaleBeforeChange;
			}
			Object.Destroy(hens);
		}
	}
}
