public class GCUserInjuryNotifyPacket : BinaryPacket
{
	public uint m_iUserId;

	public long m_iInjury_val;

	public long m_total_hp_val;

	public long m_cur_hp_val;

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_iUserId))
		{
			return false;
		}
		ulong val = 0uL;
		if (!packet.PopUInt64(ref val))
		{
			return false;
		}
		m_iInjury_val = (long)val;
		if (!packet.PopUInt64(ref val))
		{
			return false;
		}
		m_total_hp_val = (long)val;
		if (!packet.PopUInt64(ref val))
		{
			return false;
		}
		m_cur_hp_val = (long)val;
		return true;
	}
}
