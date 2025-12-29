public class BinaryPacket
{
	public uint m_iBodyLen;

	public uint m_iPacketType;

	public uint m_iVersion;

	public static Packet MakePacket(uint iPacketType, uint iBodylen, uint iVersion)
	{
		Packet packet = new Packet((int)(12 + iBodylen));
		packet.PushUInt32(12 + iBodylen);
		packet.PushUInt32(iPacketType);
		packet.PushUInt32(iVersion);
		return packet;
	}

	public virtual bool ParserPacket(Packet packet)
	{
		if (!packet.CheckBytesLeft(12))
		{
			return false;
		}
		packet.PopUInt32(ref m_iBodyLen);
		m_iBodyLen -= 12u;
		packet.PopUInt32(ref m_iPacketType);
		packet.PopUInt32(ref m_iVersion);
		return true;
	}
}
