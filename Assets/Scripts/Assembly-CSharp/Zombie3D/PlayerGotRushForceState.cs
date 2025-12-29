using UnityEngine;

namespace Zombie3D
{
	public class PlayerGotRushForceState : PlayerState
	{
		private const float gravity = 10f;

		public Vector2 force;

		private float moveDirY;

		private float startTime;

		public PlayerGotRushForceState()
		{
			state_type = PlayerStateType.GotRushHit;
		}

		public override void EnterState(Player player)
		{
			player.Animate("Damage01", WrapMode.Once);
			player.StopFire();
			moveDirY = 4f;
			startTime = Time.time;
		}

		public override void DoStateLogic(Player player, float deltaTime)
		{
			if (moveDirY > 0f)
			{
				moveDirY -= 10f * deltaTime * 2f;
			}
			else
			{
				moveDirY -= 10f * deltaTime * 1.5f;
			}
			Vector3 dir = new Vector3(force.x, moveDirY, force.y);
			player.Move(deltaTime, dir);
			if (moveDirY < 0f && player.GetTransform().position.y < 10000.5f)
			{
				player.SetState(PlayerStateType.Idle);
			}
			else if (Time.time - startTime > 1.5f)
			{
				player.SetState(PlayerStateType.Idle);
			}
		}

		public override void ExitState(Player player)
		{
		}

		public override void OnHit(Player player)
		{
			if (player.HP <= 0f)
			{
				player.SetState(PlayerStateType.Dead);
				player.OnDead();
			}
			player.CreateScreenBlood();
		}
	}
}
