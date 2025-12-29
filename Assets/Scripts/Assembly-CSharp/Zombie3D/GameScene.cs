using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zombie3D
{
	public class GameScene
	{
		public GameUIScriptNew GameGUI;

		protected Player player;

		protected BaseCameraScript camera;

		protected Hashtable enemyList;

		protected GameObject[] woodboxList;

		protected GameObject[] bonusList;

		public List<GameObject> itemList = new List<GameObject>();

		public GameObject[] scene_points;

		protected List<Vector3> sceneBorders = new List<Vector3>();

		public List<Player> m_multi_player_arr;

		public List<Player> m_player_set;

		public Enemy m_prey_enemy;

		protected List<ObjectPool> enemyObjectPool;

		protected PlayingState playingState;

		protected float Factor_EnemyHp = 1f;

		protected float Factor_EnemyAttackDamage = 1f;

		protected float Factor_EnemyLoot = 1f;

		protected float Factor_WoodboxLoot = 1f;

		protected int MaxLoot_Perday;

		protected bool is_game_excute;

		public float GameStartTime;

		protected int enemyID;

		public ArenaTriggerFromConfigScript ArenaTrigger;

		public ArenaTriggerInstanceScript ArenaTrigger_Instance;

		public ArenaTriggerBossScript ArenaTrigger_Boss;

		public Weapon UnlockWeapon { get; set; }

		public AvatarType UnlockAvatar { get; set; }

		public PlayingState GamePlayingState
		{
			get
			{
				return playingState;
			}
			set
			{
				playingState = value;
			}
		}

		public int EnemyID
		{
			get
			{
				return enemyID;
			}
			set
			{
				enemyID = value;
			}
		}

		public int EnemyKills { get; set; }

		public List<Vector3> GetSceneBorders()
		{
			return sceneBorders;
		}

		public Player GetPlayer()
		{
			return player;
		}

		public BaseCameraScript GetCamera()
		{
			return camera;
		}

		public void StopGameMusic()
		{
			camera.GetComponent<AudioSource>().Stop();
		}

		public void PlayLoseMusic()
		{
			camera.GetComponent<AudioSource>().Stop();
			camera.loseAudio.Play();
			camera.loseAudio.mute = !GameApp.GetInstance().GetGameState().SoundOn;
		}

		public Hashtable GetEnemies()
		{
			return enemyList;
		}

		public Enemy GetEnemyByID(string enemyID)
		{
			return (Enemy)enemyList[enemyID];
		}

		public int GetNextEnemyID()
		{
			if (enemyID >= int.MaxValue)
			{
				enemyID = 0;
			}
			enemyID++;
			return enemyID;
		}

		public float GetEnemyAttributesComputingFactor(string m_type)
		{
			switch (m_type)
			{
			case "hp":
				return Factor_EnemyHp;
			case "damage":
				return Factor_EnemyAttackDamage;
			case "loot":
				return Factor_EnemyLoot;
			case "woodboxLoot":
				if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
				{
					return 1f;
				}
				return Factor_WoodboxLoot;
			default:
				return 1f;
			}
		}

		public GameObject[] GetWoodBoxes()
		{
			return woodboxList;
		}

		public GameObject[] GetVSBonus()
		{
			return bonusList;
		}

		public virtual bool GetGameExcute()
		{
			return is_game_excute;
		}

		public void CalculateEnemyAttributesComputingFactorWithRound(int round)
		{
			float num = MonsterParametersConfig.endlessParameters["a1"];
			float num2 = MonsterParametersConfig.endlessParameters["a2"];
			float num3 = MonsterParametersConfig.endlessParameters["a3"];
			float num4 = MonsterParametersConfig.endlessParameters["a4"];
			float num5 = MonsterParametersConfig.endlessParameters["b1"];
			float num6 = MonsterParametersConfig.endlessParameters["b2"];
			float num7 = MonsterParametersConfig.endlessParameters["b3"];
			float num8 = MonsterParametersConfig.endlessParameters["b4"];
			float num9 = MonsterParametersConfig.endlessParameters["n1"];
			float num10 = MonsterParametersConfig.endlessParameters["n2"];
			float num11 = MonsterParametersConfig.endlessParameters["n3"];
			float num12 = MonsterParametersConfig.endlessParameters["n4"];
			float num13 = MonsterParametersConfig.endlessParameters["o"];
			int num14 = 0;
			num14 = (((float)round < num9) ? Mathf.RoundToInt(num + (float)round * num5) : (((float)round < num10) ? Mathf.RoundToInt(num2 + ((float)round - num9) * num6) : (((float)round < num11) ? Mathf.RoundToInt(num3 + ((float)round - num10) * num7) : ((!((float)round < num12)) ? ((int)num13) : Mathf.RoundToInt(num4 + ((float)round - num11) * num8)))));
			CalculateEnemyAttributesComputingFactorWithDay(num14);
		}

		public void CalculateEnemyAttributesComputingFactorWithDay(int m_level)
		{
			int num = 40;
			int num2 = 80;
			int num3 = 120;
			if (m_level < num)
			{
				Factor_EnemyHp = MonsterParametersConfig.hpParameters["a"] * Mathf.Pow(m_level, 3f) + MonsterParametersConfig.hpParameters["b"] * Mathf.Pow(m_level, 2f) + MonsterParametersConfig.hpParameters["c"] * (float)m_level + MonsterParametersConfig.hpParameters["d"];
			}
			else if (m_level < num2)
			{
				Factor_EnemyHp = MonsterParametersConfig.hpParameters["n"] + (float)(m_level - num) * MonsterParametersConfig.hpParameters["m"];
			}
			else if (m_level < num3)
			{
				Factor_EnemyHp = MonsterParametersConfig.hpParameters["o"] + (float)(m_level - num2) * MonsterParametersConfig.hpParameters["p"];
			}
			else
			{
				Factor_EnemyHp = MonsterParametersConfig.hpParameters["r"] + (float)(m_level - num3) * MonsterParametersConfig.hpParameters["t"];
			}
			if ((float)m_level >= MonsterParametersConfig.hpParameters["x3"])
			{
				Factor_EnemyHp += MonsterParametersConfig.hpParameters["y3"];
			}
			else if ((float)m_level >= MonsterParametersConfig.hpParameters["x2"])
			{
				Factor_EnemyHp += MonsterParametersConfig.hpParameters["y2"];
			}
			else if ((float)m_level >= MonsterParametersConfig.hpParameters["x1"])
			{
				Factor_EnemyHp += MonsterParametersConfig.hpParameters["y1"];
			}
			float num4 = 0f;
			int num5 = 60;
			int num6 = 120;
			num4 = ((m_level >= num5) ? MonsterParametersConfig.damageParameters["y2"] : (MonsterParametersConfig.damageParameters["x2"] + (float)(m_level - 1) * (MonsterParametersConfig.damageParameters["y2"] - MonsterParametersConfig.damageParameters["x2"]) / (float)(UpgradeParas.AvatarMaxLevel - 1)));
			float num7 = 0f;
			num7 = ((m_level >= num6) ? MonsterParametersConfig.damageParameters["y1"] : (MonsterParametersConfig.damageParameters["x1"] + (float)(m_level - 1) * (MonsterParametersConfig.damageParameters["y1"] - MonsterParametersConfig.damageParameters["x1"]) / (float)(UpgradeParas.AvatarMaxLevel - 1)));
			float num8 = 0f;
			int num9 = 10;
			int num10 = 40;
			int num11 = 80;
			int num12 = 120;
			int num13 = 300;
			num8 = ((m_level < num9) ? (MonsterParametersConfig.damageParameters["a1"] + (float)(m_level - 1) * MonsterParametersConfig.damageParameters["b1"]) : ((m_level < num10) ? (MonsterParametersConfig.damageParameters["a2"] + (float)(m_level - num9) * MonsterParametersConfig.damageParameters["b2"]) : ((m_level < num11) ? (MonsterParametersConfig.damageParameters["a3"] + (float)(m_level - num10) * MonsterParametersConfig.damageParameters["b3"]) : ((m_level < num12) ? (MonsterParametersConfig.damageParameters["a4"] + (float)(m_level - num11) * MonsterParametersConfig.damageParameters["b4"]) : ((m_level >= num13) ? 0.2f : (MonsterParametersConfig.damageParameters["a5"] + (float)(m_level - num12) * MonsterParametersConfig.damageParameters["b5"]))))));
			Factor_EnemyAttackDamage = (num4 + num7) / num8;
			Factor_EnemyLoot = MonsterParametersConfig.lootParameters["a"] * Mathf.Pow((float)m_level - MonsterParametersConfig.lootParameters["b"], MonsterParametersConfig.lootParameters["k"]) + MonsterParametersConfig.lootParameters["c"];
		}

		public int CalculateExpectedMaxLoot()
		{
			int levelNum = GameApp.GetInstance().GetGameState().LevelNum;
			float num = MonsterParametersConfig.lootParameters["a"] * Mathf.Pow((float)levelNum - MonsterParametersConfig.lootParameters["b"], MonsterParametersConfig.lootParameters["k"]) + MonsterParametersConfig.lootParameters["c"];
			float num2 = 0f;
			float num3 = MonsterParametersConfig.maxLootParameters["day1"];
			float num4 = MonsterParametersConfig.maxLootParameters["day2"];
			float num5 = MonsterParametersConfig.maxLootParameters["day3"];
			float num6 = MonsterParametersConfig.maxLootParameters["a1"];
			float num7 = MonsterParametersConfig.maxLootParameters["a2"];
			float num8 = MonsterParametersConfig.maxLootParameters["a3"];
			float num9 = MonsterParametersConfig.maxLootParameters["b1"];
			float num10 = MonsterParametersConfig.maxLootParameters["b2"];
			float num11 = MonsterParametersConfig.maxLootParameters["b3"];
			num2 = (((float)levelNum < num3) ? (num6 + (float)(levelNum - 1) * num9) : (((float)levelNum < num4) ? (num7 + ((float)levelNum - num3) * num10) : ((!((float)levelNum < num5)) ? 80f : (num8 + ((float)levelNum - num4) * num11))));
			MaxLoot_Perday = Mathf.RoundToInt(num2 * num);
			return MaxLoot_Perday;
		}

		public int CalculateExpectedMaxLootForInstanceMode(int wave)
		{
			float num = 0f;
			num = (((float)wave < InstanceModeConfig.EnemySpawnControl["waveId2"]) ? (InstanceModeConfig.EnemySpawnControl["enemyCount1"] + ((float)wave - InstanceModeConfig.EnemySpawnControl["waveId1"]) * InstanceModeConfig.EnemySpawnControl["enemyDelta1"]) : (((float)wave < InstanceModeConfig.EnemySpawnControl["waveId3"]) ? (InstanceModeConfig.EnemySpawnControl["enemyCount2"] + ((float)wave - InstanceModeConfig.EnemySpawnControl["waveId2"]) * InstanceModeConfig.EnemySpawnControl["enemyDelta2"]) : ((!((float)wave < InstanceModeConfig.EnemySpawnControl["waveId4"])) ? InstanceModeConfig.EnemySpawnControl["enemyCount4"] : (InstanceModeConfig.EnemySpawnControl["enemyCount3"] + ((float)wave - InstanceModeConfig.EnemySpawnControl["waveId3"]) * InstanceModeConfig.EnemySpawnControl["enemyDelta3"]))));
			return Mathf.RoundToInt(num * Factor_EnemyLoot * InstanceModeConfig.CashAdjust);
		}

		public void CalculateWoodboxLoot(int wave)
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				int num = (wave - 1) / 3;
				Factor_WoodboxLoot = MonsterParametersConfig.lootParameters["woodCash"] + (float)num * 0.05f;
				return;
			}
			if (wave > 85)
			{
				wave = 85;
			}
			int num2 = (wave - 1) / 3;
			Factor_WoodboxLoot = 1f + (float)num2 * 0.05f;
		}

		public ObjectPool GetEnemyPool(EnemyType eType)
		{
			foreach (ObjectPool item in enemyObjectPool)
			{
				if (item.Name == "Pool_" + eType)
				{
					return item;
				}
			}
			return null;
		}

		public void InitEnemyPool(bool isDinoHunting)
		{
			enemyObjectPool = new List<ObjectPool>();
			if (!isDinoHunting)
			{
				for (int i = 0; i < 7; i++)
				{
					enemyObjectPool.Add(new ObjectPool());
					enemyObjectPool[i].Init("Pool_" + (EnemyType)i, GameApp.GetInstance().GetEnemyResourceConfig().enemy[i], 1, 0f);
				}
			}
			else
			{
				enemyObjectPool.Add(new ObjectPool());
				enemyObjectPool[0].Init("Pool_" + EnemyType.E_DILO, GameApp.GetInstance().GetEnemyResourceConfig().enemy[7], 1, 0f);
				enemyObjectPool.Add(new ObjectPool());
				enemyObjectPool[1].Init("Pool_" + EnemyType.E_VELOCI, GameApp.GetInstance().GetEnemyResourceConfig().enemy[8], 1, 0f);
			}
		}

		public void InitEnemyPool(EnemyType mType)
		{
			if (mType < EnemyType.E_COUNT && mType > EnemyType.E_NONE)
			{
				enemyObjectPool = new List<ObjectPool>();
				enemyObjectPool.Add(new ObjectPool());
				enemyObjectPool[0].Init("Pool_" + mType, GameApp.GetInstance().GetEnemyResourceConfig().enemy[(int)mType], 1, 0f);
			}
		}

		public virtual void Init(int index)
		{
			CreateSceneData();
			CreateSceneBorderData();
			camera = GameObject.Find("Main Camera").GetComponent<TPSSimpleCameraScript>();
			UnlockAvatar = AvatarType.None;
			UnlockWeapon = null;
			player = new Player();
			player.Init();
			camera.Init();
			bonusList = new GameObject[0];
			enemyList = new Hashtable();
			playingState = PlayingState.GamePlaying;
			GameApp.GetInstance().GetGameState().Achievement.CheckAchievemnet_NeverGiveUp();
			enemyID = 0;
			GameApp.GetInstance().GetGameState().loot_cash = 0;
			GameApp.GetInstance().GetGameState().loot_exp = 0;
			EnemyKills = 0;
			Color[] array = new Color[8]
			{
				Color.white,
				Color.red,
				Color.blue,
				Color.yellow,
				Color.magenta,
				Color.gray,
				Color.grey,
				Color.cyan
			};
			int num = UnityEngine.Random.Range(0, array.Length);
			RenderSettings.ambientLight = array[num];
			GameStartTime = Time.time;
			GC.Collect();
		}

		public virtual void DoLogic(float deltaTime)
		{
			player.DoLogic(deltaTime);
			object[] array = new object[enemyList.Count];
			enemyList.Keys.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				Enemy enemy = enemyList[array[i]] as Enemy;
				enemy.DoLogic(deltaTime);
			}
		}

		public void CreateSceneBorderData()
		{
			sceneBorders.Add(GameObject.Find("Border1").transform.position);
			sceneBorders.Add(GameObject.Find("Border2").transform.position);
			sceneBorders.Add(GameObject.Find("Border3").transform.position);
			sceneBorders.Add(GameObject.Find("Border4").transform.position);
		}

		public virtual void CreateSceneData()
		{
			scene_points = GameObject.FindGameObjectsWithTag("WayPoint");
			woodboxList = GameObject.FindGameObjectsWithTag("WoodBox");
		}

		public void ClearAllSceneResources()
		{
			player = null;
			enemyList.Clear();
			enemyList = null;
			woodboxList = null;
			bonusList = null;
			camera = null;
		}

		public virtual void SaveDataReport()
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Instance)
			{
				GameObject gameObject = new GameObject("InstanceReportObj");
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				InstanceReportData instanceReportData = gameObject.AddComponent<InstanceReportData>();
				instanceReportData.enemyKills = EnemyKills;
				instanceReportData.maxWave = ArenaTrigger_Instance.FinishedWaveNum;
				instanceReportData.survivalTime = Time.time - GameStartTime;
				instanceReportData.rewardCash = GameApp.GetInstance().GetGameState().loot_cash;
				instanceReportData.score = Mathf.RoundToInt((float)instanceReportData.rewardCash * InstanceModeConfig.ScoreAdjust);
				instanceReportData.rewardExp = GameApp.GetInstance().GetGameState().loot_exp;
			}
		}

		public virtual void QuitGameForDisconnect(float time)
		{
		}

		public virtual void TimeGameOver(float time)
		{
		}

		public virtual void UpdateGameStatistics()
		{
			if (ArenaTrigger_Instance != null)
			{
				ArenaTrigger_Instance.UpdateGameStatistics();
			}
		}
	}
}
