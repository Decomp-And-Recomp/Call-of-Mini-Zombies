using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AdMobAndroidManager : MonoBehaviour
{
	[method: MethodImpl(32)]
	public static event Action dismissingScreenEvent;

	[method: MethodImpl(32)]
	public static event Action<string> failedToReceiveAdEvent;

	[method: MethodImpl(32)]
	public static event Action leavingApplicationEvent;

	[method: MethodImpl(32)]
	public static event Action presentingScreenEvent;

	[method: MethodImpl(32)]
	public static event Action receivedAdEvent;

	[method: MethodImpl(32)]
	public static event Action interstitialDismissingScreenEvent;

	[method: MethodImpl(32)]
	public static event Action<string> interstitialFailedToReceiveAdEvent;

	[method: MethodImpl(32)]
	public static event Action interstitialLeavingApplicationEvent;

	[method: MethodImpl(32)]
	public static event Action interstitialPresentingScreenEvent;

	[method: MethodImpl(32)]
	public static event Action interstitialReceivedAdEvent;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void dismissingScreen(string empty)
	{
		if (AdMobAndroidManager.dismissingScreenEvent != null)
		{
			AdMobAndroidManager.dismissingScreenEvent();
		}
	}

	public void failedToReceiveAd(string error)
	{
		if (AdMobAndroidManager.failedToReceiveAdEvent != null)
		{
			AdMobAndroidManager.failedToReceiveAdEvent(error);
		}
	}

	public void leavingApplication(string empty)
	{
		if (AdMobAndroidManager.leavingApplicationEvent != null)
		{
			AdMobAndroidManager.leavingApplicationEvent();
		}
	}

	public void presentingScreen(string empty)
	{
		if (AdMobAndroidManager.presentingScreenEvent != null)
		{
			AdMobAndroidManager.presentingScreenEvent();
		}
	}

	public void receivedAd(string empty)
	{
		if (AdMobAndroidManager.receivedAdEvent != null)
		{
			AdMobAndroidManager.receivedAdEvent();
		}
	}

	public void interstitialDismissingScreen(string empty)
	{
		if (AdMobAndroidManager.interstitialDismissingScreenEvent != null)
		{
			AdMobAndroidManager.interstitialDismissingScreenEvent();
		}
	}

	public void interstitialFailedToReceiveAd(string error)
	{
		if (AdMobAndroidManager.interstitialFailedToReceiveAdEvent != null)
		{
			AdMobAndroidManager.interstitialFailedToReceiveAdEvent(error);
		}
	}

	public void interstitialLeavingApplication(string empty)
	{
		if (AdMobAndroidManager.interstitialLeavingApplicationEvent != null)
		{
			AdMobAndroidManager.interstitialLeavingApplicationEvent();
		}
	}

	public void interstitialPresentingScreen(string empty)
	{
		if (AdMobAndroidManager.interstitialPresentingScreenEvent != null)
		{
			AdMobAndroidManager.interstitialPresentingScreenEvent();
		}
	}

	public void interstitialReceivedAd(string empty)
	{
		if (AdMobAndroidManager.interstitialReceivedAdEvent != null)
		{
			AdMobAndroidManager.interstitialReceivedAdEvent();
		}
	}
}
