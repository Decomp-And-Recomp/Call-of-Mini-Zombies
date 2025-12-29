using UnityEngine;
using Zombie3D;

public class DelayMusicPlay : MonoBehaviour
{
	private bool isPlayed;

	private float timer = 1f;

	private void Start()
	{
		isPlayed = false;
	}

	private void Update()
	{
		if (timer <= 0f)
		{
			if (!isPlayed)
			{
				base.gameObject.GetComponent<AudioSource>().Play();
				base.gameObject.GetComponent<AudioSource>().mute = !GameApp.GetInstance().GetGameState().SoundOn;
				isPlayed = true;
				base.gameObject.AddComponent<RemoveTimerScript>().life = 1.5f;
			}
		}
		else
		{
			timer -= Time.deltaTime;
		}
	}
}
