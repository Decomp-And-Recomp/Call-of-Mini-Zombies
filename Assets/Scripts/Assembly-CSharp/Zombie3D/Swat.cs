using UnityEngine;

namespace Zombie3D
{
	public class Swat : Enemy
	{
		protected GameObject hitParticles;

		protected LineRenderer lineR;

		public override void Init(GameObject gObject)
		{
			m_tip_height = 2f;
			base.Init(gObject);
			hitParticles = rConfig.hitparticles;
			lineR = enemyObject.GetComponent<Renderer>() as LineRenderer;
			mAttributes.attackRange = 14f;
			RandomRunAnimation();
			ComputeAttributes(gConfig.GetMonsterConfig("Swat"));
			if (base.IsElite)
			{
				mAttributes.MoveSpeed += 2f;
				animation[runAnimationName].speed = 1.2f;
			}
		}

		public override bool CouldEnterAttackState()
		{
			if (base.CouldEnterAttackState())
			{
				if (Mathf.Abs(enemyTransform.position.y - player.GetTransform().position.y) < 2f)
				{
					return true;
				}
				return false;
			}
			return false;
		}

		protected void RandomRunAnimation()
		{
			runAnimationName = "Run";
		}

		public override void CheckHit()
		{
			if (!attacked && IsAnimationPlayedPercentage("Attack01", 0.6f))
			{
				Vector3 vector = new Vector3(enemyTransform.position.x, enemyTransform.position.y, enemyTransform.position.z);
				float num = -1f;
				if (player == null)
				{
					return;
				}
				Vector3 vector2 = new Vector3(player.GetTransform().position.x, enemyTransform.position.y, player.GetTransform().position.z) - vector;
				float magnitude = vector2.magnitude;
				float num2 = 5f;
				float num3 = magnitude / num2;
				float num4 = (num - 0.5f * Physics.gravity.y * 0.5f * num3 * num3) / num3;
				Vector3 vector3 = Vector3.up * num4 + vector2.normalized * num2;
				GameObject gameObject = Object.Instantiate(rConfig.copBomb, vector + Vector3.up * (0f - num), Quaternion.LookRotation(-vector3)) as GameObject;
				CopBombScript component = gameObject.GetComponent<CopBombScript>();
				component.damage = mAttributes.AttackDamage;
				gameObject.GetComponent<Rigidbody>().AddForce(vector3, ForceMode.Impulse);
				attacked = true;
			}
			base.CheckHit();
		}

		public override void OnAttack()
		{
			base.OnAttack();
			Animate("Attack01", WrapMode.ClampForever);
			enemyTransform.LookAt(player.GetTransform());
			attacked = false;
			lastAttackTime = Time.time;
		}
	}
}
