using System.Collections;
using UnityEngine;
using Zombie3D;

public class ArenaTriggerFromConfigScript : MonoBehaviour
{
	protected GameScene gameScene;

	protected GameObject[] doors;

	protected int currentDoorIndex;

	private SpawnConfig spawnConfigInfo;

	private bool spawnedBoss;

	private bool isDinoHunting;

	private IEnumerator Start()
	{
		while (!GameUIScriptNew.GetGameUIScript().uiInited)
		{
			yield return 1;
		}
		gameScene = GameApp.GetInstance().GetGameScene();
		gameScene.ArenaTrigger = this;
		gameScene.InitEnemyPool(GameApp.GetInstance().GetGameState().gameMode == GameMode.DinoHunting);
		doors = GameObject.FindGameObjectsWithTag("Door");
		GameParametersXML paramXML = new GameParametersXML();
		if (Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.Android)
		{
			spawnConfigInfo = paramXML.Load(null, GameApp.GetInstance().GetGameState().LevelNum, false);
		}
		else
		{
			spawnConfigInfo = paramXML.Load("/", GameApp.GetInstance().GetGameState().LevelNum, false);
		}
		gameScene.CalculateEnemyAttributesComputingFactorWithDay(GameApp.GetInstance().GetGameState().LevelNum);
		int ExpectedMaxLoot = gameScene.CalculateExpectedMaxLoot();
		gameScene.CalculateWoodboxLoot(1);
		yield return new WaitForSeconds(3f);
		CheckDinoHunting();
		CheckSpawnPrey();
		yield return new WaitForSeconds(2f);
		while (GameApp.GetInstance().GetGameState().loot_cash < ExpectedMaxLoot)
		{
			Round round;
			if (!isDinoHunting)
			{
				int select_round = Random.Range(0, spawnConfigInfo.Rounds.Count);
				round = spawnConfigInfo.Rounds[select_round];
				while (round.EnemyInfos[0].EType > EnemyType.E_POLICE)
				{
					select_round = Random.Range(0, spawnConfigInfo.Rounds.Count);
					round = spawnConfigInfo.Rounds[select_round];
				}
			}
			else
			{
				round = spawnConfigInfo.Rounds[0];
				for (int i = 0; i < spawnConfigInfo.Rounds.Count; i++)
				{
					if (spawnConfigInfo.Rounds[i].EnemyInfos[0].EType > EnemyType.E_POLICE)
					{
						round = spawnConfigInfo.Rounds[i];
						break;
					}
				}
			}
			yield return new WaitForSeconds(round.intermission);
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.SoloBoss && !spawnedBoss && GameApp.GetInstance().GetGameState().loot_cash >= ExpectedMaxLoot / 2)
			{
				SpawnBoss();
				spawnedBoss = true;
			}
			foreach (EnemyInfo enemyInfo in round.EnemyInfos)
			{
				Transform grave = null;
				if (enemyInfo.From == SpawnFromType.Grave)
				{
					grave = CalculateGravePosition(gameScene.GetPlayer().GetTransform());
				}
				for (int j = 0; j < enemyInfo.Count; j++)
				{
					for (int enemyLeft = GameApp.GetInstance().GetGameScene().GetEnemies()
						.Count; enemyLeft >= 8; enemyLeft = GameApp.GetInstance().GetGameScene().GetEnemies()
						.Count)
					{
						yield return 0;
					}
					Vector3 spawnPosition = Vector3.zero;
					if (enemyInfo.From == SpawnFromType.Door)
					{
						spawnPosition = doors[currentDoorIndex].transform.position;
						currentDoorIndex++;
						if (currentDoorIndex == doors.Length)
						{
							currentDoorIndex = 0;
						}
					}
					else if (enemyInfo.From == SpawnFromType.Grave)
					{
						float rndX = Random.Range((0f - grave.localScale.x) / 2f, grave.localScale.x / 2f);
						float rndZ = Random.Range((0f - grave.localScale.z) / 2f, grave.localScale.z / 2f);
						spawnPosition = grave.position + new Vector3(rndX, 0f, rndZ);
						Object.Instantiate(GameApp.GetInstance().GetResourceConfig().graveRock, spawnPosition + Vector3.down * 0.3f, Quaternion.identity);
					}
					bool bElite = false;
					EnermyFactory.SpawnEnemy(isElite: EliteSpawn(enemyInfo.EType, enemyInfo.Count, j), id: gameScene.GetNextEnemyID(), isPrey: false, isBoss: false, enermyType: enemyInfo.EType, fromWhere: enemyInfo.From, position: spawnPosition);
					yield return new WaitForSeconds(0.3f);
				}
			}
		}
		int enemyCount = GameApp.GetInstance().GetGameScene().GetEnemies()
			.Count;
		while (enemyCount > 0)
		{
			enemyCount = GameApp.GetInstance().GetGameScene().GetEnemies()
				.Count;
			yield return 0;
		}
		GameApp.GetInstance().GetGameScene().GamePlayingState = PlayingState.GameWin;
		gameScene.GameGUI.dayClear.SetActive(true);
		GameApp.GetInstance().GetGameState().Achievement.CheckAchievemnet_Survivior(GameApp.GetInstance().GetGameState().LevelNum);
		GameApp.GetInstance().GetGameState().Achievement.CheckAchievemnet_LastSurvivior(GameApp.GetInstance().GetGameState().LevelNum);
		GameApp.GetInstance().GetGameState().Achievement.SubmitScore(GameApp.GetInstance().GetGameState().LevelNum);
		FlurryStatistics.SoloEndEvent("Win", GameApp.GetInstance().GetGameState().LevelNum);
		GameApp.GetInstance().GetGameState().DayUp();
		GameApp.GetInstance().Save();
		OpenClikPlugin.Show(false);
		yield return new WaitForSeconds(4f);
		if (gameScene.UnlockWeapon != null)
		{
			gameScene.GameGUI.ShowNewItemPanel(gameScene.UnlockWeapon);
			gameScene.GameGUI.UpdateNewItemPanelTimer(3);
			yield return new WaitForSeconds(1f);
			gameScene.GameGUI.UpdateNewItemPanelTimer(2);
			yield return new WaitForSeconds(1f);
			gameScene.GameGUI.UpdateNewItemPanelTimer(1);
			yield return new WaitForSeconds(1f);
			gameScene.GameGUI.UpdateNewItemPanelTimer(0);
		}
		gameScene.GameGUI.HideNewItemPanel();
		if (gameScene.UnlockAvatar != AvatarType.None)
		{
			gameScene.GameGUI.ShowNewAvatarPanel(gameScene.UnlockAvatar);
			gameScene.GameGUI.UpdateNewItemPanelTimer(3);
			yield return new WaitForSeconds(1f);
			gameScene.GameGUI.UpdateNewItemPanelTimer(2);
			yield return new WaitForSeconds(1f);
			gameScene.GameGUI.UpdateNewItemPanelTimer(1);
			yield return new WaitForSeconds(1f);
			gameScene.GameGUI.UpdateNewItemPanelTimer(0);
		}
		gameScene.GameGUI.HideNewAvatarPanel();
		if (gameScene.GameGUI.ShowItemsReportPanel())
		{
			FadeAnimationScript.GetInstance().enabled = false;
			while (!FadeAnimationScript.GetInstance().enabled || !FadeAnimationScript.GetInstance().FadeInComplete())
			{
				yield return 0;
			}
			OpenClikPlugin.Hide();
			SceneName.LoadLevel("MainMapTUI");
			Resources.UnloadUnusedAssets();
		}
		else
		{
			FadeAnimationScript.GetInstance().StartFade(new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 1f), 1f);
			yield return new WaitForSeconds(1.5f);
			OpenClikPlugin.Hide();
			SceneName.LoadLevel("MainMapTUI");
			Resources.UnloadUnusedAssets();
		}
	}

	public static bool EliteSpawn(EnemyType eType, int spawnNum, int index)
	{
		bool result = false;
		switch (eType)
		{
		case EnemyType.E_ZOMBIE:
			result = ((spawnNum >= 5) ? (index % 5 == 4) : (Random.Range(0, 100) < 5));
			break;
		case EnemyType.E_NURSE:
			result = ((spawnNum >= 4) ? (index % 4 == 3) : (Random.Range(0, 100) < 10));
			break;
		case EnemyType.E_BOOMER:
			result = Random.Range(0, 100) < 10;
			break;
		case EnemyType.E_SWAT:
			result = Random.Range(0, 100) < 15;
			break;
		case EnemyType.E_TANK:
			if (GameApp.GetInstance().GetGameState().LevelNum > 20)
			{
				result = Random.Range(0, 100) < 50;
			}
			break;
		case EnemyType.E_DOG:
		case EnemyType.E_POLICE:
		case EnemyType.E_DILO:
		case EnemyType.E_VELOCI:
			result = Random.Range(0, 100) < 15;
			break;
		}
		return result;
	}

	public static Transform CalculateGravePosition(Transform playerTrans)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Grave");
		GameObject gameObject = null;
		float num = 99999f;
		GameObject[] array2 = array;
		foreach (GameObject gameObject2 in array2)
		{
			float sqrMagnitude = (playerTrans.position - gameObject2.transform.position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				gameObject = gameObject2;
				num = sqrMagnitude;
			}
		}
		return gameObject.transform;
	}

	private void CheckSpawnPrey()
	{
		if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Hunting)
		{
			return;
		}
		Debug.Log("SpawnPreyOne:");
		EnemyType enermyType = EnemyType.E_HELL_FIRER;
		bool isElite = false;
		if (GameApp.GetInstance().GetGameState().Hunting_val == 0)
		{
			enermyType = EnemyType.E_HELL_FIRER;
		}
		else if (GameApp.GetInstance().GetGameState().Hunting_val == 1)
		{
			int num = Random.Range(0, 11);
			while (num == 3 || num == 7 || num == 8 || num == 9)
			{
				num = Random.Range(0, 11);
			}
			enermyType = (EnemyType)num;
			if (Random.Range(0, 100) < 50)
			{
				isElite = true;
			}
		}
		Vector3 position = doors[Random.Range(0, doors.Length)].transform.position;
		Enemy huntingLevelTimer = EnermyFactory.SpawnEnemy(gameScene.GetNextEnemyID(), isElite, true, false, enermyType, SpawnFromType.Door, position);
		GameUIScriptNew.GetGameUIScript().SetHuntingLevelTimer(huntingLevelTimer);
	}

	private void CheckDinoHunting()
	{
		if (GameApp.GetInstance().GetGameState().gameMode == GameMode.DinoHunting)
		{
			Vector3 position = doors[Random.Range(0, doors.Length)].transform.position;
			EnermyFactory.SpawnEnemy(gameScene.GetNextEnemyID(), true, false, true, EnemyType.E_SUPER_DINO, SpawnFromType.Door, position);
			isDinoHunting = true;
		}
	}

	private void SpawnBoss()
	{
		int num;
		for (num = Random.Range(0, 7); num == 3; num = Random.Range(0, 7))
		{
		}
		Vector3 zero = Vector3.zero;
		Transform transform = CalculateGravePosition(gameScene.GetPlayer().GetTransform());
		float x = Random.Range((0f - transform.localScale.x) / 2f, transform.localScale.x / 2f);
		float z = Random.Range((0f - transform.localScale.z) / 2f, transform.localScale.z / 2f);
		zero = transform.position + new Vector3(x, 0f, z);
		Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().graveRock, zero + Vector3.down * 0.3f, Quaternion.identity);
		EnermyFactory.SpawnEnemy(gameScene.GetNextEnemyID(), true, false, true, (EnemyType)num, SpawnFromType.Grave, zero);
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
