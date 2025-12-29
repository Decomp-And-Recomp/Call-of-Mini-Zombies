using System.Collections;
using UnityEngine;
using Zombie3D;

public class DayInfoAniamtion : MonoBehaviour
{
	public GameObject day;

	public GameObject dayCount;

	public TUIMeshSprite[] dayNumbers;

	private bool dayIn;

	private bool dayEnlarge;

	private bool dayShrink;

	private bool countIn;

	private bool countEnlarge;

	private bool countShrink;

	private bool allOut;

	private float timerForOut;

	private float delta_time;

	private IEnumerator Start()
	{
		int currentLevel = GameApp.GetInstance().GetGameState().LevelNum;
		if (currentLevel < 10)
		{
			dayNumbers[0].frameName_Accessor = "day_" + currentLevel;
			dayNumbers[1].gameObject.SetActive(false);
			dayNumbers[2].gameObject.SetActive(false);
		}
		else if (currentLevel < 100)
		{
			dayNumbers[0].frameName_Accessor = "day_" + currentLevel / 10;
			dayNumbers[1].frameName_Accessor = "day_" + currentLevel % 10;
			dayNumbers[2].gameObject.SetActive(false);
		}
		else
		{
			dayNumbers[0].frameName_Accessor = "day_" + currentLevel / 100;
			dayNumbers[1].frameName_Accessor = "day_" + currentLevel % 100 / 10;
			dayNumbers[2].frameName_Accessor = "day_" + currentLevel % 100 % 10;
		}
		delta_time = 0f;
		timerForOut = 0f;
		dayIn = false;
		FadeAnimationScript fade = FadeAnimationScript.GetInstance();
		GameUIScriptNew ui = GameUIScriptNew.GetGameUIScript();
		while (!ui.uiInited || !fade.FadeOutComplete(0.3f))
		{
			yield return 0;
		}
		dayIn = true;
	}

	private void Update()
	{
		delta_time += Time.deltaTime;
		if (!((double)delta_time >= 0.02))
		{
			return;
		}
		if (dayIn)
		{
			day.transform.position += new Vector3(-800f * delta_time, 0f, 0f);
			if (day.transform.position.x <= -38f)
			{
				dayEnlarge = true;
				dayIn = false;
			}
		}
		else if (dayEnlarge)
		{
			day.transform.localScale += new Vector3(2.5f * delta_time, 2.5f * delta_time, 0f);
			if (day.transform.localScale.x >= 1.6f)
			{
				dayShrink = true;
				dayEnlarge = false;
			}
		}
		else if (dayShrink)
		{
			day.transform.localScale -= new Vector3(2.5f * delta_time, 2.5f * delta_time, 0f);
			if (day.transform.localScale.x <= 1.35f)
			{
				countIn = true;
				dayShrink = false;
			}
		}
		else if (countIn)
		{
			dayCount.transform.position += new Vector3(-800f * delta_time, 0f, 0f);
			if (dayCount.transform.position.x <= 38f)
			{
				countEnlarge = true;
				countIn = false;
			}
		}
		else if (countEnlarge)
		{
			dayCount.transform.localScale += new Vector3(2.5f * delta_time, 2.5f * delta_time, 0f);
			if (dayCount.transform.localScale.x >= 1.3f)
			{
				countShrink = true;
				countEnlarge = false;
			}
		}
		else if (countShrink)
		{
			dayCount.transform.localScale -= new Vector3(2.5f * delta_time, 2.5f * delta_time, 0f);
			if (dayCount.transform.localScale.x <= 1.06f)
			{
				allOut = true;
				countShrink = false;
			}
		}
		else if (allOut)
		{
			timerForOut += delta_time;
			if (timerForOut >= 2.5f)
			{
				day.transform.position += new Vector3(-1000f * delta_time, 0f, 0f);
				dayCount.transform.position += new Vector3(-1000f * delta_time, 0f, 0f);
				if (dayCount.transform.position.x < -400f)
				{
					Object.Destroy(base.gameObject);
				}
			}
		}
		delta_time = 0f;
	}
}
