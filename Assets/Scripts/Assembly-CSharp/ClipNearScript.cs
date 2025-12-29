using System.Collections;
using UnityEngine;
using Zombie3D;

public class ClipNearScript : MonoBehaviour
{
	private Transform selfTrans;

	private Transform cameraTrans;

	private bool init;

	private IEnumerator Start()
	{
		yield return 0;
		selfTrans = base.transform;
		cameraTrans = GameApp.GetInstance().GetGameScene().GetCamera()
			.transform;
		init = true;
	}

	private void Update()
	{
		if (init)
		{
			if ((selfTrans.position - cameraTrans.position).sqrMagnitude < 25f)
			{
				base.GetComponent<Renderer>().enabled = false;
			}
			else
			{
				base.GetComponent<Renderer>().enabled = true;
			}
		}
	}
}
