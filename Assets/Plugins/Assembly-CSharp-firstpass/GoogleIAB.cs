using UnityEngine;

public class GoogleIAB
{
    private static AndroidJavaObject _plugin;

    static GoogleIAB()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        try
        {
            using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.prime31.GoogleIABPlugin"))
            {
                _plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
            }
        }
        catch (AndroidJavaException)
        {
            Debug.Log("GoogleIAB plugin not found, skipping.");
            _plugin = null;
        }
    }

    public static void enableLogging(bool shouldEnable)
    {
        if (_plugin != null)
        {
            if (shouldEnable)
                Debug.LogWarning("YOU HAVE ENABLED HIGH DETAIL LOGS. DO NOT DISTRIBUTE THE GENERATED APK PUBLICLY. IT WILL DUMP SENSITIVE INFORMATION TO THE CONSOLE!");

            _plugin.Call("enableLogging", shouldEnable);
        }
    }

    public static void setAutoVerifySignatures(bool shouldVerify)
    {
        if (_plugin != null)
            _plugin.Call("setAutoVerifySignatures", shouldVerify);
    }

    public static void init(string publicKey)
    {
        if (_plugin != null)
            _plugin.Call("init", publicKey);
    }

    public static void unbindService()
    {
        if (_plugin != null)
            _plugin.Call("unbindService");
    }

    public static bool areSubscriptionsSupported()
    {
        if (_plugin == null)
            return false;

        return _plugin.Call<bool>("areSubscriptionsSupported");
    }

    public static void queryInventory(string[] skus)
    {
        if (_plugin != null)
            _plugin.Call("queryInventory", new object[1] { skus });
    }

    public static void purchaseProduct(string sku)
    {
        purchaseProduct(sku, string.Empty);
    }

    public static void purchaseProduct(string sku, string developerPayload)
    {
        if (_plugin != null)
            _plugin.Call("purchaseProduct", sku, developerPayload);
    }

    public static void consumeProduct(string sku)
    {
        if (_plugin != null)
            _plugin.Call("consumeProduct", sku);
    }

    public static void consumeProducts(string[] skus)
    {
        if (_plugin != null)
            _plugin.Call("consumeProducts", new object[1] { skus });
    }
}