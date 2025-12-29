using TNetSdk.BinaryProtocol;

namespace TNetSdk
{
	public class RoomCreateResCmd : UnPacker
	{
		public enum Result
		{
			ok = 0,
			full = 1
		}

		public Result m_result;

		public ushort m_room_id;

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
			if (!PopUInt16(ref m_room_id))
			{
				return false;
			}
			return true;
		}

		public override void ToTNetEventData(Packet packet, ref TNetEventData event_data, TNetObject target)
		{
			ParserPacket(packet);
			event_data.data.Add("result", m_result);
			event_data.data.Add("roomId", m_room_id);
		}
	}
}
