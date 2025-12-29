using UnityEngine;

namespace Zombie3D
{
	public class SceneName
	{
		public class CoopBossType
		{
			public EnemyType zombieType;

			public bool isElite;

			public CoopBossType(EnemyType mType, bool mIsElite)
			{
				zombieType = mType;
				isElite = mIsElite;
			}
		}

		public const int SceneCount = 9;

		public const int SceneCountPixelVS = 10;

		public const string SCENE_ARENA = "Zombie3D_Arena";

		public const string SCENE_POWERSTATION = "Zombie3D_PowerStation";

		public const string SCENE_VILLAGE = "Zombie3D_Village";

		public const string SCENE_PARKING = "Zombie3D_ParkingLot";

		public const string SCENE_HOSPITAL = "Zombie3D_Hospital";

		public const string SCENE_CHURCH = "Zombie3D_Church";

		public const string SCENE_VILLAGE2 = "Zombie3D_Village2";

		public const string SCENE_VILLAGE2_3g = "Zombie3D_Village2_3g";

		public const string SCENE_RECYCLE = "Zombie3D_Recycle";

		public const string SCENE_PLAYGROUND = "Zombie3D_PlayGound";

		public const string SCENE_TUTORIAL = "Zombie3D_Tutorial";

		public const string SCENE_DESERT1 = "Zombie3D_Desert1";

		public const string SCENE_DESERT2 = "Zombie3D_Desert2";

		public const string SCENE_DESERT3 = "Zombie3D_Desert3";

		public const string SCENE_FOREST1 = "Zombie3D_Forest1";

		public const string SCENE_FOREST2 = "Zombie3D_Forest2";

		public const string SCENE_FOREST3 = "Zombie3D_Forest3";

		public const string SCENE_ROCK1 = "Zombie3D_Rock1";

		public const string SCENE_ROCK2 = "Zombie3D_Rock2";

		public const string SCENE_ROCK3 = "Zombie3D_Rock3";

		public const string SCENE_ROCK4 = "Zombie3D_Rock4";

		public const string START_MENU = "StartMenuTUI";

		public const string VS_AD = "VSAdTUI";

		public const string OPTION_MENU = "OptionTUI";

		public const string ACHIEVEMENT_SHOW_TUI = "AchievementTUI";

		public const string NICK_NAME_UI = "NickNameTUI";

		public const string MAIN_MAP = "MainMapTUI";

		public const string VS_HALL = "VSHallTUI";

		public const string COOP_HALL = "CoopHallTUI";

		public const string GAME_CREATE_ROOM = "CreateRoomTUI";

		public const string GAME_ROOM = "RoomTUI";

		public const string JOY_SHOW_UI = "PaperJoyAdTUI";

		public const string JOY_USE_UI = "PaperJoyMakeTUI";

		public const string SHOP_MENU = "ShopMenuTUI";

		public const string AVATAR_SHOP = "AvatarShopMenuTUI";

		public const string WEAPON_SHOP = "WeaponShopMenuTUI";

		public const string ITEM_SHOP = "ItemShopMenuTUI";

		public const string IAP_SHOP = "IapShopTUI";

		public const string MULTI_REPORT_TUI = "MultiReportTUI";

		public const string VS_REPORT_UI = "VSReportTUI";

		public const string REPORT_UI_TEMP = "VSReportTUITemp";

		public const string Instance_Report_UI = "InstanceReportTUI";

		public static int GetBonusNumberOfScene(string scene)
		{
			switch (scene)
			{
			case "Zombie3D_Arena":
			case "Zombie3D_Hospital":
			case "Zombie3D_PlayGound":
			case "Zombie3D_Village2":
			case "Zombie3D_Village2_3g":
			case "Zombie3D_Forest3":
				return 5;
			case "Zombie3D_Church":
			case "Zombie3D_ParkingLot":
				return 6;
			case "Zombie3D_PowerStation":
			case "Zombie3D_Recycle":
			case "Zombie3D_Desert1":
			case "Zombie3D_Desert2":
			case "Zombie3D_Desert3":
			case "Zombie3D_Forest1":
			case "Zombie3D_Forest2":
			case "Zombie3D_Rock1":
			case "Zombie3D_Rock2":
			case "Zombie3D_Rock3":
			case "Zombie3D_Rock4":
				return 4;
			case "Zombie3D_Village":
				return 7;
			default:
				return 0;
			}
		}

		public static void LoadLevel(string scene)
		{
			GameApp.GetInstance().GetGameState().last_scene = Application.loadedLevelName;
			Application.LoadLevel(scene);
		}

		public static void FadeOutLevel(string scene)
		{
			GameApp.GetInstance().GetGameState().last_scene = Application.loadedLevelName;
			GameObject.Find("TUI/TUIControl/Fade").GetComponent<TUIFade>().FadeOut(scene);
		}

		public static int GetNetMapIndex(string scene)
		{
			switch (scene)
			{
			case "Zombie3D_Arena":
				return 1;
			case "Zombie3D_PowerStation":
				return 2;
			case "Zombie3D_Village":
				return 3;
			case "Zombie3D_ParkingLot":
				return 4;
			case "Zombie3D_Hospital":
				return 5;
			case "Zombie3D_Church":
				return 6;
			case "Zombie3D_Village2":
			case "Zombie3D_Village2_3g":
				return 7;
			case "Zombie3D_Recycle":
				return 8;
			case "Zombie3D_PlayGound":
				return 9;
			case "Zombie3D_Desert3":
				return 10;
			case "Zombie3D_Forest3":
				return 11;
			case "Zombie3D_Rock3":
				return 12;
			case "Zombie3D_Rock4":
				return 13;
			case "Zombie3D_Desert1":
				return 14;
			case "Zombie3D_Desert2":
				return 15;
			case "Zombie3D_Forest1":
				return 16;
			case "Zombie3D_Forest2":
				return 17;
			case "Zombie3D_Rock1":
				return 18;
			case "Zombie3D_Rock2":
				return 19;
			default:
				return 0;
			}
		}

		public static string GetNetMapName(int index)
		{
			string result = string.Empty;
			switch (index)
			{
			case 1:
				result = "Zombie3D_Arena";
				break;
			case 2:
				result = "Zombie3D_PowerStation";
				break;
			case 3:
				result = "Zombie3D_Village";
				break;
			case 4:
				result = "Zombie3D_ParkingLot";
				break;
			case 5:
				result = "Zombie3D_Hospital";
				break;
			case 6:
				result = "Zombie3D_Church";
				break;
			case 7:
				result = "Zombie3D_Village2";
				result = ((SystemInfo.graphicsShaderLevel < 20) ? "Zombie3D_Village2_3g" : "Zombie3D_Village2");
				break;
			case 8:
				result = "Zombie3D_Recycle";
				break;
			case 9:
				result = "Zombie3D_PlayGound";
				break;
			case 10:
				result = "Zombie3D_Desert3";
				break;
			case 11:
				result = "Zombie3D_Forest3";
				break;
			case 12:
				result = "Zombie3D_Rock3";
				break;
			case 13:
				result = "Zombie3D_Rock4";
				break;
			case 14:
				result = "Zombie3D_Desert1";
				break;
			case 15:
				result = "Zombie3D_Desert2";
				break;
			case 16:
				result = "Zombie3D_Forest1";
				break;
			case 17:
				result = "Zombie3D_Forest2";
				break;
			case 18:
				result = "Zombie3D_Rock1";
				break;
			case 19:
				result = "Zombie3D_Rock2";
				break;
			}
			return result;
		}

		public static string GetTrueMapName(string map)
		{
			switch (map)
			{
			case "Zombie3D_Arena":
				return "Warehouse";
			case "Zombie3D_PowerStation":
				return "Underground Biolab";
			case "Zombie3D_Village":
				return "Residential Area";
			case "Zombie3D_ParkingLot":
				return "Parking Lot";
			case "Zombie3D_Hospital":
				return "Hospital";
			case "Zombie3D_Church":
				return "Church";
			case "Zombie3D_Village2":
			case "Zombie3D_Village2_3g":
				return "Smoldering Ruins";
			case "Zombie3D_Recycle":
				return "Scrap Yard";
			case "Zombie3D_PlayGound":
				return "Arena";
			case "Zombie3D_Desert1":
				return "Desert1";
			case "Zombie3D_Desert2":
				return "Desert2";
			case "Zombie3D_Forest1":
				return "Forest1";
			case "Zombie3D_Forest2":
				return "Forest2";
			case "Zombie3D_Rock1":
				return "Oasis1";
			case "Zombie3D_Rock2":
				return "Oasis2";
			case "Zombie3D_Desert3":
				return "BloodStrike-Desert";
			case "Zombie3D_Forest3":
				return "Sniper";
			case "Zombie3D_Rock3":
				return "Siege";
			case "Zombie3D_Rock4":
				return "DustWorld";
			default:
				return string.Empty;
			}
		}

		public static CoopBossType GetZombieTypeFromMap(string map)
		{
			switch (map)
			{
			case "Zombie3D_Arena":
				return new CoopBossType(EnemyType.E_ZOMBIE, true);
			case "Zombie3D_PowerStation":
				return new CoopBossType(EnemyType.E_DOG, true);
			case "Zombie3D_Village":
				return new CoopBossType(EnemyType.E_SWAT, true);
			case "Zombie3D_ParkingLot":
				return new CoopBossType(EnemyType.E_HELL_FIRER, false);
			case "Zombie3D_Hospital":
				return new CoopBossType(EnemyType.E_NURSE, true);
			case "Zombie3D_Church":
				return new CoopBossType(EnemyType.E_TANK, false);
			case "Zombie3D_Village2":
			case "Zombie3D_Village2_3g":
				return new CoopBossType(EnemyType.E_HELL_FIRER, true);
			case "Zombie3D_Recycle":
				return new CoopBossType(EnemyType.E_POLICE, true);
			case "Zombie3D_PlayGound":
				return new CoopBossType(EnemyType.E_TANK, true);
			default:
				return null;
			}
		}

		public static int GetRewardFromMap(string map)
		{
			switch (map)
			{
			case "Zombie3D_Arena":
				return 10000;
			case "Zombie3D_PowerStation":
				return 50000;
			case "Zombie3D_Village":
				return 30000;
			case "Zombie3D_ParkingLot":
				return 80000;
			case "Zombie3D_Hospital":
				return 100000;
			case "Zombie3D_Church":
				return 150000;
			case "Zombie3D_Village2":
			case "Zombie3D_Village2_3g":
				return 120000;
			case "Zombie3D_Recycle":
				return 120000;
			case "Zombie3D_PlayGound":
				return 300000;
			default:
				return 0;
			}
		}
	}
}
