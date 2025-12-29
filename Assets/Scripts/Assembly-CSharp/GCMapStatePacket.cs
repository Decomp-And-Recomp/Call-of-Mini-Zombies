using System.Collections;

public class GCMapStatePacket : BinaryPacket
{
	public uint maps_count;

	public Hashtable MapState = new Hashtable();

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		if (!packet.PopUInt32(ref maps_count))
		{
			return false;
		}
		uint val = 0u;
		uint val2 = 0u;
		for (uint num = 0u; num < maps_count; num++)
		{
			if (!packet.PopUInt32(ref val))
			{
				return false;
			}
			if (!packet.PopUInt32(ref val2))
			{
				return false;
			}
			MapState[(int)val] = (int)val2;
		}
		return true;
	}
}
