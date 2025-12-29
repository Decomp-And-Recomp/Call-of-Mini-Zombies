using UnityEngine;

public class CGEnemyLootPacket : BinaryPacket
{
	public static Packet MakePacket(uint item_type, Vector3 position)
	{
		uint iBodylen = 16u;
		Packet packet = BinaryPacket.MakePacket(65545u, iBodylen, 1u);
		packet.PushUInt32(item_type);
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
