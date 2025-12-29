using TNetSdk.BinaryProtocol;

namespace TNetSdk
{
	public class RoomStartNotifyCmd : UnPacker
	{
		public ushort m_user_id;

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
			return true;
		}

		public override void ToTNetEventData(Packet packet, ref TNetEventData event_data, TNetObject target)
		{
			ParserPacket(packet);
			if (target != null && target.CurRoom != null)
			{
				target.CurRoom.IsGaming = true;
			}
			event_data.data.Add("userId", m_user_id);
		}
	}
}
