public class GCPickItemNotifyPacket : BinaryPacket
{
	public uint id;

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		if (!packet.PopUInt32(ref id))
		{
			return false;
		}
		return true;
	}
}
