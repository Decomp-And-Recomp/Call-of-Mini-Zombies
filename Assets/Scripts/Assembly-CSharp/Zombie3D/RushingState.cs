namespace Zombie3D
{
	public class RushingState : EnemyState
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
					if ((enemy as Tank).Rush(deltaTime))
					{
						enemy.SetState(Tank.RUSHINGATTACK_STATE);
					}
				}
				else if (enemy.EnemyType == EnemyType.E_SUPER_DINO)
				{
					(enemy as SuperDinosaur).Rush(deltaTime);
				}
			}
		}

		public override void OnHit(Enemy enemy)
		{
		}
	}
}
