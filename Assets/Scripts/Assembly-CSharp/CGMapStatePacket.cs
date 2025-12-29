public class CGMapStatePacket : BinaryPacket
{
	public static Packet MakePacket()
	{
		uint iBodylen = 0u;
		return BinaryPacket.MakePacket(14u, iBodylen, 1u);
	}
}
