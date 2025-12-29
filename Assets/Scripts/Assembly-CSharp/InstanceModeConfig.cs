using System.Collections.Generic;
using Zombie3D;

public static class InstanceModeConfig
{
	public static List<Dictionary<ItemType, int>> BonusItems = new List<Dictionary<ItemType, int>>();

	public static Dictionary<string, float> EnemySpawnControl = new Dictionary<string, float>();

	public static float CashAdjust;

	public static float ExpAdjust;

	public static float ScoreAdjust;

	public static float TimeInitial;

	public static float TimeAdded;

	public static int BossWaveInterval;
}
