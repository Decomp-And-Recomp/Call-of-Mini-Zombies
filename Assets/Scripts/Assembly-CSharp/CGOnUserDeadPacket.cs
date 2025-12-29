public class CGOnUserDeadPacket : BinaryPacket
{
	public static Packet MakePacket()
	{
		uint iBodylen = 0u;
		return BinaryPacket.MakePacket(13u, iBodylen, 1u);
	}
}
