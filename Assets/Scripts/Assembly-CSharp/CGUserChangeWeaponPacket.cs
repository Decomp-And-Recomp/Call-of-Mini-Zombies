public class CGUserChangeWeaponPacket : BinaryPacket
{
	public static Packet MakePacket(uint user_id, uint weapon_index)
	{
		uint iBodylen = 8u;
		Packet packet = BinaryPacket.MakePacket(65540u, iBodylen, 1u);
		packet.PushUInt32(user_id);
		packet.PushUInt32(weapon_index);
		return packet;
	}
}
