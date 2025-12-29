using TNetSdk.BinaryProtocol;

namespace TNetSdk
{
	public class RoomDestroyResCmd : UnPacker
	{
		public enum Result
		{
			ok = 0
		}

		public Result m_result;

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
			return true;
		}

		public override void ToTNetEventData(Packet packet, ref TNetEventData event_data, TNetObject target)
		{
			ParserPacket(packet);
			event_data.data.Add("result", m_result);
		}
	}
}
