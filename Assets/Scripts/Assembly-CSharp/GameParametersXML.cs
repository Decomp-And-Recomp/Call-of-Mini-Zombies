using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using Zombie3D;

public class GameParametersXML
{
	public SpawnConfig Load(string path, int levelNum, bool isEndless)
	{
		SpawnConfig spawnConfig = new SpawnConfig();
		Stream stream = null;
		XmlDocument xmlDocument = new XmlDocument();
		if (path != null)
		{
			Debug.Log("path not null");
			path = Application.dataPath + path;
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			stream = File.Open(path + "config.xml", FileMode.Open);
			xmlDocument.Load(stream);
		}
		else
		{
			TextAsset configXml = GameApp.GetInstance().GetGloabResourceConfig().configXml;
			xmlDocument.LoadXml(configXml.text);
		}
		XmlNodeList xmlNodeList = null;
		if (isEndless)
		{
			if (levelNum < 2)
			{
				XmlNodeList xmlNodeList2 = xmlDocument.SelectNodes("Config/EnemySpawns/Endless");
				xmlNodeList = xmlNodeList2[levelNum].SelectNodes("Round");
			}
			else
			{
				xmlNodeList = xmlDocument.SelectNodes("Config/EnemySpawns/Challenge/Round");
			}
		}
		else
		{
			XmlNodeList xmlNodeList3 = xmlDocument.SelectNodes("Config/EnemySpawns/Level");
			if (levelNum <= xmlNodeList3.Count)
			{
				levelNum--;
			}
			else
			{
				int num = Random.Range(xmlNodeList3.Count - 10, xmlNodeList3.Count);
				levelNum = num;
			}
			xmlNodeList = xmlNodeList3[levelNum].SelectNodes("Round");
		}
		spawnConfig.Rounds = new List<Round>();
		foreach (XmlNode item in xmlNodeList)
		{
			Round round = new Round();
			round.EnemyInfos = new List<EnemyInfo>();
			spawnConfig.Rounds.Add(round);
			round.intermission = int.Parse(item.Attributes["intermission"].Value);
			XmlNodeList xmlNodeList4 = item.SelectNodes("Enemy");
			foreach (XmlNode item2 in xmlNodeList4)
			{
				EnemyInfo enemyInfo = new EnemyInfo();
				round.EnemyInfos.Add(enemyInfo);
				string value = item2.Attributes["id"].Value;
				enemyInfo.EType = EnemyType.E_ZOMBIE;
				for (int i = 0; i < 9; i++)
				{
					if ("E_" + value.ToUpper() == ((EnemyType)i).ToString())
					{
						enemyInfo.EType = (EnemyType)i;
						break;
					}
				}
				enemyInfo.Count = int.Parse(item2.Attributes["count"].Value);
				string value2 = item2.Attributes["from"].Value;
				if (value2 == "grave")
				{
					enemyInfo.From = SpawnFromType.Grave;
				}
				else if (value2 == "door")
				{
					enemyInfo.From = SpawnFromType.Door;
				}
			}
		}
		if (stream != null)
		{
			stream.Close();
		}
		return spawnConfig;
	}
}
