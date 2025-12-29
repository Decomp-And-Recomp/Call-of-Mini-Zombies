public class GCUserBirthNotifyPacket : BinaryPacket
{
	public uint m_iUserId;

	public uint m_iBirthPointIndex;

	public uint m_iWeaponIndex1;

	public uint m_iWeaponIndex2;

	public uint m_iWeaponIndex3;

	public long m_lLocalTime;

	public long m_lServerTime;

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_iUserId))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_iBirthPointIndex))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_iWeaponIndex1))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_iWeaponIndex2))
		{
			return false;
		}
		if (!packet.PopUInt32(ref m_iWeaponIndex3))
		{
			return false;
		}
		ulong val = 0uL;
		if (!packet.PopUInt64(ref val))
		{
			return false;
		}
		m_lLocalTime = (long)val;
		if (!packet.PopUInt64(ref val))
		{
			return false;
		}
		m_lServerTime = (long)val;
		return true;
	}
}
