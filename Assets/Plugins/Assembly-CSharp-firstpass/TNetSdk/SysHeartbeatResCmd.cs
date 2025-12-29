using System;
using TNetSdk.BinaryProtocol;

namespace TNetSdk
{
	public class SysHeartbeatResCmd : UnPacker
	{
		public long m_server_time;

		public override bool ParserPacket(Packet packet)
		{
			if (!base.ParserPacket(packet))
			{
				return false;
			}
			ulong val = 0uL;
			if (!PopUInt64(ref val))
			{
				return false;
			}
			m_server_time = (long)val;
			return true;
		}

		public override void ToTNetEventData(Packet packet, ref TNetEventData event_data, TNetObject target)
		{
			ParserPacket(packet);
			if (target != null)
			{
				target.OnHeartBeatProess(Convert.ToDouble(m_server_time));
			}
			event_data.data.Add("serverTime", m_server_time);
		}
	}
}
