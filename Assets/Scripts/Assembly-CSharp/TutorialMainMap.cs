using UnityEngine;
using Zombie3D;

public class TutorialMainMap : MonoBehaviour
{
	private int currentLevel;

	public GameObject[] buttons;

	public TUIMeshSprite[] buttons_background;

	public GameObject[] specialMode_buttons;

	public TUIMeshSprite[] specialMode_buttons_background;

	private int coopMode;

	private int vsMode = 1;

	private int scoreMode = 2;

	public static string tip_normal = "normal";

	public static string tip_boss = "boss";

	public static string tip_hunting = "hunting";

	public static string tip_dinoHunting = "dino";

	private void Start()
	{
		currentLevel = GameApp.GetInstance().GetGameState().LevelNum;
		CheckShopButton();
		CheckButtonShine();
		CheckMissionIcon();
	}

	private void CheckMissionIcon()
	{
		if (currentLevel == 1)
		{
			int num = SceneName.GetNetMapIndex("Zombie3D_Arena") - 1;
			buttons[num].SetActive(true);
			buttons[num].name = tip_normal + buttons[num].name;
			buttons_background[num].gameObject.SetActive(true);
			return;
		}
		if (currentLevel % 5 == 0)
		{
			int num2 = SceneName.GetNetMapIndex("Zombie3D_Arena") - 1;
			if (GameApp.GetInstance().GetGameState().tutorialTriggers[0])
			{
				buttons[num2].SetActive(true);
			}
			else
			{
				num2 = Random.Range(0, 9);
				buttons[num2].SetActive(true);
				if (num2 == SceneName.GetNetMapIndex("Zombie3D_Arena") - 1)
				{
					buttons[num2].transform.Find("background").gameObject.SetActive(false);
				}
			}
			buttons[num2].transform.Find("button").GetComponent<TUIMeshSprite>().frameName_Accessor += "_boss";
			buttons_background[num2].gameObject.SetActive(true);
			buttons[num2].name = tip_boss + buttons[num2].name;
			return;
		}
		int num3 = -1;
		int num4 = -1;
		num3 = Random.Range(0, 9);
		buttons[num3].SetActive(true);
		buttons[num3].name = tip_normal + buttons[num3].name;
		buttons_background[num3].gameObject.SetActive(true);
		if (currentLevel < 21 || Random.Range(0, 100) >= 33)
		{
			return;
		}
		if (Random.Range(0, 100) < 10)
		{
			num4 = Random.Range(0, 9);
			while (num4 == num3 || !GameScript.CheckDinosaurEnableScene("Zombie3D" + buttons[num4].name.Substring("Button".Length)))
			{
				num4 = Random.Range(0, 9);
			}
			buttons_background[num4].gameObject.SetActive(true);
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/DinoHunting_Tip"), Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.name = tip_dinoHunting + buttons[num4].name;
			gameObject.transform.parent = buttons[num4].transform.parent;
			gameObject.transform.localPosition = buttons[num4].transform.localPosition;
			if (!GameApp.GetInstance().GetGameState().tutorialTriggers[10])
			{
				gameObject.transform.Find("background").gameObject.SetActive(false);
			}
		}
		else
		{
			for (num4 = Random.Range(0, 9); num4 == num3; num4 = Random.Range(0, 9))
			{
			}
			buttons_background[num4].gameObject.SetActive(true);
			GameObject gameObject2 = Object.Instantiate(Resources.Load("Prefabs/TUI/Prey_Tip"), Vector3.zero, Quaternion.identity) as GameObject;
			gameObject2.name = tip_hunting + buttons[num4].name;
			gameObject2.transform.parent = buttons[num4].transform.parent;
			gameObject2.transform.localPosition = buttons[num4].transform.localPosition;
			if (!GameApp.GetInstance().GetGameState().tutorialTriggers[1])
			{
				gameObject2.transform.Find("background").gameObject.SetActive(false);
			}
		}
	}

	public static void CheckShopButton()
	{
		GameObject gameObject = GameObject.Find("Shop_Button");
		if (GameApp.GetInstance().GetGameState().show_avatar_update || GameApp.GetInstance().GetGameState().show_weapon_update || GameApp.GetInstance().GetGameState().unlockNewAvatar.Count > 0 || GameApp.GetInstance().GetGameState().unlockNewWeapon.Count > 0)
		{
			GameObject gameObject2 = Object.Instantiate(Resources.Load("Prefabs/TUI/New_Label"), Vector3.zero, Quaternion.identity) as GameObject;
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = new Vector3(-52f, 10f, -2f);
		}
	}

	private void CheckButtonShine()
	{
		if (GameApp.GetInstance().GetGameState().tutorialTriggers[2])
		{
			specialMode_buttons[coopMode].transform.Find("background").gameObject.SetActive(true);
		}
		specialMode_buttons_background[coopMode].gameObject.SetActive(true);
		if (GameApp.GetInstance().GetGameState().tutorialTriggers[3])
		{
			specialMode_buttons[vsMode].transform.Find("background").gameObject.SetActive(true);
		}
		specialMode_buttons_background[vsMode].gameObject.SetActive(true);
		if (GameApp.GetInstance().GetGameState().tutorialTriggers[9])
		{
			specialMode_buttons[scoreMode].transform.Find("background").gameObject.SetActive(true);
		}
		specialMode_buttons_background[scoreMode].gameObject.SetActive(true);
	}
}
