using TNetSdk;
using UnityEngine;
using Zombie3D;

public class VSRoomOwnerPanel : MonoBehaviour
{
	public RoomCellData[] Client_Arr;

	private int master_id;

	private void Awake()
	{
		if (GameApp.GetInstance().GetGameState().gameMode != GameMode.Vs)
		{
			base.enabled = false;
		}
	}

	private void ClearClientsData()
	{
		RoomCellData[] client_Arr = Client_Arr;
		foreach (RoomCellData roomCellData in client_Arr)
		{
			if (roomCellData != null)
			{
				roomCellData.sfs_user = null;
				roomCellData.gameObject.transform.localPosition = new Vector3(0f, 1000f, 0f);
			}
		}
	}

	private bool SetClient(int index, TNetUser client)
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
		roomCellData.sfs_user = client;
		if (client.Id == TNetConnection.Connection.Myself.Id)
		{
			roomCellData.logo.frameName_Accessor = "Avatar_" + (int)GameApp.GetInstance().GetGameState().Avatar;
			roomCellData.level.text_Accessor = "Lv: " + GameApp.GetInstance().GetGameState().LevelNum;
			roomCellData.nickName.text_Accessor = GameApp.GetInstance().GetGameState().nick_name;
		}
		else if (client.ContainsVariable(TNetUserVarType.roomState))
		{
			SFSObject sFSObject = client.GetVariable(TNetUserVarType.roomState).GetSFSObject("data") as SFSObject;
			if (!sFSObject.GetBool("InRoom"))
			{
				roomCellData.sfs_user = null;
				return false;
			}
			roomCellData.logo.frameName_Accessor = "Avatar_" + sFSObject.GetInt("avatarType");
			roomCellData.level.text_Accessor = "Lv: " + sFSObject.GetInt("avatarLevel");
			roomCellData.nickName.text_Accessor = sFSObject.GetUtfString("nickname");
		}
		if (TNetConnection.is_server)
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
		return true;
	}

	private TNetUser FindRoomMaster()
	{
		master_id = TNetConnection.Connection.CurRoom.RoomMasterID;
		foreach (TNetUser user in TNetConnection.Connection.CurRoom.UserList)
		{
			if (user.Id == master_id)
			{
				return user;
			}
		}
		return null;
	}

	public void RefrashClientCellShow()
	{
		ClearClientsData();
		int num = 0;
		TNetUser tNetUser = FindRoomMaster();
		if (tNetUser == null)
		{
			return;
		}
		if (SetClient(num, tNetUser))
		{
			Client_Arr[num].gameObject.transform.localPosition = new Vector3(0f, 45 - 40 * num, -1f);
			num++;
		}
		foreach (TNetUser user in TNetConnection.Connection.CurRoom.UserList)
		{
			if (user.Id != tNetUser.Id && SetClient(num, user))
			{
				Client_Arr[num].gameObject.transform.localPosition = new Vector3(0f, 45 - 40 * num, -1f);
				num++;
			}
		}
	}
}
