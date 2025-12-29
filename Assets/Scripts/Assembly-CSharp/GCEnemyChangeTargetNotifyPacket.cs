using System.Text;

public class GCEnemyChangeTargetNotifyPacket : BinaryPacket
{
	public string m_enemyID;

	public uint target_id;

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		uint val = 0u;
		if (!packet.PopUInt32(ref val))
		{
			return false;
		}
		if (!packet.CheckBytesLeft((int)val))
		{
			return false;
		}
		m_enemyID = Encoding.ASCII.GetString(packet.ByteArray(), packet.Position, (int)val);
		packet.Position += (int)val;
		if (!packet.PopUInt32(ref target_id))
		{
			return false;
		}
		return true;
	}
}
