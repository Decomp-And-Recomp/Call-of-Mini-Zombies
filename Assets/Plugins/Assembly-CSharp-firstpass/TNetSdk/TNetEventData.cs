using System.Collections;
using TNetSdk.BinaryProtocol;

namespace TNetSdk
{
	public class TNetEventData
	{
		public Hashtable data = new Hashtable();

		public static TNetEventData CreateDataWithPacket(TNetEventSystem type, Packet packet, TNetObject target)
		{
			TNetEventData result = new TNetEventData();
			UnPacker unPacker = null;
			switch (type)
			{
			case TNetEventSystem.HEARTBEAT:
				unPacker = new SysHeartbeatResCmd();
				break;
			case TNetEventSystem.LOGIN:
				unPacker = new SysLoginResCmd();
				break;
			}
			if (unPacker != null)
			{
				unPacker.ToTNetEventData(packet, ref result, target);
			}
			return result;
		}

		public static TNetEventData CreateDataWithPacket(TNetEventRoom type, Packet packet, TNetObject target)
		{
			TNetEventData result = new TNetEventData();
			UnPacker unPacker = null;
			switch (type)
			{
			case TNetEventRoom.GET_ROOM_LIST:
				unPacker = new RoomDragListResCmd();
				break;
			case TNetEventRoom.ROOM_CREATION:
				unPacker = new RoomCreateResCmd();
				break;
			case TNetEventRoom.ROOM_JOIN:
				unPacker = new RoomJoinResCmd();
				break;
			case TNetEventRoom.USER_ENTER_ROOM:
				unPacker = new RoomJoinNotifyCmd();
				break;
			case TNetEventRoom.USER_EXIT_ROOM:
				unPacker = new RoomLeaveNotifyCmd();
				break;
			case TNetEventRoom.USER_BE_KICKED:
				unPacker = new RoomKickUserNotifyCmd();
				break;
			case TNetEventRoom.ROOM_NAME_CHANGE:
				unPacker = new RoomRenameNotifyCmd();
				break;
			case TNetEventRoom.ROOM_VARIABLES_UPDATE:
				unPacker = new RoomVarNotifyCmd();
				break;
			case TNetEventRoom.USER_VARIABLES_UPDATE:
				unPacker = new RoomUserVarNotifyCmd();
				break;
			case TNetEventRoom.USER_STATE:
				unPacker = new RoomUserStatusNotifyCmd();
				break;
			case TNetEventRoom.OBJECT_MESSAGE:
				unPacker = new RoomMsgNotifyCmd();
				break;
			case TNetEventRoom.LOCK_STH:
				unPacker = new RoomLockResCmd();
				break;
			case TNetEventRoom.UNLOCK_STH:
				unPacker = new RoomUnlockResCmd();
				break;
			case TNetEventRoom.ROOM_START:
				unPacker = new RoomStartNotifyCmd();
				break;
			case TNetEventRoom.ROOM_MASTER_CHANGE:
				unPacker = new RoomCreaterChangeNotifyCmd();
				break;
			case TNetEventRoom.ROOM_REMOVE:
				unPacker = new RoomDestroyNotifyCmd();
				break;
			case TNetEventRoom.ROOM_REMOVE_RES:
				unPacker = new RoomDestroyResCmd();
				break;
			case TNetEventRoom.ROOM_COMMENT_CHANGE:
				unPacker = new RoomCommentChangeNotifyCmd();
				break;
			}
			if (unPacker != null)
			{
				unPacker.ToTNetEventData(packet, ref result, target);
			}
			return result;
		}
	}
}
