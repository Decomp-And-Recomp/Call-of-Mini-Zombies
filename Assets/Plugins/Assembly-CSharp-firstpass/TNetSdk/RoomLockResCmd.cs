using System.Text;
using TNetSdk.BinaryProtocol;

namespace TNetSdk
{
	public class RoomLockResCmd : UnPacker
	{
		public enum Result
		{
			ok = 0,
			error = 1
		}

		public Result m_result;

		public string m_key;

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
			ushort val2 = 0;
			if (!PopUInt16(ref val2))
			{
				return false;
			}
			if (!CheckBytesLeft(val2))
			{
				return false;
			}
			m_key = Encoding.ASCII.GetString(ByteArray(), base.Offset, val2);
			base.Offset += val2;
			return true;
		}

		public override void ToTNetEventData(Packet packet, ref TNetEventData event_data, TNetObject target)
		{
			ParserPacket(packet);
			event_data.data.Add("result", m_result);
			event_data.data.Add("key", m_key);
		}
	}
}
