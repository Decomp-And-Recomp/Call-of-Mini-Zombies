using System.Collections;
using UnityEngine;
using Zombie3D;

public class AudioPlayer
{
	protected Hashtable audioTable = new Hashtable();

	protected float lastPlayingTime;

	public void AddAudio(Transform folderTrans, string name, bool isFX)
	{
		if (folderTrans != null)
		{
			Transform transform = folderTrans.Find(name);
			if (transform != null && !audioTable.Contains(name))
			{
				AudioInfo audioInfo = new AudioInfo();
				audioInfo.audio = transform.GetComponent<AudioSource>();
				audioInfo.isFX = isFX;
				audioInfo.lastPlayingTime = Time.time;
				audioTable.Add(name, audioInfo);
			}
		}
	}

	public void AddAudio(AudioInfo audioInfo, string name)
	{
		if (!audioTable.Contains(name))
		{
			audioTable.Add(name, audioInfo);
		}
	}

	public void PlayAudio(string name)
	{
		AudioInfo audioInfo = audioTable[name] as AudioInfo;
		if (audioInfo == null)
		{
			return;
		}
		AudioSource audio = audioInfo.audio;
		if (audio == null)
		{
			return;
		}
		if (audioInfo.isFX)
		{
			if (!audio.isPlaying)
			{
				audio.mute = !GameApp.GetInstance().GetGameState().SoundOn;
				if (!audio.mute)
				{
					audio.Play();
				}
				audioInfo.lastPlayingTime = Time.time;
			}
		}
		else if (Time.time - audioInfo.lastPlayingTime > audio.clip.length)
		{
			audio.mute = !GameApp.GetInstance().GetGameState().MusicOn;
			if (!audio.mute)
			{
				audio.Play();
			}
			audioInfo.lastPlayingTime = Time.time;
		}
	}

	public static void PlayAudio(AudioSource audio, bool isFX)
	{
		if (isFX)
		{
			audio.mute = !GameApp.GetInstance().GetGameState().SoundOn;
		}
		else
		{
			audio.mute = !GameApp.GetInstance().GetGameState().MusicOn;
		}
		if (!audio.mute)
		{
			audio.Play();
		}
	}

	public static void StopAudio(AudioSource audio)
	{
		if (audio != null)
		{
			audio.Stop();
		}
	}
}
