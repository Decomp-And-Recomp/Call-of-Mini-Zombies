using UnityEngine;

public class PausePanelManager : MonoBehaviour
{
	public TUIButtonSelectText musicOn;

	public TUIButtonSelectText musicOff;

	public TUIButtonSelectText soundOn;

	public TUIButtonSelectText soundOff;

	public void SwitchMusicOn(bool On)
	{
		if (On)
		{
			musicOn.GetComponent<TUIButtonPush>().pressed = true;
			musicOn.GetComponent<TUIButtonPush>().frameNormal.SetActive(false);
			musicOn.GetComponent<TUIButtonPush>().framePressed.SetActive(true);
			SwitchMusicOff(false);
		}
		else
		{
			musicOn.GetComponent<TUIButtonPush>().pressed = false;
			musicOn.GetComponent<TUIButtonPush>().frameNormal.SetActive(true);
			musicOn.GetComponent<TUIButtonPush>().framePressed.SetActive(false);
			SwitchMusicOff(true);
		}
	}

	public void SwitchMusicOff(bool On)
	{
		if (On)
		{
			musicOff.GetComponent<TUIButtonPush>().pressed = true;
			musicOff.GetComponent<TUIButtonPush>().frameNormal.SetActive(false);
			musicOff.GetComponent<TUIButtonPush>().framePressed.SetActive(true);
			SwitchMusicOn(false);
		}
		else
		{
			musicOff.GetComponent<TUIButtonPush>().pressed = false;
			musicOff.GetComponent<TUIButtonPush>().frameNormal.SetActive(true);
			musicOff.GetComponent<TUIButtonPush>().framePressed.SetActive(false);
			SwitchMusicOn(true);
		}
	}

	public void SwitchSoundOn(bool On)
	{
		if (On)
		{
			soundOn.GetComponent<TUIButtonPush>().pressed = true;
			soundOn.GetComponent<TUIButtonPush>().frameNormal.SetActive(false);
			soundOn.GetComponent<TUIButtonPush>().framePressed.SetActive(true);
			SwitchSoundOff(false);
		}
		else
		{
			soundOn.GetComponent<TUIButtonPush>().pressed = false;
			soundOn.GetComponent<TUIButtonPush>().frameNormal.SetActive(true);
			soundOn.GetComponent<TUIButtonPush>().framePressed.SetActive(false);
			SwitchSoundOff(true);
		}
	}

	public void SwitchSoundOff(bool On)
	{
		if (On)
		{
			soundOff.GetComponent<TUIButtonPush>().pressed = true;
			soundOff.GetComponent<TUIButtonPush>().frameNormal.SetActive(false);
			soundOff.GetComponent<TUIButtonPush>().framePressed.SetActive(true);
			SwitchSoundOn(false);
		}
		else
		{
			soundOff.GetComponent<TUIButtonPush>().pressed = false;
			soundOff.GetComponent<TUIButtonPush>().frameNormal.SetActive(true);
			soundOff.GetComponent<TUIButtonPush>().framePressed.SetActive(false);
			SwitchSoundOn(true);
		}
	}
}
