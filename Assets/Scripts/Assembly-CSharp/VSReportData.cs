using System.Collections.Generic;
using UnityEngine;
using Zombie3D;

public class VSReportData : MonoBehaviour
{
	public List<VSPlayerReport> player_reports;

	public int combo_kill_count;

	public int total_kill_count;

	public float freshman_time;

	public List<VsAchievementCfg> Achievement_Finished_Array = new List<VsAchievementCfg>();

	public void CheckMultiAchievementFinishStatus()
	{
		VsAchievementCfg vsAchievementCfg = null;
		foreach (VsAchievementCfg item in GameApp.GetInstance().GetGameConfig().Vs_AchievementConfTable)
		{
			vsAchievementCfg = null;
			switch (item.type)
			{
			case VsAchievementType.A_Combo_Kill:
				if (combo_kill_count >= item.total_kill)
				{
					vsAchievementCfg = item;
				}
				break;
			case VsAchievementType.A_Map_Kill:
				if (SceneName.GetNetMapIndex(GameApp.GetInstance().GetGameState().cur_net_map) == item.mission_type && total_kill_count >= item.total_kill)
				{
					vsAchievementCfg = item;
				}
				break;
			case VsAchievementType.A_Freshman:
				if (freshman_time >= item.battle_time)
				{
					vsAchievementCfg = item;
				}
				break;
			}
			if (vsAchievementCfg != null && GameApp.GetInstance().GetGameState().VsAchievementData[vsAchievementCfg.m_index] == 0)
			{
				GameApp.GetInstance().GetGameState().VsAchievementData[vsAchievementCfg.m_index] = 1;
				vsAchievementCfg.finish = true;
				Achievement_Finished_Array.Add(vsAchievementCfg);
			}
		}
		Debug.Log("Finish Achievement Count : " + Achievement_Finished_Array.Count);
	}

	public void GiveReward()
	{
		if (Achievement_Finished_Array.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (VsAchievementCfg item in Achievement_Finished_Array)
		{
			if (item.reward_cash > 0)
			{
				GameApp.GetInstance().GetGameState().AddCash(item.reward_cash);
			}
			if (item.reward_avata != AvatarType.None && GameApp.GetInstance().GetGameState().GetAvatarByType(item.reward_avata)
				.existState != 0)
			{
				GameApp.GetInstance().GetGameState().GetAvatarByType(item.reward_avata)
					.existState = AvatarState.Avaliable;
				GameApp.GetInstance().GetGameState().Achievement.GotNewAvatar();
				GameApp.GetInstance().GetGameState().unlockNewAvatar.Add(item.reward_avata);
			}
			if (item.reward_weapon != null)
			{
				list.Add(item.reward_weapon);
			}
		}
		List<Weapon> weapons = GameApp.GetInstance().GetGameState().GetWeapons();
		if (list.Count <= 0)
		{
			return;
		}
		foreach (string item2 in list)
		{
			foreach (Weapon item3 in weapons)
			{
				if (item3.Name == item2 && item3.Exist == WeaponExistState.Locked)
				{
					item3.Exist = WeaponExistState.Unlocked;
					GameApp.GetInstance().GetGameState().unlockNewWeapon.Add(item3.weapon_index);
				}
			}
		}
	}
}
