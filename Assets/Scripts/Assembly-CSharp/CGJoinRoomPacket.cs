using System.Text;

public class CGJoinRoomPacket : BinaryPacket
{
	public static Packet MakePacket(uint iRoomId, long lLocalTime, string strNickname, uint avatarType, uint level)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(strNickname);
		uint iBodylen = (uint)(16 + bytes.Length + 4 + 4);
		Packet packet = BinaryPacket.MakePacket(5u, iBodylen, 1u);
		packet.PushUInt32(iRoomId);
		packet.PushUInt64((ulong)lLocalTime);
		packet.PushUInt32((uint)bytes.Length);
		packet.PushByteArray(bytes, bytes.Length);
		packet.PushUInt32(avatarType);
		packet.PushUInt32(level);
		return packet;
	}
}
