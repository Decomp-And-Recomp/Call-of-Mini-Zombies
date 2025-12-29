using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class ArenaTriggerInstanceScript : MonoBehaviour
{
	private GameScene gameScene;

	private int waveNum;

	private List<SpawnConfig> spawnConfigs;

	private List<SpawnConfig> spawnConfigs_random;

	private GameObject[] doors;

	public int FinishedWaveNum;

	public void UpdateGameStatistics()
	{
		gameScene.GameGUI.SetInstanceStatistics(waveNum, GameApp.GetInstance().GetGameState().instance_score);
	}

	private IEnumerator Start()
	{
		while (!GameUIScriptNew.GetGameUIScript().uiInited)
		{
			yield return 1;
		}
		gameScene = GameApp.GetInstance().GetGameScene();
		gameScene.ArenaTrigger_Instance = this;
		gameScene.InitEnemyPool(false);
		doors = GameObject.FindGameObjectsWithTag("Door");
		GameParametersXML paramXML = new GameParametersXML();
		spawnConfigs = new List<SpawnConfig>();
		spawnConfigs_random = new List<SpawnConfig>();
		if (Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.Android)
		{
			int wave_idx2 = 1;
			for (int day_idx2 = (wave_idx2 - 1) * 5 + 1; day_idx2 <= 30; day_idx2 = (wave_idx2 - 1) * 5 + 1)
			{
				spawnConfigs.Add(paramXML.Load(null, day_idx2, false));
				wave_idx2++;
			}
			spawnConfigs_random.Add(spawnConfigs[spawnConfigs.Count - 1]);
			for (int j = 30; j > 26; j--)
			{
				spawnConfigs_random.Add(paramXML.Load(null, j, false));
			}
		}
		else
		{
			int wave_idx = 1;
			for (int day_idx = (wave_idx - 1) * 5 + 1; day_idx <= 30; day_idx = (wave_idx - 1) * 5 + 1)
			{
				spawnConfigs.Add(paramXML.Load("/", day_idx, false));
				wave_idx++;
			}
			spawnConfigs_random.Add(spawnConfigs[spawnConfigs.Count - 1]);
			for (int i = 30; i > 26; i--)
			{
				spawnConfigs_random.Add(paramXML.Load("/", i, false));
			}
		}
		waveNum = 1;
		UpdateGameStatistics();
		gameScene.GameGUI.instanceStatistics.GetComponent<InstanceModeGameStatisticsUI>().OnEnd = OnTimerOut;
		while (waveNum < int.MaxValue)
		{
			UpdateGameStatistics();
			SpawnConfig currentSpawnConfig;
			if (waveNum <= spawnConfigs.Count)
			{
				int dayNum2 = (waveNum - 1) * 5 + 1;
				currentSpawnConfig = spawnConfigs[waveNum - 1];
			}
			else
			{
				int ran = Random.Range(0, 5);
				int dayNum2 = 26 + ran;
				currentSpawnConfig = spawnConfigs_random[ran];
			}
			gameScene.CalculateEnemyAttributesComputingFactorWithDay((waveNum - 1) * 5 + 1);
			int ExpectedMaxLoot = gameScene.CalculateExpectedMaxLootForInstanceMode(waveNum);
			int GotLoot = 0;
			Debug.Log("Wave num: " + waveNum + "  standard loot: " + gameScene.GetEnemyAttributesComputingFactor("loot"));
			bool spawnedBoss = false;
			while (GotLoot < ExpectedMaxLoot)
			{
				Debug.Log("spawned enemy total cash: " + GotLoot + "  expected enemy total cash: " + ExpectedMaxLoot);
				int select_round2 = Random.Range(0, currentSpawnConfig.Rounds.Count);
				Round round = currentSpawnConfig.Rounds[select_round2];
				while (round.EnemyInfos[0].EType > EnemyType.E_POLICE)
				{
					select_round2 = Random.Range(0, currentSpawnConfig.Rounds.Count);
					round = currentSpawnConfig.Rounds[select_round2];
				}
				yield return new WaitForSeconds(round.intermission);
				if (waveNum % InstanceModeConfig.BossWaveInterval == 0 && !spawnedBoss && GotLoot >= ExpectedMaxLoot / 2)
				{
					SpawnBoss();
					spawnedBoss = true;
				}
				foreach (EnemyInfo enemyInfo in round.EnemyInfos)
				{
					Transform grave = null;
					if (enemyInfo.From == SpawnFromType.Grave)
					{
						grave = ArenaTriggerFromConfigScript.CalculateGravePosition(gameScene.GetPlayer().GetTransform());
					}
					for (int k = 0; k < enemyInfo.Count; k++)
					{
						for (int m_enemyLeft = GameApp.GetInstance().GetGameScene().GetEnemies()
							.Count; m_enemyLeft >= 8; m_enemyLeft = GameApp.GetInstance().GetGameScene().GetEnemies()
							.Count)
						{
							yield return 0;
						}
						Vector3 spawnPosition = Vector3.zero;
						if (enemyInfo.From == SpawnFromType.Door)
						{
							int doorIdx = Random.Range(0, doors.Length);
							spawnPosition = doors[doorIdx].transform.position;
						}
						else if (enemyInfo.From == SpawnFromType.Grave)
						{
							float rndX = Random.Range((0f - grave.localScale.x) / 2f, grave.localScale.x / 2f);
							float rndZ = Random.Range((0f - grave.localScale.z) / 2f, grave.localScale.z / 2f);
							spawnPosition = grave.position + new Vector3(rndX, 0f, rndZ);
							Object.Instantiate(GameApp.GetInstance().GetResourceConfig().graveRock, spawnPosition + Vector3.down * 0.3f, Quaternion.identity);
						}
						bool bElite = false;
						Enemy m_enemy = EnermyFactory.SpawnEnemy(isElite: ArenaTriggerFromConfigScript.EliteSpawn(enemyInfo.EType, enemyInfo.Count, k), id: gameScene.GetNextEnemyID(), isPrey: false, isBoss: false, enermyType: enemyInfo.EType, fromWhere: enemyInfo.From, position: spawnPosition);
						GotLoot += m_enemy.Attributes.Loot;
						yield return new WaitForSeconds(0.3f);
					}
				}
			}
			for (int enemyLeft = GameApp.GetInstance().GetGameScene().GetEnemies()
				.Count; enemyLeft > 0; enemyLeft = GameApp.GetInstance().GetGameScene().GetEnemies()
				.Count)
			{
				yield return 1;
			}
			while (gameScene.GameGUI.instanceStatistics == null)
			{
				yield return 0;
			}
			Debug.Log("******************wave completed**********************");
			FinishedWaveNum = waveNum;
			if (waveNum % InstanceModeConfig.BossWaveInterval == 0)
			{
				gameScene.GetPlayer().pickupItemsPacket.Clear();
				int id = waveNum / InstanceModeConfig.BossWaveInterval - 1;
				if (id >= InstanceModeConfig.BonusItems.Count)
				{
					id = InstanceModeConfig.BonusItems.Count - 1;
				}
				foreach (ItemType key in InstanceModeConfig.BonusItems[id].Keys)
				{
					gameScene.GetPlayer().pickupItemsPacket.Add(key, InstanceModeConfig.BonusItems[id][key]);
				}
				gameScene.GameGUI.ShowItemsReportPanel();
				Time.timeScale = 0f;
			}
			gameScene.GameGUI.instanceStatistics.GetComponent<InstanceModeGameStatisticsUI>().AddTimeEffect();
			waveNum++;
		}
	}

	private void OnTimerOut()
	{
		gameScene.SaveDataReport();
		gameScene.GameGUI.ShowGameOverPanel(GameOverType.instanceTimeOut);
		base.enabled = false;
	}

	private void SpawnBoss()
	{
		int num = Random.Range(0, 7);
		while (num == 2 || num == 3)
		{
			num = Random.Range(0, 7);
		}
		Vector3 zero = Vector3.zero;
		int num2 = Random.Range(0, doors.Length);
		zero = doors[num2].transform.position;
		EnermyFactory.SpawnEnemy(gameScene.GetNextEnemyID(), true, false, true, (EnemyType)num, SpawnFromType.Door, zero);
	}
}
