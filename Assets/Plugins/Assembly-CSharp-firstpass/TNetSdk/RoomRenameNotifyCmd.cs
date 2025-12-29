using System.Text;
using TNetSdk.BinaryProtocol;

namespace TNetSdk
{
	public class RoomRenameNotifyCmd : UnPacker
	{
		public ushort m_user_id;

		public string m_room_name;

		public override bool ParserPacket(Packet packet)
		{
			if (!base.ParserPacket(packet))
			{
				return false;
			}
			if (!PopUInt16(ref m_user_id))
			{
				return false;
			}
			ushort val = 0;
			if (!PopUInt16(ref val))
			{
				return false;
			}
			if (!CheckBytesLeft(val))
			{
				return false;
			}
			m_room_name = Encoding.ASCII.GetString(ByteArray(), base.Offset, val);
			base.Offset += val;
			return true;
		}

		public override void ToTNetEventData(Packet packet, ref TNetEventData event_data, TNetObject target)
		{
			ParserPacket(packet);
			if (target != null && target.CurRoom != null)
			{
				target.CurRoom.Name = m_room_name;
			}
			event_data.data.Add("userId", m_user_id);
			event_data.data.Add("roomName", m_room_name);
		}
	}
}
