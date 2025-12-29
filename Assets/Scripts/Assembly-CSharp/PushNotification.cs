using System;
using System.Collections.Generic;

public class PushNotification
{
	public static void ReSetNotifications()
	{
		LocalNotificationWrapper.CancelAll();
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		list.Add("Violence isn't always the answer, but it is in VS!");
		list.Add("Co-op is here!Take on zombies with your friends for awesome rewards!");
		list.Add("The zombie infection is spreading! Save the world before it's too late!");
		list.Add("Invite your friends to play co-op for totally new achievements!");
		list.Add("Grab a friend and try co-op to get sweet new rewards and achievements!");
		list.Add("Still missing guns or avatars? Play co-op to unlock them all!");
		list.Add("The zombie horde is descending; hurry up and fight them off!");
		list.Add("A huge zombie is on the warpath; grab your gun and start shooting!");
		list.Add("Your friends are under siege; help them out before it's too late!");
		Random random = new Random();
		foreach (string item in list)
		{
			int index = random.Next(list2.Count + 1);
			list2.Insert(index, item);
		}
		int num = 1;
		foreach (string item2 in list2)
		{
			LocalNotificationWrapper.Schedule(item2, 259200 * num);
			num++;
		}
	}
}
