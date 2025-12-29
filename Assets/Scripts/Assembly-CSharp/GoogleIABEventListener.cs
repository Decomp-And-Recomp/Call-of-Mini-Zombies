using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class GoogleIABEventListener : MonoBehaviour
{
	public IapShopTUI iap_panel;

	public static string ProductID = string.Empty;

	private void OnEnable()
	{
		GoogleIABManager.billingSupportedEvent += billingSupportedEvent;
		GoogleIABManager.billingNotSupportedEvent += billingNotSupportedEvent;
		GoogleIABManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += purchaseCompleteAwaitingVerificationEvent;
		GoogleIABManager.purchaseSucceededEvent += purchaseSucceededEvent;
		GoogleIABManager.purchaseFailedEvent += purchaseFailedEvent;
		GoogleIABManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
		GoogleIABManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
	}

	private void OnDisable()
	{
		GoogleIABManager.billingSupportedEvent -= billingSupportedEvent;
		GoogleIABManager.billingNotSupportedEvent -= billingNotSupportedEvent;
		GoogleIABManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += purchaseCompleteAwaitingVerificationEvent;
		GoogleIABManager.purchaseSucceededEvent -= purchaseSucceededEvent;
		GoogleIABManager.purchaseFailedEvent -= purchaseFailedEvent;
		GoogleIABManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
		GoogleIABManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
	}

	private void billingSupportedEvent()
	{
		Debug.Log("billingSupportedEvent");
	}

	private void billingNotSupportedEvent(string error)
	{
		Debug.Log("billingNotSupportedEvent: " + error);
	}

	private void queryInventorySucceededEvent(List<GooglePurchase> purchases, List<GoogleSkuInfo> skus)
	{
		Debug.Log(string.Format("queryInventorySucceededEvent. total purchases: {0}, total skus: {1}", purchases.Count, skus.Count));
		Prime31.Utils.logObject(purchases);
		Prime31.Utils.logObject(skus);
	}

	private void queryInventoryFailedEvent(string error)
	{
		Debug.Log("queryInventoryFailedEvent: " + error);
	}

	private void purchaseCompleteAwaitingVerificationEvent(string purchaseData, string signature)
	{
		Debug.Log("purchaseCompleteAwaitingVerificationEvent. purchaseData: " + purchaseData + ", signature: " + signature);
	}

	private void purchaseSucceededEvent(GooglePurchase purchase)
	{
		Debug.Log("purchaseSucceededEvent: " + purchase.productId);
		if (iap_panel != null)
		{
			iap_panel.PurchaseFinished(purchase.productId);
			GoogleIAB.consumeProduct(purchase.productId);
		}
	}

	private void purchaseFailedEvent(string error)
	{
		Debug.Log("purchaseFailedEvent: " + error);
		if (iap_panel != null)
		{
			iap_panel.PurchaseCanceled(error);
			if (ProductID != string.Empty)
			{
				GoogleIAB.consumeProduct(ProductID);
			}
		}
	}

	private void consumePurchaseSucceededEvent(GooglePurchase purchase)
	{
		Debug.Log("consumePurchaseSucceededEvent: " + purchase.productId);
	}

	private void consumePurchaseFailedEvent(string error)
	{
		Debug.Log("consumePurchaseFailedEvent: " + error);
	}
}
