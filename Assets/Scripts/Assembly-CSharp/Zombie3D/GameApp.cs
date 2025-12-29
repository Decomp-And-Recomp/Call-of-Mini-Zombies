using System;
using System.IO;
using UnityEngine;

namespace Zombie3D
{
	public class GameApp
	{
		protected static GameApp instance;

		protected GameConfig gameConfig = new GameConfig();

		protected GameState gameState = new GameState();

		protected GameScene scene;

		protected GameScript script;

		protected ResourceConfigScript _GamerRsourceConfig;

		protected EnemyConfigScript _EnemyResourceConfig;

		protected MenuConfigScript _MenuResourceConfig;

		protected GloabConfigScript _GloabResourceConfig;

		protected string path = Application.persistentDataPath + "/";

		public DeviceOrientation PreviousOrientation { get; set; }

		protected GameApp()
		{
		}

		public static GameApp GetInstance()
		{
			if (instance == null)
			{
				instance = new GameApp();
				instance.PreviousOrientation = DeviceOrientation.Portrait;
			}
			return instance;
		}

		public void PlayerPrefsSave()
		{
			string empty = string.Empty;
			empty = "ver";
			PlayerPrefs.SetFloat(empty, 4.4f);
			empty = "Review_count";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().review_count);
			empty = "Hunting_Val";
			PlayerPrefs.SetInt(empty, GetInstance().GetGameState().Hunting_val);
			empty = "Music";
			PlayerPrefs.SetInt(empty, Convert.ToInt32(GetInstance().GetGameState().MusicOn));
			empty = "Sound";
			PlayerPrefs.SetInt(empty, Convert.ToInt32(GetInstance().GetGameState().SoundOn));
			empty = "Paper_Model_Show";
			PlayerPrefs.SetInt(empty, Convert.ToInt32(GetInstance().GetGameState().PaperModelShow));
			empty = "Nick_Name";
			PlayerPrefs.SetString(empty, GetInstance().GetGameState().nick_name);
			for (int i = 0; i < GetInstance().GetGameState().tutorialTriggers.Length; i++)
			{
				PlayerPrefs.SetInt(((TutorialTrigger)i).ToString(), GetInstance().GetGameState().tutorialTriggers[i] ? 1 : 0);
			}
			empty = "MacOs_Sensitivity";
			PlayerPrefs.SetFloat(empty, GetInstance().GetGameState().macos_sen);
		}

		public void PlayerPrefsLoad()
		{
			if (PlayerPrefs.HasKey("ver"))
			{
				GetInstance().GetGameState().Game_ver = PlayerPrefs.GetFloat("ver");
				if (GetInstance().GetGameState().Game_ver != 4.4f)
				{
					if (GetInstance().GetGameState().Game_ver < 4.29f)
					{
						GetInstance().GetGameState().makeUpCrystal = true;
					}
					GetInstance().GetGameState().show_avatar_update = true;
					GetInstance().GetGameState().Game_last_ver = GetInstance().GetGameState().Game_ver;
					GetInstance().GetGameState().Game_ver = 4.4f;
					PlayerPrefs.SetFloat("ver", 4.4f);
				}
			}
			else
			{
				GetInstance().GetGameState().Game_last_ver = 0f;
				GetInstance().GetGameState().Game_ver = 4.4f;
				PlayerPrefs.SetFloat("ver", 4.4f);
			}
			if (PlayerPrefs.HasKey("Review_count"))
			{
				GetInstance().GetGameState().review_count = PlayerPrefs.GetInt("Review_count");
			}
			else
			{
				GetInstance().GetGameState().review_count = 0;
				PlayerPrefs.SetInt("Review_count", 0);
			}
			if (PlayerPrefs.HasKey("Hunting_Val"))
			{
				GetInstance().GetGameState().Hunting_val = PlayerPrefs.GetInt("Hunting_Val");
			}
			else
			{
				GetInstance().GetGameState().Hunting_val = 0;
				PlayerPrefs.SetInt("Hunting_Val", 0);
			}
			if (PlayerPrefs.HasKey("Music"))
			{
				GetInstance().GetGameState().MusicOn = Convert.ToBoolean(PlayerPrefs.GetInt("Music"));
			}
			else
			{
				GetInstance().GetGameState().MusicOn = true;
				PlayerPrefs.SetInt("Music", 1);
			}
			if (PlayerPrefs.HasKey("Sound"))
			{
				GetInstance().GetGameState().SoundOn = Convert.ToBoolean(PlayerPrefs.GetInt("Sound"));
			}
			else
			{
				GetInstance().GetGameState().SoundOn = true;
				PlayerPrefs.SetInt("Sound", 1);
			}
			for (int i = 0; i < GetInstance().GetGameState().tutorialTriggers.Length; i++)
			{
				if (PlayerPrefs.HasKey(((TutorialTrigger)i).ToString()))
				{
					GetInstance().GetGameState().tutorialTriggers[i] = PlayerPrefs.GetInt(((TutorialTrigger)i).ToString()) == 1;
					continue;
				}
				GetInstance().GetGameState().tutorialTriggers[i] = true;
				PlayerPrefs.SetInt(((TutorialTrigger)i).ToString(), 1);
			}
			if (PlayerPrefs.HasKey("Nick_Name"))
			{
				GetInstance().GetGameState().nick_name = PlayerPrefs.GetString("Nick_Name");
			}
			else
			{
				GetInstance().GetGameState().nick_name = "None";
				PlayerPrefs.SetString("Nick_Name", "None");
			}
			if (PlayerPrefs.HasKey("Paper_Model_Show"))
			{
				GetInstance().GetGameState().PaperModelShow = Convert.ToBoolean(PlayerPrefs.GetInt("Paper_Model_Show"));
			}
			else
			{
				GetInstance().GetGameState().PaperModelShow = false;
				PlayerPrefs.SetInt("Paper_Model_Show", 0);
			}
			if (PlayerPrefs.HasKey("MacOs_Sensitivity"))
			{
				GetInstance().GetGameState().macos_sen = PlayerPrefs.GetFloat("MacOs_Sensitivity");
				return;
			}
			GetInstance().GetGameState().macos_sen = 1f;
			PlayerPrefs.SetFloat("MacOs_Sensitivity", 1f);
		}

		public void Save()
		{
			GameMode gameMode = gameState.gameMode;
			if (gameState.gameMode == GameMode.Vs)
			{
				gameState.gameMode = GameMode.None;
			}
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			gameState.SaveDataNew();
			PlayerPrefsSave();
			gameState.gameMode = gameMode;
		}

		public bool Load()
		{
            try
            {
                if (File.Exists(path + "CallMini_New.save"))
                {
                    gameState.LoadDataNew(true);
                    return true;
                }
                if (File.Exists(path + "CallMini.save"))
                {
                    gameState.LoadDataNew(false);
                    gameState.SaveDataNew();
                    return true;
                }
                if (File.Exists(path + "MySavedGame.game"))
                {
                    Stream stream = File.Open(path + "MySavedGame.game", FileMode.Open);
                    BinaryReader binaryReader = new BinaryReader(stream);
                    gameState.LoadData(binaryReader);
                    binaryReader.Close();
                    stream.Close();
                    gameState.SaveDataNew();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }

			Save();
			return false;
		}

		public void Init()
		{
			LoadResource();
			LoadConfig();
			LoadMultiAchievementConf();
			LoadVsAchievementConf();
			gameState.Init();
		}

		public void InitForMenu()
		{
			LoadResourceMenu();
			LoadConfig();
			gameState.Init();
		}

		public void LoadResource()
		{
			_GamerRsourceConfig = ResourceConfigScript.instance;
			_EnemyResourceConfig = EnemyConfigScript.instance;
			_GloabResourceConfig = GloabConfigScript.instance;
		}

		public void LoadResourceMenu()
		{
			_GloabResourceConfig = GloabConfigScript.instance;
			_MenuResourceConfig = MenuConfigScript.instance;
		}

		public void LoadConfig()
		{
			if (gameConfig.GetWeapons().Count == 0)
			{
                /*if (Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.Android)
				{
					gameConfig.LoadFromXML(null);
				}
				else
				{
					gameConfig.LoadFromXML("/");
				}*/
                gameConfig.LoadFromXML(null);
            }
		}

		public void LoadMultiAchievementConf()
		{
			if (gameConfig.Multi_AchievementConfTable.Count == 0)
			{
                /*if (Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.Android)
				{
					gameConfig.LoadMultiAchievementFromXML(null);
				}
				else
				{
					gameConfig.LoadMultiAchievementFromXML("/");
				}*/
                gameConfig.LoadMultiAchievementFromXML(null);
            }
		}

		public void LoadVsAchievementConf()
		{
			if (gameConfig.Vs_AchievementConfTable.Count == 0)
			{
                /*if (Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.Android)
				{
					gameConfig.LoadVsAchievementFromXML(null);
				}
				else
				{
					gameConfig.LoadVsAchievementFromXML("/");
				}*/
                gameConfig.LoadVsAchievementFromXML(null);
            }
		}

		public void CreateScene()
		{
			script = GameObject.Find("GameApp").GetComponent<GameScript>();
			if (GetInstance().GetGameState().gameMode == GameMode.Coop)
			{
				scene = new GameMultiplayerScene();
			}
			else if (GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				scene = new GameVSScene();
			}
			else
			{
				scene = new GameScene();
			}
			scene.Init(Application.loadedLevel - 1);
		}

		public void ClearScene()
		{
			if (scene != null)
			{
				scene.ClearAllSceneResources();
				scene = null;
			}
		}

		public void Loop(float deltaTime)
		{
			if (scene != null)
			{
				scene.DoLogic(deltaTime);
			}
		}

		public ResourceConfigScript GetGameResourceConfig()
		{
			return _GamerRsourceConfig;
		}

		public EnemyConfigScript GetEnemyResourceConfig()
		{
			return _EnemyResourceConfig;
		}

		public MenuConfigScript GetMenuResourceConfig()
		{
			return _MenuResourceConfig;
		}

		public GloabConfigScript GetGloabResourceConfig()
		{
			return _GloabResourceConfig;
		}

		public ResourceConfigScript GetResourceConfig()
		{
			return _GamerRsourceConfig;
		}

		public GameScene GetGameScene()
		{
			return scene;
		}

		public GameState GetGameState()
		{
			return gameState;
		}

		public GameConfig GetGameConfig()
		{
			return gameConfig;
		}
	}
}
