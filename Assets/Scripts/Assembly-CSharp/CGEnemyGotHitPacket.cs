using System.Text;

public class CGEnemyGotHitPacket : BinaryPacket
{
	public static Packet MakePacket(string enemyID, long damage, uint weapon_type, uint criticalAttack)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(enemyID);
		uint iBodylen = (uint)(4 + bytes.Length + 8 + 4 + 4);
		Packet packet = BinaryPacket.MakePacket(65544u, iBodylen, 1u);
		packet.PushUInt32((uint)bytes.Length);
		packet.PushByteArray(bytes, bytes.Length);
		packet.PushUInt64((ulong)damage);
		packet.PushUInt32(weapon_type);
		packet.PushUInt32(criticalAttack);
		return packet;
	}
}
