using System.Text;
using UnityEngine;

public class CGEnemyStatusPacket : BinaryPacket
{
	public static Packet MakePacket(string enemyID, Vector3 position, Vector3 rotation, Vector3 dir)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(enemyID);
		uint iBodylen = (uint)(4 + bytes.Length + 36);
		Packet packet = BinaryPacket.MakePacket(65543u, iBodylen, 1u);
		packet.PushUInt32((uint)bytes.Length);
		packet.PushByteArray(bytes, bytes.Length);
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
		num = (int)(dir.x * 100f);
		packet.PushUInt32((uint)num);
		num = (int)(dir.y * 100f);
		packet.PushUInt32((uint)num);
		num = (int)(dir.z * 100f);
		packet.PushUInt32((uint)num);
		return packet;
	}
}
