using System.Collections;
using UnityEngine;
using Zombie3D;

public class ArenaTriggerBossScript : MonoBehaviour
{
	private const float spawnChildInterval = 30f;

	private EnemyType bossType;

	private EnemyType childType;

	private bool isElite;

	private CoopBossConfig bossConfig;

	private GameMultiplayerScene gameScene;

	private GameObject[] doors;

	private GameObject[] graves;

	private Enemy gameBoss;

	private float spawnChildTimer;

	private bool beginSpawnChild;

	private IEnumerator Start()
	{
		while (GameApp.GetInstance().GetGameScene() == null)
		{
			yield return 1;
		}
		gameScene = GameApp.GetInstance().GetGameScene() as GameMultiplayerScene;
		gameScene.ArenaTrigger_Boss = this;
		bossConfig = GameApp.GetInstance().GetGameConfig().GetCoopBossConfig(GameApp.GetInstance().GetGameState().cur_net_map);
		SceneName.CoopBossType btype = SceneName.GetZombieTypeFromMap(GameApp.GetInstance().GetGameState().cur_net_map);
		isElite = btype.isElite;
		bossType = btype.zombieType;
		if (bossType == EnemyType.E_TANK)
		{
			childType = EnemyType.E_BOOMER;
		}
		else if (bossType == EnemyType.E_HELL_FIRER)
		{
			if (isElite)
			{
				childType = EnemyType.E_NURSE;
			}
			else
			{
				childType = EnemyType.E_POLICE;
			}
		}
		else
		{
			childType = bossType;
		}
		gameScene.InitEnemyPool(childType);
		doors = GameObject.FindGameObjectsWithTag("Door");
		graves = GameObject.FindGameObjectsWithTag("Grave");
		beginSpawnChild = false;
		spawnChildTimer = 30f;
		gameScene.CalculateEnemyAttributesComputingFactorWithDay(bossConfig.day);
		yield return new WaitForSeconds(2f);
		if (GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master)
		{
			CheckSpawnBoss();
		}
	}

	private void Update()
	{
		if (GameApp.GetInstance().GetGameState().net_com.m_netUserInfo.is_master && gameBoss != null)
		{
			spawnChildTimer -= Time.deltaTime;
			if (!(spawnChildTimer > 0f) && !beginSpawnChild)
			{
				StartCoroutine("SpawnChild");
				beginSpawnChild = true;
			}
		}
	}

	public void CheckSpawnBoss()
	{
		if (gameScene.GetEnemies().Count > 0)
		{
			foreach (Enemy value in gameScene.GetEnemies().Values)
			{
				if (value.IsSuperBoss)
				{
					return;
				}
			}
		}
		Vector3 zero = Vector3.zero;
		int num = Random.Range(0, doors.Length);
		zero = doors[num].transform.position;
		gameBoss = EnermyFactory.SpawnEnemy(gameScene.GetNextEnemyID(), isElite, false, true, bossType, SpawnFromType.Door, zero);
		Packet packet = CGEnemyBirthPacket.MakePacket(0u, (uint)gameScene.EnemyID, (uint)bossType, isElite ? 1u : 0u, 0u, 1u, zero, (uint)gameBoss.TargetPlayer.m_multi_id);
		GameApp.GetInstance().GetGameState().net_com.Send(packet);
		spawnChildTimer = 30f;
		beginSpawnChild = false;
	}

	private IEnumerator SpawnChild()
	{
		while (gameScene.GetEnemies().Count > 1)
		{
			spawnChildTimer = -1f;
			yield return 1;
		}
		Transform nearestGrave = null;
		float minDisSqr = float.MaxValue;
		GameObject[] array = graves;
		foreach (GameObject g in array)
		{
			float disSqr = (gameBoss.GetPosition() - g.transform.position).sqrMagnitude;
			if (disSqr < minDisSqr)
			{
				nearestGrave = g.transform;
				minDisSqr = disSqr;
			}
		}
		for (int i = 0; i < 8; i++)
		{
			float rndX = Random.Range((0f - nearestGrave.localScale.x) / 2f, nearestGrave.localScale.x / 2f);
			float rndZ = Random.Range((0f - nearestGrave.localScale.z) / 2f, nearestGrave.localScale.z / 2f);
			Vector3 spawnPosition = nearestGrave.position + new Vector3(rndX, 0f, rndZ);
			Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().graveRock, spawnPosition + Vector3.down * 0.3f, Quaternion.identity);
			Packet packet = CGEnemyBirthPacket.MakePacket(target_id: (uint)EnermyFactory.SpawnEnemy(gameScene.GetNextEnemyID(), isElite, false, false, childType, SpawnFromType.Grave, spawnPosition).TargetPlayer.m_multi_id, referenced_day: 0u, enemy_id: (uint)gameScene.EnemyID, enemy_type: (uint)childType, isElite: isElite ? 1u : 0u, isGrave: 1u, isSuperBoss: 0u, position: spawnPosition);
			GameApp.GetInstance().GetGameState().net_com.Send(packet);
			yield return new WaitForSeconds(0.3f);
		}
		spawnChildTimer = 30f;
		beginSpawnChild = false;
	}

	public static void SpawnMultiEnemy(EnemyType m_type, int m_id, bool m_isElite, bool m_isSuperBoss, Vector3 m_position, bool m_isGrave, int m_targetId)
	{
		Enemy enemy = EnermyFactory.SpawnEnemy(m_id, m_isElite, false, m_isSuperBoss, m_type, (!m_isGrave) ? SpawnFromType.Door : SpawnFromType.Grave, m_position);
		GameApp.GetInstance().GetGameScene().EnemyID = m_id;
		if (m_isGrave)
		{
			Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().graveRock, m_position + Vector3.down * 0.3f, Quaternion.identity);
		}
		foreach (Player item in GameApp.GetInstance().GetGameScene().m_multi_player_arr)
		{
			if (item.m_multi_id == m_targetId)
			{
				enemy.TargetPlayer = item;
				break;
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (gameScene == null || gameScene.GetEnemies() == null)
		{
			return;
		}
		foreach (Enemy value in gameScene.GetEnemies().Values)
		{
			if (value.LastTarget != Vector3.zero)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere(value.LastTarget, 0.3f);
				Gizmos.DrawLine(value.ray.origin, value.rayhit.point);
			}
		}
	}
}
