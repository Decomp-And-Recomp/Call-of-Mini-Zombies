public class GCUserChangeWeaponNotifyPacket : BinaryPacket
{
	public uint m_iUserId;

	public uint m_iWeaponIndex;

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
		if (!packet.PopUInt32(ref m_iWeaponIndex))
		{
			return false;
		}
		return true;
	}
}
