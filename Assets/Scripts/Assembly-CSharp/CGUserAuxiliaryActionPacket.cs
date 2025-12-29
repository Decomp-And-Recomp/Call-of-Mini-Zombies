public class CGUserAuxiliaryActionPacket : BinaryPacket
{
	public static Packet MakePacket(uint user_id, uint bonusAction)
	{
		uint iBodylen = 8u;
		Packet packet = BinaryPacket.MakePacket(65553u, iBodylen, 1u);
		packet.PushUInt32(user_id);
		packet.PushUInt32(bonusAction);
		return packet;
	}
}
