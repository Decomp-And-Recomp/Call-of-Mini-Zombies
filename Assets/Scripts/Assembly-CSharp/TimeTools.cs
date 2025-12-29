using System;

public class TimeTools
{
	public static string ToHourFormat(float time)
	{
		return new TimeSpan(0, 0, (int)time).ToString();
	}

	public static string ToMinsFormat(float time)
	{
		TimeSpan timeSpan = new TimeSpan(0, 0, (int)time);
		string empty = string.Empty;
		return timeSpan.Minutes + ":" + timeSpan.Seconds;
	}
}
