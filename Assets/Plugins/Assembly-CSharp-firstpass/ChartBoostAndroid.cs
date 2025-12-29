using UnityEngine;

public class ChartBoostAndroid
{
    private static AndroidJavaObject _plugin;

    static ChartBoostAndroid()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        try
        {
            using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.prime31.ChartBoostPlugin"))
            {
                _plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
            }
        }
        catch (AndroidJavaException)
        {
            Debug.Log("ChartBoost plugin not found, skipping.");
            _plugin = null;
        }
    }

    public static void init(string appId, string appSignature)
    {
        if (_plugin != null)
        {
            _plugin.Call("init", appId, appSignature);
        }
    }

    public static void cacheInterstitial(string location)
    {
        if (_plugin != null)
        {
            _plugin.Call("cacheInterstitial", location ?? string.Empty);
        }
    }

    public static bool hasCachedInterstitial(string location)
    {
        if (_plugin == null)
            return false;

        return _plugin.Call<bool>("hasCachedInterstitial", location ?? string.Empty);
    }

    public static void showInterstitial(string location)
    {
        if (_plugin != null)
        {
            _plugin.Call("showInterstitial", location ?? string.Empty);
        }
    }

    public static void cacheMoreApps()
    {
        if (_plugin != null)
            _plugin.Call("cacheMoreApps");
    }

    public static bool hasCachedMoreApps()
    {
        if (_plugin == null)
            return false;

        return _plugin.Call<bool>("hasCachedMoreApps");
    }

    public static void showMoreApps()
    {
        if (_plugin != null)
            _plugin.Call("showMoreApps");
    }
}
