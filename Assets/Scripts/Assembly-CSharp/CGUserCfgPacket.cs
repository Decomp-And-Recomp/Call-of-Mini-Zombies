public class CGUserCfgPacket : BinaryPacket
{
	public static Packet MakePacket()
	{
		uint iBodylen = 0u;
		return BinaryPacket.MakePacket(65537u, iBodylen, 1u);
	}
}
