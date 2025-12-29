using UnityEngine;

namespace Zombie3D
{
	public class RushingStartState : EnemyState
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
					enemy.Audio.PlayAudio("Shout");
					if (enemy.IsAnimationPlayedPercentage("Rush01", 1f))
					{
						enemy.Animate("Rush02", WrapMode.Loop);
						enemy.SetState(Tank.RUSHING_STATE);
					}
				}
				else
				{
					if (enemy.EnemyType != EnemyType.E_SUPER_DINO)
					{
						return;
					}
					if ((enemy as SuperDinosaur).specialState == SuperDinosaur.SpecialState.Dash)
					{
						if (enemy.IsAnimationPlayedPercentage("Rush01", 1f))
						{
							enemy.Animate("Rush02", WrapMode.Loop);
							enemy.SetState(SuperDinosaur.RUSHING_STATE);
						}
					}
					else if ((enemy as SuperDinosaur).specialState == SuperDinosaur.SpecialState.Rush && enemy.IsAnimationPlayedPercentage("Ready", 1f))
					{
						enemy.Animate("Rush", WrapMode.Loop);
						enemy.SetState(SuperDinosaur.RUSHING_STATE);
					}
				}
			}
		}

		public override void OnHit(Enemy enemy)
		{
		}
	}
}
