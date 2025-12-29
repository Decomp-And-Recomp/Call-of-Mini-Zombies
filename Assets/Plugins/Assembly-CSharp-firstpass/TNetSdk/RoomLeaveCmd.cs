namespace TNetSdk
{
	public class RoomLeaveCmd : RoomCmd
	{
		public Packet MakePacket()
		{
			return MakePacket(CMD.room_leave);
		}
	}
}
