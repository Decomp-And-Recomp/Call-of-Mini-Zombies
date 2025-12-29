using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

[AddComponentMenu("TPS/ProjectileScript")]
public class ProjectileScript : MonoBehaviour
{
	public GameObject hitObjectEffect;

	public Transform targetTransform;

	protected Transform proTransform;

	protected WeaponType gunType;

	protected ResourceConfigScript rConf;

	private bool objectDestroyed;

	public Vector3 dir;

	public float hitForce;

	public float explodeRadius;

	public float flySpeed;

	public Vector3 speed;

	public float life = 2f;

	public float damage;

	public Vector3 targetPos;

	public float initAngel = 40f;

	public float angelDelta;

	protected float createdTime;

	protected float deltaTime;

	protected Vector3 lastPos;

	protected float lastCheckPosTime;

	public Player player;

	public WeaponType GunType
	{
		set
		{
			gunType = value;
		}
	}

	public void Start()
	{
		rConf = GameApp.GetInstance().GetResourceConfig();
		proTransform = base.transform;
		createdTime = Time.time;
		lastCheckPosTime = Time.time;
		lastPos = proTransform.position;
		objectDestroyed = false;
	}

	public void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime < 0.03f)
		{
			return;
		}
		if (gunType == WeaponType.Sniper)
		{
			if (damage != 0f && targetTransform != null)
			{
				if (targetTransform.gameObject.activeInHierarchy)
				{
					dir = (targetTransform.position - proTransform.position).normalized;
					targetPos = targetTransform.position;
				}
				else
				{
					targetTransform = null;
				}
			}
			proTransform.LookAt(targetPos);
			initAngel -= deltaTime * 80f;
			if (initAngel <= 0f)
			{
				initAngel = 0f;
			}
			proTransform.rotation = Quaternion.AngleAxis(initAngel, -1f * proTransform.right) * proTransform.rotation;
			proTransform.RotateAround(proTransform.forward, Time.time * 10f);
			dir = proTransform.forward;
			if (Time.time - lastCheckPosTime > 0.3f)
			{
				lastCheckPosTime = Time.time;
				if ((proTransform.position - lastPos).sqrMagnitude < 2f)
				{
					Object.Destroy(base.gameObject);
					objectDestroyed = true;
					return;
				}
				lastPos = proTransform.position;
			}
		}
		else if (gunType == WeaponType.M32)
		{
			proTransform.LookAt(targetPos);
			initAngel -= deltaTime * angelDelta;
			if (initAngel <= -40f)
			{
				initAngel = -40f;
			}
			proTransform.rotation = Quaternion.AngleAxis(initAngel, -1f * proTransform.right) * proTransform.rotation;
			dir = proTransform.forward;
		}
		if (gunType == WeaponType.NurseSaliva || gunType == WeaponType.DiloSpitFire)
		{
			speed += Physics.gravity.y * Vector3.up * deltaTime;
			proTransform.Translate(speed * deltaTime, Space.World);
			proTransform.LookAt(proTransform.position + speed * 10f);
		}
		else
		{
			proTransform.Translate(flySpeed * dir * deltaTime, Space.World);
			if (gunType == WeaponType.RocketLauncher)
			{
				proTransform.Rotate(Vector3.forward, deltaTime * 1000f, Space.Self);
			}
		}
		if (Time.time - createdTime > life && base.gameObject != null)
		{
			Object.Destroy(base.gameObject);
			objectDestroyed = true;
		}
		deltaTime = 0f;
	}

	private void OnTriggerStay(Collider other)
	{
		if (gunType != WeaponType.LaserGun)
		{
			return;
		}
		GameScene gameScene = GameApp.GetInstance().GetGameScene();
		Weapon weapon = this.player.GetWeapon();
		if (weapon.GetWeaponType() != WeaponType.LaserGun)
		{
			return;
		}
		LaserGun laserGun = weapon as LaserGun;
		if (laserGun == null || !laserGun.CouldMakeNextShoot())
		{
			return;
		}
		if (other.gameObject.layer == 9)
		{
			Enemy enemyByID = gameScene.GetEnemyByID(other.gameObject.name);
			if (enemyByID != null && enemyByID.GetState() != Enemy.DEAD_STATE)
			{
				Object.Instantiate(hitObjectEffect, enemyByID.GetPosition(), Quaternion.identity);
				DamageProperty damageProperty = new DamageProperty();
				damageProperty.hitForce = (dir + new Vector3(0f, 0.18f, 0f)) * hitForce;
				damageProperty.damage = damage;
				enemyByID.OnHit(damageProperty, gunType, false, this.player);
			}
		}
		else if (other.gameObject.layer == 8 && GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
		{
			if (!(this.player.PlayerObject == other.gameObject))
			{
				Player player = other.gameObject.GetComponent<PlayerShell>().m_player;
				if (player != null && player.GetPlayerState() != null && player.GetPlayerState().GetStateType() != PlayerStateType.Dead && damage > 0f)
				{
					player.OnVsInjured(this.player.tnet_user, damage, (int)gunType);
				}
			}
		}
		else if (other.gameObject.layer == 19)
		{
			WoodBoxScript component = other.gameObject.GetComponent<WoodBoxScript>();
			component.OnHit(damage);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (objectDestroyed)
		{
			return;
		}
		GameScene gameScene = GameApp.GetInstance().GetGameScene();
		if (gunType == WeaponType.ElectricGun || gunType == WeaponType.RocketLauncher || gunType == WeaponType.Sniper || gunType == WeaponType.M32)
		{
			if (this.player.PlayerObject == other.gameObject)
			{
				return;
			}
			Object.Destroy(base.gameObject);
			objectDestroyed = true;
			Ray ray = new Ray(base.transform.position + Vector3.up * 1f, Vector3.down);
			float num = 10000.1f;
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 100f, 32768))
			{
				num = hitInfo.point.y;
			}
			Vector3 position = new Vector3(base.transform.position.x, num + 0.1f, base.transform.position.z);
			Object.Instantiate(rConf.GetExplosionFloor(Application.loadedLevelName), position, Quaternion.Euler(270f, 0f, 0f));
			GameObject gameObject = Object.Instantiate(hitObjectEffect, proTransform.position + Vector3.up * 0.1f, Quaternion.identity) as GameObject;
			AudioSource component = gameObject.GetComponent<AudioSource>();
			if (component != null)
			{
				component.mute = !GameApp.GetInstance().GetGameState().SoundOn;
			}
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				GameVSScene gameVSScene = gameScene as GameVSScene;
				{
					foreach (Player value in gameVSScene.SFS_Player_Arr.Values)
					{
						if (value == null || value == this.player || (value.GetPlayerState() != null && value.GetPlayerState().GetStateType() == PlayerStateType.Dead))
						{
							continue;
						}
						float sqrMagnitude = (value.GetTransform().position - proTransform.position).sqrMagnitude;
						float num2 = explodeRadius * explodeRadius;
						if (sqrMagnitude < num2 && damage > 0f)
						{
							float num3 = damage;
							if (sqrMagnitude * 4f >= num2)
							{
								num3 = damage / 2f;
							}
							value.OnVsInjured(this.player.tnet_user, num3, (int)gunType);
						}
					}
					return;
				}
			}
			int num4 = 0;
			foreach (Enemy value2 in gameScene.GetEnemies().Values)
			{
				if (value2.GetState() == Enemy.DEAD_STATE)
				{
					continue;
				}
				float sqrMagnitude2 = (value2.GetPosition() - proTransform.position).sqrMagnitude;
				float num5 = explodeRadius * explodeRadius;
				if (sqrMagnitude2 < num5)
				{
					DamageProperty damageProperty = new DamageProperty();
					damageProperty.hitForce = (dir + new Vector3(0f, 0.18f, 0f)) * hitForce;
					if (sqrMagnitude2 * 4f < num5)
					{
						damageProperty.damage = damage;
						value2.OnHit(damageProperty, gunType, true, this.player);
					}
					else
					{
						damageProperty.damage = damage / 2f;
						value2.OnHit(damageProperty, gunType, false, this.player);
					}
				}
				if (value2.Attributes.Hp <= 0.0)
				{
					num4++;
				}
			}
			if (num4 >= 4)
			{
				GameApp.GetInstance().GetGameState().Achievement.CheckAchievemnet_WeaponMaster();
			}
			GameObject[] woodBoxes = gameScene.GetWoodBoxes();
			GameObject[] array = woodBoxes;
			foreach (GameObject gameObject2 in array)
			{
				if (gameObject2 != null)
				{
					float sqrMagnitude3 = (gameObject2.transform.position - proTransform.position).sqrMagnitude;
					float num6 = explodeRadius * explodeRadius;
					if (sqrMagnitude3 < num6)
					{
						WoodBoxScript component2 = gameObject2.GetComponent<WoodBoxScript>();
						component2.OnHit(damage);
					}
				}
			}
		}
		else if (gunType == WeaponType.NurseSaliva || gunType == WeaponType.DiloSpitFire)
		{
			Player player = gameScene.GetPlayer();
			if (other.gameObject.layer == 9)
			{
				return;
			}
			Ray ray2 = new Ray(proTransform.position + Vector3.up * 1f, Vector3.down);
			float num7 = 10000.1f;
			RaycastHit hitInfo2;
			if (Physics.Raycast(ray2, out hitInfo2, 100f, 32768))
			{
				num7 = hitInfo2.point.y;
			}
			Object.Instantiate(hitObjectEffect, new Vector3(proTransform.position.x, num7 + 0.1f, proTransform.position.z), Quaternion.identity);
			GameObject gameObject3 = null;
			Vector3 position2 = new Vector3(proTransform.position.x, num7 + 0.1f, proTransform.position.z);
			if (gunType == WeaponType.NurseSaliva)
			{
				gameObject3 = Object.Instantiate(rConf.nurseSaliva, position2, Quaternion.identity) as GameObject;
				gameObject3.transform.Rotate(270f, 0f, 0f);
			}
			else if (gunType == WeaponType.DiloSpitFire)
			{
				gameObject3 = Object.Instantiate(rConf.diloVenomHitFloor, position2, Quaternion.identity) as GameObject;
			}
			AudioSource component3 = gameObject3.GetComponent<AudioSource>();
			if (component3 != null)
			{
				component3.mute = !GameApp.GetInstance().GetGameState().SoundOn;
			}
			Object.Destroy(base.gameObject);
			objectDestroyed = true;
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				List<Player> list = new List<Player>();
				foreach (Player item in GameApp.GetInstance().GetGameScene().m_multi_player_arr)
				{
					if ((item.GetTransform().position - proTransform.position).sqrMagnitude < explodeRadius * explodeRadius)
					{
						list.Add(item);
					}
				}
				{
					foreach (Player item2 in list)
					{
						item2.OnHit(damage * player.PowerBuff);
					}
					return;
				}
			}
			if ((player.GetTransform().position - proTransform.position).sqrMagnitude < explodeRadius * explodeRadius)
			{
				player.OnHit(damage * player.PowerBuff);
			}
		}
		else
		{
			if (gunType != WeaponType.Crossbow || this.player.PlayerObject == other.gameObject)
			{
				return;
			}
			Object.Destroy(base.gameObject);
			objectDestroyed = true;
			GameObject gameObject4 = Object.Instantiate(hitObjectEffect, proTransform.position, Quaternion.identity) as GameObject;
			GameObject gameObject5 = Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().crossbowExplosion, proTransform.position, Quaternion.identity) as GameObject;
			gameObject5.GetComponent<CrossbowExplosionScript>().followObject = other.gameObject;
			gameObject5.GetComponent<CrossbowExplosionScript>().EnvokeTime = Time.time;
			gameObject5.GetComponent<CrossbowExplosionScript>().player = this.player;
			gameObject5.GetComponent<CrossbowExplosionScript>().explodeRadius = explodeRadius;
			gameObject5.GetComponent<CrossbowExplosionScript>().damage = damage;
			if (other.gameObject.layer == 8)
			{
				if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
				{
					Player player2 = other.GetComponent<PlayerShell>().m_player;
					if (player2 != null && player2.GetPlayerState() != null && player2.GetPlayerState().GetStateType() != PlayerStateType.Dead)
					{
						player2.OnVsInjured(this.player.tnet_user, damage, 12);
					}
				}
				return;
			}
			if (other.gameObject.layer == 9)
			{
				foreach (Enemy value3 in gameScene.GetEnemies().Values)
				{
					if (other.gameObject.name == value3.Name)
					{
						DamageProperty damageProperty2 = new DamageProperty();
						damageProperty2.hitForce = (dir + new Vector3(0f, 0.18f, 0f)) * hitForce;
						damageProperty2.damage = damage;
						value3.OnHit(damageProperty2, gunType, true, this.player);
						break;
					}
				}
				return;
			}
			if (other.gameObject.layer == 19 && other.gameObject != null && other.gameObject.GetComponent<WoodBoxScript>() != null)
			{
				other.gameObject.GetComponent<WoodBoxScript>().OnHit(damage);
			}
		}
	}
}
