using System.Text;

public class GCEnemyGotHitNotifyPacket : BinaryPacket
{
	public string m_enemyID;

	public long m_iDamage;

	public uint m_weapon_type;

	public uint m_critical_attack;

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
		ulong val2 = 0uL;
		if (!packet.PopUInt64(ref val2))
		{
			return false;
		}
		m_iDamage = (long)val2;
		if (!packet.PopUInt32(ref m_weapon_type))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_critical_attack))
		{
			return false;
		}
		return true;
	}
}
