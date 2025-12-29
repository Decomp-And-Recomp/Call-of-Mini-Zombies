using TNetSdk.BinaryProtocol;

namespace TNetSdk
{
	public class SysCmd : Packer
	{
		public Packet MakePacket(CMD cmd)
		{
			return MakePacket(1, (ushort)cmd);
		}
	}
}
