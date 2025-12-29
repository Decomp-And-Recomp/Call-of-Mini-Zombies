public class GCEnemyDeadPacket : BinaryPacket
{
	public uint m_iResult;

	public uint m_enemy_type;

	public uint bElite;

	public uint weapon_type;

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_iResult))
		{
			return false;
		}
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
