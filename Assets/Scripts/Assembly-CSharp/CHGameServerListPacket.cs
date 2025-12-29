public class CHGameServerListPacket : BinaryPacket
{
	public static Packet MakePacket()
	{
		return BinaryPacket.MakePacket(1u, 0u, 1u);
	}
}
