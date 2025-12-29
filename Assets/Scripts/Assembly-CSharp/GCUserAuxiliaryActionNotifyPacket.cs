public class GCUserAuxiliaryActionNotifyPacket : BinaryPacket
{
	public uint m_iUserId;

	public uint m_iBonusAction;

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
		if (!packet.PopUInt32(ref m_iBonusAction))
		{
			return false;
		}
		return true;
	}
}
