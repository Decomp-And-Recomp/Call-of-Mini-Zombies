public class GCCoopWinnerNotifyPacket : BinaryPacket
{
	public uint iWinnerId;

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		if (!packet.PopUInt32(ref iWinnerId))
		{
			return false;
		}
		return true;
	}
}
