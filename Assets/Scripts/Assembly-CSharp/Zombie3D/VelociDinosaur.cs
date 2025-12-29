using UnityEngine;

namespace Zombie3D
{
	public class VelociDinosaur : Enemy
	{
		protected Collider headCollider;

		protected Vector3 targetPosition;

		protected Vector3[] p = new Vector3[4];

		public override void Init(GameObject gObject)
		{
			m_tip_height = 2f;
			base.Init(gObject);
			headCollider = enemyTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Neck/Bip01 Neck1/Bip01 Head/Bip01 HeadNub").gameObject.GetComponent<Collider>();
			aimedTransform = enemyTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Neck/Bip01 Neck1/Bip01 Head");
			runAnimationName = "Run";
			animation["Damage"].speed = 2.5f;
			ComputeAttributes(gConfig.GetMonsterConfig("Veloci"));
			TimerManager.GetInstance().SetTimer(7, 8f, true);
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
			if (TimerManager.GetInstance().Ready(7))
			{
				audio.PlayAudio("Shout");
				TimerManager.GetInstance().Do(7);
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
