public class GCLeaveRoomPacket : BinaryPacket
{
	public uint m_iResult;

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
		return true;
	}
}
