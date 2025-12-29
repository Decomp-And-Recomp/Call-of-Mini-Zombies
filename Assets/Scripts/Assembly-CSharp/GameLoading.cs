using System.Collections;
using UnityEngine;

public class GameLoading : MonoBehaviour
{
	protected AsyncOperation async;

	private IEnumerator Start()
	{
		yield return 1;
		async = Application.LoadLevelAdditiveAsync("Zombie3D_Village2");
		yield return 1;
	}

	private void Update()
	{
		if (async != null && !async.isDone)
		{
			Debug.Log("porsess async: " + async.progress + Time.time);
		}
	}
}
