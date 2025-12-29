using UnityEngine;

public class AdMobAndroidAdapter : MonoBehaviour
{
	public string adMobPublisherId;

	[SerializeField]
	public AdMobAdPlacement placement = AdMobAdPlacement.BottomCenter;

	private bool _bannerCreated;

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		if (!_bannerCreated)
		{
			createBanner();
		}
		_bannerCreated = true;
	}

	private void OnApplicationPause(bool isPaused)
	{
		if (!isPaused)
		{
			createBanner();
		}
	}

	private void createBanner()
	{
		AdMobAndroid.init(adMobPublisherId);
		AdMobAndroid.createBanner(AdMobAndroidAd.smartBanner, placement);
	}
}
