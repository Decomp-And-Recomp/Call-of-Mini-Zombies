using UnityEngine;

namespace Zombie3D
{
	public class HellFirer : Enemy
	{
		protected Vector3 targetPosition;

		protected Vector3[] p = new Vector3[4];

		protected GameObject Hell_Fire;

		protected ParticleEmitter FireDream;

		protected ParticleEmitter FireHeart1;

		protected ParticleEmitter FireHeart2;

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
			FireDream = gameObject.GetComponent<ParticleEmitter>();
			gameObject = Hell_Fire.transform.Find("gun_fire_new/hellfire/hellfire_02").gameObject;
			FireHeart1 = gameObject.GetComponent<ParticleEmitter>();
			gameObject = Hell_Fire.transform.Find("gun_fire_new/hellfire/hellfire_03").gameObject;
			FireHeart2 = gameObject.GetComponent<ParticleEmitter>();
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
			FireDream.emit = true;
			FireHeart1.emit = true;
			FireHeart2.emit = true;
		}

		public void StopFire()
		{
			FireDream.emit = false;
			FireHeart1.emit = false;
			FireHeart2.emit = false;
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
