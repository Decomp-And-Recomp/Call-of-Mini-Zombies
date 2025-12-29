using UnityEngine;

public class ApplicationActive : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnApplicationFocus(bool focus)
	{
		Debug.Log("OnApplicationFocus:" + focus);
	}

	private void OnApplicationPause(bool pause)
	{
		Debug.Log("OnApplicationPause:" + pause);
	}
}
