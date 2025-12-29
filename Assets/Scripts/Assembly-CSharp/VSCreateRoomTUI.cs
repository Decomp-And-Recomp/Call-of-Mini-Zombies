using TNetSdk;
using UnityEngine;
using Zombie3D;

public class VSCreateRoomTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	private AudioPlayer audioPlayer = new AudioPlayer();

	private TNetObject tnetObj;

	public GameObject CreatePasswordPanel;

	public GameObject ChosenFrame;

	public GameObject IndicatorPanel;

	public GameObject MsgBox;

	public TUIMeshText Title;

	public GameObject maskBlock;

	private int currentChosenScene;

	public void Awake()
	{
		if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Vs)
		{
			base.enabled = false;
			return;
		}
		base.enabled = true;
		GameScript.CheckFillBlack();
		GameScript.CheckMenuResourceConfig();
		GameScript.CheckGlobalResourceConfig();
		if (GameObject.Find("Music") == null)
		{
			GameApp.GetInstance().InitForMenu();
			GameObject gameObject = new GameObject("Music");
			Object.DontDestroyOnLoad(gameObject);
			gameObject.transform.position = new Vector3(0f, 1f, -10f);
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.clip = GameApp.GetInstance().GetMenuResourceConfig().menuAudio;
			audioSource.loop = true;
			audioSource.bypassEffects = true;
			audioSource.rolloffMode = AudioRolloffMode.Linear;
			audioSource.mute = !GameApp.GetInstance().GetGameState().MusicOn;
			audioSource.Play();
		}
		if (TNetConnection.IsInitialized)
		{
			tnetObj = TNetConnection.Connection;
			tnetObj.AddEventListener(TNetEventSystem.CONNECTION_KILLED, OnConnectionLost);
			tnetObj.AddEventListener(TNetEventRoom.ROOM_CREATION, OnRoomCreation);
			tnetObj.AddEventListener(TNetEventRoom.ROOM_JOIN, OnRoomJoin);
			tnetObj.AddEventListener(TNetEventSystem.REVERSE_HEART_RENEW, OnReverseHearRenew);
			tnetObj.AddEventListener(TNetEventSystem.REVERSE_HEART_TIMEOUT, OnReverseHearTimeout);
			tnetObj.AddEventListener(TNetEventSystem.REVERSE_HEART_WAITING, OnReverseHearWaiting);
			TNetConnection.is_server = false;
		}
		else
		{
			Debug.Log("tnet init error!");
		}
	}

	private void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		audioPlayer.AddAudio(base.transform, "Button", true);
		audioPlayer.AddAudio(base.transform, "Back", true);
		IndicatorPanel.GetComponent<IndicatorTUI>().automaticClose = OnConnectingTimeOut;
		Title.text_Accessor = "VS";
		Title.color_Accessor = ColorName.fontColor_blue;
		AdjustSceneLogo();
		OpenClikPlugin.Hide();
		Resources.UnloadUnusedAssets();
	}

	private void AdjustSceneLogo()
	{
		TUIMeshSprite[] componentsInChildren = GameObject.Find("TUI/TUIControl/PanelBase/ScrollObjects").GetComponentsInChildren<TUIMeshSprite>();
		if (VSHallTUI.isPixelVS)
		{
			GameObject.Find("TUI/TUIControl/Title/text").GetComponent<TUIMeshText>().text_Accessor = "VS-PIXEL";
			TUIScroll component = GameObject.Find("TUI/TUIControl/PanelBase/Scroll").GetComponent<TUIScroll>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				int num = int.Parse(componentsInChildren[i].name.Substring("scene".Length));
				componentsInChildren[i].name = "scene" + (9 + num);
				componentsInChildren[i].frameName_Accessor = SceneName.GetNetMapName(9 + num);
				if (num == 1)
				{
					ChooseScene(componentsInChildren[i].gameObject);
				}
				if (GameApp.GetInstance().GetGameState().show_pixelMap_update && num <= 4)
				{
					GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/New_Label")) as GameObject;
					gameObject.name = "NewLabel";
					gameObject.transform.parent = componentsInChildren[i].transform;
					gameObject.transform.localPosition = new Vector3(-52f, 22f, -0.2f);
				}
			}
			GameApp.GetInstance().GetGameState().show_pixelMap_update = false;
			return;
		}
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			int num2 = int.Parse(componentsInChildren[j].name.Substring("scene".Length));
			if (num2 > 9)
			{
				componentsInChildren[j].gameObject.SetActive(false);
			}
		}
		GameObject.Find("TUI/TUIControl/Title/text").GetComponent<TUIMeshText>().text_Accessor = "VS-NORMAL";
		ChooseScene(GameObject.Find("scene1"));
	}

	private void FixedUpdate()
	{
		if (tnetObj != null)
		{
			tnetObj.Update(Time.fixedDeltaTime);
		}
	}

	private void Update()
	{
		input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.name == "Back_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Back");
			SceneName.FadeOutLevel("VSHallTUI");
		}
		else if (control.name.StartsWith("scene") && eventType == 1)
		{
			audioPlayer.PlayAudio("Button");
			ChooseScene(control.gameObject);
		}
		else if (control.name == "Create_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			CreatePasswordPanel.transform.localPosition = new Vector3(0f, 0f, CreatePasswordPanel.transform.localPosition.z);
		}
		else if (control.name == "Create_Password_Close_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			CreatePasswordPanel.transform.localPosition = new Vector3(0f, 1000f, CreatePasswordPanel.transform.localPosition.z);
		}
		else if (control.name == "Create_Password_OK_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			CreatePasswordPanel.transform.localPosition = new Vector3(0f, 1000f, CreatePasswordPanel.transform.localPosition.z);
			control.gameObject.SetActive(false);
			StartServer(CreatePasswordPanel.GetComponent<TUITextField>().GetText());
			maskBlock.transform.localPosition = new Vector3(0f, 0f, maskBlock.transform.localPosition.z);
		}
		else if (control.name == "Msg_OK_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			MsgBox.GetComponent<MsgBoxDelegate>().Hide();
			if (MsgBox.GetComponent<MsgBoxDelegate>().m_type == MsgBoxType.ContectingTimeout)
			{
				SceneName.FadeOutLevel("VSHallTUI");
				FlurryStatistics.ServerDisconnectEvent("VS");
			}
		}
	}

	private void ChooseScene(GameObject button)
	{
		if (button.transform.Find("NewLabel") != null)
		{
			Object.Destroy(button.transform.Find("NewLabel").gameObject);
		}
		currentChosenScene = int.Parse(button.name.Substring("scene".Length));
		ChosenFrame.GetComponent<ChosenFrameManager>().ChosenObject = button;
	}

	private void StartServer(string password)
	{
		string room_name = tnetObj.Myself.Name + "|open";
		int num = VSHallTUI.groupIdMin + currentChosenScene - 1;
		if (num <= VSHallTUI.groupIdMax)
		{
			tnetObj.Send(new CreateRoomRequest(room_name, password, num, 4, RoomCreateCmd.RoomType.open, RoomCreateCmd.RoomSwitchMasterType.Auto, GameApp.GetInstance().GetGameState().LevelNum.ToString()));
		}
		Debug.Log("start vs scene group id: " + num + ", scene name: " + SceneName.GetNetMapName(currentChosenScene));
	}

	private void OnConnectingTimeOut()
	{
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
		if (MsgBox != null)
		{
			MsgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.netDisconnected, MsgBoxType.ContectingTimeout);
		}
		maskBlock.transform.localPosition = new Vector3(0f, -1000f, maskBlock.transform.localPosition.z);
		TNetConnection.UnregisterSFSSceneCallbacks();
		TNetConnection.Disconnect();
	}

	private void OnConnectionLost(TNetEventData evt)
	{
		Debug.Log("Connection was lost.");
		maskBlock.transform.localPosition = new Vector3(0f, -1000f, maskBlock.transform.localPosition.z);
		TNetConnection.UnregisterSFSSceneCallbacks();
		TNetConnection.Disconnect();
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
		if (MsgBox != null)
		{
			MsgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.netUnableConnect, MsgBoxType.ContectingTimeout);
		}
	}

	private void OnRoomCreation(TNetEventData evt)
	{
		Debug.Log("OnRoomCreation...");
		if ((int)evt.data["result"] == 0)
		{
			Debug.Log("Room create success.");
			TNetConnection.is_server = true;
		}
		else
		{
			maskBlock.transform.localPosition = new Vector3(0f, -1000f, maskBlock.transform.localPosition.z);
			Debug.Log("An error occurred while attempting to create a room!");
		}
	}

	private void OnRoomJoin(TNetEventData evt)
	{
		Debug.Log("OnRoomJoin...");
		if ((int)evt.data["result"] == 0)
		{
			Debug.Log("Room join success.");
			GameApp.GetInstance().GetGameState().cur_net_map = SceneName.GetNetMapName(currentChosenScene);
			SceneName.FadeOutLevel("RoomTUI");
			FlurryStatistics.VsChooseMap(GameApp.GetInstance().GetGameState().cur_net_map);
		}
		else
		{
			maskBlock.transform.localPosition = new Vector3(0f, -1000f, maskBlock.transform.localPosition.z);
			Debug.Log("An error occurred while attempting to create a room!");
		}
	}

	private void OnReverseHearWaiting(TNetEventData evt)
	{
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().SetContent("WAITING FOR SERVER.");
			IndicatorPanel.GetComponent<IndicatorTUI>().Show(IndicatorTUI.IndicatorType.HeartWaiting);
		}
	}

	private void OnReverseHearRenew(TNetEventData evt)
	{
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
	}

	private void OnReverseHearTimeout(TNetEventData evt)
	{
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
		if (MsgBox != null)
		{
			MsgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.netDisconnected, MsgBoxType.ContectingTimeout);
		}
		maskBlock.transform.localPosition = new Vector3(0f, -1000f, maskBlock.transform.localPosition.z);
		TNetConnection.UnregisterSFSSceneCallbacks();
		TNetConnection.Disconnect();
	}

	private void OnDestroy()
	{
		TNetConnection.UnregisterSFSSceneCallbacks();
		Debug.Log("unregister callbacks...");
	}
}
