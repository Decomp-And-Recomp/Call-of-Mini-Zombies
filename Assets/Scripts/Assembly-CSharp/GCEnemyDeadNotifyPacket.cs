using System.Text;

public class GCEnemyDeadNotifyPacket : BinaryPacket
{
	public uint iPlayerId;

	public string enemy_id;

	public uint m_enemy_type;

	public uint bElite;

	public uint weapon_type;

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		if (!packet.PopUInt32(ref iPlayerId))
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
		enemy_id = Encoding.ASCII.GetString(packet.ByteArray(), packet.Position, (int)val);
		packet.Position += (int)val;
		if (!packet.PopUInt32(ref m_enemy_type))
		{
			return false;
		}
		if (!packet.PopUInt32(ref bElite))
		{
			return false;
		}
		if (!packet.PopUInt32(ref weapon_type))
		{
			return false;
		}
		return true;
	}
}
