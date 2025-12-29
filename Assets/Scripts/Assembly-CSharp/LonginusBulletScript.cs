using UnityEngine;
using Zombie3D;

public class LonginusBulletScript : MonoBehaviour
{
	protected Transform proTransform;

	protected ResourceConfigScript rConf;

	private bool objectDestroyed;

	public Vector3 dir;

	public float hitForce;

	public float explodeRadius;

	public float flySpeed;

	public float damage;

	public Vector3 startPos;

	protected float createdTime;

	protected float deltaTime;

	protected float life = 0.25f;

	public Player player;

	public void Start()
	{
		rConf = GameApp.GetInstance().GetResourceConfig();
		proTransform = base.transform;
		createdTime = Time.time;
		objectDestroyed = false;
	}

	public void Update()
	{
		deltaTime += Time.deltaTime;
		if (!(deltaTime < 0.03f))
		{
			proTransform.Translate(flySpeed * dir * deltaTime, Space.World);
			if (!objectDestroyed && Time.time - createdTime > life)
			{
				DoDamage();
			}
			deltaTime = 0f;
		}
	}

	private void DoDamage()
	{
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
		GameObject gameObject = Object.Instantiate(rConf.longinusGoldExplosion, proTransform.position + Vector3.up * 0.1f, Quaternion.LookRotation(base.transform.forward)) as GameObject;
		gameObject.GetComponent<AudioSource>().mute = !GameApp.GetInstance().GetGameState().SoundOn;
		GameScene gameScene = GameApp.GetInstance().GetGameScene();
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
					value2.OnHit(damageProperty, WeaponType.ElectricGun, true, player);
				}
				else
				{
					damageProperty.damage = damage / 2f;
					value2.OnHit(damageProperty, WeaponType.ElectricGun, false, player);
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
					WoodBoxScript component = gameObject2.GetComponent<WoodBoxScript>();
					component.OnHit(damage);
				}
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!objectDestroyed && player.PlayerObject != other.gameObject)
		{
			DoDamage();
		}
	}
}
