using UnityEngine;
using Zombie3D;

public class CoopCreateRoomTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	private AudioPlayer audioPlayer = new AudioPlayer();

	private NetworkObj net_com;

	public GameObject CreatePasswordPanel;

	public GameObject ChosenFrame;

	public GameObject IndicatorPanel;

	public GameObject MsgBox;

	public TUIMeshText Title;

	public GameObject maskBlock;

	private int currentChosenScene;

	private int currentChosenMapId;

	public void Awake()
	{
		if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Coop)
		{
			base.enabled = false;
			return;
		}
		base.enabled = true;
		GameScript.CheckFillBlack();
		GameScript.CheckMenuResourceConfig();
		GameScript.CheckGlobalResourceConfig();
		NetworkObj.CheckNetWorkCom();
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
		net_com = GameApp.GetInstance().GetGameState().net_com;
		net_com.packet_delegate = OnPacket;
		net_com.close_delegate = OnClosed;
		net_com.contecting_timeout = OnContectingTimeout;
		net_com.contecting_lost = OnContectingLost;
		net_com.reverse_heart_timeout_delegate = OnReverseHearTimeout;
		net_com.reverse_heart_renew_delegate = OnReverseHearRenew;
		net_com.reverse_heart_waiting_delegate = OnReverseHearWaiting;
	}

	private void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		audioPlayer.AddAudio(base.transform, "Button", true);
		audioPlayer.AddAudio(base.transform, "Back", true);
		IndicatorPanel.GetComponent<IndicatorTUI>().automaticClose = OnContectingTimeout;
		Title.text_Accessor = "BOSS RAID";
		Title.color_Accessor = ColorName.fontColor_green;
		AdjustSceneLogo();
		ChooseScene(GameObject.Find("scene1"));
		OpenClikPlugin.Hide();
		Resources.UnloadUnusedAssets();
	}

	private void AdjustSceneLogo()
	{
		TUIMeshSprite[] componentsInChildren = GameObject.Find("TUI/TUIControl/PanelBase/ScrollObjects").GetComponentsInChildren<TUIMeshSprite>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].frameName_Accessor = componentsInChildren[i].frameName_Accessor + "_m";
			int num = int.Parse(componentsInChildren[i].name.Substring("scene".Length));
			if (num > 9)
			{
				componentsInChildren[i].gameObject.SetActive(false);
			}
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
			SceneName.FadeOutLevel("CoopHallTUI");
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
			Packet packet = CGCreateRoomPacket.MakePacket((uint)currentChosenMapId, (long)(Time.time * 1000f), GameApp.GetInstance().GetGameState().nick_name, (uint)GameApp.GetInstance().GetGameState().Avatar, (uint)GameApp.GetInstance().GetGameState().LevelNum, CreatePasswordPanel.GetComponent<TUITextField>().GetText());
			net_com.Send(packet);
			maskBlock.transform.localPosition = new Vector3(0f, 0f, maskBlock.transform.localPosition.z);
		}
		else if (control.name == "Msg_OK_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			MsgBox.GetComponent<MsgBoxDelegate>().Hide();
			switch (MsgBox.GetComponent<MsgBoxDelegate>().m_type)
			{
			case MsgBoxType.ContectingTimeout:
			case MsgBoxType.ContectingLost:
				SceneName.FadeOutLevel("VSHallTUI");
				FlurryStatistics.ServerDisconnectEvent("COOP");
				break;
			case MsgBoxType.OnClosed:
				SceneName.FadeOutLevel("VSHallTUI");
				break;
			case MsgBoxType.JoinRommFailed:
				break;
			}
		}
	}

	private void ChooseScene(GameObject button)
	{
		currentChosenScene = int.Parse(button.name.Substring("scene".Length));
		ChosenFrame.GetComponent<ChosenFrameManager>().ChosenObject = button;
		currentChosenMapId = CoopHallTUI.mapIdMin + currentChosenScene - 1;
	}

	public void OnPacket(Packet packet)
	{
		uint val = 0u;
		if (packet.WatchUInt32(ref val, 4))
		{
			switch ((PROTOCOLS)val)
			{
			case PROTOCOLS.CG_HEARTBEAT:
				Debug.Log("CG_HEARTBEAT revceived!!");
				break;
			case PROTOCOLS.GC_CREATEROOM:
				OnRoomCreation(packet);
				break;
			}
		}
	}

	private void OnRoomCreation(Packet packet)
	{
		GCCreateRoomPacket gCCreateRoomPacket = new GCCreateRoomPacket();
		if (!gCCreateRoomPacket.ParserPacket(packet))
		{
			return;
		}
		if (gCCreateRoomPacket.m_iResult == 0)
		{
			float num = (Time.time - (float)gCCreateRoomPacket.m_lLocalTime / 1000f) / 2f;
			net_com.m_netUserInfo.SetNetUserInfo(true, GameApp.GetInstance().GetGameState().nick_name, (int)gCCreateRoomPacket.m_iRoomId, (int)gCCreateRoomPacket.m_iUserId, num, currentChosenMapId, 0, (int)GameApp.GetInstance().GetGameState().Avatar, GameApp.GetInstance().GetGameState().LevelNum);
			Debug.Log("Create map now! ping : " + num + "map id:" + currentChosenMapId);
			GameApp.GetInstance().GetGameState().cur_net_map = SceneName.GetNetMapName(currentChosenScene);
			SceneName.FadeOutLevel("RoomTUI");
			net_com.ClearNetUserArr();
			net_com.netUserInfo_array[0] = net_com.m_netUserInfo;
			FlurryStatistics.CoopChooseMap(GameApp.GetInstance().GetGameState().cur_net_map);
		}
		else
		{
			Debug.Log("result error : " + gCCreateRoomPacket.m_iResult);
			maskBlock.transform.localPosition = new Vector3(0f, -1000f, maskBlock.transform.localPosition.z);
			if (MsgBox != null)
			{
				MsgBox.GetComponent<MsgBoxDelegate>().Show("Unable to create room, \nplease try again.".ToUpper(), MsgBoxType.CrateRommFailed);
			}
		}
	}

	private void OnClosed()
	{
		NetworkObj.DestroyNetCom();
		if (MsgBox != null)
		{
			MsgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.netDisconnected, MsgBoxType.OnClosed);
		}
		maskBlock.transform.localPosition = new Vector3(0f, -1000f, maskBlock.transform.localPosition.z);
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
	}

	private void OnContectingLost()
	{
		NetworkObj.DestroyNetCom();
		if (MsgBox != null)
		{
			MsgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.netDisconnected, MsgBoxType.ContectingLost);
		}
		maskBlock.transform.localPosition = new Vector3(0f, -1000f, maskBlock.transform.localPosition.z);
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
	}

	private void OnContectingTimeout()
	{
		Debug.Log("Server Connecting Timeount...");
		IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		NetworkObj.DestroyNetCom();
		if (MsgBox != null)
		{
			MsgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.netUnableConnect, MsgBoxType.ContectingTimeout);
		}
		maskBlock.transform.localPosition = new Vector3(0f, -1000f, maskBlock.transform.localPosition.z);
	}

	private void OnReverseHearWaiting()
	{
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().SetContent("WAITING FOR SERVER.");
			IndicatorPanel.GetComponent<IndicatorTUI>().Show(IndicatorTUI.IndicatorType.HeartWaiting);
		}
	}

	private void OnReverseHearRenew()
	{
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
	}

	private void OnReverseHearTimeout()
	{
		NetworkObj.DestroyNetCom();
		if (IndicatorPanel != null)
		{
			IndicatorPanel.GetComponent<IndicatorTUI>().Hide();
		}
		maskBlock.transform.localPosition = new Vector3(0f, -1000f, maskBlock.transform.localPosition.z);
		if (MsgBox != null)
		{
			MsgBox.GetComponent<MsgBoxDelegate>().Show(DialogString.netDisconnected, MsgBoxType.ContectingTimeout);
		}
	}

	private void OnDestroy()
	{
		if (net_com != null)
		{
			net_com.UnregisterCallbacks();
			Debug.Log("unregister callbacks...");
		}
	}
}
