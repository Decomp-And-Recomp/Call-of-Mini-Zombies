using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using Zombie3D;

public class GameConfig
{
	private Dictionary<AvatarType, AvatarConfig> avatarConfTable = new Dictionary<AvatarType, AvatarConfig>();

	private Hashtable monsterConfTable = new Hashtable();

	private ArrayList weaponConfTable = new ArrayList();

	private List<ItemConfig> itemsConfTable = new List<ItemConfig>();

	private List<CoopBossConfig> coopBossConfTable = new List<CoopBossConfig>();

	public List<MultiAchievementCfg> Multi_AchievementConfTable = new List<MultiAchievementCfg>();

	public List<VsAchievementCfg> Vs_AchievementConfTable = new List<VsAchievementCfg>();

	public MonsterConfig GetMonsterConfig(string name)
	{
		return monsterConfTable[name] as MonsterConfig;
	}

	public CoopBossConfig GetCoopBossConfig(string mapName)
	{
		foreach (CoopBossConfig item in coopBossConfTable)
		{
			if (item.map == mapName)
			{
				return item;
			}
		}
		return null;
	}

	public WeaponConfig GetWeaponConfig(string name)
	{
		foreach (WeaponConfig item in weaponConfTable)
		{
			if (item.name == name)
			{
				return item;
			}
		}
		return null;
	}

	public AvatarConfig GetAvatarConfig(AvatarType type)
	{
		return avatarConfTable[type];
	}

	public List<AvatarConfig> GetAvatars()
	{
		List<AvatarConfig> list = new List<AvatarConfig>();
		foreach (AvatarType key in avatarConfTable.Keys)
		{
			list.Add(avatarConfTable[key]);
		}
		return list;
	}

	public ItemConfig GetItemConfig(ItemType m_type)
	{
		foreach (ItemConfig item in itemsConfTable)
		{
			if (item.iType == m_type)
			{
				return item;
			}
		}
		return null;
	}

	public List<ItemConfig> GetItems()
	{
		return itemsConfTable;
	}

	public List<WeaponConfig> GetWeapons()
	{
		List<WeaponConfig> list = new List<WeaponConfig>();
		foreach (WeaponConfig item in weaponConfTable)
		{
			list.Add(item);
		}
		return list;
	}

	public void LoadFromXML(string path)
	{
		XmlReader xmlReader = null;
		StringReader stringReader = null;
		Stream stream = null;
		if (path != null)
		{
			path = Application.dataPath + path;
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			stream = File.Open(path + "config.xml", FileMode.Open);
			xmlReader = XmlReader.Create(stream);
		}
		else
		{
			TextAsset configXml = GameApp.GetInstance().GetGloabResourceConfig().configXml;
			stringReader = new StringReader(configXml.text);
			xmlReader = XmlReader.Create(stringReader);
		}
		WeaponConfig weaponConfig = null;
		while (xmlReader.Read())
		{
			switch (xmlReader.NodeType)
			{
			case XmlNodeType.Element:
				if (xmlReader.Name == "Global")
				{
					LoadGlobalConf(xmlReader);
				}
				else if (xmlReader.Name == "Avatar")
				{
					LoadAvatarConf(xmlReader);
				}
				else if (xmlReader.Name.StartsWith("Monster"))
				{
					LoadMonstersConf(xmlReader);
				}
				else if (xmlReader.Name == "Weapon")
				{
					weaponConfig = new WeaponConfig();
					LoadWeaponConf(xmlReader, weaponConfig);
					weaponConfTable.Add(weaponConfig);
				}
				else if (xmlReader.Name == "AttributesUpgrade")
				{
					LoadUpgradeConf(xmlReader, weaponConfig);
				}
				else if (xmlReader.Name == "VSDamagePara")
				{
					LoadVSWeaponDamagePara(xmlReader, weaponConfig);
				}
				else if (xmlReader.Name == "VSFrequencyPara")
				{
					LoadVSWeaponFrequencyPara(xmlReader, weaponConfig);
				}
				else if (xmlReader.Name == "Item")
				{
					itemsConfTable.Add(LoadItemsConf(xmlReader));
				}
				else if (xmlReader.Name.StartsWith("Instance"))
				{
					LoadInstanceModeConfig(xmlReader);
				}
				else if (xmlReader.Name == "BossMap")
				{
					LoadCoopBossConf(xmlReader);
				}
				break;
			}
		}
		CalculateItemDropRate();
		if (xmlReader != null)
		{
			xmlReader.Close();
		}
		if (stringReader != null)
		{
			stringReader.Close();
		}
		if (stream != null)
		{
			stream.Close();
		}
	}

	private void LoadVSWeaponDamagePara(XmlReader reader, WeaponConfig weaponConf)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "base")
			{
				weaponConf.vsDamageBase = float.Parse(reader.Value);
			}
			else if (reader.Name == "delta")
			{
				weaponConf.vsDamageDelta = float.Parse(reader.Value);
			}
			else if (reader.Name == "delta2")
			{
				weaponConf.vsDamageDelta2 = float.Parse(reader.Value);
			}
		}
	}

	private void LoadVSWeaponFrequencyPara(XmlReader reader, WeaponConfig weaponConf)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "base")
			{
				weaponConf.vsFrequencyBase = float.Parse(reader.Value);
			}
			else if (reader.Name == "delta")
			{
				weaponConf.vsFrequencyDelta = float.Parse(reader.Value);
			}
		}
	}

	private void LoadAvatarConf(XmlReader reader)
	{
		AvatarConfig avatarConfig = new AvatarConfig();
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			switch (reader.Name)
			{
			case "name":
			{
				for (int i = 0; i < 19; i++)
				{
					if (((AvatarType)i).ToString() == reader.Value)
					{
						avatarConfig.name = (AvatarType)i;
						if (avatarConfig.name >= AvatarType.HumanPixel)
						{
							avatarConfig.isPixel = true;
						}
						else
						{
							avatarConfig.isPixel = false;
						}
						break;
					}
				}
				avatarConfTable.Add(avatarConfig.name, avatarConfig);
				break;
			}
			case "damageInitial":
				avatarConfig.damageInitial = float.Parse(reader.Value);
				break;
			case "damageFinal":
				avatarConfig.damageFinal = float.Parse(reader.Value);
				break;
			case "hpInitial":
				avatarConfig.hpInitial = float.Parse(reader.Value);
				break;
			case "hpFinal":
				avatarConfig.hpFinal = float.Parse(reader.Value);
				break;
			case "speedInitial":
				avatarConfig.speedInitial = float.Parse(reader.Value);
				break;
			case "speedFinal":
				avatarConfig.speedFinal = float.Parse(reader.Value);
				break;
			case "isCrystalBuy":
				avatarConfig.isCrystalBuy = int.Parse(reader.Value) == 1;
				break;
			case "unlockDay":
				avatarConfig.unlockDay = int.Parse(reader.Value);
				break;
			case "price":
				avatarConfig.price = int.Parse(reader.Value);
				break;
			case "hpWeight":
				avatarConfig.hpBuyWeight = float.Parse(reader.Value);
				break;
			case "hpPriceWeight":
				avatarConfig.hpBuyPriceWeight = float.Parse(reader.Value);
				break;
			}
		}
	}

	private void LoadGlobalConf(XmlReader reader)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "avatarUpgradeMaxLevel")
			{
				UpgradeParas.AvatarMaxLevel = int.Parse(reader.Value);
			}
			else if (reader.Name == "avatarHpBuyMaxLevel")
			{
				UpgradeParas.AvatarHpMaxLevel = int.Parse(reader.Value);
			}
			else if (reader.Name == "weaponUpgradeMidLevel")
			{
				UpgradeParas.WeaponMidLevel = int.Parse(reader.Value);
			}
			else if (reader.Name == "weaponUpgradeMaxLevel")
			{
				UpgradeParas.WeaponMaxLevel = int.Parse(reader.Value);
			}
			else if (reader.Name.StartsWith("player_upgradeExp"))
			{
				UpgradeParas.PlayerParas.Add(reader.Name.Substring(reader.Name.Length - 1), float.Parse(reader.Value));
			}
			else if (reader.Name.StartsWith("weapon_upgrade"))
			{
				UpgradeParas.WeaponParas.Add(reader.Name.Substring(reader.Name.Length - 1), float.Parse(reader.Value));
			}
			else if (reader.Name.StartsWith("avatar_hpBuy_"))
			{
				UpgradeParas.AvatarHpParas.Add(reader.Name.Substring(reader.Name.Length - 1), float.Parse(reader.Value));
			}
			else if (reader.Name.StartsWith("avatar_hpBuyPrice"))
			{
				UpgradeParas.AvatarHpPriceParas.Add(reader.Name.Substring(reader.Name.Length - 1), float.Parse(reader.Value));
			}
		}
	}

	private void LoadMonstersConf(XmlReader reader)
	{
		switch (reader.Name)
		{
		case "Monster_Hp_Computing":
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					MonsterParametersConfig.hpParameters.Add(reader.Name, float.Parse(reader.Value));
				}
			}
			break;
		case "Monster_Damage_Computing":
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					MonsterParametersConfig.damageParameters.Add(reader.Name, float.Parse(reader.Value));
				}
			}
			break;
		case "Monster_Loot_Computing":
		{
			Dictionary<string, float> dictionary = new Dictionary<string, float>();
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					dictionary.Add(reader.Name, float.Parse(reader.Value));
				}
			}
			float num = (Mathf.Pow((dictionary["y1"] - dictionary["c"]) / (dictionary["y2"] - dictionary["c"]), 1f / dictionary["k"]) * dictionary["x2"] - dictionary["x1"]) / (Mathf.Pow((dictionary["y1"] - dictionary["c"]) / (dictionary["y2"] - dictionary["c"]), 1f / dictionary["k"]) - 1f);
			float value = (dictionary["y2"] - dictionary["c"]) / Mathf.Pow(dictionary["x2"] - num, dictionary["k"]);
			MonsterParametersConfig.lootParameters.Add("a", value);
			MonsterParametersConfig.lootParameters.Add("b", num);
			MonsterParametersConfig.lootParameters.Add("c", dictionary["c"]);
			MonsterParametersConfig.lootParameters.Add("k", dictionary["k"]);
			MonsterParametersConfig.lootParameters.Add("woodCash", dictionary["woodCash"]);
			break;
		}
		case "Monster_Hunting_Computing":
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					MonsterParametersConfig.huntingParameters.Add(reader.Name, float.Parse(reader.Value));
				}
			}
			break;
		case "Monster_Endless_Computing":
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					MonsterParametersConfig.endlessParameters.Add(reader.Name, float.Parse(reader.Value));
				}
			}
			break;
		case "Monster_MaxLoot_Expected_Computing":
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					MonsterParametersConfig.maxLootParameters.Add(reader.Name, float.Parse(reader.Value));
				}
			}
			break;
		case "Monster":
		{
			MonsterConfig monsterConfig = new MonsterConfig();
			if (!reader.HasAttributes)
			{
				break;
			}
			while (reader.MoveToNextAttribute())
			{
				switch (reader.Name)
				{
				case "name":
					monsterConfTable.Add(reader.Value, monsterConfig);
					break;
				case "hpWeight":
					monsterConfig.hpWeight = float.Parse(reader.Value);
					break;
				case "damageWeight":
					monsterConfig.damageWeight = float.Parse(reader.Value);
					break;
				case "lootWeight":
					monsterConfig.lootWeight = float.Parse(reader.Value);
					break;
				case "attackFrequency":
					monsterConfig.attackFrequency = float.Parse(reader.Value);
					break;
				case "walkSpeed":
					monsterConfig.walkSpeed = float.Parse(reader.Value);
					break;
				case "rushSpeed":
					monsterConfig.rushSpeed = float.Parse(reader.Value);
					break;
				case "rushDamage":
					monsterConfig.rushDamage = float.Parse(reader.Value);
					break;
				case "rushAttack":
					monsterConfig.rushAttackDamage = float.Parse(reader.Value);
					break;
				case "rushRate":
					monsterConfig.rushInterval = float.Parse(reader.Value);
					break;
				case "dashSpeed":
					monsterConfig.dashSpeed = float.Parse(reader.Value);
					break;
				case "dashDamage":
					monsterConfig.dashDamage = float.Parse(reader.Value);
					break;
				}
			}
			break;
		}
		}
	}

	private void LoadCoopBossConf(XmlReader reader)
	{
		CoopBossConfig coopBossConfig = new CoopBossConfig();
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			switch (reader.Name)
			{
			case "name":
				coopBossConfig.map = "Zombie3D_" + reader.Value;
				coopBossConfTable.Add(coopBossConfig);
				break;
			case "day":
				coopBossConfig.day = int.Parse(reader.Value);
				break;
			case "bossDamage":
				coopBossConfig.bossDamage = float.Parse(reader.Value);
				break;
			case "bossHp":
				coopBossConfig.bossHp = double.Parse(reader.Value);
				break;
			}
		}
	}

	private ItemConfig LoadItemsConf(XmlReader reader)
	{
		ItemConfig itemConfig = new ItemConfig();
		if (reader.HasAttributes)
		{
			while (reader.MoveToNextAttribute())
			{
				switch (reader.Name)
				{
				case "type":
					itemConfig.iType = Item.GetItemTypeByName(reader.Value);
					break;
				case "price":
					itemConfig.price = int.Parse(reader.Value);
					break;
				case "isCrystalBuy":
					if (int.Parse(reader.Value) == 1)
					{
						itemConfig.isCrystalBuy = true;
					}
					else
					{
						itemConfig.isCrystalBuy = false;
					}
					break;
				case "buyCount":
					itemConfig.buyCount = int.Parse(reader.Value);
					break;
				case "lastDuration":
					itemConfig.lastDuration = float.Parse(reader.Value);
					break;
				}
			}
		}
		return itemConfig;
	}

	private void LoadInstanceModeConfig(XmlReader reader)
	{
		switch (reader.Name)
		{
		case "Instance_ItemsConfig":
		{
			if (!reader.HasAttributes)
			{
				break;
			}
			Dictionary<ItemType, int> dictionary = new Dictionary<ItemType, int>();
			ItemType key = ItemType.NONE;
			int num = 0;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name.StartsWith("name"))
				{
					key = Item.GetItemTypeByName(reader.Value);
				}
				else if (reader.Name.StartsWith("count"))
				{
					num = int.Parse(reader.Value);
					dictionary.Add(key, num);
				}
			}
			InstanceModeConfig.BonusItems.Add(dictionary);
			break;
		}
		case "Instance_Enemy_Spawn_Control":
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					InstanceModeConfig.EnemySpawnControl.Add(reader.Name, float.Parse(reader.Value));
				}
			}
			break;
		case "Instance_Loot_Adjust":
			if (!reader.HasAttributes)
			{
				break;
			}
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "cashAdjust")
				{
					InstanceModeConfig.CashAdjust = float.Parse(reader.Value);
				}
				else if (reader.Name == "expAdjust")
				{
					InstanceModeConfig.ExpAdjust = float.Parse(reader.Value);
				}
				else if (reader.Name == "scoreAdjust")
				{
					InstanceModeConfig.ScoreAdjust = float.Parse(reader.Value);
				}
			}
			break;
		case "Instance_Time_Control":
			if (!reader.HasAttributes)
			{
				break;
			}
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "timeBase")
				{
					InstanceModeConfig.TimeInitial = float.Parse(reader.Value);
				}
				else if (reader.Name == "timeAdd")
				{
					InstanceModeConfig.TimeAdded = float.Parse(reader.Value);
				}
				else if (reader.Name == "bossWaveInterval")
				{
					InstanceModeConfig.BossWaveInterval = int.Parse(reader.Value);
				}
			}
			break;
		}
	}

	private void CalculateItemDropRate()
	{
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < itemsConfTable.Count; i++)
		{
			if (itemsConfTable[i].isCrystalBuy)
			{
				num2 += (float)itemsConfTable[i].price / (float)itemsConfTable[i].buyCount;
			}
			else
			{
				num += (float)itemsConfTable[i].price / (float)itemsConfTable[i].buyCount;
			}
		}
		float num3 = 0f;
		float num4 = 0f;
		for (int j = 0; j < itemsConfTable.Count; j++)
		{
			if (itemsConfTable[j].isCrystalBuy)
			{
				itemsConfTable[j].dropRate = (float)itemsConfTable[j].price / (float)itemsConfTable[j].buyCount / num2;
				itemsConfTable[j].dropRate = 1f / itemsConfTable[j].dropRate;
				num3 += itemsConfTable[j].dropRate;
			}
			else
			{
				itemsConfTable[j].dropRate = (float)itemsConfTable[j].price / (float)itemsConfTable[j].buyCount / num;
				itemsConfTable[j].dropRate = 1f / itemsConfTable[j].dropRate;
				num4 += itemsConfTable[j].dropRate;
			}
		}
		for (int k = 0; k < itemsConfTable.Count; k++)
		{
			if (itemsConfTable[k].isCrystalBuy)
			{
				itemsConfTable[k].dropRate /= num3;
			}
			else
			{
				itemsConfTable[k].dropRate /= num4;
			}
		}
	}

	private void LoadWeaponConf(XmlReader reader, WeaponConfig weaponConf)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "name")
			{
				weaponConf.name = reader.Value;
			}
			else if (reader.Name == "type")
			{
				for (int i = 1; i < 15; i++)
				{
					if (((WeaponType)i).ToString() == reader.Value)
					{
						weaponConf.wType = (WeaponType)i;
					}
				}
			}
			else if (reader.Name == "moveSpeedDrag")
			{
				weaponConf.moveSpeedDrag = float.Parse(reader.Value);
			}
			else if (reader.Name == "range")
			{
				weaponConf.range = float.Parse(reader.Value);
			}
			else if (reader.Name == "price")
			{
				weaponConf.price = int.Parse(reader.Value);
			}
			else if (reader.Name == "bulletPrice")
			{
				weaponConf.bulletPrice = int.Parse(reader.Value);
			}
			else if (reader.Name == "initBullet")
			{
				weaponConf.initBullet = int.Parse(reader.Value);
			}
			else if (reader.Name == "bullet")
			{
				weaponConf.bulletEachAdd = int.Parse(reader.Value);
			}
			else if (reader.Name == "flySpeed")
			{
				weaponConf.flySpeed = float.Parse(reader.Value);
			}
			else if (reader.Name == "startEquip")
			{
				weaponConf.startEquip = (WeaponExistState)int.Parse(reader.Value);
			}
			else if (reader.Name == "crystalBuy")
			{
				int num = int.Parse(reader.Value);
				if (num == 1)
				{
					weaponConf.isCrystalBuy = true;
				}
				else
				{
					weaponConf.isCrystalBuy = false;
				}
			}
			else if (reader.Name == "unlockLevel")
			{
				weaponConf.UnlockLevel = int.Parse(reader.Value);
			}
		}
	}

	private void LoadUpgradeConf(XmlReader reader, WeaponConfig weaponConf)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			switch (reader.Name)
			{
			case "damageInitial":
				weaponConf.damageInitial = float.Parse(reader.Value);
				break;
			case "damageFinal":
				weaponConf.damageFinal = float.Parse(reader.Value);
				break;
			case "frequencyInitial":
				weaponConf.frequencyInitial = float.Parse(reader.Value);
				break;
			case "frequencyFinal":
				weaponConf.frequencyFinal = float.Parse(reader.Value);
				break;
			case "accuracyInitial":
				weaponConf.accuracyInitial = float.Parse(reader.Value);
				break;
			case "accuracyFinal":
				weaponConf.accuracyFinal = float.Parse(reader.Value);
				break;
			case "priceWeight":
				weaponConf.upgradePriceWeight = float.Parse(reader.Value);
				break;
			}
		}
	}

	public void LoadMultiAchievementFromXML(string path)
	{
		XmlReader xmlReader = null;
		StringReader stringReader = null;
		Stream stream = null;
		if (path != null)
		{
			path = Application.dataPath + path;
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			stream = File.Open(path + "config.xml", FileMode.Open);
			xmlReader = XmlReader.Create(stream);
		}
		else
		{
			TextAsset configXml = GameApp.GetInstance().GetGloabResourceConfig().configXml;
			stringReader = new StringReader(configXml.text);
			xmlReader = XmlReader.Create(stringReader);
		}
		MultiAchievementCfg multiAchievementCfg = null;
		int num = 0;
		while (xmlReader.Read())
		{
			if (xmlReader.NodeType == XmlNodeType.Element)
			{
				if (xmlReader.Name == "Achievement")
				{
					multiAchievementCfg = new MultiAchievementCfg();
					LoadMultiAchieveMentConf(xmlReader, multiAchievementCfg);
					multiAchievementCfg.m_index = num++;
					multiAchievementCfg.finishGameCount = GameApp.GetInstance().GetGameState().MultiAchievementData[multiAchievementCfg.m_index];
					Multi_AchievementConfTable.Add(multiAchievementCfg);
				}
				else if (xmlReader.Name == "A_Content")
				{
					LoadMultiAchievementContentConf(xmlReader, multiAchievementCfg);
				}
			}
		}
		Debug.Log("coop achi count:" + num);
		if (xmlReader != null)
		{
			xmlReader.Close();
		}
		if (stringReader != null)
		{
			stringReader.Close();
		}
		if (stream != null)
		{
			stream.Close();
		}
	}

	public void LoadVsAchievementFromXML(string path)
	{
		XmlReader xmlReader = null;
		StringReader stringReader = null;
		Stream stream = null;
		if (path != null)
		{
			path = Application.dataPath + path;
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			stream = File.Open(path + "config.xml", FileMode.Open);
			xmlReader = XmlReader.Create(stream);
		}
		else
		{
			TextAsset configXml = GameApp.GetInstance().GetGloabResourceConfig().configXml;
			stringReader = new StringReader(configXml.text);
			xmlReader = XmlReader.Create(stringReader);
		}
		VsAchievementCfg vsAchievementCfg = null;
		int num = 0;
		while (xmlReader.Read())
		{
			switch (xmlReader.NodeType)
			{
			case XmlNodeType.Element:
				if (xmlReader.Name == "VSAchievement")
				{
					vsAchievementCfg = new VsAchievementCfg();
					LoadVsAchieveMentConf(xmlReader, vsAchievementCfg);
					vsAchievementCfg.m_index = num++;
					vsAchievementCfg.finish = GameApp.GetInstance().GetGameState().VsAchievementData[vsAchievementCfg.m_index] == 1;
					Vs_AchievementConfTable.Add(vsAchievementCfg);
				}
				else if (xmlReader.Name == "VSA_Reward")
				{
					LoadVsAchieveMentConfReward(xmlReader, vsAchievementCfg);
				}
				else if (xmlReader.Name == "VSA_Time")
				{
					LoadVsAchieveMentConfTime(xmlReader, vsAchievementCfg);
				}
				else if (xmlReader.Name == "VSA_Monster")
				{
					LoadVsAchieveMentConfMonster(xmlReader, vsAchievementCfg);
				}
				break;
			}
		}
		Debug.Log("vs achi count:" + num);
		if (xmlReader != null)
		{
			xmlReader.Close();
		}
		if (stringReader != null)
		{
			stringReader.Close();
		}
		if (stream != null)
		{
			stream.Close();
		}
	}

	private void LoadMultiAchieveMentConf(XmlReader reader, MultiAchievementCfg AchievementConf)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			switch (reader.Name)
			{
			case "type":
				AchievementConf.SetTypeWith(reader.Value);
				break;
			case "content":
				AchievementConf.content = reader.Value;
				break;
			case "class":
				AchievementConf.m_class = reader.Value;
				break;
			case "level":
				AchievementConf.level = int.Parse(reader.Value);
				break;
			case "icon":
				AchievementConf.icon = reader.Value;
				break;
			}
		}
	}

	private void LoadMultiAchievementContentConf(XmlReader reader, MultiAchievementCfg AchievementConf)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			switch (reader.Name)
			{
			case "map":
				AchievementConf.map = "Zombie3D_" + reader.Value;
				break;
			case "count":
				AchievementConf.gameCount = int.Parse(reader.Value);
				if (AchievementConf.gameCount == AchievementConf.finishGameCount)
				{
					AchievementConf.finish = true;
				}
				break;
			case "avatar":
			{
				for (int i = 0; i < 19; i++)
				{
					if (reader.Value == ((AvatarType)i).ToString())
					{
						AchievementConf.needAvatar = (AvatarType)i;
						break;
					}
				}
				break;
			}
			case "weapon":
				AchievementConf.needWeapon = reader.Value;
				break;
			case "Reward":
				AchievementConf.rewardCash = int.Parse(reader.Value);
				break;
			}
		}
	}

	private void LoadVsAchieveMentConf(XmlReader reader, VsAchievementCfg AchievementConf)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "type")
			{
				string value = reader.Value;
				AchievementConf.SetTypeWith(value);
			}
			else if (reader.Name == "content")
			{
				AchievementConf.content = reader.Value;
			}
			else if (reader.Name == "class")
			{
				AchievementConf.m_class = reader.Value;
			}
			else if (reader.Name == "level")
			{
				AchievementConf.level = int.Parse(reader.Value);
			}
			else if (reader.Name == "icon")
			{
				AchievementConf.icon = reader.Value;
			}
		}
	}

	private void LoadVsAchieveMentConfReward(XmlReader reader, VsAchievementCfg AchievementConf)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "cash")
			{
				AchievementConf.reward_cash = int.Parse(reader.Value);
			}
			else if (reader.Name == "avata")
			{
				AchievementConf.reward_avata = (AvatarType)int.Parse(reader.Value);
			}
			else if (reader.Name == "weapon")
			{
				AchievementConf.reward_weapon = reader.Value;
			}
		}
	}

	private void LoadVsAchieveMentConfTime(XmlReader reader, VsAchievementCfg AchievementConf)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "val")
			{
				AchievementConf.battle_time = float.Parse(reader.Value);
			}
		}
	}

	private void LoadVsAchieveMentConfMonster(XmlReader reader, VsAchievementCfg AchievementConf)
	{
		if (!reader.HasAttributes)
		{
			return;
		}
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "type")
			{
				AchievementConf.mission_type = int.Parse(reader.Value);
			}
			else if (reader.Name == "count")
			{
				AchievementConf.total_kill = int.Parse(reader.Value);
			}
		}
	}
}
