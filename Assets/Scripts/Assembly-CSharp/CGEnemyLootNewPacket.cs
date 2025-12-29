using UnityEngine;

public class CGEnemyLootNewPacket : BinaryPacket
{
	public static Packet MakePacket(uint item_type, uint id, Vector3 position)
	{
		uint iBodylen = 20u;
		Packet packet = BinaryPacket.MakePacket(65554u, iBodylen, 1u);
		packet.PushUInt32(item_type);
		packet.PushUInt32(id);
		int num = 0;
		num = (int)(position.x * 100f);
		packet.PushUInt32((uint)num);
		num = (int)(position.y * 100f);
		packet.PushUInt32((uint)num);
		num = (int)(position.z * 100f);
		packet.PushUInt32((uint)num);
		return packet;
	}
}
