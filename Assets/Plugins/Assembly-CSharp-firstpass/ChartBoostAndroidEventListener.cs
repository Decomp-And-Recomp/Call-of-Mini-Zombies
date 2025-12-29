using UnityEngine;

public class ChartBoostAndroidEventListener : MonoBehaviour
{
	private void OnEnable()
	{
		ChartBoostAndroidManager.didFailToLoadMoreAppsEvent += didFailToLoadMoreAppsEvent;
		ChartBoostAndroidManager.didCacheInterstitialEvent += didCacheInterstitialEvent;
		ChartBoostAndroidManager.didCacheMoreAppsEvent += didCacheMoreAppsEvent;
		ChartBoostAndroidManager.didFinishInterstitialEvent += didFinishInterstitialEvent;
		ChartBoostAndroidManager.didFinishMoreAppsEvent += didFinishMoreAppsEvent;
		ChartBoostAndroidManager.didCloseMoreAppsEvent += didCloseMoreAppsEvent;
		ChartBoostAndroidManager.didFailToLoadInterstitialEvent += didFailToLoadInterstitialEvent;
		ChartBoostAndroidManager.didShowInterstitialEvent += didShowInterstitialEvent;
		ChartBoostAndroidManager.didShowMoreAppsEvent += didShowMoreAppsEvent;
	}

	private void OnDisable()
	{
		ChartBoostAndroidManager.didFailToLoadMoreAppsEvent -= didFailToLoadMoreAppsEvent;
		ChartBoostAndroidManager.didCacheInterstitialEvent -= didCacheInterstitialEvent;
		ChartBoostAndroidManager.didCacheMoreAppsEvent -= didCacheMoreAppsEvent;
		ChartBoostAndroidManager.didFinishInterstitialEvent -= didFinishInterstitialEvent;
		ChartBoostAndroidManager.didFinishMoreAppsEvent -= didFinishMoreAppsEvent;
		ChartBoostAndroidManager.didCloseMoreAppsEvent -= didCloseMoreAppsEvent;
		ChartBoostAndroidManager.didFailToLoadInterstitialEvent -= didFailToLoadInterstitialEvent;
		ChartBoostAndroidManager.didShowInterstitialEvent -= didShowInterstitialEvent;
		ChartBoostAndroidManager.didShowMoreAppsEvent -= didShowMoreAppsEvent;
	}

	private void didFailToLoadMoreAppsEvent()
	{
		Debug.Log("didFailToLoadMoreAppsEvent");
	}

	private void didCacheInterstitialEvent(string location)
	{
		Debug.Log("didCacheInterstitialEvent: " + location);
	}

	private void didCacheMoreAppsEvent()
	{
		Debug.Log("didCacheMoreAppsEvent");
	}

	private void didFinishInterstitialEvent(string param)
	{
		Debug.Log("didFinishInterstitialEvent: " + param);
	}

	private void didFinishMoreAppsEvent(string param)
	{
		Debug.Log("didFinishMoreAppsEvent: " + param);
	}

	private void didCloseMoreAppsEvent()
	{
		Debug.Log("didCloseMoreAppsEvent");
	}

	private void didFailToLoadInterstitialEvent(string location)
	{
		Debug.Log("didFailToLoadInterstitialEvent: " + location);
	}

	private void didShowInterstitialEvent(string location)
	{
		Debug.Log("didShowInterstitialEvent: " + location);
	}

	private void didShowMoreAppsEvent()
	{
		Debug.Log("didShowMoreAppsEvent");
	}
}
