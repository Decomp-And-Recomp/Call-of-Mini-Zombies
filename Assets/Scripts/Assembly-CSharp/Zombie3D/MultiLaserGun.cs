using UnityEngine;

namespace Zombie3D
{
	public class MultiLaserGun : Weapon
	{
		protected float flySpeed;

		private GameObject laserObj;

		protected Vector3 laserStartScale;

		protected float lastLaserHitInitiatTime;

		public MultiLaserGun()
		{
			maxCapacity = 9999;
			base.IsSelectedForBattle = true;
		}

		public override void Init()
		{
			base.MultiInit();
			gunfire = gun.transform.Find("gun_fire_new").gameObject;
		}

		public override void LoadConfig()
		{
			base.LoadConfig();
			flySpeed = base.WConf.flySpeed;
		}

		public void PlayShootAudio()
		{
			if (shootAudio != null)
			{
				AudioPlayer.PlayAudio(shootAudio, true);
			}
		}

		public void SetShootTimeNow()
		{
			lastShootTime = Time.time;
		}

		public override void FireUpdate(float deltaTime)
		{
			if (!(laserObj != null))
			{
				return;
			}
			Vector3 vector = gunfire.transform.TransformPoint(0f, 0f, 50f);
			Ray ray = new Ray(gunfire.transform.position, vector - gunfire.transform.position);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 1000f, 34816))
			{
				aimTarget = hitInfo.point;
			}
			else
			{
				aimTarget = gunfire.transform.TransformPoint(0f, 0f, 1000f);
			}
			Vector3 normalized = (aimTarget - gunfire.transform.position).normalized;
			float magnitude = (aimTarget - gunfire.transform.position).magnitude;
			laserObj.transform.position = gunfire.transform.position;
			laserObj.transform.LookAt(aimTarget);
			if (hitInfo.collider != null)
			{
				laserObj.transform.localScale = new Vector3(laserObj.transform.localScale.x, laserObj.transform.localScale.y, magnitude / 48.76f);
			}
			if (Time.time - lastLaserHitInitiatTime > 0.03f && (aimTarget - normalized - gunfire.transform.position).sqrMagnitude > 9f)
			{
				Object.Instantiate(rConf.laserHit, aimTarget, Quaternion.identity);
				lastLaserHitInitiatTime = Time.time;
			}
			if (!CouldMakeNextShoot())
			{
				return;
			}
			if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Vs)
			{
				GameObject[] woodBoxes = gameScene.GetWoodBoxes();
				GameObject[] array = woodBoxes;
				foreach (GameObject gameObject in array)
				{
					if (gameObject != null && laserObj.GetComponent<Collider>().bounds.Intersects(gameObject.GetComponent<Collider>().bounds))
					{
						WoodBoxScript component = gameObject.GetComponent<WoodBoxScript>();
						component.OnHit(0f);
					}
				}
			}
			if (shootAudio != null && !shootAudio.isPlaying)
			{
				AudioPlayer.PlayAudio(shootAudio, true);
			}
			lastShootTime = Time.time;
		}

		public override void Fire(float deltaTime)
		{
			if (gunfire.TryGetComponent(out Renderer gunfireRenderer)) gunfireRenderer.enabled = true;
			else if (gunfire.TryGetComponent(out ParticleSystem gunfireParticles)) gunfireParticles.Play();

			Vector3 vector = gunfire.transform.TransformPoint(0f, 0f, 50f);
			Ray ray = new Ray(gunfire.transform.position, vector - gunfire.transform.position);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 1000f, 35328))
			{
				aimTarget = hitInfo.point;
			}
			else
			{
				aimTarget = gunfire.transform.TransformPoint(0f, 0f, 1000f);
			}
			Vector3 normalized = (aimTarget - gunfire.transform.position).normalized;
			if (laserObj == null)
			{
				laserObj = Object.Instantiate(rConf.laser, gunfire.transform.position, Quaternion.LookRotation(normalized)) as GameObject;
				laserStartScale = laserObj.transform.localScale;
				ProjectileScript component = laserObj.GetComponent<ProjectileScript>();
				component.dir = normalized;
				component.flySpeed = flySpeed;
				component.explodeRadius = 0f;
				component.hitObjectEffect = GameApp.GetInstance().GetGameResourceConfig().laserHit;
				component.hitForce = mAttributes.hitForce;
				component.life = 8f;
				component.damage = 0f;
				component.GunType = WeaponType.LaserGun;
				component.player = player;
			}
			lastShootTime = Time.time;
		}

		public override void StopFire()
		{
			if (laserObj != null)
			{
				Object.Destroy(laserObj);
				laserObj = null;
			}
			if (shootAudio != null)
			{
				shootAudio.Stop();
			}
			if (gunfire != null)
			{
				if (gunfire.TryGetComponent(out Renderer gunfireRenderer)) gunfireRenderer.enabled = false;
				else if (gunfire.TryGetComponent(out ParticleSystem gunfireParticles)) gunfireParticles.Stop();
			}
		}

		public override WeaponType GetWeaponType()
		{
			return WeaponType.LaserGun;
		}
	}
}
