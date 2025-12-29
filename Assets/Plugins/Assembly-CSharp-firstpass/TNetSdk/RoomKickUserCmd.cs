namespace TNetSdk
{
	public class RoomKickUserCmd : RoomCmd
	{
		public RoomKickUserCmd(ushort user_id)
		{
			PushUInt16(user_id);
		}

		public Packet MakePacket()
		{
			return MakePacket(CMD.room_kick_user);
		}
	}
}
