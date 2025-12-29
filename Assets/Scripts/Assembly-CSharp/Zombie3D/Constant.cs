using UnityEngine;

namespace Zombie3D
{
	public class Constant
	{
		public const float GAME_VER = 4.4f;

		public const string SupportURL = "http://www.trinitigame.com/support?game=comz&version=4.3.2";

		public const string ComZombiesURL = "http://itunes.apple.com/us/app/call-of-mini-zombies/id431213733?mt=8";

		public const string ComZombies2URL = "https://itunes.apple.com/us/app/call-of-mini-zombies-2/id605681399?ls=1&mt=8";

		public const string ComInfinityURL = "https://itunes.apple.com/us/app/call-of-mini-infinity/id605676336?ls=1&mt=8";

		public const bool LOG_SCREEN = true;

		public const bool AMAZON_ANDROID = false;

		public const string GAME_SAVE_FILE_OLD = "CallMini.save";

		public const string GAME_SAVE_FILE = "CallMini_New.save";

		public const int save_key = 71;

		public const float CAMERA_ZOOM_SPEED = 10f;

		public const int added_new_avatar_count = 1;

		public const int added_new_weapon_count = 0;

		public const int HUNTINT_START = 21;

		public const float COOP_Dino_Rate = 0.1f;

		public const float HUNTING_Rate = 0.33f;

		public const float HUNTING_Dino_Rate = 0.1f;

		public const int EnemySpawnLimit = 8;

		public const double enemy_hp_factor_added_for_elite = 2.0;

		public const float enemy_damage_factor_added_for_elite = 1.5f;

		public const double enemy_hp_factor_added_for_solo_boss = 3.0;

		public const float PASTOR_AFFECT_SPEED = 0.5f;

		public const float PASTOR_AFFECT_TIME = 3f;

		public const float COOP_MAP_VIEW_VAL = 38f;

		public const float FLOORHEIGHT = 10000.1f;

		public const string ENEMY_NAME = "E_";

		public const float SuperEnemyMaxHeight = 2.8f;

		public const float BONUS_TIME_INTERVAL = 30f;

		public const float BONUS_SPAWN_START = 15f;

		public const float PLAYING_WALKINGSPEED_DISCOUNT_WHEN_SHOOTING = 0.8f;

		public const float ANIMATION_ENDS_PERCENT = 1f;

		public const float SPARK_MIN_DISTANCE = 2f;

		public const float DOCTOR_HP_RECOVERY = 1f;

		public const int ExpBuff_Duration_15 = 10;

		public const int ExpBuff_Duration_25 = 15;

		public const float POWER_NORMAL = 1f;

		public const float POWER_BUFF = 2f;

		public const float DAMAGE_NORMAL = 1f;

		public const float DAMAGE_BUFF = 0.3f;

		public const float PixelWeapon_Damage_BUFF = 1.1f;

		public const float PixelAvatar_Hp_BUFF = 1.1f;

		public const float SUPER_MAX_HP = 50000f;

		public const int WOODBOX_LOOT = 300;

		public const int PREY_VAL = 5;

		public const int MAX_CASH = 99999999;

		public const int MAX_CRYSTAL = 99999;

		public const int Multiplayer_Limit = 4;

		public const string OPEN_CLIK_KEY = "567D21BF-DA59-41F2-B7CC-9951F6187640";

		public const int Multi_Achievement_count = 54;

		public const int Vs_Achievement_count = 36;

		public const float Multiplayer_Game_Rebirth_Timer = 10f;

		public const float Multiplayer_Game_Rebirth_Timer_Delay = 5f;

		public const float VS_TIME = 600f;

		public const float VS_CheckRoomHostTimeStep = 2f;

		public const float VS_CheckHostIdentificationTimeStep = 4f;

		public const float VS_CheckHostIdentificationTimeOut = 12f;

		public const int autoRect_width = 40;

		public const int autoRect_height = 40;

		public static float FloorHeight_Real(Vector3 playerPos)
		{
			return (!(playerPos.y < 10005f)) ? 10008.2f : 10000.1f;
		}
	}
}
