using System.Text;
using TNetSdk.BinaryProtocol;

namespace TNetSdk
{
	public class RoomJoinNotifyCmd : UnPacker
	{
		public ushort m_user_id;

		public string m_nickname;

		public ushort m_sit_position;

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
			m_nickname = Encoding.ASCII.GetString(ByteArray(), base.Offset, val);
			base.Offset += val;
			if (!PopUInt16(ref m_sit_position))
			{
				return false;
			}
			return true;
		}

		public override void ToTNetEventData(Packet packet, ref TNetEventData event_data, TNetObject target)
		{
			ParserPacket(packet);
			TNetUser tNetUser = new TNetUser(m_user_id, m_nickname);
			tNetUser.SetIndex(m_sit_position);
			if (target != null && target.Myself != null && m_user_id != target.Myself.Id && target.CurRoom != null)
			{
				target.CurRoom.AddUser(tNetUser);
				if (target.CurRoom.RoomMasterID == m_user_id)
				{
					target.CurRoom.RoomMaster = tNetUser;
				}
			}
			event_data.data.Add("user", tNetUser);
		}
	}
}
