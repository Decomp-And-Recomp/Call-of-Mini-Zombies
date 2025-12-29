using System;
using TNetSdk;
using UnityEngine;
using Zombie3D;

public class VSGameMissionTimer : MonoBehaviour
{
	private float deltaTime;

	public bool isMissionOver { get; set; }

	public float missionTotalTime { get; set; }

	public float missionCurrentTime { get; set; }

	public double missionStartTime { get; set; }

	public bool inited { get; set; }

	public void Init()
	{
		isMissionOver = false;
		missionStartTime = TNetConnection.Connection.CurRoom.GetVariable(TNetRoomVarType.GameStarted).GetDouble("GameStartTime");
		missionTotalTime = 600f;
		missionCurrentTime = 600f;
		deltaTime = 0f;
		inited = true;
	}

	private void Update()
	{
		if (!TNetConnection.IsInitialized || !inited)
		{
			return;
		}
		deltaTime += Time.deltaTime;
		if ((double)deltaTime < 0.03)
		{
			return;
		}
		deltaTime = 0f;
		if (!isMissionOver)
		{
			if (TNetConnection.Connection.TimeManager != null && TNetConnection.Connection.TimeManager.IsSynchronized())
			{
				missionCurrentTime = missionTotalTime - (float)(TNetConnection.Connection.TimeManager.NetworkTime - missionStartTime) / 1000f;
			}
			if (missionCurrentTime <= 0f)
			{
				missionCurrentTime = 0f;
				isMissionOver = true;
				Debug.Log("Time out and game over.");
				(GameApp.GetInstance().GetGameScene() as GameVSScene).GetLastMasterKiller();
				(GameApp.GetInstance().GetGameScene() as GameVSScene).QuitGameForDisconnect(8f);
				GameApp.GetInstance().GetGameScene().GameGUI.ShowGameOverPanel(GameOverType.vsTimeOut);
			}
		}
		TimeSpan timeSpan = new TimeSpan(0, 0, (int)missionCurrentTime);
		base.gameObject.GetComponent<TUIMeshText>().text_Accessor = timeSpan.ToString();
	}
}
