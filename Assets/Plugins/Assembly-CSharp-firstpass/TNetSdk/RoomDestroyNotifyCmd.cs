using TNetSdk.BinaryProtocol;

namespace TNetSdk
{
	public class RoomDestroyNotifyCmd : UnPacker
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
}
