public class GameCenterPlugin
{
	public static void Initialize()
	{
	}

	public static void Uninitialize()
	{
	}

	public static bool IsSupported()
	{
		return true;
	}

	public static bool IsLogin()
	{
		return false;
	}

	public static bool Login()
	{
		return false;
	}

	public static int LoginStatus()
	{
		return 0;
	}

	public static string GetAccount()
	{
		return string.Empty;
	}

	public static string GetName()
	{
		return string.Empty;
	}

	public static bool SubmitScore(string category, int score)
	{
		return false;
	}

	public static int SubmitScoreStatus(string category, int score)
	{
		return 0;
	}

	public static bool SubmitAchievement(string category, int percent)
	{
		return false;
	}

	public static int SubmitAchievementStatus(string category, int percent)
	{
		return 0;
	}

	public static bool OpenLeaderboard()
	{
		return false;
	}

	public static bool OpenLeaderboard(string category)
	{
		return false;
	}

	public static bool OpenAchievement()
	{
		return false;
	}
}
