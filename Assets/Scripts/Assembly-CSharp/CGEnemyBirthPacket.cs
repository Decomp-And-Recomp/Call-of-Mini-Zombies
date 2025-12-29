using UnityEngine;

public class CGEnemyBirthPacket : BinaryPacket
{
	public static Packet MakePacket(uint referenced_day, uint enemy_id, uint enemy_type, uint isElite, uint isGrave, uint isSuperBoss, Vector3 position, uint target_id)
	{
		uint iBodylen = 40u;
		Packet packet = BinaryPacket.MakePacket(65542u, iBodylen, 1u);
		packet.PushUInt32(referenced_day);
		packet.PushUInt32(enemy_id);
		packet.PushUInt32(enemy_type);
		packet.PushUInt32(isElite);
		packet.PushUInt32(isGrave);
		packet.PushUInt32(isSuperBoss);
		int num = 0;
		num = (int)(position.x * 100f);
		packet.PushUInt32((uint)num);
		num = (int)(position.y * 100f);
		packet.PushUInt32((uint)num);
		num = (int)(position.z * 100f);
		packet.PushUInt32((uint)num);
		packet.PushUInt32(target_id);
		return packet;
	}
}
