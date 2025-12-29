using System;
using System.Collections;
using UnityEngine;
using Zombie3D;

public class VSReportUITemp : MonoBehaviour
{
	public static string nextScene = string.Empty;

	private IEnumerator Start()
	{
		yield return 1;
		GameApp.GetInstance().ClearScene();
		Resources.UnloadUnusedAssets();
		yield return 1;
		GC.Collect();
		yield return 3;
		SceneName.LoadLevel(nextScene);
	}
}
