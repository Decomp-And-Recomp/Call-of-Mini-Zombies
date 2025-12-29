using System.Text;

public class CGCreateRoomPacket : BinaryPacket
{
	public static Packet MakePacket(uint iMapId, long lLocalTime, string strNickname, uint avatarType, uint level, string password)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(strNickname);
		byte[] bytes2 = Encoding.ASCII.GetBytes(password);
		uint iBodylen = (uint)(16 + bytes.Length + 4 + 4 + 4 + bytes2.Length);
		Packet packet = BinaryPacket.MakePacket(3u, iBodylen, 1u);
		packet.PushUInt32(iMapId);
		packet.PushUInt64((ulong)lLocalTime);
		packet.PushUInt32((uint)bytes.Length);
		packet.PushByteArray(bytes, bytes.Length);
		packet.PushUInt32(avatarType);
		packet.PushUInt32(level);
		packet.PushUInt32((uint)bytes2.Length);
		packet.PushByteArray(bytes2, bytes2.Length);
		return packet;
	}
}
