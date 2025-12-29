using UnityEngine;
using Zombie3D;

public class AirGunBulletScript : MonoBehaviour
{
	private Transform proTransform;

	private ResourceConfigScript rConf;

	public Vector3 dir;

	public float hitForce;

	public float explodeRadius;

	public float flySpeed;

	public float life = 2f;

	public float damage;

	public Player player;

	public GameObject hitObjectEffect;

	private float createdTime;

	private float deltaTime;

	private int oneShotKills;

	private void Start()
	{
		rConf = GameApp.GetInstance().GetResourceConfig();
		proTransform = base.transform;
		oneShotKills = 0;
		createdTime = Time.time;
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (!((double)deltaTime < 0.02))
		{
			proTransform.Translate(flySpeed * dir * deltaTime, Space.World);
			if (Time.time - createdTime > life && base.gameObject != null)
			{
				Object.DestroyObject(base.gameObject);
			}
			deltaTime = 0f;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		GameScene gameScene = GameApp.GetInstance().GetGameScene();
		if (other.gameObject.layer == 11 || other.gameObject.layer == 15)
		{
			base.gameObject.transform.Find("Air_Gun_01").gameObject.SetActive(false);
			base.gameObject.GetComponent<SphereCollider>().enabled = false;
		}
		if (player.PlayerObject == other.gameObject)
		{
			return;
		}
		Ray ray = new Ray(base.transform.position + Vector3.up * 1f, Vector3.down);
		float num = 10000.1f;
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f, 32768))
		{
			num = hitInfo.point.y;
		}
		Vector3 position = new Vector3(base.transform.position.x, num + 0.1f, base.transform.position.z);
		Object.Instantiate(rConf.GetExplosionFloor(Application.loadedLevelName), position, Quaternion.Euler(270f, 0f, 0f));
		Object.Instantiate(hitObjectEffect, proTransform.position + Vector3.up * 0.1f, Quaternion.identity);
		if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
		{
			GameVSScene gameVSScene = gameScene as GameVSScene;
			{
				foreach (Player value in gameVSScene.SFS_Player_Arr.Values)
				{
					if (value == null || value == player || (value.GetPlayerState() != null && value.GetPlayerState().GetStateType() == PlayerStateType.Dead))
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
						value.OnVsInjured(player.tnet_user, num3, 12);
					}
				}
				return;
			}
		}
		foreach (Enemy value2 in gameScene.GetEnemies().Values)
		{
			if (value2.GetState() == Enemy.DEAD_STATE)
			{
				continue;
			}
			float sqrMagnitude2 = (value2.GetPosition() - proTransform.position).sqrMagnitude;
			float num4 = explodeRadius * explodeRadius;
			if (sqrMagnitude2 < num4)
			{
				DamageProperty damageProperty = new DamageProperty();
				damageProperty.hitForce = (dir + new Vector3(0f, 0.18f, 0f)) * hitForce;
				if (sqrMagnitude2 * 4f < num4)
				{
					damageProperty.damage = damage;
					value2.OnHit(damageProperty, WeaponType.ElectricGun, true, player);
				}
				else
				{
					damageProperty.damage = damage / 2f;
					value2.OnHit(damageProperty, WeaponType.ElectricGun, false, player);
				}
			}
			if (value2.Attributes.Hp <= 0.0 && oneShotKills != -1)
			{
				oneShotKills++;
			}
		}
		if (oneShotKills >= 4)
		{
			GameApp.GetInstance().GetGameState().Achievement.CheckAchievemnet_WeaponMaster();
			oneShotKills = -1;
		}
		GameObject[] woodBoxes = gameScene.GetWoodBoxes();
		GameObject[] array = woodBoxes;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				float sqrMagnitude3 = (gameObject.transform.position - proTransform.position).sqrMagnitude;
				float num5 = explodeRadius * explodeRadius;
				if (sqrMagnitude3 < num5)
				{
					WoodBoxScript component = gameObject.GetComponent<WoodBoxScript>();
					component.OnHit(damage);
				}
			}
		}
	}
}
