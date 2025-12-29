using UnityEngine;

namespace Zombie3D
{
	public class AttackState : EnemyState
	{
		public override void NextState(Enemy enemy, float deltaTime, Player player)
		{
			if (enemy.Attributes.Hp <= 0.0)
			{
				enemy.OnDead();
				enemy.SetState(Enemy.DEAD_STATE);
				return;
			}
			if (enemy.CouldMakeNextAttack())
			{
				enemy.OnAttack();
			}
			else if (enemy.AttackAnimationEnds())
			{
				enemy.Animate("Idle01", WrapMode.Loop);
			}
			enemy.CheckHit();
			if (enemy.SqrDistanceFromPlayer >= enemy.Attributes.attackRange * enemy.Attributes.attackRange && enemy.AttackAnimationEnds())
			{
				enemy.SetState(Enemy.CATCHING_STATE);
			}
		}
	}
}
