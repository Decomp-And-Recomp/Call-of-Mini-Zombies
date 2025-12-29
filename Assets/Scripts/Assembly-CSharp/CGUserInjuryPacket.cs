public class CGUserInjuryPacket : BinaryPacket
{
	public static Packet MakePacket(uint user_id, long hit_val, long total_val, long cur_val)
	{
		uint iBodylen = 28u;
		Packet packet = BinaryPacket.MakePacket(65546u, iBodylen, 1u);
		packet.PushUInt32(user_id);
		packet.PushUInt64((ulong)hit_val);
		packet.PushUInt64((ulong)total_val);
		packet.PushUInt64((ulong)cur_val);
		return packet;
	}
}
