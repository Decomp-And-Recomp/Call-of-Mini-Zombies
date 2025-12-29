using UnityEngine;
using Zombie3D;

public class Step8Script : MonoBehaviour, ITutorialStep, ITutorialUIEvent
{
	protected ITutorialGameUI guis;

	protected TutorialScript ts;

	protected Enemy enemy;

	public void StartStep(TutorialScript ts, Player player)
	{
		this.ts = ts;
		player.InputController.EnableMoveInput = true;
		player.InputController.EnableTurningAround = true;
		player.InputController.EnableShootingInput = true;
		GameObject gameObject = (GameObject)Object.Instantiate(GameApp.GetInstance().GetEnemyResourceConfig().enemy[0], new Vector3(26.7f, 10000f, 0.17f), Quaternion.Euler(0f, 0f, 0f));
		gameObject.name = "E_" + GameApp.GetInstance().GetGameScene().GetNextEnemyID();
		Enemy enemy = new Zombie();
		enemy.Init(gameObject);
		enemy.Attributes.Hp = 18.0;
		enemy.EnemyType = EnemyType.E_ZOMBIE;
		enemy.Name = gameObject.name;
		GameApp.GetInstance().GetGameScene().GetEnemies()
			.Add(gameObject.name, enemy);
	}

	public void UpdateTutorialStep(float deltaTime, Player player)
	{
		if (GameApp.GetInstance().GetGameScene().GetEnemies()
			.Count == 0)
		{
			ts.GoToNextStep();
		}
	}

	public void EndStep(Player player)
	{
	}

	public void SetGameGUI(ITutorialGameUI guis)
	{
		this.guis = guis;
	}

	public void OK(Player player)
	{
	}
}
