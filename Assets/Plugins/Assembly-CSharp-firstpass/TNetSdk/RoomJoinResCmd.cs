using System.Text;
using TNetSdk.BinaryProtocol;

namespace TNetSdk
{
	public class RoomJoinResCmd : UnPacker
	{
		public enum Result
		{
			ok = 0,
			full = 1,
			no_exist = 2,
			gaming = 3,
			pwd_error = 4
		}

		public Result m_result;

		public ushort m_sit_position;

		public RoomDragListResCmd.RoomInfo m_room_info;

		public override bool ParserPacket(Packet packet)
		{
			if (!base.ParserPacket(packet))
			{
				return false;
			}
			ushort val = 0;
			if (!PopUInt16(ref val))
			{
				return false;
			}
			m_result = (Result)val;
			if (!PopUInt16(ref m_sit_position))
			{
				return false;
			}
			m_room_info = new RoomDragListResCmd.RoomInfo();
			if (!PopUInt16(ref m_room_info.m_room_id))
			{
				return false;
			}
			if (!PopUInt16(ref m_room_info.m_group_id))
			{
				return false;
			}
			if (!PopUInt16(ref m_room_info.m_master_id))
			{
				return false;
			}
			if (!PopUInt16(ref m_room_info.m_online_user))
			{
				return false;
			}
			if (!PopUInt16(ref m_room_info.m_max_user))
			{
				return false;
			}
			if (!PopUInt16(ref m_room_info.m_state))
			{
				return false;
			}
			if (!PopUInt16(ref m_room_info.m_passworded))
			{
				return false;
			}
			if (!CheckBytesLeft(32))
			{
				return false;
			}
			m_room_info.m_creater_name = Encoding.ASCII.GetString(ByteArray(), base.Offset, 16);
			m_room_info.m_creater_name = m_room_info.m_creater_name.Substring(0, m_room_info.m_creater_name.IndexOf('\0'));
			base.Offset += 16;
			m_room_info.m_room_name = Encoding.ASCII.GetString(ByteArray(), base.Offset, 16);
			m_room_info.m_room_name = m_room_info.m_room_name.Substring(0, m_room_info.m_room_name.IndexOf('\0'));
			base.Offset += 16;
			m_room_info.m_comment = Encoding.ASCII.GetString(ByteArray(), base.Offset, 64);
			m_room_info.m_comment = m_room_info.m_comment.Substring(0, m_room_info.m_comment.IndexOf('\0'));
			base.Offset += 64;
			return true;
		}

		public override void ToTNetEventData(Packet packet, ref TNetEventData event_data, TNetObject target)
		{
			ParserPacket(packet);
			event_data.data.Add("result", m_result);
			if (m_result == Result.ok)
			{
				target.CurRoom = TNetRoom.FromRoomInfo(m_room_info);
				target.CurRoom.AddUser(target.Myself);
				target.CurRoom.Joined();
				target.Myself.SetIndex(m_sit_position);
				if (target.CurRoom.RoomMasterID == target.Myself.Id)
				{
					target.CurRoom.RoomMaster = target.Myself;
				}
				event_data.data.Add("room", target.CurRoom);
			}
		}
	}
}
