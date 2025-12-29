using UnityEngine;

public class CGUserSniperFirePacket : BinaryPacket
{
	public static Packet MakePacket(uint user_id, Vector3 position)
	{
		uint iBodylen = 16u;
		Packet packet = BinaryPacket.MakePacket(65541u, iBodylen, 1u);
		packet.PushUInt32(user_id);
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
