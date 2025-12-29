public class GCMultiplayerInjuryNotifyPacket : BinaryPacket
{
	public uint m_playerId;

	public long m_damage;

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_playerId))
		{
			return false;
		}
		ulong val = 0uL;
		if (!packet.PopUInt64(ref val))
		{
			return false;
		}
		m_damage = (long)val;
		return true;
	}
}
