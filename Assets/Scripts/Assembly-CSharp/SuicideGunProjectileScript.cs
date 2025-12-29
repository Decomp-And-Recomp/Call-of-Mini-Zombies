using UnityEngine;
using Zombie3D;

public class SuicideGunProjectileScript : MonoBehaviour
{
	protected GameObject explodeObject;

	protected ResourceConfigScript rConf;

	protected GameScene gameScene;

	private bool objectDestroyed;

	private float deltaTime;

	private float createdTime;

	public float flySpeed { get; set; }

	public Vector3 dir { get; set; }

	public Player player { get; set; }

	public float explodeRadius { get; set; }

	public float damage { get; set; }

	public float hitForce { get; set; }

	private void Start()
	{
		deltaTime = 0f;
		createdTime = Time.time;
		rConf = GameApp.GetInstance().GetResourceConfig();
		gameScene = GameApp.GetInstance().GetGameScene();
		explodeObject = rConf.suicideGunExplosion;
		objectDestroyed = false;
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (!(deltaTime < 0.03f))
		{
			if (Time.time - createdTime > 100f && base.gameObject != null)
			{
				Object.Destroy(base.gameObject);
				objectDestroyed = true;
			}
			deltaTime = 0f;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (objectDestroyed)
		{
			return;
		}
		Object.Destroy(base.gameObject);
		objectDestroyed = true;
		GameObject gameObject = Object.Instantiate(explodeObject, base.transform.position + Vector3.up * 0.1f, Quaternion.identity) as GameObject;
		string text = ((Random.Range(0, 100) >= 50) ? "audio2" : "audio1");
		AudioSource component = gameObject.transform.Find(text).GetComponent<AudioSource>();
		component.mute = !GameApp.GetInstance().GetGameState().SoundOn;
		component.Play();
		if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
		{
			GameVSScene gameVSScene = gameScene as GameVSScene;
			{
				foreach (Player value in gameVSScene.SFS_Player_Arr.Values)
				{
					if (value == null || (value.GetPlayerState() != null && value.GetPlayerState().GetStateType() == PlayerStateType.Dead))
					{
						continue;
					}
					float sqrMagnitude = (value.GetTransform().position - base.transform.position).sqrMagnitude;
					float num = explodeRadius * explodeRadius;
					if (sqrMagnitude < num && damage > 0f)
					{
						float num2 = damage;
						if (sqrMagnitude * 4f >= num)
						{
							num2 = damage / 2f;
						}
						if (value == player)
						{
							value.OnHit(num2);
						}
						else
						{
							value.OnVsInjured(player.tnet_user, num2, 13);
						}
					}
				}
				return;
			}
		}
		if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
		{
			foreach (Player item in gameScene.m_multi_player_arr)
			{
				float sqrMagnitude2 = (item.GetTransform().position - base.transform.position).sqrMagnitude;
				float num3 = explodeRadius * explodeRadius;
				if (sqrMagnitude2 < num3 && damage > 0f)
				{
					float num4 = damage;
					if (sqrMagnitude2 * 4f >= num3)
					{
						num4 = damage / 2f;
					}
					if (item == player)
					{
						item.OnHit(num4);
					}
					else
					{
						item.OnMultiInjured(num4);
					}
				}
			}
		}
		int num5 = 0;
		foreach (Enemy value2 in gameScene.GetEnemies().Values)
		{
			if (value2.GetState() == Enemy.DEAD_STATE)
			{
				continue;
			}
			float sqrMagnitude3 = (value2.GetPosition() - base.transform.position).sqrMagnitude;
			float num6 = explodeRadius * explodeRadius;
			if (sqrMagnitude3 < num6)
			{
				DamageProperty damageProperty = new DamageProperty();
				damageProperty.hitForce = (dir + new Vector3(0f, 0.18f, 0f)) * hitForce;
				if (sqrMagnitude3 * 4f < num6)
				{
					damageProperty.damage = damage;
					value2.OnHit(damageProperty, WeaponType.SuicideGun, true, player);
				}
				else
				{
					damageProperty.damage = damage / 2f;
					value2.OnHit(damageProperty, WeaponType.SuicideGun, false, player);
				}
			}
			if (value2.Attributes.Hp <= 0.0)
			{
				num5++;
			}
		}
		if (num5 >= 4)
		{
			GameApp.GetInstance().GetGameState().Achievement.CheckAchievemnet_WeaponMaster();
		}
		GameObject[] woodBoxes = gameScene.GetWoodBoxes();
		GameObject[] array = woodBoxes;
		foreach (GameObject gameObject2 in array)
		{
			if (gameObject2 != null)
			{
				float sqrMagnitude4 = (gameObject2.transform.position - base.transform.position).sqrMagnitude;
				float num7 = explodeRadius * explodeRadius;
				if (sqrMagnitude4 < num7)
				{
					WoodBoxScript component2 = gameObject2.GetComponent<WoodBoxScript>();
					component2.OnHit(damage);
				}
			}
		}
	}
}
