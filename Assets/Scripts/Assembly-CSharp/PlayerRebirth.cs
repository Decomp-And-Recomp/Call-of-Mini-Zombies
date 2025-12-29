using UnityEngine;
using Zombie3D;

public class PlayerRebirth : MonoBehaviour
{
	public float _Rebirth_Time = 2f;

	protected float _Rebirth_Time_Interval;

	protected bool _Is_Rebirthing;

	protected Player tar_player;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != 27 || _Is_Rebirthing)
		{
			return;
		}
		PlayerShell component = base.gameObject.GetComponent<PlayerShell>();
		if (component == null || component.m_player == null || component.m_player.carryItemsPacket == null)
		{
			return;
		}
		PlayerShell component2 = other.gameObject.GetComponent<PlayerShell>();
		if (component2 != null)
		{
			tar_player = component2.m_player;
			if (GameApp.GetInstance().GetGameState().Medpack > 0)
			{
				_Rebirth_Time_Interval = 0f;
				_Is_Rebirthing = true;
				tar_player.OnRebirthStart();
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer != 27 || !_Is_Rebirthing)
		{
			return;
		}
		PlayerShell component = base.gameObject.GetComponent<PlayerShell>();
		if (component == null || component.m_player == null || component.m_player.carryItemsPacket == null)
		{
			return;
		}
		PlayerShell component2 = other.gameObject.GetComponent<PlayerShell>();
		if (!(component2 != null) || tar_player.m_multi_id != component2.m_player.m_multi_id)
		{
			return;
		}
		_Rebirth_Time_Interval += Time.deltaTime;
		tar_player.OnRebirthStay(_Rebirth_Time_Interval);
		if (_Rebirth_Time_Interval > _Rebirth_Time && component2.m_player.GetType() == typeof(Multiplayer))
		{
			_Rebirth_Time_Interval = 0f;
			_Is_Rebirthing = false;
			if (component.m_player.m_multi_id == GameApp.GetInstance().GetGameScene().GetPlayer()
				.m_multi_id && GameApp.GetInstance().GetGameState().Medpack > 0)
			{
				Packet packet = CGUserDoRebirthPacket.MakePacket((uint)component.m_player.m_multi_id, (uint)tar_player.m_multi_id);
				GameApp.GetInstance().GetGameState().net_com.Send(packet);
				Debug.Log("CGUserDoRebirthPacket id1:" + component.m_player.m_multi_id + " id2:" + tar_player.m_multi_id);
				GameApp.GetInstance().GetGameState().Medpack--;
				GameUIScriptNew.GetGameUIScript().playerInfo.SetMedpackCount();
			}
			tar_player.OnRebirthFinish();
			tar_player = null;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 27 && _Is_Rebirthing)
		{
			PlayerShell component = other.gameObject.GetComponent<PlayerShell>();
			if (component != null && tar_player.m_multi_id == component.m_player.m_multi_id)
			{
				_Rebirth_Time_Interval = 0f;
				_Is_Rebirthing = false;
				tar_player.OnRebirthExit();
				tar_player = null;
			}
		}
	}

	public void CancelRebirth()
	{
		if (tar_player != null)
		{
			_Rebirth_Time_Interval = 0f;
			_Is_Rebirthing = false;
			tar_player.OnRebirthExit();
			tar_player = null;
		}
	}
}
