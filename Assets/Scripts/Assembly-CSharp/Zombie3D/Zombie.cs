using UnityEngine;

namespace Zombie3D
{
	public class Zombie : Enemy
	{
		protected Collider handCollider;

		protected Vector3 targetPosition;

		protected void RandomRunAnimation()
		{
			int num = Random.Range(0, 10);
			if (num < 7)
			{
				runAnimationName = "Run";
			}
			else
			{
				switch (num)
				{
				case 7:
					runAnimationName = "Forward01";
					break;
				case 8:
					runAnimationName = "Forward02";
					break;
				case 9:
					runAnimationName = "Forward03";
					break;
				}
			}
			if (base.IsElite)
			{
				runAnimationName = "Run";
			}
		}

		public override void Init(GameObject gObject)
		{
			m_tip_height = 2f;
			base.Init(gObject);
			handCollider = enemyTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand").gameObject.GetComponent<Collider>();
			lastTarget = Vector3.zero;
			RandomRunAnimation();
			ComputeAttributes(gConfig.GetMonsterConfig("Zombie"));
			if (base.IsElite)
			{
				mAttributes.MoveSpeed += 2f;
				animation[runAnimationName].speed = 1.5f;
			}
			TimerManager.GetInstance().SetTimer(0, 8f, true);
		}

		public override void CheckHit()
		{
			if (!attacked && IsAnimationPlayedPercentage("Attack01", 0.4f))
			{
				if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Coop)
				{
					Collider collider = player.Collider;
					if (collider != null && handCollider.bounds.Intersects(collider.bounds))
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
						if (collider2 != null && handCollider.bounds.Intersects(collider2.bounds))
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
			if (TimerManager.GetInstance().Ready(0))
			{
				audio.PlayAudio("Shout");
				TimerManager.GetInstance().Do(0);
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
