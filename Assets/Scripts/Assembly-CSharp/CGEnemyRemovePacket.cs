using System.Text;

public class CGEnemyRemovePacket : BinaryPacket
{
	public static Packet MakePacket(string enemyID)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(enemyID);
		uint iBodylen = (uint)(4 + bytes.Length);
		Packet packet = BinaryPacket.MakePacket(65549u, iBodylen, 1u);
		packet.PushUInt32((uint)bytes.Length);
		packet.PushByteArray(bytes, bytes.Length);
		return packet;
	}
}
