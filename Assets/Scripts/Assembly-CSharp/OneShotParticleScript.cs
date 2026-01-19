using System.Collections;
using UnityEngine;

public class OneShotParticleScript : MonoBehaviour
{
	private IEnumerator Start()
	{
		var main = base.GetComponent<ParticleSystem>().main;
		yield return new WaitForSeconds(main.startLifetime.constant / 2f);
		base.GetComponent<ParticleSystem>().Stop(true);
	}

	private void Update()
	{
	}
}
