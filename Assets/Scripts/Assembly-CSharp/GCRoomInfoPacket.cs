using System.Text;
using UnityEngine;

public class GCRoomInfoPacket : BinaryPacket
{
	public uint m_iResult;

	public GCRoomListPacket.RoomInfo m_roomInfo = new GCRoomListPacket.RoomInfo();

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
		uint val = 0u;
		uint val2 = 0u;
		if (!packet.PopUInt32(ref m_roomInfo.m_iRoomId))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_roomInfo.m_iMapId))
		{
			return false;
		}
		if (!packet.PopUInt32(ref val))
		{
			return false;
		}
		if (!packet.CheckBytesLeft(16))
		{
			return false;
		}
		Debug.Log(val);
		m_roomInfo.m_strCreaterNickname = Encoding.ASCII.GetString(packet.ByteArray(), packet.Position, (int)val);
		packet.Position += 16;
		if (!packet.PopUInt32(ref m_roomInfo.m_iOnlineNum))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_roomInfo.m_iMaxUserNum))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_roomInfo.m_room_status))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_roomInfo.m_Creater_level))
		{
			return false;
		}
		if (!packet.PopUInt32(ref val2))
		{
			return false;
		}
		if (!packet.CheckBytesLeft(7))
		{
			return false;
		}
		m_roomInfo.m_password = Encoding.ASCII.GetString(packet.ByteArray(), packet.Position, (int)val2);
		packet.Position += 7;
		return true;
	}
}
