namespace Zombie3D
{
	public class RushingAttackState : EnemyState
	{
		public override void NextState(Enemy enemy, float deltaTime, Player player)
		{
			if (enemy.Attributes.Hp <= 0.0)
			{
				enemy.OnDead();
				enemy.SetState(Enemy.DEAD_STATE);
			}
			else
			{
				if (enemy == null)
				{
					return;
				}
				if (enemy.EnemyType == EnemyType.E_TANK)
				{
					if ((enemy as Tank).RushAttack(deltaTime))
					{
						enemy.SetState(Enemy.IDLE_STATE);
					}
				}
				else if (enemy.EnemyType == EnemyType.E_SUPER_DINO)
				{
					enemy.Audio.PlayAudio("Dash");
					if ((enemy as SuperDinosaur).RushAttack(deltaTime))
					{
						enemy.SetState(Enemy.IDLE_STATE);
					}
				}
			}
		}

		public override void OnHit(Enemy enemy)
		{
		}
	}
}
