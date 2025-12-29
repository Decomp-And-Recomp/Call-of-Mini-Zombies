namespace TNetSdk
{
	public class RoomSetUserVarCmd : RoomCmd
	{
		public RoomSetUserVarCmd(ushort key, byte[] msg_bytes)
		{
			PushUInt16(key);
			PushUInt16((ushort)msg_bytes.Length);
			PushByteArray(msg_bytes, msg_bytes.Length);
		}

		public Packet MakePacket()
		{
			return MakePacket(CMD.room_set_user_var);
		}
	}
}
