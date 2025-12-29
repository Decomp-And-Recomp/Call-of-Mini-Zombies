using UnityEngine;
using Zombie3D;

public class EnemyConfigScript : MonoBehaviour
{
	public static EnemyConfigScript instance;

	public GameObject[] enemy = new GameObject[9];

	public GameObject[] enemy_elite = new GameObject[9];

	public GameObject deadbody_zombie;

	public GameObject deadbody_nurse;

	public GameObject deadbody_boomer;

	public GameObject deadbody_swat;

	public GameObject deadhead_zombie;

	public GameObject deadhead_nurse;

	public GameObject deadhead_boomer;

	public GameObject deadhead_swat;

	public GameObject GetDeadbody(EnemyType type)
	{
		switch (type)
		{
		case EnemyType.E_ZOMBIE:
			return deadbody_zombie;
		case EnemyType.E_NURSE:
			return deadbody_nurse;
		case EnemyType.E_BOOMER:
			return deadbody_boomer;
		case EnemyType.E_SWAT:
			return deadbody_swat;
		default:
			return deadbody_zombie;
		}
	}

	public GameObject GetDeadHead(EnemyType type)
	{
		switch (type)
		{
		case EnemyType.E_ZOMBIE:
			return deadhead_zombie;
		case EnemyType.E_NURSE:
			return deadhead_nurse;
		case EnemyType.E_BOOMER:
			return deadhead_boomer;
		case EnemyType.E_SWAT:
			return deadhead_swat;
		default:
			return deadhead_zombie;
		}
	}

	private void Awake()
	{
		instance = this;
		DontDestroyOnLoad(gameObject);
	}
}
