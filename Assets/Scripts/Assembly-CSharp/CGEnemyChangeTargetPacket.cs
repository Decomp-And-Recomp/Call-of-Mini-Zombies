using System.Text;

public class CGEnemyChangeTargetPacket : BinaryPacket
{
	public static Packet MakePacket(string enemyID, uint target_id)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(enemyID);
		uint iBodylen = (uint)(4 + bytes.Length + 4);
		Packet packet = BinaryPacket.MakePacket(65547u, iBodylen, 1u);
		packet.PushUInt32((uint)bytes.Length);
		packet.PushByteArray(bytes, bytes.Length);
		packet.PushUInt32(target_id);
		return packet;
	}
}
