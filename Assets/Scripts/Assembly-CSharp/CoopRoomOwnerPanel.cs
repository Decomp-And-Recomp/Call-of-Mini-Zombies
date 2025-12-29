using UnityEngine;
using Zombie3D;

public class CoopRoomOwnerPanel : MonoBehaviour
{
	public RoomCellData[] Client_Arr;

	private int master_id;

	private NetworkObj net_com;

	public int roomUserCount;

	private void Awake()
	{
		if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Coop)
		{
			base.enabled = false;
		}
		else
		{
			net_com = GameApp.GetInstance().GetGameState().net_com;
		}
	}

	private void ClearClientsData()
	{
		RoomCellData[] client_Arr = Client_Arr;
		foreach (RoomCellData roomCellData in client_Arr)
		{
			if (roomCellData != null)
			{
				roomCellData.net_user = null;
				roomCellData.gameObject.transform.localPosition = new Vector3(0f, 1000f, 0f);
				roomCellData.logo.frameName_Accessor = string.Empty;
				roomCellData.level.text_Accessor = string.Empty;
				roomCellData.nickName.text_Accessor = string.Empty;
				roomCellData.kickButton.SetActive(false);
			}
		}
	}

	private bool SetClient(int index, NetUserInfo client)
	{
		if (index >= 4)
		{
			Debug.LogError("index out of rang!");
			return false;
		}
		RoomCellData roomCellData = Client_Arr[index];
		if (roomCellData == null || client == null)
		{
			Debug.Log("error!:" + index);
			return false;
		}
		roomCellData.net_user = client;
		if (client.user_id == net_com.m_netUserInfo.user_id)
		{
			roomCellData.logo.frameName_Accessor = "Avatar_" + (int)GameApp.GetInstance().GetGameState().Avatar;
			roomCellData.level.text_Accessor = "Lv: " + GameApp.GetInstance().GetGameState().LevelNum;
			roomCellData.nickName.text_Accessor = GameApp.GetInstance().GetGameState().nick_name;
		}
		else
		{
			roomCellData.logo.frameName_Accessor = "Avatar_" + client.avatarType;
			roomCellData.level.text_Accessor = "Lv: " + client.levelDays;
			roomCellData.nickName.text_Accessor = client.nick_name;
		}
		if (net_com.m_netUserInfo.is_master)
		{
			if (index == 0)
			{
				roomCellData.kickButton.SetActive(false);
			}
			else
			{
				roomCellData.kickButton.SetActive(true);
			}
		}
		else
		{
			roomCellData.kickButton.SetActive(false);
		}
		roomUserCount = index + 1;
		return true;
	}

	private NetUserInfo FindRoomMaster()
	{
		for (int i = 0; i < 4; i++)
		{
			if (net_com.netUserInfo_array[i] != null && net_com.netUserInfo_array[i].is_master)
			{
				return net_com.netUserInfo_array[i];
			}
		}
		return null;
	}

	public void RefrashClientCellShow()
	{
		ClearClientsData();
		int num = 0;
		NetUserInfo netUserInfo = FindRoomMaster();
		if (netUserInfo == null)
		{
			return;
		}
		if (SetClient(num, netUserInfo))
		{
			Client_Arr[num].gameObject.transform.localPosition = new Vector3(0f, 45 - 40 * num, -1f);
			num++;
		}
		NetUserInfo[] netUserInfo_array = net_com.netUserInfo_array;
		foreach (NetUserInfo netUserInfo2 in netUserInfo_array)
		{
			if (netUserInfo2 != null && !netUserInfo2.is_master && SetClient(num, netUserInfo2))
			{
				Client_Arr[num].gameObject.transform.localPosition = new Vector3(0f, 45 - 40 * num, -1f);
				num++;
			}
		}
	}
}
