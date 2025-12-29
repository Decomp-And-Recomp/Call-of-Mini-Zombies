using UnityEngine;

public class AdMobUIManager : MonoBehaviour
{
	private void OnGUI()
	{
		float num = 5f;
		float left = 5f;
		float num2 = ((Screen.width < 800 && Screen.height < 800) ? 160 : 320);
		float num3 = ((Screen.width < 800 && Screen.height < 800) ? 35 : 70);
		float num4 = num3 + 10f;
		if (GUI.Button(new Rect(left, num, num2, num3), "Init"))
		{
			AdMobAndroid.init("YOUR_APP_ID_HERE");
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Set Test Devices"))
		{
			AdMobAndroid.setTestDevices(new string[3] { "6D13FA054BC989C5AC41900EE14B0C1B", "123456678", "3BAB93112BBB08713B6D6D0A09EDABA1" });
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Create Smart Banner"))
		{
			AdMobAndroid.createBanner(AdMobAndroidAd.smartBanner, AdMobAdPlacement.BottomCenter);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Create 320x50 banner"))
		{
			AdMobAndroid.createBanner(AdMobAndroidAd.phone320x50, AdMobAdPlacement.TopCenter);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Create 300x250 banner"))
		{
			AdMobAndroid.createBanner(AdMobAndroidAd.tablet300x250, AdMobAdPlacement.BottomCenter);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Destroy Banner"))
		{
			AdMobAndroid.destroyBanner();
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Hide Banner"))
		{
			AdMobAndroid.hideBanner(true);
		}
		left = (float)Screen.width - num2 - 5f;
		num = 5f;
		if (GUI.Button(new Rect(left, num, num2, num3), "Refresh Ad"))
		{
			AdMobAndroid.refreshAd();
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Request Interstitial"))
		{
			AdMobAndroid.requestInterstital("a14de56b4e8babd");
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Is Interstitial Ready?"))
		{
			bool flag = AdMobAndroid.isInterstitalReady();
			Debug.Log("is interstitial ready? " + flag);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Display Interstitial"))
		{
			AdMobAndroid.displayInterstital();
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Show Banner"))
		{
			AdMobAndroid.hideBanner(false);
		}
	}
}
