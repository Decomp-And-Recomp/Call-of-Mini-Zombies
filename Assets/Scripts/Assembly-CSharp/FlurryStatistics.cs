using System;
using System.Collections;
using UnityEngine;
using Zombie3D;

public class FlurryStatistics
{
	public class EventName
	{
		public static string FightCoop = "Fight_Coop";

		public static string FightVS = "Fight_VS";

		public static string FightNormal = "Fight_Normal";

		public static string FightEndess = "Fight_Endless";

		public static string FightQuit = "Fight_Quit";

		public static string OnlineConnect = "Online_Connect";

		public static string OnlineDisconnect = "Online_Disconnect";

		public static string FightLose = "Fight_Lose";

		public static string ShopEnter = "Shop_Enter";

		public static string ShopBuy = "Shop_Buy";

		public static string ShopUpgrade = "Shop_Upgrade";

		public static string BuyAvatar = "Shop_BuyAvatar";

		public static string BuyWeapon = "Shop_BuyWeapon";

		public static string BuyAmmo = "Shop_BuyAmmo";

		public static string BuyItem = "Shop_ItemBuy";

		public static string IAPBuy = "IAP_Buy";

		public static string ShopGold = "Shop_Gold";

		public static string ShopCrystal = "Shop_tCrystal";

		public static string CoopChooseMap = "Coop_Choose_Map";

		public static string VsChooseMap = "VS_Choose_Map";
	}

	public static void EnterShopEvent()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Avatar", GameApp.GetInstance().GetGameState().Avatar.ToString());
		hashtable.Add("Weapon1", GameApp.GetInstance().GetGameState().GetBattleWeapons()[0].Name);
		hashtable.Add("Weapon2", GameApp.GetInstance().GetGameState().GetBattleWeapons()[1].Name);
		if (GameApp.GetInstance().GetGameState().GetBattleWeapons()
			.Count > 2)
		{
			hashtable.Add("Weapon3", GameApp.GetInstance().GetGameState().GetBattleWeapons()[2].Name);
		}
		else
		{
			hashtable.Add("Weapon3", "None");
		}
		hashtable.Add("Item1", GameApp.GetInstance().GetGameState().GetItemsCarried()[0].iType.ToString());
		hashtable.Add("Item2", GameApp.GetInstance().GetGameState().GetItemsCarried()[1].iType.ToString());
		hashtable.Add("Avatar_Level", GameApp.GetInstance().GetGameState().GetAvatarByType(GameApp.GetInstance().GetGameState().Avatar)
			.level.ToString());
		FlurryPlugin.logEvent(EventName.ShopEnter, hashtable);
	}

	public static void BuyAvatarEvent(AvatarType avatar, bool isCrystal)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Type", "Avatar");
		FlurryPlugin.logEvent(EventName.ShopBuy, hashtable);
		hashtable = new Hashtable();
		hashtable.Add("Avatar", avatar.ToString());
		FlurryPlugin.logEvent(EventName.BuyAvatar, hashtable);
		hashtable = new Hashtable();
		hashtable.Add("Type", "Avatar");
		if (isCrystal)
		{
			FlurryPlugin.logEvent(EventName.ShopCrystal, hashtable);
		}
		else
		{
			FlurryPlugin.logEvent(EventName.ShopGold, hashtable);
		}
	}

	public static void BuyWeaponEvent(Weapon weapon)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Type", "Weapon");
		FlurryPlugin.logEvent(EventName.ShopBuy, hashtable);
		hashtable = new Hashtable();
		hashtable.Add("Weapon", weapon.Name);
		FlurryPlugin.logEvent(EventName.BuyWeapon, hashtable);
		hashtable = new Hashtable();
		hashtable.Add("Type", "Weapon");
		if (weapon.WConf.isCrystalBuy)
		{
			FlurryPlugin.logEvent(EventName.ShopCrystal, hashtable);
		}
		else
		{
			FlurryPlugin.logEvent(EventName.ShopGold, hashtable);
		}
	}

	public static void BuyAmmoEvent(Weapon weapon)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Type", "Ammo");
		FlurryPlugin.logEvent(EventName.ShopBuy, hashtable);
		hashtable = new Hashtable();
		hashtable.Add("Weapon", weapon.Name);
		FlurryPlugin.logEvent(EventName.BuyAmmo, hashtable);
		hashtable = new Hashtable();
		hashtable.Add("Type", "Ammo");
		FlurryPlugin.logEvent(EventName.ShopGold, hashtable);
	}

	public static void UpgradeWeaponEvent(Weapon weapon)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Type", "Upgrade");
		FlurryPlugin.logEvent(EventName.ShopBuy, hashtable);
		hashtable = new Hashtable();
		hashtable.Add("Weapon", weapon.Name);
		hashtable.Add("Weapon_Level", weapon.Level.ToString());
		FlurryPlugin.logEvent(EventName.ShopUpgrade, hashtable);
		hashtable = new Hashtable();
		hashtable.Add("Type", "Upgrade");
		FlurryPlugin.logEvent(EventName.ShopGold, hashtable);
	}

	public static void BuyItems(Item item)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Type", "Item");
		FlurryPlugin.logEvent(EventName.ShopBuy, hashtable);
		hashtable = new Hashtable();
		hashtable.Add("Item", item.iType.ToString());
		FlurryPlugin.logEvent(EventName.BuyItem, hashtable);
		hashtable = new Hashtable();
		hashtable.Add("Type", "Item");
		FlurryPlugin.logEvent(EventName.ShopGold, hashtable);
	}

	public static void IapBuy(IAPName iap_name)
	{
		Hashtable hashtable = new Hashtable();
		string value = string.Empty;
		switch (iap_name)
		{
		case IAPName.Cash1:
			value = "120,000 COIN PACK";
			break;
		case IAPName.Cash2:
			value = "330,000 COIN PACK";
			break;
		case IAPName.Cash3:
			value = "760,000 COIN PACK";
			break;
		case IAPName.Cash4:
			value = "1,500,000 COIN PACK";
			break;
		case IAPName.Cash5:
			value = "4,200,000 COIN PACK";
			break;
		case IAPName.Crystal1:
			value = "50 tCRYSTAL PACK";
			break;
		case IAPName.Crystal2:
			value = "150 tCRYSTAL PACK";
			break;
		case IAPName.Crystal3:
			value = "350 tCRYSTAL PACK";
			break;
		case IAPName.Crystal4:
			value = "800 tCRYSTAL PACK";
			break;
		case IAPName.Crystal5:
			value = "2500 tCRYSTAL PACK";
			break;
		case IAPName.Exp15:
			value = "1.5x EXP CARD";
			break;
		case IAPName.Exp25:
			value = "2.5x EXP CARD";
			break;
		case IAPName.IapDiscountActive:
			value = "$11.99 DISCOUNT PACK";
			break;
		case IAPName.IapDiscountInactive1:
			value = "$2.99 DISCOUNT PACK";
			break;
		case IAPName.IapDiscountInactive2:
			value = "$1.99 DISCOUNT PACK";
			break;
		case IAPName.IapDiscountInactive3:
			value = "$0.99 DISCOUNT PACK";
			break;
		case IAPName.Newbie1:
			value = "$0.99 NEWBIE PACK";
			break;
		case IAPName.Newbie2:
			value = "$5.99 NEWBIE PACK";
			break;
		case IAPName.PaperJoy:
			value = "PAPER JOY";
			break;
		case IAPName.PaperJoyRestore:
			value = "PAPER JOY RESTORE";
			break;
		}
		hashtable.Add("IAP", value);
		hashtable.Add("Current_Level", GameApp.GetInstance().GetGameState().LevelNum.ToString());
		FlurryPlugin.logEvent(EventName.IAPBuy, hashtable);
	}

	public static void CoopStartEvent()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Avatar", GameApp.GetInstance().GetGameState().Avatar.ToString());
		hashtable.Add("Weapon1", GameApp.GetInstance().GetGameState().GetBattleWeapons()[0].Name);
		hashtable.Add("Weapon2", GameApp.GetInstance().GetGameState().GetBattleWeapons()[1].Name);
		if (GameApp.GetInstance().GetGameState().GetBattleWeapons()
			.Count > 2)
		{
			hashtable.Add("Weapon3", GameApp.GetInstance().GetGameState().GetBattleWeapons()[2].Name);
		}
		else
		{
			hashtable.Add("Weapon3", "None");
		}
		hashtable.Add("Item1", GameApp.GetInstance().GetGameState().GetItemsCarried()[0].iType.ToString());
		hashtable.Add("Item2", GameApp.GetInstance().GetGameState().GetItemsCarried()[1].iType.ToString());
		hashtable.Add("Avatar_Level", GameApp.GetInstance().GetGameState().GetAvatarByType(GameApp.GetInstance().GetGameState().Avatar)
			.level.ToString());
		FlurryPlugin.logEvent(EventName.FightCoop, hashtable, true);
	}

	public static void CoopEndEvent(string result, int deaths)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Result", result);
		hashtable.Add("Deaths", deaths.ToString());
		FlurryPlugin.endTimedEvent(EventName.FightCoop, hashtable);
	}

	public static void VsStartEvent()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Avatar", GameApp.GetInstance().GetGameState().Avatar.ToString());
		hashtable.Add("Weapon1", GameApp.GetInstance().GetGameState().GetBattleWeapons()[0].Name);
		hashtable.Add("Weapon2", GameApp.GetInstance().GetGameState().GetBattleWeapons()[1].Name);
		if (GameApp.GetInstance().GetGameState().GetBattleWeapons()
			.Count > 2)
		{
			hashtable.Add("Weapon3", GameApp.GetInstance().GetGameState().GetBattleWeapons()[2].Name);
		}
		else
		{
			hashtable.Add("Weapon3", "None");
		}
		hashtable.Add("Item1", GameApp.GetInstance().GetGameState().GetItemsCarried()[0].iType.ToString());
		hashtable.Add("Item2", GameApp.GetInstance().GetGameState().GetItemsCarried()[1].iType.ToString());
		hashtable.Add("Avatar_Level", GameApp.GetInstance().GetGameState().GetAvatarByType(GameApp.GetInstance().GetGameState().Avatar)
			.level.ToString());
		FlurryPlugin.logEvent(EventName.FightVS, hashtable, true);
	}

	public static void VsEndEvent(string result, int kills, int deaths)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Result", result);
		hashtable.Add("KILLS", kills.ToString());
		hashtable.Add("Deaths", deaths.ToString());
		FlurryPlugin.endTimedEvent(EventName.FightVS, hashtable);
	}

	public static void SoloStartEvent()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Avatar", GameApp.GetInstance().GetGameState().Avatar.ToString());
		hashtable.Add("Weapon1", GameApp.GetInstance().GetGameState().GetBattleWeapons()[0].Name);
		hashtable.Add("Weapon2", GameApp.GetInstance().GetGameState().GetBattleWeapons()[1].Name);
		if (GameApp.GetInstance().GetGameState().GetBattleWeapons()
			.Count > 2)
		{
			hashtable.Add("Weapon3", GameApp.GetInstance().GetGameState().GetBattleWeapons()[2].Name);
		}
		else
		{
			hashtable.Add("Weapon3", "None");
		}
		hashtable.Add("Item1", GameApp.GetInstance().GetGameState().GetItemsCarried()[0].iType.ToString());
		hashtable.Add("Item2", GameApp.GetInstance().GetGameState().GetItemsCarried()[1].iType.ToString());
		hashtable.Add("Avatar_Level", GameApp.GetInstance().GetGameState().GetAvatarByType(GameApp.GetInstance().GetGameState().Avatar)
			.level.ToString());
		FlurryPlugin.logEvent(EventName.FightNormal, hashtable, true);
	}

	public static void SoloEndEvent(string result, int cur_level)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Result", result);
		hashtable.Add("Stage_Level", cur_level.ToString());
		FlurryPlugin.endTimedEvent(EventName.FightNormal, hashtable);
	}

	public static void EndlessStartEvent()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Avatar", GameApp.GetInstance().GetGameState().Avatar.ToString());
		hashtable.Add("Weapon1", GameApp.GetInstance().GetGameState().GetBattleWeapons()[0].Name);
		hashtable.Add("Weapon2", GameApp.GetInstance().GetGameState().GetBattleWeapons()[1].Name);
		if (GameApp.GetInstance().GetGameState().GetBattleWeapons()
			.Count > 2)
		{
			hashtable.Add("Weapon3", GameApp.GetInstance().GetGameState().GetBattleWeapons()[2].Name);
		}
		else
		{
			hashtable.Add("Weapon3", "None");
		}
		hashtable.Add("Item1", GameApp.GetInstance().GetGameState().GetItemsCarried()[0].iType.ToString());
		hashtable.Add("Item2", GameApp.GetInstance().GetGameState().GetItemsCarried()[1].iType.ToString());
		hashtable.Add("Avatar_Level", GameApp.GetInstance().GetGameState().GetAvatarByType(GameApp.GetInstance().GetGameState().Avatar)
			.level.ToString());
		FlurryPlugin.logEvent(EventName.FightEndess, hashtable, true);
	}

	public static void EndlessEndEvent(string result, int cur_level, int score, int wave, int lootCash, int lootExp, float duration)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Result", result);
		hashtable.Add("Stage_Level", cur_level.ToString());
		hashtable.Add("Score", score.ToString());
		hashtable.Add("Wave", wave.ToString());
		hashtable.Add("LootCash", lootCash.ToString());
		hashtable.Add("LootExp", lootExp.ToString());
		hashtable.Add("Duration", new TimeSpan(0, 0, (int)duration).ToString());
		FlurryPlugin.endTimedEvent(EventName.FightNormal, hashtable);
	}

	public static void QuitGameEvent(string type)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Type", type);
		FlurryPlugin.logEvent(EventName.FightQuit, hashtable);
	}

	public static void LoseGameEvent()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Stage_Level", GameApp.GetInstance().GetGameState().LevelNum.ToString());
		hashtable.Add("Avatar_Level", GameApp.GetInstance().GetGameState().GetAvatarByType(GameApp.GetInstance().GetGameState().Avatar)
			.level.ToString());
		FlurryPlugin.logEvent(EventName.FightLose, hashtable);
	}

	public static void ConnectServerEvent(string type)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Type", type);
		FlurryPlugin.logEvent(EventName.OnlineConnect, hashtable);
	}

	public static void ServerDisconnectEvent(string type)
	{
		if (Application.internetReachability != 0)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("Type", type);
			FlurryPlugin.logEvent(EventName.OnlineDisconnect, hashtable);
		}
	}

	public static void CoopChooseMap(string scene)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Scene", scene);
		FlurryPlugin.logEvent(EventName.CoopChooseMap, hashtable);
	}

	public static void VsChooseMap(string scene)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Scene", scene);
		FlurryPlugin.logEvent(EventName.VsChooseMap, hashtable);
	}
}
