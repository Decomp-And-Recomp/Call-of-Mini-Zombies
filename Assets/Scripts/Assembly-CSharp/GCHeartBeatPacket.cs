public class GCHeartBeatPacket : BinaryPacket
{
	public long m_lLocalTime;

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		ulong val = 0uL;
		if (!packet.PopUInt64(ref val))
		{
			return false;
		}
		m_lLocalTime = (long)val;
		return true;
	}
}
