using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

internal class LootManagerScript : MonoBehaviour
{
	private const float ITEM_LOWPOINT = 1f;

	public static int itemIndexFactor = -1;

	public float dropRate = 1f;

	public ItemType[] itemTables;

	public float[] rateTables;

	public static void LootSpawnItem(ItemType itemType, Vector3 position, int id)
	{
		GameObject gameObject = null;
		switch (itemType)
		{
		case ItemType.Hp:
		case ItemType.Power:
		case ItemType.Gold:
		case ItemType.Gold_Big:
		case ItemType.InstantStealth:
		case ItemType.InstantSuper:
		case ItemType.Shield:
		{
			GameObject itemObjectFromType = GetItemObjectFromType(itemType);
			if (itemObjectFromType != null)
			{
				gameObject = Object.Instantiate(itemObjectFromType, position, Quaternion.identity) as GameObject;
				gameObject.GetComponent<ItemScript>().itemType = itemType;
				gameObject.GetComponent<ItemScript>().GameItemID = id;
				GameApp.GetInstance().GetGameScene().itemList.Add(gameObject);
			}
			break;
		}
		case ItemType.RandomBullets:
		{
			Weapon weapon = GameApp.GetInstance().GetGameState().RandomBattleWeapons();
			gameObject = Object.Instantiate(weapon.WeaponBulletObject, position, Quaternion.identity) as GameObject;
			gameObject.GetComponent<ItemScript>().GameItemID = id;
			GameApp.GetInstance().GetGameScene().itemList.Add(gameObject);
			break;
		}
		}
	}

	public void SpawnItem(ItemType itemType)
	{
		itemIndexFactor++;
		int gameItemID = 4 * itemIndexFactor + (int)GameApp.GetInstance().GetGameScene().GetPlayer()
			.birth_point_index;
		ResourceConfigScript resourceConfig = GameApp.GetInstance().GetResourceConfig();
		GameObject gameObject = null;
		Ray ray = new Ray(base.transform.position + Vector3.up * 1f, Vector3.down);
		float num = 10000.1f;
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f, 32768))
		{
			num = hitInfo.point.y;
		}
		Vector3 position = new Vector3(base.transform.position.x, num + 1f, base.transform.position.z);
		switch (itemType)
		{
		case ItemType.Hp:
		case ItemType.Power:
		case ItemType.Gold:
		case ItemType.Gold_Big:
		case ItemType.InstantStealth:
		case ItemType.InstantSuper:
		case ItemType.Shield:
			gameObject = Object.Instantiate(GetItemObjectFromType(itemType), position, Quaternion.identity) as GameObject;
			gameObject.GetComponent<ItemScript>().GameItemID = gameItemID;
			gameObject.GetComponent<ItemScript>().itemType = itemType;
			GameApp.GetInstance().GetGameScene().itemList.Add(gameObject);
			break;
		case ItemType.Monster:
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop || GameApp.GetInstance().GetGameScene().GetPlayer()
				.AvatarType == AvatarType.Pastor)
			{
				gameObject = Object.Instantiate(resourceConfig.itemHP, position, Quaternion.identity) as GameObject;
				gameObject.GetComponent<ItemScript>().GameItemID = gameItemID;
				gameObject.GetComponent<ItemScript>().itemType = ItemType.Hp;
				GameApp.GetInstance().GetGameScene().itemList.Add(gameObject);
				break;
			}
			gameObject = Object.Instantiate(GameApp.GetInstance().GetEnemyResourceConfig().enemy[5], position, Quaternion.identity) as GameObject;
			gameObject.name = "E_" + GameApp.GetInstance().GetGameScene().GetNextEnemyID();
			Enemy enemy = new Dog();
			enemy.IsElite = false;
			enemy.IsPrey = false;
			enemy.IsSuperBoss = false;
			enemy.Init(gameObject);
			enemy.EnemyType = EnemyType.E_DOG;
			enemy.Name = gameObject.name;
			GameApp.GetInstance().GetGameScene().GetEnemies()
				.Add(gameObject.name, enemy);
			break;
		}
		case ItemType.RandomBullets:
		{
			Weapon weapon = GameApp.GetInstance().GetGameState().RandomBattleWeapons();
			gameObject = Object.Instantiate(weapon.WeaponBulletObject, position, Quaternion.identity) as GameObject;
			gameObject.GetComponent<ItemScript>().GameItemID = gameItemID;
			GameApp.GetInstance().GetGameScene().itemList.Add(gameObject);
			break;
		}
		}
		if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop)
		{
			Packet packet = ((itemType != ItemType.Monster) ? CGEnemyLootNewPacket.MakePacket((uint)itemType, (uint)gameObject.GetComponent<ItemScript>().GameItemID, position) : CGEnemyLootNewPacket.MakePacket(0u, (uint)gameObject.GetComponent<ItemScript>().GameItemID, position));
			GameApp.GetInstance().GetGameState().net_com.Send(packet);
		}
	}

	public void OnLoot(bool isPrey)
	{
		if (isPrey)
		{
			SpawnItem(ItemType.Gold_Big);
			return;
		}
		float value = Random.value;
		if (!(value < dropRate))
		{
			return;
		}
		value = Random.value;
		float num = 0f;
		for (int i = 0; i < itemTables.Length; i++)
		{
			if (value <= num + rateTables[i])
			{
				SpawnItem(itemTables[i]);
				break;
			}
			num += rateTables[i];
		}
	}

	public void OnSuperBossLoot()
	{
		ItemType itemType = ItemType.BigHp;
		float value = Random.value;
		float num = 0f;
		List<ItemType> crystalBuyItems = Item.GetCrystalBuyItems();
		for (int i = 0; i < crystalBuyItems.Count; i++)
		{
			float num2 = GameApp.GetInstance().GetGameConfig().GetItemConfig(crystalBuyItems[i])
				.dropRate;
			if (value <= num + num2)
			{
				itemType = crystalBuyItems[i];
				Debug.Log("spawn crystal buy item: " + itemType.ToString() + ", drop rate: " + num2);
				break;
			}
			num += num2;
		}
		Debug.Log("super boss drop item: " + itemType);
		bool flag = false;
		foreach (ItemType key3 in GameApp.GetInstance().GetGameScene().GetPlayer()
			.pickupItemsPacket.Keys)
		{
			if (key3 == itemType)
			{
				Dictionary<ItemType, int> pickupItemsPacket;
				Dictionary<ItemType, int> dictionary = (pickupItemsPacket = GameApp.GetInstance().GetGameScene().GetPlayer()
					.pickupItemsPacket);
				ItemType key;
				ItemType key2 = (key = key3);
				int num3 = pickupItemsPacket[key];
				dictionary[key2] = num3 + 1;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			GameApp.GetInstance().GetGameScene().GetPlayer()
				.pickupItemsPacket.Add(itemType, 1);
		}
		GameApp.GetInstance().GetGameScene().GameGUI.ShowPickupInGame(itemType);
		GameApp.GetInstance().GetGameScene().GetPlayer()
			.GetAudioPlayer()
			.PlayAudio("GetItem");
	}

	public static GameObject GetItemObjectFromType(ItemType type)
	{
		ResourceConfigScript resourceConfig = GameApp.GetInstance().GetResourceConfig();
		switch (type)
		{
		case ItemType.Hp:
			return resourceConfig.itemHP;
		case ItemType.Power:
			return resourceConfig.itemPower;
		case ItemType.Gold:
			return resourceConfig.itemGold;
		case ItemType.Gold_Big:
			return resourceConfig.itemGold_Big;
		case ItemType.InstantSuper:
			return resourceConfig.itemSuper;
		case ItemType.InstantStealth:
			return resourceConfig.itemStealth;
		case ItemType.Shield:
			return resourceConfig.itemShield;
		case ItemType.SuicideGun:
			return resourceConfig.itemSuicideGun;
		default:
			return null;
		}
	}
}
