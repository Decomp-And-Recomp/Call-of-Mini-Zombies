using Zombie3D;

public class MultiAchievementCfg
{
	public MultiAchievementType type;

	public int m_index = -1;

	public bool finish;

	public int finishGameCount;

	public string icon;

	public string content;

	public string m_class;

	public int level;

	public string map;

	public int gameCount;

	public string needWeapon;

	public AvatarType needAvatar;

	public int rewardCash;

	public void SetTypeWith(string typeStr)
	{
		switch (typeStr)
		{
		case "AllType":
			type = MultiAchievementType.A_AllType;
			break;
		case "MVP":
			type = MultiAchievementType.A_MVP;
			break;
		case "BossType":
			type = MultiAchievementType.A_BossType;
			break;
		case "WeaponType":
			type = MultiAchievementType.A_WeaponType;
			break;
		case "AvatarType":
			type = MultiAchievementType.A_AvatarType;
			break;
		}
	}
}
