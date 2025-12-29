using System;
using UnityEngine;
using Zombie3D;

public class InstanceReportTUI : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	protected TUIInput[] input;

	private AudioPlayer audioPlayer = new AudioPlayer();

	public TUIMeshText Score;

	public TUIMeshText EnemyKills;

	public TUIMeshText MaxWave;

	public TUIMeshText SurvivialTime;

	public TUIMeshText RewardCash;

	private void Awake()
	{
		GameScript.CheckFillBlack();
		GameApp.GetInstance().ClearScene();
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	private void Start()
	{
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		m_tui.transform.Find("TUIControl").Find("Fade").GetComponent<TUIFade>()
			.FadeIn();
		Transform folderTrans = base.transform.Find("Audio");
		audioPlayer.AddAudio(folderTrans, "Button", true);
		audioPlayer.AddAudio(folderTrans, "Achiv", true);
		audioPlayer.AddAudio(folderTrans, "Back", true);
		GameObject gameObject = GameObject.Find("InstanceReportObj");
		if ((bool)gameObject)
		{
			Score.text_Accessor = gameObject.GetComponent<InstanceReportData>().score.ToString();
			EnemyKills.text_Accessor = gameObject.GetComponent<InstanceReportData>().enemyKills.ToString();
			MaxWave.text_Accessor = gameObject.GetComponent<InstanceReportData>().maxWave.ToString();
			TimeSpan timeSpan = new TimeSpan(0, 0, (int)gameObject.GetComponent<InstanceReportData>().survivalTime);
			SurvivialTime.text_Accessor = timeSpan.ToString();
			RewardCash.text_Accessor = "$" + gameObject.GetComponent<InstanceReportData>().rewardCash;
		}
		OpenClikPlugin.Hide();
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
		if (control.name == "Quit_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			SceneName.FadeOutLevel("MainMapTUI");
		}
		else if (control.name == "Retry_Button" && eventType == 3)
		{
			audioPlayer.PlayAudio("Button");
			SceneName.FadeOutLevel(GameApp.GetInstance().GetGameState().last_scene);
		}
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(GameObject.Find("InstanceReportObj"));
	}
}
