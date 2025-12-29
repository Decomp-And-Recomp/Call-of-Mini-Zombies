using System.Text;

public class CGEnemyDeadPacket : BinaryPacket
{
	public static Packet MakePacket(uint iPlayerId, string enemy_id, uint enemy_type, uint bElite, uint weapon_type)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(enemy_id);
		uint iBodylen = (uint)(8 + bytes.Length + 4 + 8);
		Packet packet = BinaryPacket.MakePacket(10u, iBodylen, 1u);
		packet.PushUInt32(iPlayerId);
		packet.PushUInt32((uint)bytes.Length);
		packet.PushByteArray(bytes, bytes.Length);
		packet.PushUInt32(enemy_type);
		packet.PushUInt32(bElite);
		packet.PushUInt32(weapon_type);
		return packet;
	}
}
