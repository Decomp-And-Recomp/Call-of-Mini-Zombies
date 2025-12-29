using System.Collections;
using UnityEngine;
using Zombie3D;

public class GameOverTUI : MonoBehaviour
{
	public GameObject quitButton;

	public GameObject retryButton;

	public GameObject gameOverLogo;

	public int totalKills;

	public int totalDeaths;

	public bool vsChampion;

	private Vector3 maxScale = default(Vector3);

	private Vector3 targetScale = default(Vector3);

	private bool enlarge;

	private float delta_time;

	public GameOverType gameoverType { get; set; }

	private IEnumerator Start()
	{
		GameState gameState = GameApp.GetInstance().GetGameState();
		GameScene gameScene = GameApp.GetInstance().GetGameScene();
		switch (gameoverType)
		{
		case GameOverType.soloLose:
			if (gameState.LevelNum > 0)
			{
				FlurryStatistics.SoloEndEvent("Lose", gameState.LevelNum);
				FlurryStatistics.LoseGameEvent();
			}
			break;
		case GameOverType.soloQuit:
			FlurryStatistics.SoloEndEvent("Quit", gameState.LevelNum);
			FlurryStatistics.QuitGameEvent("Normal");
			break;
		case GameOverType.instanceLose:
		{
			GameObject reportData3 = GameObject.Find("InstanceReportObj");
			if (reportData3 != null)
			{
				FlurryStatistics.EndlessEndEvent("Lose", gameState.LevelNum, reportData3.GetComponent<InstanceReportData>().score, reportData3.GetComponent<InstanceReportData>().maxWave, reportData3.GetComponent<InstanceReportData>().rewardCash, reportData3.GetComponent<InstanceReportData>().rewardExp, reportData3.GetComponent<InstanceReportData>().survivalTime);
			}
			FlurryStatistics.LoseGameEvent();
			break;
		}
		case GameOverType.instanceTimeOut:
		{
			GameObject reportData3 = GameObject.Find("InstanceReportObj");
			if (reportData3 != null)
			{
				FlurryStatistics.EndlessEndEvent("Time Out", gameState.LevelNum, reportData3.GetComponent<InstanceReportData>().score, reportData3.GetComponent<InstanceReportData>().maxWave, reportData3.GetComponent<InstanceReportData>().rewardCash, reportData3.GetComponent<InstanceReportData>().rewardExp, reportData3.GetComponent<InstanceReportData>().survivalTime);
			}
			FlurryStatistics.LoseGameEvent();
			break;
		}
		case GameOverType.instanceQuit:
		{
			GameObject reportData3 = GameObject.Find("InstanceReportObj");
			if (reportData3 != null)
			{
				FlurryStatistics.EndlessEndEvent("Quit", gameState.LevelNum, reportData3.GetComponent<InstanceReportData>().score, reportData3.GetComponent<InstanceReportData>().maxWave, reportData3.GetComponent<InstanceReportData>().rewardCash, reportData3.GetComponent<InstanceReportData>().rewardExp, reportData3.GetComponent<InstanceReportData>().survivalTime);
			}
			FlurryStatistics.QuitGameEvent("Endless");
			break;
		}
		case GameOverType.coopLose:
			FlurryStatistics.CoopEndEvent("Finish", totalDeaths);
			break;
		case GameOverType.coopQuit:
		case GameOverType.coopCloseConnection:
			FlurryStatistics.CoopEndEvent("Quit", totalDeaths);
			FlurryStatistics.QuitGameEvent("COOP");
			break;
		case GameOverType.coopLostConnection:
			FlurryStatistics.CoopEndEvent("ConnectionLost", totalDeaths);
			FlurryStatistics.ServerDisconnectEvent("COOP");
			break;
		case GameOverType.vsTimeOut:
			FlurryStatistics.VsEndEvent("Finish", totalKills, totalDeaths);
			break;
		case GameOverType.vsQuit:
		case GameOverType.vsCloseConnection:
			FlurryStatistics.VsEndEvent("Quit", totalKills, totalDeaths);
			FlurryStatistics.QuitGameEvent("VS");
			break;
		case GameOverType.vsLostConnection:
			FlurryStatistics.VsEndEvent("ConnectionLost", totalKills, totalDeaths);
			FlurryStatistics.ServerDisconnectEvent("VS");
			break;
		}
		gameScene.StopGameMusic();
		if (gameoverType == GameOverType.soloQuit || gameoverType == GameOverType.instanceQuit)
		{
			gameOverLogo.SetActive(false);
			retryButton.SetActive(false);
			quitButton.SetActive(false);
			if (gameState.gameMode == GameMode.Hunting || gameState.gameMode == GameMode.SoloBoss || gameState.gameMode == GameMode.DinoHunting)
			{
				if (!gameScene.GameGUI.ShowItemsReportPanel())
				{
					FadeAnimationScript.GetInstance().StartFade(new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 1f), 1f);
					yield return new WaitForSeconds(2f);
					SceneName.LoadLevel("MainMapTUI");
				}
			}
			else if (gameState.gameMode == GameMode.Solo)
			{
				FadeAnimationScript.GetInstance().StartFade(new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 1f), 1f);
				yield return new WaitForSeconds(2f);
				SceneName.LoadLevel("MainMapTUI");
			}
			else if (gameState.gameMode == GameMode.Instance)
			{
				FadeAnimationScript.GetInstance().StartFade(new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 1f), 1f);
				yield return new WaitForSeconds(2f);
				SceneName.LoadLevel("InstanceReportTUI");
			}
		}
		else
		{
			if (gameScene.GamePlayingState != PlayingState.GameQuit && gameState.gameMode != GameMode.Instance)
			{
				gameScene.PlayLoseMusic();
			}
			if (gameoverType == GameOverType.instanceTimeOut)
			{
				gameOverLogo.GetComponent<TUIMeshSprite>().frameName_Accessor = "TimeUp";
			}
			else
			{
				gameOverLogo.GetComponent<TUIMeshSprite>().frameName_Accessor = "GameOver";
			}
			gameOverLogo.transform.localPosition = new Vector3(0f, 40f, -5f);
			retryButton.SetActive(false);
			quitButton.SetActive(false);
			maxScale = gameOverLogo.transform.localScale * 1.2f;
			targetScale = gameOverLogo.transform.localScale * 1f;
			gameOverLogo.transform.localScale = gameOverLogo.transform.localScale * 0.25f;
			enlarge = true;
			delta_time = 0f;
		}
		yield return 0;
	}

	private void Update()
	{
		delta_time += Time.deltaTime;
		if (!((double)delta_time >= 0.02))
		{
			return;
		}
		if (enlarge)
		{
			if (gameOverLogo.transform.localScale.magnitude < maxScale.magnitude)
			{
				gameOverLogo.transform.localScale += gameOverLogo.transform.localScale * delta_time;
			}
			else
			{
				enlarge = false;
			}
		}
		else if (gameOverLogo.transform.localScale.magnitude > targetScale.magnitude)
		{
			gameOverLogo.transform.localScale -= gameOverLogo.transform.localScale * delta_time;
		}
		else
		{
			base.enabled = false;
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Solo)
			{
				retryButton.SetActive(true);
				quitButton.SetActive(true);
			}
			else if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Tutorial)
			{
				retryButton.SetActive(true);
				retryButton.transform.localPosition = new Vector3(0f, retryButton.transform.localPosition.y, retryButton.transform.localPosition.z);
			}
			else if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Hunting || GameApp.GetInstance().GetGameState().gameMode == GameMode.DinoHunting || GameApp.GetInstance().GetGameState().gameMode == GameMode.SoloBoss)
			{
				if (!GameApp.GetInstance().GetGameScene().GameGUI.ShowItemsReportPanel())
				{
					retryButton.SetActive(true);
					quitButton.SetActive(true);
				}
			}
			else if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Instance)
			{
				FadeAnimationScript.GetInstance().StartFade(new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 1f), 1.5f);
				FadeAnimationScript.GetInstance().onEnd = OnFadeEnd;
			}
		}
		delta_time = 0f;
	}

	private void OnFadeEnd()
	{
		SceneName.LoadLevel("InstanceReportTUI");
	}

	public void CheckCoopVsLoseCount(GameOverType m_type)
	{
		if (!GameApp.GetInstance().GetGameState().soldOutNewbie1)
		{
			if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Coop && GameApp.GetInstance().GetGameState().coopLoseCount == 0)
			{
				GameApp.GetInstance().GetGameState().coopLoseCount++;
				MainMapTUI.showNewbie1Intro = true;
			}
			else if (GameApp.GetInstance().GetGameState().gameMode == GameMode.Vs && GameApp.GetInstance().GetGameState().vsLoseCount == 0)
			{
				GameApp.GetInstance().GetGameState().vsLoseCount++;
				MainMapTUI.showNewbie1Intro = true;
			}
		}
		if (GameApp.GetInstance().GetGameState().soldOutNewbie2)
		{
			return;
		}
		switch (m_type)
		{
		case GameOverType.coopLose:
			GameApp.GetInstance().GetGameState().coopLoseCount++;
			if (GameApp.GetInstance().GetGameState().coopLoseCount > 0 && GameApp.GetInstance().GetGameState().coopLoseCount % 5 == 0)
			{
				MainMapTUI.showNewbie2Intro = true;
			}
			break;
		case GameOverType.vsTimeOut:
		case GameOverType.vsQuit:
			if (!vsChampion)
			{
				GameApp.GetInstance().GetGameState().vsLoseCount++;
				if (GameApp.GetInstance().GetGameState().vsLoseCount > 0 && GameApp.GetInstance().GetGameState().vsLoseCount % 5 == 0)
				{
					MainMapTUI.showNewbie2Intro = true;
				}
			}
			break;
		}
	}
}
