using System.Collections;
using UnityEngine;

public class ChartBoostObj : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private IEnumerator Start()
	{
		ChartBoostAndroid.init("50e696ab17ba47463c000000", "9f4d7fbea56d97f676dc5a478057eced41e327a2");
		yield return 1;
		ChartBoostAndroid.cacheInterstitial(null);
		yield return 1;
	}
}
