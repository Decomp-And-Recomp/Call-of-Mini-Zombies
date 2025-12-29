using System.Text;

public class GCJoinRoomNotifyPacket : BinaryPacket
{
	public uint m_iUserId;

	public string m_strNickname;

	public uint m_iAvatarType;

	public uint m_iLevel;

	public uint m_room_index;

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
		uint val = 0u;
		if (!packet.PopUInt32(ref val))
		{
			return false;
		}
		if (!packet.CheckBytesLeft((int)val))
		{
			return false;
		}
		m_strNickname = Encoding.ASCII.GetString(packet.ByteArray(), packet.Position, (int)val);
		packet.Position += (int)val;
		if (!packet.PopUInt32(ref m_iAvatarType))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_iLevel))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_room_index))
		{
			return false;
		}
		return true;
	}
}
