using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class MultiReportData : MonoBehaviour
{
	public List<MultiAchievementCfg> Achievement_Finished_Array = new List<MultiAchievementCfg>();

	public float play_time;

	public Dictionary<string, int> userReport = new Dictionary<string, int>();

	public bool isMVP;

	public string map = string.Empty;

	public AvatarType avatar;

	public List<string> weapons;

	public void CheckMultiAchievementFinishStatus()
	{
		int num = 18;
		for (int i = 0; i < num; i++)
		{
			int num2 = i * 3;
			MultiAchievementCfg multiAchievementCfg = GameApp.GetInstance().GetGameConfig().Multi_AchievementConfTable[num2];
			switch (multiAchievementCfg.type)
			{
			case MultiAchievementType.A_AllType:
				AddCurrentAchiClass(num2);
				break;
			case MultiAchievementType.A_MVP:
				if (isMVP)
				{
					AddCurrentAchiClass(num2);
				}
				break;
			case MultiAchievementType.A_AvatarType:
				if (avatar == multiAchievementCfg.needAvatar)
				{
					AddCurrentAchiClass(num2);
				}
				break;
			case MultiAchievementType.A_BossType:
				if (map == multiAchievementCfg.map)
				{
					AddCurrentAchiClass(num2);
				}
				break;
			case MultiAchievementType.A_WeaponType:
				if (map == multiAchievementCfg.map && weapons.Contains(multiAchievementCfg.needWeapon))
				{
					AddCurrentAchiClass(num2);
				}
				break;
			}
		}
	}

	private void AddCurrentAchiClass(int class_index)
	{
		for (int i = 0; i < 3; i++)
		{
			MultiAchievementCfg multiAchievementCfg = GameApp.GetInstance().GetGameConfig().Multi_AchievementConfTable[class_index + i];
			if (!multiAchievementCfg.finish)
			{
				multiAchievementCfg.finishGameCount++;
				GameApp.GetInstance().GetGameState().MultiAchievementData[multiAchievementCfg.m_index] = multiAchievementCfg.finishGameCount;
				if (multiAchievementCfg.finishGameCount == multiAchievementCfg.gameCount)
				{
					multiAchievementCfg.finish = true;
					Achievement_Finished_Array.Add(multiAchievementCfg);
					GameApp.GetInstance().GetGameState().AddCash(multiAchievementCfg.rewardCash);
				}
				break;
			}
		}
	}
}
