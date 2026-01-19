using UnityEngine;
using Zombie3D;

public class CrossbowExplosionScript : MonoBehaviour
{
	public GameObject followObject;

	public float EnvokeTime;

	public float startDelay;

	public Player player;

	public float explodeRadius;

	public float damage;

	private bool isEmit;

	private ParticleSystem[] particles;

	private bool isFollowOject;

	private GameObject redpointObj;

	private void Awake()
	{
		particles = base.transform.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < particles.Length; i++)
		{
			var e = particles[i].emission;
			e.enabled = false;
		}
	}

	private void Start()
	{
		if (followObject != null && followObject.activeInHierarchy && (followObject.layer == 8 || followObject.layer == 9))
		{
			redpointObj = Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().crossbowRedpoint, Vector3.zero, Quaternion.identity) as GameObject;
			redpointObj.transform.parent = followObject.transform;
			BoxCollider component = followObject.GetComponent<BoxCollider>();
			CharacterController component2 = followObject.GetComponent<CharacterController>();
			if (component != null)
			{
				redpointObj.transform.localPosition = component.center + Vector3.forward * 1.2f;
			}
			else if (component2 != null)
			{
				redpointObj.transform.localPosition = component2.center + Vector3.forward * 1.2f;
			}
			redpointObj.GetComponent<AudioSource>().mute = !GameApp.GetInstance().GetGameState().SoundOn;
			isFollowOject = true;
		}
		base.GetComponent<AudioSource>().Stop();
	}

	private void Update()
	{
		if (isFollowOject)
		{
			if (followObject != null && followObject.activeInHierarchy)
			{
				if (followObject.layer == 8)
				{
					bool flag = false;
					PlayerShell component = followObject.GetComponent<PlayerShell>();
					if (component != null && component.m_player.HP > 0f && Time.time - EnvokeTime <= startDelay)
					{
						base.transform.position = followObject.transform.position;
						flag = true;
					}
					if (!flag)
					{
						isFollowOject = false;
						DoExplosion();
					}
				}
				else if (followObject.layer == 9)
				{
					bool flag2 = false;
					foreach (Enemy value in GameApp.GetInstance().GetGameScene().GetEnemies()
						.Values)
					{
						if (value.Name == followObject.name)
						{
							if (value.Attributes.Hp > 0.0 && Time.time - EnvokeTime <= startDelay)
							{
								base.transform.position = followObject.transform.position;
								flag2 = true;
							}
							break;
						}
					}
					if (!flag2)
					{
						isFollowOject = false;
						DoExplosion();
					}
				}
				else
				{
					isFollowOject = false;
					DoExplosion();
				}
			}
			else
			{
				isFollowOject = false;
				DoExplosion();
			}
		}
		else if (!isEmit && Time.time - EnvokeTime > startDelay)
		{
			DoExplosion();
		}
	}

	private void DoExplosion()
	{
		base.GetComponent<AudioSource>().mute = !GameApp.GetInstance().GetGameState().SoundOn;
		base.GetComponent<AudioSource>().Play();
		for (int i = 0; i < particles.Length; i++)
		{
			particles[i].Stop();
		}
		isEmit = true;
		base.gameObject.AddComponent<RemoveTimerScript>().life = 2f;
		CauseDamage();
		if (redpointObj != null)
		{
			Object.Destroy(redpointObj);
		}
	}

	private void CauseDamage()
	{
		if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
		{
			GameVSScene gameVSScene = GameApp.GetInstance().GetGameScene() as GameVSScene;
			{
				foreach (Player value in gameVSScene.SFS_Player_Arr.Values)
				{
					if (value == null || value == player || (value.GetPlayerState() != null && value.GetPlayerState().GetStateType() == PlayerStateType.Dead))
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
							value.OnVsInjured(player.tnet_user, num2, 14);
						}
					}
				}
				return;
			}
		}
		GameScene gameScene = GameApp.GetInstance().GetGameScene();
		int num3 = 0;
		foreach (Enemy value2 in gameScene.GetEnemies().Values)
		{
			if (value2.GetState() == Enemy.DEAD_STATE)
			{
				continue;
			}
			float sqrMagnitude2 = (value2.GetPosition() - base.transform.position).sqrMagnitude;
			float num4 = explodeRadius * explodeRadius;
			if (sqrMagnitude2 < num4)
			{
				Vector3 normalized = (value2.GetPosition() - base.transform.position).normalized;
				DamageProperty damageProperty = new DamageProperty();
				damageProperty.hitForce = (normalized + new Vector3(0f, 0.18f, 0f)) * 30f;
				if (sqrMagnitude2 * 4f < num4)
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
				num3++;
			}
		}
		if (num3 >= 4)
		{
			GameApp.GetInstance().GetGameState().Achievement.CheckAchievemnet_WeaponMaster();
		}
		GameObject[] woodBoxes = gameScene.GetWoodBoxes();
		GameObject[] array = woodBoxes;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				float sqrMagnitude3 = (gameObject.transform.position - base.transform.position).sqrMagnitude;
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
