using System.Collections;
using UnityEngine;

public class OneShotParticleScript : MonoBehaviour
{
	private IEnumerator Start()
	{
		yield return new WaitForSeconds(base.GetComponent<ParticleEmitter>().minEnergy / 2f);
		base.GetComponent<ParticleEmitter>().emit = false;
	}

	private void Update()
	{
	}
}
