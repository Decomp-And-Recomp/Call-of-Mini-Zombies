using System;
using System.Runtime.CompilerServices;
using Prime31;

public class ChartBoostAndroidManager : AbstractManager
{
	[method: MethodImpl(32)]
	public static event Action didFailToLoadMoreAppsEvent;

	[method: MethodImpl(32)]
	public static event Action<string> didCacheInterstitialEvent;

	[method: MethodImpl(32)]
	public static event Action didCacheMoreAppsEvent;

	[method: MethodImpl(32)]
	public static event Action<string> didFinishInterstitialEvent;

	[method: MethodImpl(32)]
	public static event Action<string> didFinishMoreAppsEvent;

	[method: MethodImpl(32)]
	public static event Action didCloseMoreAppsEvent;

	[method: MethodImpl(32)]
	public static event Action<string> didFailToLoadInterstitialEvent;

	[method: MethodImpl(32)]
	public static event Action<string> didShowInterstitialEvent;

	[method: MethodImpl(32)]
	public static event Action didShowMoreAppsEvent;

	static ChartBoostAndroidManager()
	{
		AbstractManager.initialize(typeof(ChartBoostAndroidManager));
	}

	public void didFailToLoadMoreApps(string empty)
	{
		if (ChartBoostAndroidManager.didFailToLoadMoreAppsEvent != null)
		{
			ChartBoostAndroidManager.didFailToLoadMoreAppsEvent();
		}
	}

	public void didCacheInterstitial(string location)
	{
		if (ChartBoostAndroidManager.didCacheInterstitialEvent != null)
		{
			ChartBoostAndroidManager.didCacheInterstitialEvent(location);
		}
	}

	public void didCacheMoreApps(string empty)
	{
		if (ChartBoostAndroidManager.didCacheMoreAppsEvent != null)
		{
			ChartBoostAndroidManager.didCacheMoreAppsEvent();
		}
	}

	public void didFinishInterstitial(string param)
	{
		if (ChartBoostAndroidManager.didFinishInterstitialEvent != null)
		{
			ChartBoostAndroidManager.didFinishInterstitialEvent(param);
		}
	}

	public void didFinishMoreApps(string param)
	{
		if (ChartBoostAndroidManager.didFinishMoreAppsEvent != null)
		{
			ChartBoostAndroidManager.didFinishMoreAppsEvent(param);
		}
	}

	public void didCloseMoreApps(string empty)
	{
		if (ChartBoostAndroidManager.didCloseMoreAppsEvent != null)
		{
			ChartBoostAndroidManager.didCloseMoreAppsEvent();
		}
	}

	public void didFailToLoadInterstitial(string location)
	{
		if (ChartBoostAndroidManager.didFailToLoadInterstitialEvent != null)
		{
			ChartBoostAndroidManager.didFailToLoadInterstitialEvent(location);
		}
	}

	public void didShowInterstitial(string location)
	{
		if (ChartBoostAndroidManager.didShowInterstitialEvent != null)
		{
			ChartBoostAndroidManager.didShowInterstitialEvent(location);
		}
	}

	public void didShowMoreApps(string empty)
	{
		if (ChartBoostAndroidManager.didShowMoreAppsEvent != null)
		{
			ChartBoostAndroidManager.didShowMoreAppsEvent();
		}
	}
}
