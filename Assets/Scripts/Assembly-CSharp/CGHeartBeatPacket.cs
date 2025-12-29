public class CGHeartBeatPacket : BinaryPacket
{
	public static Packet MakePacket(long lLocalTime)
	{
		uint iBodylen = 8u;
		Packet packet = BinaryPacket.MakePacket(1048576u, iBodylen, 1u);
		packet.PushUInt64((ulong)lLocalTime);
		return packet;
	}
}
