using UnityEngine;
using Zombie3D;

public class Step1Script : MonoBehaviour, ITutorialStep, ITutorialUIEvent
{
	protected ITutorialGameUI guis;

	protected TutorialScript ts;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void StartStep(TutorialScript ts, Player player)
	{
		this.ts = ts;
		player.InputController.EnableMoveInput = false;
		player.InputController.EnableTurningAround = false;
		player.InputController.EnableShootingInput = false;
		player.InputController.CameraRotation = Vector2.zero;
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/Halo"), player.GetTransform().TransformPoint(Vector3.forward * 15f), Quaternion.Euler(90f, 0f, 0f)) as GameObject;
		gameObject.name = "Halo";
		GameUIScriptNew.GetGameUIScript().ShowTutorialTipsPanel();
		TutorialTipsTUI component = GameUIScriptNew.GetGameUIScript().tutorialTipsPanel.GetComponent<TutorialTipsTUI>();
		component.tips[0].GetComponent<TUIMeshText>().text_Accessor = "\nUSE THE LEFT JOYSTICK TO WALK";
		component.tips[1].GetComponent<TUIMeshText>().text_Accessor = "\nAROUND. GO AHEAD AND WALK";
		component.tips[2].GetComponent<TUIMeshText>().text_Accessor = "\nTOWARDS THE CIRCLE OF PULSATING";
		component.tips[3].GetComponent<TUIMeshText>().text_Accessor = "\nLIGHT.";
		GameObject.Find("GameGUI/TUI/TUIControl/PauseButton").SetActive(false);
	}

	public void UpdateTutorialStep(float deltaTime, Player player)
	{
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
		ts.GoToNextStep();
	}
}
