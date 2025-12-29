using UnityEngine;

namespace Zombie3D
{
	public class DiloDinosaur : Enemy
	{
		private Transform mouthTrans;

		public override void Init(GameObject gObject)
		{
			m_tip_height = 1.8f;
			base.Init(gObject);
			mAttributes.attackRange = 10f;
			runAnimationName = "Run";
			animation["Damage"].speed = 2.5f;
			mouthTrans = enemyTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Neck/Bip01 Neck1/Bip01 Head/Bip01 Mouth");
			aimedTransform = enemyTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Neck/Bip01 Neck1/Bip01 Head");
			ComputeAttributes(gConfig.GetMonsterConfig("Dilo"));
			TimerManager.GetInstance().SetTimer(7, 8f, true);
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
			float num = enemyTransform.position.y - mouthTrans.position.y;
			Vector3 vector = new Vector3(player.GetTransform().position.x - enemyTransform.position.x, 0f, player.GetTransform().position.z - enemyTransform.position.z);
			float magnitude = vector.magnitude;
			float num2 = 12f;
			float num3 = magnitude / num2;
			float num4 = (num - 0.5f * Physics.gravity.y * num3 * num3) / num3;
			Vector3 vector2 = Vector3.up * num4 + vector.normalized * num2;
			Object.Instantiate(rConfig.diloVenom, mouthTrans.position, Quaternion.LookRotation(vector2));
			GameObject gameObject = Object.Instantiate(rConfig.diloVenomProjectile, mouthTrans.position, Quaternion.LookRotation(vector2)) as GameObject;
			ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
			component.dir = vector2;
			component.speed = vector2;
			component.explodeRadius = 2f;
			component.hitObjectEffect = GameApp.GetInstance().GetGameResourceConfig().diloVenomExplosion;
			component.hitForce = 0f;
			component.GunType = WeaponType.DiloSpitFire;
			component.damage = mAttributes.AttackDamage;
			lastAttackTime = Time.time;
		}
	}
}
