using UnityEngine;

namespace Zombie3D
{
	public class Dog : Enemy
	{
		protected Collider headCollider;

		protected Vector3 targetPosition;

		protected Vector3[] p = new Vector3[4];

		protected void RandomRunAnimation()
		{
			runAnimationName = "Run01";
		}

		public override void Init(GameObject gObject)
		{
			if (base.IsElite)
			{
				m_tip_height = 2.8f;
			}
			else
			{
				m_tip_height = 1.8f;
			}
			base.Init(gObject);
			headCollider = enemyTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/Bip01 HeadNub").gameObject.GetComponent<Collider>();
			lastTarget = Vector3.zero;
			RandomRunAnimation();
			ComputeAttributes(gConfig.GetMonsterConfig("Dog"));
			if (base.IsElite)
			{
				mAttributes.attackRange = 2f;
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
					if (collider != null && headCollider.bounds.Intersects(collider.bounds))
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
						if (collider2 != null && headCollider.bounds.Intersects(collider2.bounds))
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

		public override void OnDead()
		{
			audio.PlayAudio("Dead");
			base.OnDead();
		}
	}
}
