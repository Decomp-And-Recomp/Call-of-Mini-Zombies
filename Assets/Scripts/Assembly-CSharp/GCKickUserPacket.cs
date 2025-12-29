public class GCKickUserPacket : BinaryPacket
{
	public uint m_iResult;

	public uint m_iUserId;

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_iResult))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_iUserId))
		{
			return false;
		}
		return true;
	}
}
