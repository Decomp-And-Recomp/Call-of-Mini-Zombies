public class GCDestroyRoomNotifyPacket : BinaryPacket
{
	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		return true;
	}
}
