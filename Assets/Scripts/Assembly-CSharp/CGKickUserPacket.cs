public class CGKickUserPacket : BinaryPacket
{
	public static Packet MakePacket(uint iUserId)
	{
		uint iBodylen = 4u;
		Packet packet = BinaryPacket.MakePacket(7u, iBodylen, 1u);
		packet.PushUInt32(iUserId);
		return packet;
	}
}
