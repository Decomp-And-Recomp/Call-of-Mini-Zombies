using UnityEngine;

namespace Zombie3D
{
	public class Nurse : Enemy
	{
		protected GameObject hitParticles;

		protected LineRenderer lineR;

		protected void RandomRunAnimation()
		{
			int num = Random.Range(0, 100);
			if (num < 50)
			{
				runAnimationName = "Run";
			}
			else if (num < 75)
			{
				runAnimationName = "Forward01";
			}
			else
			{
				runAnimationName = "Forward02";
			}
		}

		public override void Init(GameObject gObject)
		{
			m_tip_height = 2f;
			base.Init(gObject);
			hitParticles = rConfig.hitparticles;
			lineR = enemyObject.GetComponent<Renderer>() as LineRenderer;
			mAttributes.attackRange = 10f;
			RandomRunAnimation();
			ComputeAttributes(gConfig.GetMonsterConfig("Nurse"));
			if (base.IsElite)
			{
				mAttributes.MoveSpeed += 1.5f;
				animation[runAnimationName].speed = 1.2f;
			}
			TimerManager.GetInstance().SetTimer(1, 12f, true);
		}

		public override void DoLogic(float deltaTime)
		{
			base.DoLogic(deltaTime);
			if (TimerManager.GetInstance().Ready(1))
			{
				audio.PlayAudio("Shout");
				TimerManager.GetInstance().Do(1);
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

		public override void OnAttack()
		{
			base.OnAttack();
			Animate("Attack01", WrapMode.ClampForever);
			enemyTransform.LookAt(player.GetTransform());
			Vector3 vector = new Vector3(enemyTransform.position.x, enemyTransform.position.y, enemyTransform.position.z);
			float num = -1f;
			Vector3 vector2 = new Vector3(player.GetTransform().position.x, enemyTransform.position.y, player.GetTransform().position.z) - vector;
			float magnitude = vector2.magnitude;
			float num2 = 12f;
			float num3 = magnitude / num2;
			float num4 = (num - 0.5f * Physics.gravity.y * num3 * num3) / num3;
			Vector3 vector3 = Vector3.up * num4 + vector2.normalized * num2;
			GameObject gameObject = Object.Instantiate(rConfig.nurseSalivaProjectile, vector + Vector3.up * (0f - num), Quaternion.LookRotation(-vector3)) as GameObject;
			ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
			component.dir = vector3;
			component.speed = vector3;
			component.explodeRadius = 2f;
			component.hitObjectEffect = GameApp.GetInstance().GetGameResourceConfig().salivaExplosion;
			component.hitForce = 0f;
			component.GunType = WeaponType.NurseSaliva;
			component.damage = mAttributes.AttackDamage;
			lastAttackTime = Time.time;
			Object.Instantiate(rConfig.salivaExplosion, enemyTransform.position + Vector3.up * 0.5f, Quaternion.identity);
		}
	}
}
