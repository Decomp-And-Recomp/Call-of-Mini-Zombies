public class CGStartGamePacket : BinaryPacket
{
	public static Packet MakePacket()
	{
		uint iBodylen = 0u;
		return BinaryPacket.MakePacket(8u, iBodylen, 1u);
	}
}
