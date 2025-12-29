public class CGCoopWinnerPacket : BinaryPacket
{
	public static Packet MakePacket(uint iWinnerId)
	{
		uint iBodylen = 4u;
		Packet packet = BinaryPacket.MakePacket(65550u, iBodylen, 1u);
		packet.PushUInt32(iWinnerId);
		return packet;
	}
}
