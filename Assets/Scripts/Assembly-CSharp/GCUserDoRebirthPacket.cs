public class GCUserDoRebirthPacket : BinaryPacket
{
	public uint m_iResult;

	public uint rebirth_user_id;

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
		if (!packet.PopUInt32(ref rebirth_user_id))
		{
			return false;
		}
		return true;
	}
}
