public class CGUserRebirthPacket : BinaryPacket
{
	public static Packet MakePacket(uint user_id)
	{
		uint iBodylen = 4u;
		Packet packet = BinaryPacket.MakePacket(65548u, iBodylen, 1u);
		packet.PushUInt32(user_id);
		return packet;
	}
}
