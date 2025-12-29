using System.Collections;
using UnityEngine;
using Zombie3D;

public class PlayerShell : MonoBehaviour
{
	public Player m_player;

#if UNITY_STANDALONE || UNITY_EDITOR
	void Update()
	{
		if (m_player.InputController == null) return;

		m_player.InputController.ProcessInput(Time.deltaTime, m_player.InputController.InputInfo);
	}
#endif

	public void OnDeadCameraChange(Player mPlayer)
	{
		StartCoroutine(CameraChangeTarget(mPlayer));
	}

	public IEnumerator CameraChangeTarget(Player mPlayer)
	{
		yield return new WaitForSeconds(2f);
		if (mPlayer != null)
		{
			(GameApp.GetInstance().GetGameScene().GetCamera() as TPSSimpleCameraScript).LockCameraVSKiller(mPlayer.GetTransform());
		}
	}

	public void OnAvatarShowCameraChange(bool isMyself, Player mPlayer)
	{
		if (isMyself)
		{
			StartCoroutine(CameraChangeMe(mPlayer));
		}
		else
		{
			StartCoroutine(CameraChangeWinner(mPlayer));
		}
	}

	public IEnumerator CameraChangeMe(Player mPlayer)
	{
		yield return new WaitForSeconds(2f);
		if (mPlayer != null)
		{
			mPlayer.ResetSawAnimation();
			mPlayer.Animate("Idle01" + mPlayer.WeaponNameEnd, WrapMode.Loop);
			mPlayer.StopFire();
			GameObject crownObj = Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().crown, mPlayer.GetTransform().TransformPoint(Vector3.up * 2.1f), Quaternion.identity) as GameObject;
			crownObj.transform.parent = mPlayer.GetTransform();
			GameApp.GetInstance().GetGameScene().GetCamera()
				.player = mPlayer;
			GameApp.GetInstance().GetGameScene().GamePlayingState = PlayingState.GameWin;
		}
	}

	public IEnumerator CameraChangeWinner(Player mPlayer)
	{
		yield return new WaitForSeconds(2f);
		if (mPlayer != null)
		{
			mPlayer.ResetSawAnimation();
			mPlayer.Animate("Idle01" + mPlayer.WeaponNameEnd, WrapMode.Loop);
			mPlayer.StopFire();
			GameObject crownObj = Object.Instantiate(GameApp.GetInstance().GetGameResourceConfig().crown, mPlayer.GetTransform().TransformPoint(Vector3.up * 2.1f), Quaternion.identity) as GameObject;
			crownObj.transform.parent = mPlayer.GetTransform();
			GameApp.GetInstance().GetGameScene().GetCamera()
				.player = mPlayer;
			GameApp.GetInstance().GetGameScene().GamePlayingState = PlayingState.GameWin;
		}
	}
}
