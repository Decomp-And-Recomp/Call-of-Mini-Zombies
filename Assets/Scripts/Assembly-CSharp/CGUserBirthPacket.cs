public class CGUserBirthPacket : BinaryPacket
{
	public static Packet MakePacket(long lLocalTime, uint birthPointIndex, uint weapon1, uint weapon2, uint weapon3)
	{
		uint iBodylen = 24u;
		Packet packet = BinaryPacket.MakePacket(9u, iBodylen, 1u);
		packet.PushUInt64((ulong)lLocalTime);
		packet.PushUInt32(birthPointIndex);
		packet.PushUInt32(weapon1);
		packet.PushUInt32(weapon2);
		packet.PushUInt32(weapon3);
		return packet;
	}
}
