using UnityEngine;
using Zombie3D;

public class GameScript : MonoBehaviour
{
	/*private void Awake()
	{
		CheckGameResourceConfig();
		CheckEnemyResourceConfig();
		CheckGlobalResourceConfig();
	}*/

	private void Start()
	{
		GameApp.GetInstance().Init();
		GameApp.GetInstance().CreateScene();
	}

	private void Update()
	{
		GameApp.GetInstance().Loop(Time.deltaTime);
	}

	/*public static void CheckGameResourceConfig()
	{
		if (GameObject.Find("GameResourceConfig") == null)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/GameResourceConfig")) as GameObject;
			gameObject.name = "GameResourceConfig";
		}
	}

	public static void CheckEnemyResourceConfig()
	{
		if (GameObject.Find("EnemyResourceConfig") == null)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/EnemyResourceConfig")) as GameObject;
			gameObject.name = "EnemyResourceConfig";
		}
	}*/

	public static void CheckGlobalResourceConfig()
	{
		/*if (GameObject.Find("GlobalResourceConfig") == null)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/Global")) as GameObject;
			gameObject.name = "GlobalResourceConfig";
			Object.DontDestroyOnLoad(gameObject);
		}*/
	}

	public static void CheckMenuResourceConfig()
	{
		/*if (GameObject.Find("MenuResourceConfig") == null)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/MenuResourceConfig")) as GameObject;
			gameObject.name = "MenuResourceConfig";
		}*/
	}

	public static void CheckFillBlack()
	{
		if (AutoRect.GetPlatform() == Platform.iPhone5)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/Fill_Black_1136"), Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.transform.localPosition = new Vector3(0f, 0f, -30f);
		}
		else if (AutoRect.GetPlatform() == Platform.IPad)
		{
			GameObject gameObject2 = Object.Instantiate(Resources.Load("Prefabs/TUI/Fill_Black_1024"), Vector3.zero, Quaternion.identity) as GameObject;
			gameObject2.transform.localPosition = new Vector3(0f, 0f, -30f);
		}
		else if (AutoRect.GetPlatform() == Platform.Android)
		{
			GameObject gameObject3 = Object.Instantiate(Resources.Load("Prefabs/TUI/Fill_Black_Android_left")) as GameObject;
			gameObject3.transform.position = new Vector3(-360f, 0f, -30f);
			GameObject gameObject4 = Object.Instantiate(Resources.Load("Prefabs/TUI/Fill_Black_Android_right")) as GameObject;
			gameObject4.transform.position = new Vector3(360f, 0f, -30f);
		}
	}

	public static bool CheckDinosaurEnableScene(string sceneName)
	{
		switch (sceneName)
		{
		case "Zombie3D_ParkingLot":
		case "Zombie3D_Hospital":
		case "Zombie3D_Recycle":
			return false;
		default:
			return true;
		}
	}
}
