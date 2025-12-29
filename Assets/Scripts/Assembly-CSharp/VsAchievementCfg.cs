using Zombie3D;

public class VsAchievementCfg
{
	public VsAchievementType type;

	public int m_index = -1;

	public bool finish;

	public string icon;

	public string content;

	public string m_class;

	public int level;

	public float battle_time;

	public int total_kill;

	public int mission_type;

	public int reward_cash;

	public AvatarType reward_avata = AvatarType.None;

	public string reward_weapon;

	public void SetTypeWith(string typeStr)
	{
		switch (typeStr)
		{
		case "ComboKill":
			type = VsAchievementType.A_Combo_Kill;
			break;
		case "FreshMan":
			type = VsAchievementType.A_Freshman;
			break;
		case "MapKill":
			type = VsAchievementType.A_Map_Kill;
			break;
		}
	}
}
