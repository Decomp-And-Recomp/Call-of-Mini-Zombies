using UnityEngine;

namespace Zombie3D
{
	public class RoarState : EnemyState
	{
		public override void NextState(Enemy enemy, float deltaTime, Player player)
		{
			if (enemy.Attributes.Hp <= 0.0)
			{
				enemy.OnDead();
				enemy.SetState(Enemy.DEAD_STATE);
				return;
			}
			enemy.Animate("Roar", WrapMode.Once);
			enemy.Audio.PlayAudio("Shout");
			if (enemy.IsAnimationPlayedPercentage("Roar", 0.98f))
			{
				enemy.SetState(Enemy.IDLE_STATE);
			}
			else if (enemy.IsAnimationPlayedPercentage("Roar", 0.5f))
			{
				(enemy as SuperDinosaur).CallTinyDino();
			}
		}

		public override void OnHit(Enemy enemy)
		{
		}
	}
}
