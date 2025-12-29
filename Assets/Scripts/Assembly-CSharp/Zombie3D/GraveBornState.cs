using UnityEngine;

namespace Zombie3D
{
	public class GraveBornState : EnemyState
	{
		public override void NextState(Enemy enemy, float deltaTime, Player player)
		{
			if (enemy.Attributes.Hp <= 0.0)
			{
				enemy.OnDead();
				enemy.SetState(Enemy.DEAD_STATE);
			}
			enemy.Animate("Idle01", WrapMode.Loop);
			if (enemy.MoveFromGrave(deltaTime))
			{
				enemy.SetInGrave(false);
				enemy.SetState(Enemy.IDLE_STATE);
			}
		}
	}
}
