public class CGQuickRoomListPacket : BinaryPacket
{
	public static Packet MakePacket(uint iPage, uint map_id)
	{
		uint iBodylen = 8u;
		Packet packet = BinaryPacket.MakePacket(15u, iBodylen, 1u);
		packet.PushUInt32(iPage);
		packet.PushUInt32(map_id);
		return packet;
	}
}
