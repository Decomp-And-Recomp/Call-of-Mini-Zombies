public class CGLeaveRoomPacket : BinaryPacket
{
	public static Packet MakePacket()
	{
		uint iBodylen = 0u;
		return BinaryPacket.MakePacket(6u, iBodylen, 1u);
	}
}
