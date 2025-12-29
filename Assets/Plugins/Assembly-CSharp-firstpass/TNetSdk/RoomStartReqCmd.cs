namespace TNetSdk
{
	public class RoomStartReqCmd : RoomCmd
	{
		public Packet MakePacket()
		{
			return MakePacket(CMD.room_start);
		}
	}
}
