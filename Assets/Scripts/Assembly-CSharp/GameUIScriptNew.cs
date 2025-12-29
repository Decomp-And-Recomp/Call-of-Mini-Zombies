using System.Collections;
using TNetSdk;
using UnityEngine;
using Zombie3D;

public class GameUIScriptNew : MonoBehaviour, TUIHandler, ITutorialGameUI
{
	private TUI m_tui;

	private Transform m_tuiControl;

	private TUIInput[] input;

	private AudioPlayer audioPlayer;

	public GameUIPlayerInfo playerInfo;

	public GameUIWeaponInfo weaponInfo;

	public GameUIItemInfo itemInfo;

	public GameObject dayClear;

	public GameObject dayInfo;

	public GameObject huntingTimer;

	public GameObject instanceStatistics;

	public GameObject pausePanel;

	public GameObject gameOverPanel;

	public GameObject newItemPanel;

	public GameObject dialogPanel;

	public GameObject tutorialTipsPanel;

	public GameObject msgBox;

	public GameObject msgBoxYesOrNo;

	public GameObject waitingPanel;

	public GameObject MaskBlock;

	public GameObject fireStick;

	public GameObject rotateStick;

	public GameObject mapShow;

	public GameObject[] playerMarks;

	public TUIMeshText vsLabelMissionTime;

	public TUIMeshText vsLabelKillCount;

	public TUIMeshText vsLabelDeathCount;

	public GameMessagePanel vsMessagePanel;

	public VsSeatState vsSeatState;

	private GameObject vsLabelCombo;

	protected GameObject Items_Report_Panel;

	private GameScene gameScene;

	private GameState gameState;

	private Player player;

	public bool uiInited;

	private float lastUpdateTime;

	private int multiPlayerIndex;

	protected ITutorialUIEvent tutorialUIEvent;

	public void SetTutorialUIEvent(ITutorialUIEvent tEvent)
	{
		tutorialUIEvent = tEvent;
	}

	public void SetTutorialText(string text)
	{
	}

	public static GameUIScriptNew GetGameUIScript()
	{
		GameObject gameObject = GameObject.Find("GameGUI/SceneTUI");
		if ((bool)gameObject)
		{
			return gameObject.GetComponent<GameUIScriptNew>();
		}
		return null;
	}

	private IEnumerator Start()
	{
#if UNITY_STANDALONE
		Transform p = transform.parent;

		p.Find("TUI/TUIControl/PauseButton").gameObject.SetActive(false);
		p.Find("TUI/TUIControl/MoveJoystick").gameObject.SetActive(false);
		p.Find("TUI/TUIControl/FireJoystick").gameObject.SetActive(false);
		p.Find("TUI/TUIControl/WeaponInfo/weaponScan").gameObject.SetActive(false);
		mapShow.transform.parent.position = new Vector3(-185, -111, -10);
		var c = mapShow.transform.parent.GetComponent<GameUIAutoLayout>();
		if (c) c.left = true;
#endif

		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tuiControl = m_tui.transform.Find("TUIControl");
		audioPlayer = new AudioPlayer();
		audioPlayer.AddAudio(m_tui.transform.Find("TUICamera"), "Button", true);
		gameState = GameApp.GetInstance().GetGameState();
		switch (gameState.gameMode)
		{
		case GameMode.Coop:
			FlurryStatistics.CoopStartEvent();
			break;
		case GameMode.Vs:
			FlurryStatistics.VsStartEvent();
			break;
		case GameMode.Solo:
		case GameMode.SoloBoss:
		case GameMode.Hunting:
		case GameMode.DinoHunting:
			FlurryStatistics.SoloStartEvent();
			break;
		case GameMode.Instance:
			FlurryStatistics.EndlessStartEvent();
			break;
		case GameMode.Tutorial:
		{
			GameObject tutorial = Object.Instantiate(Resources.Load("Prefabs/TUI/TipsDialogPanel"), new Vector3(0f, 0f, -20f), Quaternion.identity) as GameObject;
			tutorial.transform.parent = m_tuiControl;
			tutorialTipsPanel = tutorial;
			break;
		}
		}
		for (gameScene = GameApp.GetInstance().GetGameScene(); gameScene == null; gameScene = GameApp.GetInstance().GetGameScene())
		{
			yield return 1;
		}
		gameScene.GameGUI = this;
		for (player = gameScene.GetPlayer(); player == null; player = gameScene.GetPlayer())
		{
			yield return 1;
		}
		playerInfo.Init();
		weaponInfo.Init();
		itemInfo.Init();
		if (gameState.gameMode != GameMode.Coop)
		{
			Object.Destroy(mapShow);
			for (int i = 0; i < playerMarks.Length; i++)
			{
				Object.Destroy(playerMarks[i]);
			}
			playerMarks = null;
		}
		if (gameState.gameMode != GameMode.Vs)
		{
			Object.Destroy(vsLabelDeathCount.gameObject);
			Object.Destroy(vsLabelKillCount.gameObject);
			Object.Destroy(vsLabelMissionTime.gameObject);
			Object.Destroy(vsMessagePanel.gameObject);
			Object.Destroy(vsSeatState.gameObject);
		}
		if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Solo && GameApp.GetInstance().GetGameState().gameMode != GameMode.SoloBoss && GameApp.GetInstance().GetGameState().gameMode != GameMode.Hunting && GameApp.GetInstance().GetGameState().gameMode != GameMode.DinoHunting)
		{
			Object.Destroy(dayInfo);
			Object.Destroy(dayClear);
		}
		else
		{
			dayClear.SetActive(false);
		}
		if (gameState.gameMode == GameMode.Coop)
		{
			while (gameState.net_com == null)
			{
				yield return 1;
			}
			if ((bool)gameState.net_com)
			{
				gameState.net_com.reverse_heart_timeout_delegate = OnReverseHearTimeout;
				gameState.net_com.reverse_heart_renew_delegate = OnReverseHearRenew;
				gameState.net_com.reverse_heart_waiting_delegate = OnReverseHearWaiting;
				gameState.net_com.close_delegate = OnClosed;
				gameState.net_com.contecting_lost = OnConnectingLost;
			}
		}
		if (gameState.gameMode == GameMode.Vs)
		{
			while (!TNetConnection.IsInitialized)
			{
				yield return 1;
			}
			TNetConnection.Connection.AddEventListener(TNetEventSystem.REVERSE_HEART_TIMEOUT, OnReverseHearTimeout);
			TNetConnection.Connection.AddEventListener(TNetEventSystem.REVERSE_HEART_RENEW, OnReverseHearRenew);
			TNetConnection.Connection.AddEventListener(TNetEventSystem.REVERSE_HEART_WAITING, OnReverseHearWaiting);
			vsLabelMissionTime.GetComponent<VSGameMissionTimer>().Init();
			vsSeatState.RefrashSeatList((gameScene as GameVSScene).SFS_Player_Arr.Count);
		}
		uiInited = true;
		playerInfo.SetInited();
		weaponInfo.SetInited();
		itemInfo.SetInited();
		lastUpdateTime = Time.time;
		OpenClikPlugin.Hide();
		Resources.UnloadUnusedAssets();
	}

	public void AddMultiplayerHpBar(Multiplayer multi_player)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/multiplayerLogoInfo"), Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.transform.parent = GameObject.Find("MultiplayerInfo").transform;
		gameObject.transform.localPosition = new Vector3(0f, (float)multiPlayerIndex * -26f, 0f);
		gameObject.name = "multiplayer" + multi_player.birth_point_index;
		gameObject.GetComponent<GameUIMultiplayerInfo>().player = multi_player;
		gameObject.GetComponent<GameUIMultiplayerInfo>().Init();
		multiPlayerIndex++;
	}

	public void ShowPickupInGame(ItemType m_type)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/PickupShow"), Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.transform.parent = m_tuiControl;
		gameObject.transform.localPosition = new Vector3(0f, 55f, -20f);
		gameObject.GetComponent<PickupShowInGame>().itemType = m_type;
		gameObject.GetComponent<PickupShowInGame>().targetPosition = playerInfo.logo.transform.parent.localPosition;
	}

	public void AddMultiplayerMiniMapMark(Player m_player, int index)
	{
		for (int i = 0; i < playerMarks.Length; i++)
		{
			if (playerMarks[i].GetComponent<PlayerMark>().m_player == null && index == i)
			{
				playerMarks[i].GetComponent<PlayerMark>().SetPlayer(m_player);
				break;
			}
		}
	}

	public void RemoveMultiplayerMiniMapMark(int id)
	{
		for (int i = 0; i < playerMarks.Length; i++)
		{
			if (playerMarks[i].GetComponent<PlayerMark>().m_player != null && playerMarks[i].GetComponent<PlayerMark>().m_player.m_multi_id == id)
			{
				playerMarks[i].GetComponent<PlayerMark>().RemoveMark();
				break;
			}
		}
	}

	public void SetKillCountLabel(int count)
	{
		if (vsLabelKillCount != null)
		{
			vsLabelKillCount.text_Accessor = "KILLS:" + count;
		}
	}

	public void SetDeathCountLabel(int count)
	{
		if (vsLabelDeathCount != null)
		{
			vsLabelDeathCount.text_Accessor = "DEATHS:" + count;
		}
	}

	public void SetComboCountLabel(string nickname, int count)
	{
		if (!vsLabelCombo)
		{
			vsLabelCombo = Object.Instantiate(Resources.Load("Prefabs/TUI/Combo_Text_Eff")) as GameObject;
			vsLabelCombo.transform.parent = vsLabelKillCount.transform.parent;
			vsLabelCombo.transform.localPosition = new Vector3(240f, 47f, -17f);
		}
		vsLabelCombo.GetComponent<ComboTextScript>().Show(nickname, count);
	}

	public void SetHuntingLevelTimer(Enemy enermy)
	{
		huntingTimer = Object.Instantiate(Resources.Load("Prefabs/TUI/HuntingTimer"), Vector3.zero, Quaternion.identity) as GameObject;
		huntingTimer.transform.parent = m_tuiControl;
		huntingTimer.transform.localPosition = new Vector3(50f, 136f, -20f);
		huntingTimer.GetComponent<HuntingTimerManager>().enermy = enermy;
		huntingTimer.GetComponent<HuntingTimerManager>().enabled = true;
		huntingTimer.GetComponent<HuntingTimerManager>().timer = MonsterParametersConfig.huntingParameters["time"];
	}

	public void SetInstanceStatistics(int wave, int score)
	{
		if (instanceStatistics == null)
		{
			instanceStatistics = Object.Instantiate(Resources.Load("Prefabs/TUI/InstanceModeStatistics"), Vector3.zero, Quaternion.identity) as GameObject;
			instanceStatistics.transform.parent = m_tuiControl;
			instanceStatistics.transform.localPosition = new Vector3(0f, 0f, -17f);
			instanceStatistics.GetComponent<InstanceModeGameStatisticsUI>().UpdateWave(wave);
			instanceStatistics.GetComponent<InstanceModeGameStatisticsUI>().UpdateScore(score);
			instanceStatistics.GetComponent<InstanceModeGameStatisticsUI>().timer = InstanceModeConfig.TimeInitial;
		}
		else
		{
			instanceStatistics.GetComponent<InstanceModeGameStatisticsUI>().UpdateWave(wave);
			instanceStatistics.GetComponent<InstanceModeGameStatisticsUI>().UpdateScore(score);
		}
	}

	public void SetChallengeStatistics(int wave, int progress)
	{
	}

	private void Update()
	{
		if (uiInited)
		{
			input = TUIInputManager.GetInput();
			for (int i = 0; i < input.Length; i++)
			{
				m_tui.HandleInput(input[i]);
			}
		}
		if (!(Time.time - lastUpdateTime < 0.03f) && uiInited)
		{
			lastUpdateTime = Time.time;
		}

#if UNITY_STANDALONE || UNITY_EDITOR
		if (IsUIBlocking)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pausePanel.transform.position = new Vector3(0f, 1000f, -20f);
            return;
        }

        Cursor.lockState = !CanPlayerMove ? CursorLockMode.None : CursorLockMode.Locked;
		Cursor.visible = !CanPlayerMove;
#endif
	}

    public static bool IsPaused
	{
		get
		{
			return GetGameUIScript().pausePanel.transform.position == new Vector3(0f, 0f, -20f);
        }
	}

	public static bool IsUIBlocking
    {
        get
        {
            var ui = GetGameUIScript();
            return
                ui.newItemPanel.transform.position == new Vector3(0f, 0f, -20f) ||
                ui.dialogPanel.transform.position == new Vector3(0f, 0f, -20f) ||
                ui.msgBox.transform.position == new Vector3(0f, 0f, -20f) ||
                ui.msgBoxYesOrNo.transform.position == new Vector3(0f, 0f, -20f) ||
                (ui.tutorialTipsPanel && ui.tutorialTipsPanel.transform.position == new Vector3(0f, 0f, -20f)) ||
                ui.Items_Report_Panel;
        }
    }

    public static bool CanPlayerMove
	{
		get
		{
			return !IsPaused && !IsUIBlocking && GetGameUIScript().gameScene.GamePlayingState == PlayingState.GamePlaying;
		}
	}

    public void Pause()
	{
		audioPlayer.PlayAudio("Button");
		if (gameScene.GamePlayingState == PlayingState.GamePlaying)
		{
			if (gameState.gameMode == GameMode.Coop || gameState.gameMode == GameMode.Vs)
			{
				ShowPausePanel();
			}
			else
			{
				Time.timeScale = 0f;
				ShowPausePanel();
			}
		}
		else if (gameScene.GamePlayingState == PlayingState.GameLose && gameState.gameMode == GameMode.Coop && msgBoxYesOrNo.GetComponent<MsgBoxDelegate>().m_type != MsgBoxType.Rebirth && msgBoxYesOrNo.GetComponent<MsgBoxDelegate>().m_type != MsgBoxType.CrystalRebirth && (gameScene as GameMultiplayerScene).m_multi_player_arr.Count > 0)
		{
			ShowPausePanel();
		}
	}

	public void UnPause()
	{
		audioPlayer.PlayAudio("Button");
		Time.timeScale = 1f;
		HidePausePanel();
	}

    public void PauseToggle()
    {
        if (IsUIBlocking) return;

        if (!IsPaused)
            Pause();
        else
            UnPause();
    }


    public void TryBuyAmmo()
	{
		if (!weaponInfo.buyAmmo.activeSelf) return;

		audioPlayer.PlayAudio("Button");
		if (gameState.BuyBullets(player.GetWeapon(), player.GetWeapon().WConf.bulletEachAdd, player.GetWeapon().WConf.bulletPrice))
		{
			weaponInfo.buyAmmo.SetActive(false);
			weaponInfo.bulletCount.SetActive(true);
			weaponInfo.bulletLogo.SetActive(true);
			FlurryStatistics.BuyAmmoEvent(player.GetWeapon());
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "FireJoystick")
		{
			player.InputController.ProcessFireInput(eventType, wparam, lparam, (TUIInput)data);
		}
		else if (control.name == "MoveJoystick")
		{
			player.InputController.ProcessMoveInput(eventType, wparam, lparam);
		}
		else if (control.name == "RotateJoystick")
		{
			Vector2 vector = new Vector2(((TUIInput)data).position.x, ((TUIInput)data).position.y);
			Vector2 vector2 = new Vector2(fireStick.transform.position.x, fireStick.transform.position.y);
			if ((vector - vector2).magnitude < fireStick.GetComponent<TUIButtonJoystickEx>().m_MaxDistance)
			{
				fireStick.transform.GetComponent<TUIButtonJoystickEx>().HandleInput((TUIInput)data);
			}
			player.InputController.ProcessRotateInput(eventType, (TUIInput)data);
		}
		else if (control.name == "PauseButton" && eventType == 3)
		{
			Pause();
		}
		else if ((control.name == "SwitchButton" || control.name == "weaponScan") && eventType == 3)
		{
			if (player.PlayerBonusState == null || player.PlayerBonusState.StateType != PlayerBonusStateType.Suicidegun)
			{
				player.NextWeapon();
			}
		}
		else if (control.name == "BuyAmmoButton" && eventType == 3)
		{
			TryBuyAmmo();
		}
		else if (control.name == "ResumePause" && eventType == 3)
		{
			UnPause();
		}
		else if (control.name == "QuitPause" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			HidePausePanel();
			ShowDialogPanel();
		}
		else if (control.name == "Music_Off_Button" && eventType == 1)
		{
			audioPlayer.PlayAudio("Button");
			gameState.MusicOn = false;
			GameApp.GetInstance().PlayerPrefsSave();
			AudioSource component = gameScene.GetCamera().GetComponent<AudioSource>();
			if (component != null)
			{
				component.mute = !gameState.MusicOn;
			}
		}
		else if (control.name == "Music_On_Button" && eventType == 1)
		{
			audioPlayer.PlayAudio("Button");
			gameState.MusicOn = true;
			GameApp.GetInstance().PlayerPrefsSave();
			AudioSource component2 = gameScene.GetCamera().GetComponent<AudioSource>();
			if (component2 != null)
			{
				component2.mute = !gameState.MusicOn;
			}
		}
		else if (control.name == "Sound_Off_Button" && eventType == 1)
		{
			audioPlayer.PlayAudio("Button");
			gameState.SoundOn = false;
			GameApp.GetInstance().PlayerPrefsSave();
		}
		else if (control.name == "Sound_On_Button" && eventType == 1)
		{
			audioPlayer.PlayAudio("Button");
			gameState.SoundOn = true;
			GameApp.GetInstance().PlayerPrefsSave();
		}
		else if (control.name == "dialog_yes" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			Time.timeScale = 1f;
			HideDialogPanel();
			gameScene.GamePlayingState = PlayingState.GameQuit;
			if (gameState.gameMode == GameMode.Coop)
			{
				gameScene.QuitGameForDisconnect(5f);
				ShowGameOverPanel(GameOverType.coopQuit);
			}
			else if (gameState.gameMode == GameMode.Vs)
			{
				gameScene.QuitGameForDisconnect(5f);
				ShowGameOverPanel(GameOverType.vsQuit);
			}
			else if (gameState.gameMode == GameMode.Instance)
			{
				gameScene.SaveDataReport();
				ShowGameOverPanel(GameOverType.instanceQuit);
			}
			else
			{
				ShowGameOverPanel(GameOverType.soloQuit);
			}
		}
		else if (control.name == "dialog_no" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			HideDialogPanel();
			ShowPausePanel();
		}
		else if (control.name == "QuitGameOver" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			SceneName.LoadLevel("MainMapTUI");
			OpenClikPlugin.Hide();
		}
		else if (control.name == "RetryGameOver" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			SceneName.LoadLevel(Application.loadedLevelName);
			OpenClikPlugin.Hide();
		}
		else if (control.name == "TapToDismiss" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			HideTutorialTipsPanel();
			tutorialUIEvent.OK(player);
		}
		else if (control.name == "Msg_OK_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			msgBox.GetComponent<MsgBoxDelegate>().Hide();
			Time.timeScale = 1f;
			switch (msgBox.GetComponent<MsgBoxDelegate>().m_type)
			{
			case MsgBoxType.ContectingTimeout:
			case MsgBoxType.ContectingLost:
				if (gameState.gameMode == GameMode.Coop)
				{
					gameScene.QuitGameForDisconnect(5f);
					ShowGameOverPanel(GameOverType.coopLostConnection);
				}
				else if (gameState.gameMode == GameMode.Vs)
				{
					gameScene.QuitGameForDisconnect(5f);
					ShowGameOverPanel(GameOverType.vsLostConnection);
				}
				break;
			case MsgBoxType.OnClosed:
				if (gameState.gameMode == GameMode.Coop)
				{
					gameScene.QuitGameForDisconnect(5f);
					ShowGameOverPanel(GameOverType.coopQuit);
				}
				else if (gameState.gameMode == GameMode.Vs)
				{
					gameScene.QuitGameForDisconnect(5f);
					ShowGameOverPanel(GameOverType.vsQuit);
				}
				break;
			case MsgBoxType.JoinRommFailed:
				break;
			}
		}
		else if (control.name == "Msg_Yes_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			msgBoxYesOrNo.GetComponent<MsgBoxDelegate>().Hide();
			switch (msgBoxYesOrNo.GetComponent<MsgBoxDelegate>().m_type)
			{
			case MsgBoxType.Rebirth:
			{
				player.OnRebirth();
				GameApp.GetInstance().GetGameState().Revivepack--;
				Packet packet = CGUserRebirthPacket.MakePacket((uint)gameScene.GetPlayer().m_multi_id);
				gameState.net_com.Send(packet);
				GameObject gameObject = Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().itemFullReviveEffect, Vector3.zero, Quaternion.identity) as GameObject;
				gameObject.transform.parent = player.GetTransform();
				gameObject.transform.localPosition = Vector3.zero;
				msgBoxYesOrNo.GetComponent<MsgBoxDelegate>().m_type = MsgBoxType.None;
				break;
			}
			case MsgBoxType.CrystalRebirth:
			{
				player.OnRebirth();
				GameApp.GetInstance().GetGameState().LoseCrystal(1);
				Packet packet = CGUserRebirthPacket.MakePacket((uint)gameScene.GetPlayer().m_multi_id);
				gameState.net_com.Send(packet);
				GameObject gameObject = Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().itemFullReviveEffect, Vector3.zero, Quaternion.identity) as GameObject;
				gameObject.transform.parent = player.GetTransform();
				gameObject.transform.localPosition = Vector3.zero;
				msgBoxYesOrNo.GetComponent<MsgBoxDelegate>().m_type = MsgBoxType.None;
				break;
			}
			}
		}
		else if (control.name == "Msg_No_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			player.PlayerRealDead();
			HideRebirthMsgBox();
		}
		else if (control.name.StartsWith("itemPacket_"))
		{
			switch (eventType)
			{
			case 3:
			{
				int num = int.Parse(control.name.Substring("itemPacket_".Length));
				ItemType itemTypeByName = Item.GetItemTypeByName(itemInfo.itemLogo[num].frameName_Accessor.Substring("item_".Length));
				if (itemInfo.isBuyItem[num])
				{
					gameState.BuyItem(gameState.GetItemByType(itemTypeByName));
					player.carryItemsPacket[itemTypeByName] = gameState.GetItemByType(itemTypeByName).OwnedCount;
					itemInfo.UpdateCarryItemPacket(itemTypeByName, player.carryItemsPacket[itemTypeByName]);
					player.OnUseCarryItem(itemTypeByName);
				}
				else
				{
					player.OnUseCarryItem(itemTypeByName);
				}
				break;
			}
			case 4:
				rotateStick.GetComponent<TUIEventRect>().HandleInput((TUIInput)data);
				break;
			}
		}
		else if (control.name.StartsWith("OK_Button_Pickup") && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			HideItemsReportPanel();
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Instance || GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
			{
				Time.timeScale = 1f;
			}
			else if (GameApp.GetInstance().GetGameScene().GamePlayingState == PlayingState.GameWin)
			{
				FadeAnimationScript.GetInstance().enabled = true;
				FadeAnimationScript.GetInstance().StartFade(new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 1f), 1f);
			}
			else if (GameApp.GetInstance().GetGameScene().GamePlayingState == PlayingState.GameQuit)
			{
				SceneName.LoadLevel("MainMapTUI");
			}
			else
			{
				gameOverPanel.GetComponent<GameOverTUI>().retryButton.SetActive(true);
				gameOverPanel.GetComponent<GameOverTUI>().quitButton.SetActive(true);
			}
		}
	}

	private void ShowBlock(Vector3 pos)
	{
		MaskBlock.transform.localPosition = pos + new Vector3(0f, 0f, 1f);
	}

	private void HideBlock()
	{
		MaskBlock.transform.localPosition = new Vector3(0f, 5000f, 0f);
	}

	public void ShowPausePanel()
	{
		pausePanel.transform.position = new Vector3(0f, 0f, -20f);
		if (tutorialTipsPanel != null)
		{
			HideTutorialTipsPanel();
		}
		pausePanel.GetComponent<PausePanelManager>().musicOn.SetSelected(gameState.MusicOn);
		pausePanel.GetComponent<PausePanelManager>().musicOff.SetSelected(!gameState.MusicOn);
		pausePanel.GetComponent<PausePanelManager>().soundOn.SetSelected(gameState.SoundOn);
		pausePanel.GetComponent<PausePanelManager>().soundOff.SetSelected(!gameState.SoundOn);
		ShowBlock(pausePanel.transform.localPosition);
	}

	public void HidePausePanel()
	{
		pausePanel.transform.position = new Vector3(0f, 1000f, -20f);
		HideBlock();
	}

	public void ShowGameOverPanel(GameOverType overType)
	{
		gameOverPanel.transform.position = new Vector3(0f, 0f, -20f);
		gameOverPanel.GetComponent<GameOverTUI>().gameoverType = overType;
		gameOverPanel.GetComponent<GameOverTUI>().enabled = true;
		ShowBlock(gameOverPanel.transform.localPosition);
		if (msgBox != null)
		{
			msgBox.GetComponent<MsgBoxDelegate>().Hide();
		}
		if (msgBoxYesOrNo != null)
		{
			msgBoxYesOrNo.GetComponent<MsgBoxDelegate>().Hide();
		}
		GameApp.GetInstance().Save();
		OpenClikPlugin.Show(false);
	}

	public void HideGameOverPanel()
	{
		gameOverPanel.transform.position = new Vector3(0f, 2000f, -20f);
		gameOverPanel.GetComponent<GameOverTUI>().enabled = false;
		HideBlock();
	}

	public bool ShowItemsReportPanel()
	{
		if (player.pickupItemsPacket != null && player.pickupItemsPacket.Count > 0)
		{
			if (dayClear != null)
			{
				dayClear.SetActive(false);
			}
			Items_Report_Panel = Object.Instantiate(Resources.Load("Prefabs/TUI/Pickups_Report"), new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
			Items_Report_Panel.transform.parent = m_tuiControl;
			Items_Report_Panel.transform.localPosition = new Vector3(0f, 0f, -50f);
			ShowBlock(Items_Report_Panel.transform.localPosition);
			Transform transform = Items_Report_Panel.transform.Find("ScrollObjects");
			TUIRect component = Items_Report_Panel.transform.Find("rect").GetComponent<TUIRect>();
			int num = 0;
			foreach (ItemType key in player.pickupItemsPacket.Keys)
			{
				if (key == ItemType.Crystal && GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs)
				{
					GameApp.GetInstance().GetGameState().AddCrystal(player.pickupItemsPacket[key]);
				}
				GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/Report_pickup")) as GameObject;
				gameObject.GetComponent<TUIMeshSprite>().frameName_Accessor = "item_report_" + key;
				gameObject.transform.parent = transform;
				gameObject.transform.localPosition = new Vector3(80 * num, 0f, -2f);
				gameObject.GetComponent<TUIMeshSpriteClip>().clip = component;
				TUIMeshTextClip component2 = gameObject.transform.Find("count").GetComponent<TUIMeshTextClip>();
				component2.clip = component;
				component2.text_Accessor = "x" + player.pickupItemsPacket[key];
				num++;
				if (gameState.GetItemByType(key) != null)
				{
					gameState.GetItemByType(key).OwnedCount += player.pickupItemsPacket[key];
				}
			}
			if (num > 1)
			{
				TUIScroll component3 = transform.transform.parent.Find("Scroll").GetComponent<TUIScroll>();
				component3.borderXMin = -80 * (num - 1);
				component3.rangeXMin = component3.borderXMin;
			}
			GameApp.GetInstance().Save();
			return true;
		}
		return false;
	}

	public void HideItemsReportPanel()
	{
		if (Items_Report_Panel != null)
		{
			Items_Report_Panel.GetComponent<PanelParameter>().HidePanel();
			HideBlock();
		}
	}

	public void ShowNewAvatarPanel(AvatarType a)
	{
		if (dayClear != null)
		{
			dayClear.SetActive(false);
		}
		newItemPanel.transform.position = new Vector3(0f, 0f, -20f);
		ShowBlock(newItemPanel.transform.localPosition);
		if (a != AvatarType.None)
		{
			TUIMeshSprite component = newItemPanel.transform.Find("weaponLogo").gameObject.GetComponent<TUIMeshSprite>();
			int num = (int)a;
			component.frameName_Accessor = "playerLogo_" + num;
			newItemPanel.transform.Find("unlockText").gameObject.GetComponent<TUIMeshText>().text_Accessor = a.ToString() + " IS AVAILABLE FOR PURCHASE!";
		}
	}

	public void HideNewAvatarPanel()
	{
		newItemPanel.transform.position = new Vector3(0f, 3000f, -20f);
		HideBlock();
	}

	public void ShowNewItemPanel(Weapon weapon)
	{
		if (dayClear != null)
		{
			dayClear.SetActive(false);
		}
		newItemPanel.transform.position = new Vector3(0f, 0f, -20f);
		ShowBlock(newItemPanel.transform.localPosition);
		if (weapon != null)
		{
			newItemPanel.transform.Find("weaponLogo").gameObject.GetComponent<TUIMeshSprite>().frameName_Accessor = "weaponLogo_" + weapon.Name;
			if (weapon.Name != "Chainsaw")
			{
				newItemPanel.transform.Find("unlockText").gameObject.GetComponent<TUIMeshText>().text_Accessor = weapon.Name + " IS AVAILABLE FOR PURCHASE!";
			}
			else
			{
				newItemPanel.transform.Find("unlockText").gameObject.GetComponent<TUIMeshText>().text_Accessor = weapon.Name + " IS AVAILABLE TO USE!";
			}
		}
	}

	public void HideNewItemPanel()
	{
		newItemPanel.transform.position = new Vector3(0f, 3000f, -20f);
		HideBlock();
	}

	public void UpdateNewItemPanelTimer(int timer)
	{
		newItemPanel.transform.Find("Timer").GetComponent<TUIMeshText>().text_Accessor = "00:0" + timer;
	}

	public void ShowDialogPanel()
	{
		dialogPanel.transform.position = new Vector3(0f, 0f, -20f);
		ShowBlock(dialogPanel.transform.localPosition);
	}

	public void HideDialogPanel()
	{
		dialogPanel.transform.position = new Vector3(0f, 4000f, -20f);
		HideBlock();
	}

	public void ShowTutorialTipsPanel()
	{
		tutorialTipsPanel.transform.position = new Vector3(0f, 0f, -20f);
		ShowBlock(tutorialTipsPanel.transform.localPosition);
	}

	public void HideTutorialTipsPanel()
	{
		tutorialTipsPanel.transform.position = new Vector3(0f, -1000f, -20f);
		HideBlock();
	}

	public void ShowRebirthMsgBox()
	{
		if (GameApp.GetInstance().GetGameState().Revivepack > 0)
		{
			msgBoxYesOrNo.GetComponent<MsgBoxDelegate>().Show("USE A COMEBACK PACK?", MsgBoxType.Rebirth);
			msgBoxYesOrNo.GetComponent<MsgBoxDelegate>().activeTime = 15f;
		}
		else if (GameApp.GetInstance().GetGameState().GetCrystal() > 0)
		{
			msgBoxYesOrNo.GetComponent<MsgBoxDelegate>().Show("RESPAWN NOW?", MsgBoxType.CrystalRebirth);
			msgBoxYesOrNo.GetComponent<MsgBoxDelegate>().activeTime = 15f;
		}
		else
		{
			player.PlayerRealDead();
		}
	}

	public void HideRebirthMsgBox()
	{
		if (msgBoxYesOrNo != null)
		{
			msgBoxYesOrNo.GetComponent<MsgBoxDelegate>().Hide();
			msgBoxYesOrNo.GetComponent<MsgBoxDelegate>().m_type = MsgBoxType.None;
		}
	}

	public void OnReverseHearWaiting(TNetEventData evt)
	{
		if (gameScene.GetGameExcute())
		{
			waitingPanel.GetComponent<WaitingPanelManager>().Show();
			Debug.Log("OnReverseHearWaiting...");
		}
	}

	public void OnReverseHearWaiting()
	{
		if (gameScene.GetGameExcute())
		{
			waitingPanel.GetComponent<WaitingPanelManager>().Show();
			Debug.Log("OnReverseHearWaiting...");
		}
	}

	public void OnReverseHearRenew(TNetEventData evt)
	{
		if (gameScene.GetGameExcute())
		{
			waitingPanel.GetComponent<WaitingPanelManager>().Hide();
			Debug.Log("OnReverseHearRenew...");
		}
	}

	public void OnReverseHearRenew()
	{
		if (gameScene.GetGameExcute())
		{
			waitingPanel.GetComponent<WaitingPanelManager>().Hide();
			Debug.Log("OnReverseHearRenew...");
		}
	}

	public void OnReverseHearTimeout(TNetEventData evt)
	{
		if (gameScene.GetGameExcute())
		{
			waitingPanel.GetComponent<WaitingPanelManager>().Hide();
			msgBoxYesOrNo.GetComponent<MsgBoxDelegate>().Hide();
			msgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.netDisconnected, MsgBoxType.ContectingTimeout);
			Time.timeScale = 0f;
		}
	}

	public void OnReverseHearTimeout()
	{
		if (gameScene.GetGameExcute())
		{
			waitingPanel.GetComponent<WaitingPanelManager>().Hide();
			msgBoxYesOrNo.GetComponent<MsgBoxDelegate>().Hide();
			msgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.netDisconnected, MsgBoxType.ContectingTimeout);
			Time.timeScale = 0f;
		}
	}

	public void OnClosed()
	{
		Time.timeScale = 0f;
		Debug.Log("Game Scene OnClosed-----------");
		waitingPanel.transform.localPosition = new Vector3(0f, -5000f, -100f);
		msgBoxYesOrNo.GetComponent<MsgBoxDelegate>().Hide();
		msgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.netDisconnected, MsgBoxType.OnClosed);
	}

	public void OnConnectingLost()
	{
		Time.timeScale = 0f;
		waitingPanel.transform.localPosition = new Vector3(0f, -5000f, -100f);
		msgBoxYesOrNo.GetComponent<MsgBoxDelegate>().Hide();
		msgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.netDisconnected, MsgBoxType.ContectingLost);
	}

	private void OnDestroy()
	{
		TNetConnection.UnregisterSFSSceneCallbacks();
		if (gameState.net_com != null)
		{
			gameState.net_com.UnregisterCallbacks();
		}
		NetworkObj.DestroyNetCom();
	}
}
