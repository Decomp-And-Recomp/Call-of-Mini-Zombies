public class GCJoinRoomPacket : BinaryPacket
{
	public uint m_iResult;

	public uint m_iRoomId;

	public long m_lLocalTime;

	public long m_lServerTime;

	public uint m_iUserId;

	public uint m_room_index;

	public uint m_map_id;

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
		if (!packet.PopUInt32(ref m_iRoomId))
		{
			return false;
		}
		ulong val = 0uL;
		if (!packet.PopUInt64(ref val))
		{
			return false;
		}
		m_lLocalTime = (long)val;
		if (!packet.PopUInt64(ref val))
		{
			return false;
		}
		m_lServerTime = (long)val;
		if (!packet.PopUInt32(ref m_iUserId))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_room_index))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_map_id))
		{
			return false;
		}
		return true;
	}
}
