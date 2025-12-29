namespace TNetSdk
{
	public class RoomDestroyCmd : RoomCmd
	{
		public Packet MakePacket()
		{
			return MakePacket(CMD.sys_logout);
		}
	}
}
