using UnityEngine;
using Zombie3D;

public class TapJoyScript : MonoBehaviour
{
	private void Start()
	{
		Object.Destroy(base.gameObject);
	}

	public void TapjoyConnectSuccess(string message)
	{
		Debug.Log(message);
	}

	public void TapjoyConnectFail(string message)
	{
		Debug.Log(message);
	}

	public void TapPointsLoaded(string message)
	{
		Debug.Log("TapPointsLoaded: " + message);
		int num = TapjoyPlugin.QueryTapPoints() - GameApp.GetInstance().GetGameState().tapjoyPoints;
		if (num != 0)
		{
			GameApp.GetInstance().GetGameState().AddCrystal(num);
			GameApp.GetInstance().GetGameState().tapjoyPoints = TapjoyPlugin.QueryTapPoints();
			if (Application.loadedLevelName == "MainMapTUI")
			{
				GameObject.Find("Main Camera").GetComponent<MainMapTUI>().label_crystal.text_Accessor = GameApp.GetInstance().GetGameState().GetCrystal()
					.ToString("N0");
			}
			else if (Application.loadedLevelName == "IapShopTUI")
			{
				GameObject.Find("Main Camera").GetComponent<IapShopTUI>().label_crystal.text_Accessor = GameApp.GetInstance().GetGameState().GetCrystal()
					.ToString("N0");
			}
			GameApp.GetInstance().Save();
		}
	}

	public void TapPointsLoadedError(string message)
	{
		Debug.Log("TapPointsLoadedError: " + message);
	}

	public void TapPointsSpent(string message)
	{
		Debug.Log("TapPointsSpent: " + message);
	}

	public void TapPointsSpendError(string message)
	{
		Debug.Log("TapPointsSpendError: " + message);
	}

	public void TapPointsAwarded(string message)
	{
		Debug.Log("TapPointsAwarded: " + message);
	}

	public void TapPointsAwardError(string message)
	{
		Debug.Log("TapPointsAwardError: " + message);
	}

	public void CurrencyEarned(string message)
	{
		Debug.Log("CurrencyEarned: " + message);
		TapjoyPlugin.ShowDefaultEarnedCurrencyAlert();
	}

	public void FullScreenAdLoaded(string message)
	{
		Debug.Log("FullScreenAdLoaded: " + message);
		TapjoyPlugin.ShowFullScreenAd();
	}

	public void FullScreenAdError(string message)
	{
		Debug.Log("FullScreenAdError: " + message);
	}

	public void DailyRewardAdLoaded(string message)
	{
		Debug.Log("DailyRewardAd: " + message);
		TapjoyPlugin.ShowDailyRewardAd();
	}

	public void DailyRewardAdError(string message)
	{
		Debug.Log("DailyRewardAd: " + message);
	}

	public void DisplayAdLoaded(string message)
	{
		Debug.Log("DisplayAdLoaded: " + message);
		TapjoyPlugin.ShowDisplayAd();
	}

	public void DisplayAdError(string message)
	{
		Debug.Log("DisplayAdError: " + message);
	}

	public void VideoAdStart(string message)
	{
		Debug.Log("VideoAdStart: " + message);
	}

	public void VideoAdError(string message)
	{
		Debug.Log("VideoAdError: " + message);
	}

	public void VideoAdComplete(string message)
	{
		Debug.Log("VideoAdComplete: " + message);
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause && !CheckTapjoyObjectExist())
		{
		}
	}

	public static bool CheckTapjoyObjectExist()
	{
		if (GameObject.Find("TapJoyObj") != null)
		{
			return true;
		}
		return false;
	}
}
