using UnityEngine;

public class IAP
{
    // Stubbed for Android IL2CPP
    protected static void PurchaseProduct(string productId, string productCount)
    {
        Debug.Log("PurchaseProduct stub called: " + productId + " x " + productCount);
    }

    protected static int PurchaseStatus()
    {
        Debug.Log("PurchaseStatus stub called");
        return 1;
    }

    public static void ToPurchaseProduct(string productId, string productCount)
    {
        PurchaseProduct(productId, productCount);
    }

    public static void NowPurchaseProduct(string productId, string productCount)
    {
        if (Utils.IsIAPCrack())
        {
            Debug.Log("IsIAPCrack!!!!!!");
            return;
        }

        Debug.Log("Now consume PurchaseProduct ANDROID");
        if (GoogleIABManager.is_billingSupported)
        {
            GoogleIAB.purchaseProduct(productId);
            GoogleIABEventListener.ProductID = productId;
        }
        else
        {
            Debug.LogError("billing not Supported!");
        }
    }

    public static int purchaseStatus(object stateInfo)
    {
        return GetPurchaseStatus();
    }

    public static int OnPurchaseStatus()
    {
        return PurchaseStatus();
    }

    public static int GetPurchaseStatus()
    {
        return 1;
    }

    public static void DoRestoreProduct() { }

    public static int DoRestoreStatus() { return 1; }

    public static string[] DoRestoreGetProductId()
    {
        string empty = string.Empty;
        return empty.Split('|');
    }
}
