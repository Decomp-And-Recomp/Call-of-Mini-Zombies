using UnityEngine;

public class PushLocalNotification : MonoBehaviour
{
	public static void SetPushNotification()
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/LocalNotification"), Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.name = "LocalNotification";
	}

	public static PushLocalNotification GetPushNotification()
	{
		GameObject gameObject = GameObject.Find("LocalNotification").gameObject;
		if (gameObject != null)
		{
			return gameObject.GetComponent<PushLocalNotification>();
		}
		return null;
	}
}
