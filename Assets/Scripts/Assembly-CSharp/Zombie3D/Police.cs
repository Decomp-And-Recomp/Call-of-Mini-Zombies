using UnityEngine;

namespace Zombie3D
{
	public class Police : Enemy
	{
		protected Collider shieldCollider;

		protected Vector3 targetPosition;

		protected Vector3[] p = new Vector3[4];

		protected void RandomRunAnimation()
		{
			runAnimationName = "Run01";
		}

		public override void Init(GameObject gObject)
		{
			m_tip_height = 2f;
			base.Init(gObject);
			shieldCollider = enemyTransform.Find("Bip01/Bip01 Prop1/Shield").gameObject.GetComponent<Collider>();
			lastTarget = Vector3.zero;
			RandomRunAnimation();
			ComputeAttributes(gConfig.GetMonsterConfig("Police"));
			if (base.IsElite)
			{
				mAttributes.MoveSpeed += 2f;
				animation[runAnimationName].speed = 1.5f;
			}
			TimerManager.GetInstance().SetTimer(6, 8f, true);
		}

		public override void CheckHit()
		{
			if (!attacked && IsAnimationPlayedPercentage("Attack01", 0.4f))
			{
				if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Coop)
				{
					Collider collider = player.Collider;
					if (collider != null && shieldCollider.bounds.Intersects(collider.bounds))
					{
						player.OnHit(mAttributes.AttackDamage);
						attacked = true;
					}
				}
				else
				{
					foreach (Player item in GameApp.GetInstance().GetGameScene().m_multi_player_arr)
					{
						Collider collider2 = item.Collider;
						if (collider2 != null && shieldCollider.bounds.Intersects(collider2.bounds))
						{
							item.OnHit(mAttributes.AttackDamage);
							attacked = true;
							break;
						}
					}
				}
			}
			base.CheckHit();
		}

		public override void DoLogic(float deltaTime)
		{
			base.DoLogic(deltaTime);
			if (TimerManager.GetInstance().Ready(6))
			{
				audio.PlayAudio("Shout");
				TimerManager.GetInstance().Do(6);
			}
		}

		public override void OnAttack()
		{
			base.OnAttack();
			Animate("Attack01", WrapMode.ClampForever);
			attacked = false;
			lastAttackTime = Time.time;
		}
	}
}
