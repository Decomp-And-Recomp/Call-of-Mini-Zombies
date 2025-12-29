public class CGMultiplayerInjuryPacket : BinaryPacket
{
	public static Packet MakePacket(uint player_id, long damage)
	{
		uint iBodylen = 12u;
		Packet packet = BinaryPacket.MakePacket(65555u, iBodylen, 1u);
		packet.PushUInt32(player_id);
		packet.PushUInt64((ulong)damage);
		return packet;
	}
}
