using UnityEngine;

namespace Zombie3D
{
	public class PreyGoneState : EnemyState
	{
		public override void NextState(Enemy enemy, float deltaTime, Player player)
		{
			enemy.Animate("Idle01", WrapMode.Loop);
			if (enemy.MoveToMucilage(deltaTime))
			{
				enemy.SetState(Enemy.IDLE_STATE);
				enemy.SetInPreyGone(false);
			}
		}

		public override void OnHit(Enemy enemy)
		{
		}
	}
}
