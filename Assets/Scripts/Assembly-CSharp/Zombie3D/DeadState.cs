using UnityEngine;

namespace Zombie3D
{
	public class DeadState : EnemyState
	{
		public override void NextState(Enemy enemy, float deltaTime, Player player)
		{
			if (Time.time - startTime >= 2f)
			{
				enemy.RemoveDeadbodyCheck();
			}
		}

		public override void OnHit(Enemy enemy)
		{
		}
	}
}
