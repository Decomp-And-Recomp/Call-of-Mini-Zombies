public class GCUserDoRebirthNotifyPacket : BinaryPacket
{
	public uint action_user_id;

	public uint rebirth_user_id;

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		if (!packet.PopUInt32(ref action_user_id))
		{
			return false;
		}
		if (!packet.PopUInt32(ref rebirth_user_id))
		{
			return false;
		}
		return true;
	}
}
