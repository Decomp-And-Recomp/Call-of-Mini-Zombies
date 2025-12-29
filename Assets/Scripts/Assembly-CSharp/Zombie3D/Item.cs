using System.Collections.Generic;
using UnityEngine;

namespace Zombie3D
{
	public class Item
	{
		public static ItemType[] GameItemCollection = new ItemType[7]
		{
			ItemType.SmallHp,
			ItemType.BigHp,
			ItemType.Shield,
			ItemType.Power,
			ItemType.InstantStealth,
			ItemType.InstantSuper,
			ItemType.SuicideGun
		};

		public static int MaxOwnedCount = 99;

		public bool isCarryInGame;

		public int carryPacketIndex;

		public ItemType iType { get; set; }

		public ItemConfig iConf { get; set; }


        private SafeInteger ownedCount;

        public int OwnedCount
        {
            get { return ownedCount; }
            set { ownedCount = Mathf.Clamp(value, 0, MaxOwnedCount); }
        }

        public int pickupCount { get; set; }

		public Item(ItemType type)
		{
			iType = type;
			switch (type)
			{
			case ItemType.InstantSuper:
				OwnedCount = 1;
				isCarryInGame = true;
				carryPacketIndex = 1;
				break;
			case ItemType.SmallHp:
				OwnedCount = 1;
				isCarryInGame = true;
				carryPacketIndex = 2;
				break;
			default:
				OwnedCount = 0;
				isCarryInGame = false;
				carryPacketIndex = 0;
				break;
			}
			pickupCount = 0;
		}

		public int GetCarryInGameCount()
		{
			return OwnedCount;
		}

		public static ItemType GetItemTypeByName(string name)
		{
			for (int i = 0; i < GameItemCollection.Length; i++)
			{
				if (name == GameItemCollection[i].ToString())
				{
					return GameItemCollection[i];
				}
			}
			return ItemType.NONE;
		}

		public static int GetItemCollectionIndexByType(ItemType type)
		{
			for (int i = 0; i < GameItemCollection.Length; i++)
			{
				if (GameItemCollection[i] == type)
				{
					return i;
				}
			}
			return -1;
		}

		public static List<ItemType> GetCrystalBuyItems()
		{
			List<ItemType> list = new List<ItemType>();
			for (int i = 0; i < GameItemCollection.Length; i++)
			{
				if (GameApp.GetInstance().GetGameConfig().GetItemConfig(GameItemCollection[i])
					.isCrystalBuy)
				{
					list.Add(GameItemCollection[i]);
				}
			}
			return list;
		}

		public static List<ItemType> GetCashBuyItems()
		{
			List<ItemType> list = new List<ItemType>();
			for (int i = 0; i < GameItemCollection.Length; i++)
			{
				if (!GameApp.GetInstance().GetGameConfig().GetItemConfig(GameItemCollection[i])
					.isCrystalBuy)
				{
					list.Add(GameItemCollection[i]);
				}
			}
			return list;
		}
	}
}
