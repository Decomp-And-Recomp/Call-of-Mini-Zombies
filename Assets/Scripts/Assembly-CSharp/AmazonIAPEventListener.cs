using System.Collections.Generic;
using UnityEngine;

public class AmazonIAPEventListener : MonoBehaviour
{
	public IapShopTUI iap_panel;

	private void OnEnable()
	{
		AmazonIAPManager.itemDataRequestFailedEvent += itemDataRequestFailedEvent;
		AmazonIAPManager.itemDataRequestFinishedEvent += itemDataRequestFinishedEvent;
		AmazonIAPManager.purchaseFailedEvent += purchaseFailedEvent;
		AmazonIAPManager.purchaseSuccessfulEvent += purchaseSuccessfulEvent;
		AmazonIAPManager.purchaseUpdatesRequestFailedEvent += purchaseUpdatesRequestFailedEvent;
		AmazonIAPManager.purchaseUpdatesRequestSuccessfulEvent += purchaseUpdatesRequestSuccessfulEvent;
		AmazonIAPManager.onSdkAvailableEvent += onSdkAvailableEvent;
		AmazonIAPManager.onGetUserIdResponseEvent += onGetUserIdResponseEvent;
	}

	private void OnDisable()
	{
		AmazonIAPManager.itemDataRequestFailedEvent -= itemDataRequestFailedEvent;
		AmazonIAPManager.itemDataRequestFinishedEvent -= itemDataRequestFinishedEvent;
		AmazonIAPManager.purchaseFailedEvent -= purchaseFailedEvent;
		AmazonIAPManager.purchaseSuccessfulEvent -= purchaseSuccessfulEvent;
		AmazonIAPManager.purchaseUpdatesRequestFailedEvent -= purchaseUpdatesRequestFailedEvent;
		AmazonIAPManager.purchaseUpdatesRequestSuccessfulEvent -= purchaseUpdatesRequestSuccessfulEvent;
		AmazonIAPManager.onSdkAvailableEvent -= onSdkAvailableEvent;
		AmazonIAPManager.onGetUserIdResponseEvent -= onGetUserIdResponseEvent;
	}

	private void itemDataRequestFailedEvent()
	{
		Debug.Log("itemDataRequestFailedEvent");
	}

	private void itemDataRequestFinishedEvent(List<string> unavailableSkus, List<AmazonItem> availableItems)
	{
		Debug.Log("itemDataRequestFinishedEvent. unavailable skus: " + unavailableSkus.Count + ", avaiable items: " + availableItems.Count);
	}

	private void purchaseFailedEvent(string reason)
	{
		Debug.Log("purchaseFailedEvent: " + reason);
		if (iap_panel != null)
		{
			iap_panel.PurchaseCanceled(reason);
		}
	}

	private void purchaseSuccessfulEvent(AmazonReceipt receipt)
	{
		Debug.Log("purchaseSuccessfulEvent: " + receipt);
		if (iap_panel != null)
		{
			iap_panel.PurchaseFinished(receipt.type);
		}
	}

	private void purchaseUpdatesRequestFailedEvent()
	{
		Debug.Log("purchaseUpdatesRequestFailedEvent");
		if (iap_panel != null)
		{
			iap_panel.PurchaseCanceled(string.Empty);
		}
	}

	private void purchaseUpdatesRequestSuccessfulEvent(List<string> revokedSkus, List<AmazonReceipt> receipts)
	{
		Debug.Log("purchaseUpdatesRequestSuccessfulEvent. revoked skus: " + revokedSkus.Count);
		foreach (AmazonReceipt receipt in receipts)
		{
			Debug.Log(receipt);
		}
	}

	private void onSdkAvailableEvent(bool isTestMode)
	{
		Debug.Log("onSdkAvailableEvent. isTestMode: " + isTestMode);
	}

	private void onGetUserIdResponseEvent(string userId)
	{
		Debug.Log("onGetUserIdResponseEvent: " + userId);
	}
}
