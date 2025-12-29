public class CGDestroyRoomPacket : BinaryPacket
{
	public static Packet MakePacket()
	{
		uint iBodylen = 0u;
		return BinaryPacket.MakePacket(4u, iBodylen, 1u);
	}
}
