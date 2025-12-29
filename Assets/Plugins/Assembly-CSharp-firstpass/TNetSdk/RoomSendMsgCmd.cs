namespace TNetSdk
{
	public class RoomSendMsgCmd : RoomCmd
	{
		public RoomSendMsgCmd(ushort user_id, byte[] msg_bytes)
		{
			PushUInt16(user_id);
			PushUInt16((ushort)msg_bytes.Length);
			PushByteArray(msg_bytes, msg_bytes.Length);
		}

		public Packet MakePacket()
		{
			return MakePacket(CMD.room_send_msg);
		}
	}
}
