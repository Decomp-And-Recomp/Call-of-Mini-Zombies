using UnityEngine;
using Zombie3D;

public class Step9Script : MonoBehaviour, ITutorialStep, ITutorialUIEvent
{
	protected ITutorialGameUI guis;

	protected TutorialScript ts;

	protected Enemy enemy;

	private bool showUnlockWeapon;

	private float showUnlockTimer = 3f;

	private TUIMeshText unlockTimerText;

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
		player.InputController.InputInfo.fire = false;
		string text = "\nCongrats, you've completed the call of mini bootcamp! Get ready to fight for survival, one day at a time.";
		text = text.ToUpper();
		GameUIScriptNew.GetGameUIScript().ShowTutorialTipsPanel();
		TutorialTipsTUI component = GameUIScriptNew.GetGameUIScript().tutorialTipsPanel.GetComponent<TutorialTipsTUI>();
		component.tips[0].GetComponent<TUIMeshText>().text_Accessor = "\nCONGRATS, YOU'VE COMPLETED";
		component.tips[1].GetComponent<TUIMeshText>().text_Accessor = "\nTHE CALL OF MINI BOOTCAMP!";
		component.tips[2].GetComponent<TUIMeshText>().text_Accessor = "\nGET READY TO FIGHT FOR SURVIVAL,";
		component.tips[3].GetComponent<TUIMeshText>().text_Accessor = "\nONE DAY AT A TIME.";
	}

	public void UpdateTutorialStep(float deltaTime, Player player)
	{
		if (showUnlockWeapon)
		{
			showUnlockTimer -= Time.deltaTime;
			if (showUnlockTimer < 1f)
			{
				GameApp.GetInstance().GetGameState().LevelNum++;
				GameApp.GetInstance().Save();
				GameApp.GetInstance().GetGameState().gameMode = GameMode.Solo;
				SceneName.LoadLevel("Zombie3D_Arena");
			}
			else
			{
				unlockTimerText.text_Accessor = "00:0" + (int)showUnlockTimer;
			}
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
		Weapon weaponByName = GameApp.GetInstance().GetGameState().GetWeaponByName("Chainsaw");
		GameApp.GetInstance().GetGameScene().GameGUI.ShowNewItemPanel(weaponByName);
		unlockTimerText = GameApp.GetInstance().GetGameScene().GameGUI.newItemPanel.transform.Find("Timer").GetComponent<TUIMeshText>();
		GameApp.GetInstance().GetGameState().RectToWeaponMap[1] = weaponByName.weapon_index;
		weaponByName.IsSelectedForBattle = true;
		showUnlockTimer = 4f;
		showUnlockWeapon = true;
	}
}
