using TNetSdk;
using UnityEngine;

public class TNetConnection : MonoBehaviour
{
	private static TNetConnection mInstance;

	private static TNetObject tnetObj;

	public static bool is_server;

	public static TNetObject Connection
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new GameObject("TNetConnection").AddComponent(typeof(TNetConnection)) as TNetConnection;
			}
			return tnetObj;
		}
		set
		{
			if (mInstance == null)
			{
				mInstance = new GameObject("TNetConnection").AddComponent(typeof(TNetConnection)) as TNetConnection;
			}
			tnetObj = value;
		}
	}

	public static bool IsInitialized
	{
		get
		{
			return tnetObj != null;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnApplicationQuit()
	{
		if (Application.platform != RuntimePlatform.IPhonePlayer && tnetObj == null)
		{
		}
	}

	public static void UnregisterSFSSceneCallbacks()
	{
		if (tnetObj != null)
		{
			tnetObj.RemoveAllEventListeners();
		}
	}

	public static void Disconnect()
	{
		if (Application.platform != RuntimePlatform.IPhonePlayer)
		{
		}
		tnetObj = null;
	}
}
