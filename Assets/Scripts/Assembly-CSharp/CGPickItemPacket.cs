public class CGPickItemPacket : BinaryPacket
{
	public static Packet MakePacket(uint id)
	{
		uint iBodylen = 4u;
		Packet packet = BinaryPacket.MakePacket(65552u, iBodylen, 1u);
		packet.PushUInt32(id);
		return packet;
	}
}
