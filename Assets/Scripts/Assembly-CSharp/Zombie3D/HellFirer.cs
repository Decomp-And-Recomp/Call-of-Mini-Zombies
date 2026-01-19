using UnityEngine;

namespace Zombie3D
{
	public class HellFirer : Enemy
	{
		protected Vector3 targetPosition;

		protected Vector3[] p = new Vector3[4];

		protected GameObject Hell_Fire;

		protected ParticleSystem FireDream;

		protected ParticleSystem FireHeart1;

		protected ParticleSystem FireHeart2;

		public override void Init(GameObject gObject)
		{
			m_tip_height = 2f;
			base.Init(gObject);
			lastTarget = Vector3.zero;
			runAnimationName = "Forward01";
			mAttributes.attackRange = 7f;
			ComputeAttributes(gConfig.GetMonsterConfig("HellFirer"));
			if (base.IsElite)
			{
				mAttributes.MoveSpeed += 2f;
				animation[runAnimationName].speed = 1.5f;
			}
			Hell_Fire = enemyTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Weapon_Dummy/FireGun").gameObject;
			GameObject gameObject = Hell_Fire.transform.Find("gun_fire_new/hellfire/hellfire_01").gameObject;
			gameObject.GetComponent<HellFireEnemyScript>().damage = mAttributes.AttackDamage;
			FireDream = gameObject.GetComponent<ParticleSystem>();
			gameObject = Hell_Fire.transform.Find("gun_fire_new/hellfire/hellfire_02").gameObject;
			FireHeart1 = gameObject.GetComponent<ParticleSystem>();
			gameObject = Hell_Fire.transform.Find("gun_fire_new/hellfire/hellfire_03").gameObject;
			FireHeart2 = gameObject.GetComponent<ParticleSystem>();
		}

		public override void OnAttack()
		{
			base.OnAttack();
			Animate("Fire01", WrapMode.ClampForever);
			attacked = false;
			lastAttackTime = Time.time;
			Fire();
		}

		public override void DoMove(float deltaTime)
		{
			enemyTransform.Translate(moveDirection * mAttributes.MoveSpeed * deltaTime, Space.World);
			StopFire();
		}

		public void Fire()
		{
			enemyTransform.LookAt(player.GetTransform());
			FireDream.Play(true);
			FireHeart1.Play(true);
			FireHeart2.Play(true);
		}

		public void StopFire()
		{
			FireDream.Stop(true);
			FireHeart1.Stop(true);
			FireHeart2.Stop(true);
		}

		public override void Animate(string animationName, WrapMode wrapMode)
		{
			if (animationName == "Damage")
			{
				animationName = "Damage01";
			}
			animation[animationName].wrapMode = wrapMode;
			if (!animation.IsPlaying("Damage01"))
			{
				if (wrapMode == WrapMode.Loop || (!animation.IsPlaying(animationName) && animationName != "Damage01"))
				{
					animation.CrossFade(animationName);
					return;
				}
				animation.Stop();
				animation.Play(animationName);
			}
		}

		public override bool AttackAnimationEnds()
		{
			if (Time.time - lastAttackTime > animation["Fire01"].clip.length)
			{
				return true;
			}
			return false;
		}
	}
}
