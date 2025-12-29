using UnityEngine;

namespace Zombie3D
{
	public class CatchingState : EnemyState
	{
		public override void NextState(Enemy enemy, float deltaTime, Player player)
		{
			if (enemy.Attributes.Hp <= 0.0)
			{
				enemy.OnDead();
				enemy.SetState(Enemy.DEAD_STATE);
				return;
			}
			enemy.FindPath();
			enemy.DoMove(deltaTime);
			enemy.Animate(enemy.RunAnimationName, WrapMode.Loop);
			EnemyState enemyState = enemy.EnterSpecialState(deltaTime);
			if (enemyState != null)
			{
				enemy.SetState(enemyState);
			}
			else if (enemy.CouldEnterAttackState())
			{
				enemy.SetState(Enemy.ATTACK_STATE);
			}
		}
	}
}
