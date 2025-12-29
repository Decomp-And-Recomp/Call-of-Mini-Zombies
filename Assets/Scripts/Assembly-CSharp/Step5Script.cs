using UnityEngine;
using Zombie3D;

public class Step5Script : MonoBehaviour, ITutorialStep, ITutorialUIEvent
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
		player.InputController.InputInfo.IsMoving = false;
		player.InputController.InputInfo.moveDirection = Vector3.zero;
		GameUIScriptNew.GetGameUIScript().ShowTutorialTipsPanel();
		TutorialTipsTUI component = GameUIScriptNew.GetGameUIScript().tutorialTipsPanel.GetComponent<TutorialTipsTUI>();
		component.tips[0].GetComponent<TUIMeshText>().text_Accessor = "\nNOW YOU'RE READY TO TEST YOUR";
		component.tips[1].GetComponent<TUIMeshText>().text_Accessor = "\nWEAPON. HOLD THE RIGHT JOYSTICK";
		component.tips[2].GetComponent<TUIMeshText>().text_Accessor = "\nTO SHOOT AND AIM AT THE GLOWING";
		component.tips[3].GetComponent<TUIMeshText>().text_Accessor = "\nCRATE.";
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
