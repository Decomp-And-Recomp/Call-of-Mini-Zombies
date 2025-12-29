public class CGUserActionPacket : BinaryPacket
{
	public static Packet MakePacket(uint user_id, uint action)
	{
		uint iBodylen = 8u;
		Packet packet = BinaryPacket.MakePacket(65539u, iBodylen, 1u);
		packet.PushUInt32(user_id);
		packet.PushUInt32(action);
		return packet;
	}
}
