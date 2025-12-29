using System.Collections.Generic;
using System.Text;

public class GCQuickRoomListPacket : BinaryPacket
{
	public uint m_iCurpage;

	public uint m_pagesum;

	public List<GCRoomListPacket.RoomInfo> m_vRoomList = new List<GCRoomListPacket.RoomInfo>();

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		uint val = 0u;
		if (!packet.PopUInt32(ref m_iCurpage))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_pagesum))
		{
			return false;
		}
		if (!packet.PopUInt32(ref val))
		{
			return false;
		}
		for (uint num = 0u; num < val; num++)
		{
			GCRoomListPacket.RoomInfo roomInfo = new GCRoomListPacket.RoomInfo();
			uint val2 = 0u;
			uint val3 = 0u;
			if (!packet.PopUInt32(ref roomInfo.m_iRoomId))
			{
				return false;
			}
			if (!packet.PopUInt32(ref roomInfo.m_iMapId))
			{
				return false;
			}
			if (!packet.PopUInt32(ref val2))
			{
				return false;
			}
			if (!packet.CheckBytesLeft(16))
			{
				return false;
			}
			roomInfo.m_strCreaterNickname = Encoding.ASCII.GetString(packet.ByteArray(), packet.Position, (int)val2);
			packet.Position += 16;
			if (!packet.PopUInt32(ref roomInfo.m_iOnlineNum))
			{
				return false;
			}
			if (!packet.PopUInt32(ref roomInfo.m_iMaxUserNum))
			{
				return false;
			}
			if (!packet.PopUInt32(ref roomInfo.m_room_status))
			{
				return false;
			}
			if (!packet.PopUInt32(ref roomInfo.m_Creater_level))
			{
				return false;
			}
			if (!packet.PopUInt32(ref val3))
			{
				return false;
			}
			if (!packet.CheckBytesLeft(7))
			{
				return false;
			}
			roomInfo.m_password = Encoding.ASCII.GetString(packet.ByteArray(), packet.Position, (int)val3);
			packet.Position += 7;
			m_vRoomList.Add(roomInfo);
		}
		return true;
	}
}
