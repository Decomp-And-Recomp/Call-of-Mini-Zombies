public class CGUserDoRebirthPacket : BinaryPacket
{
	public static Packet MakePacket(uint action_user_id, uint rebirth_user_id)
	{
		uint iBodylen = 8u;
		Packet packet = BinaryPacket.MakePacket(12u, iBodylen, 1u);
		packet.PushUInt32(action_user_id);
		packet.PushUInt32(rebirth_user_id);
		return packet;
	}
}
