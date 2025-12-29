using UnityEngine;

namespace Zombie3D
{
	public class GotHitState : EnemyState
	{
		public override void NextState(Enemy enemy, float deltaTime, Player player)
		{
			if (enemy.Attributes.Hp <= 0.0)
			{
				enemy.OnDead();
				enemy.SetState(Enemy.DEAD_STATE);
				return;
			}
			enemy.Animate("Damage", WrapMode.Once);
			string empty = string.Empty;
			empty = ((enemy.EnemyType != EnemyType.E_HELL_FIRER) ? "Damage" : "Damage01");
			float num = enemy.GetEnemyObject().GetComponent<Animation>()[empty].clip.length / enemy.GetEnemyObject().GetComponent<Animation>()[empty].speed;
			if (Time.time - startTime > num)
			{
				if (enemy.EnemyType == EnemyType.E_SUPER_DINO)
				{
					enemy.SetState(SuperDinosaur.ROAR_SATE);
				}
				else
				{
					enemy.SetState(Enemy.IDLE_STATE);
				}
			}
		}

		public override void OnHit(Enemy enemy)
		{
		}
	}
}
