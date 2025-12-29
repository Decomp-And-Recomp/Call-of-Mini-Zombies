using TNetSdk.BinaryProtocol;

namespace TNetSdk
{
	public class RoomCmd : Packer
	{
		public Packet MakePacket(CMD cmd)
		{
			return MakePacket(2, (ushort)cmd);
		}
	}
}
