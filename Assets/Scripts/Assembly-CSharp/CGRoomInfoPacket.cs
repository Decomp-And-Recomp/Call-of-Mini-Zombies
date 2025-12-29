public class CGRoomInfoPacket : BinaryPacket
{
	public static Packet MakePacket(uint room_id)
	{
		uint iBodylen = 4u;
		Packet packet = BinaryPacket.MakePacket(16u, iBodylen, 1u);
		packet.PushUInt32(room_id);
		return packet;
	}
}
