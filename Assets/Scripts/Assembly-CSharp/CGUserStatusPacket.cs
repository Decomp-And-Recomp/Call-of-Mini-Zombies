using UnityEngine;

public class CGUserStatusPacket : BinaryPacket
{
	public static Packet MakePacket(uint iUserId, Vector3 position, Vector3 rotation, Vector3 scale, ulong lLocalPingTime)
	{
		uint iBodylen = 48u;
		Packet packet = BinaryPacket.MakePacket(65538u, iBodylen, 1u);
		packet.PushUInt32(iUserId);
		int num = 0;
		num = (int)(position.x * 100f);
		packet.PushUInt32((uint)num);
		num = (int)(position.y * 100f);
		packet.PushUInt32((uint)num);
		num = (int)(position.z * 100f);
		packet.PushUInt32((uint)num);
		num = (int)(rotation.x * 100f);
		packet.PushUInt32((uint)num);
		num = (int)(rotation.y * 100f);
		packet.PushUInt32((uint)num);
		num = (int)(rotation.z * 100f);
		packet.PushUInt32((uint)num);
		num = (int)(scale.x * 100f);
		packet.PushUInt32((uint)num);
		num = (int)(scale.y * 100f);
		packet.PushUInt32((uint)num);
		num = (int)(scale.z * 100f);
		packet.PushUInt32((uint)num);
		packet.PushUInt64(lLocalPingTime);
		return packet;
	}
}
