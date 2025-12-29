using UnityEngine;
using Zombie3D;

public class PlayerMark : MonoBehaviour
{
	public Player m_player;

	protected float ori_x;

	protected float ori_y;

	protected float tar_x;

	protected float tar_y;

	protected string ori_frame = string.Empty;

	protected Vector3 tem_pos;

	protected Vector3 tem_rot;

	protected Quaternion q;

	protected Vector2 tem;

	private void Start()
	{
		ori_frame = GetComponent<TUIMeshSprite>().frameName_Accessor;
		base.transform.localPosition = new Vector3(-5000f, 0f, base.transform.localPosition.z);
	}

	private void Update()
	{
		if (m_player == null || !GameUIScriptNew.GetGameUIScript().uiInited)
		{
			return;
		}
		Player player = GameApp.GetInstance().GetGameScene().GetPlayer();
		if (m_player.m_multi_id == player.m_multi_id)
		{
			base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		}
		else
		{
			tem_pos = player.GetTransform().InverseTransformPoint(m_player.GetTransform().position);
			tem = new Vector2(tem_pos.x, tem_pos.z);
			tem_rot = player.GetTransform().InverseTransformDirection(m_player.GetTransform().forward);
			q = Quaternion.FromToRotation(Vector3.forward, tem_rot);
			if (tem.sqrMagnitude > 1444f)
			{
				tem_pos = tem_pos.normalized * 38f;
			}
			base.transform.localPosition = new Vector3(tem_pos.x, tem_pos.z, base.transform.localPosition.z);
			base.transform.localEulerAngles = new Vector3(0f, 0f, 0f - q.eulerAngles.y);
		}
		if (m_player.GetPlayerState() != null && m_player.GetPlayerState().GetStateType() == PlayerStateType.Dead)
		{
			base.gameObject.GetComponent<TUIMeshSprite>().frameName_Accessor = "PlayerMarkDead";
			base.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		}
		else
		{
			base.gameObject.GetComponent<TUIMeshSprite>().frameName_Accessor = ori_frame;
		}
	}

	public void SetPlayer(Player mPlayer)
	{
		if (mPlayer != null)
		{
			m_player = mPlayer;
		}
	}

	public void RemoveMark()
	{
		m_player = null;
		base.transform.localPosition = new Vector3(-5000f, 0f, base.transform.localPosition.z);
	}
}
