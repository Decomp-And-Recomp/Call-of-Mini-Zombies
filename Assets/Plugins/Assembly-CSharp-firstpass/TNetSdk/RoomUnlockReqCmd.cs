using System.Text;

namespace TNetSdk
{
	public class RoomUnlockReqCmd : RoomCmd
	{
		public RoomUnlockReqCmd(string key)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(key);
			PushUInt16((ushort)bytes.Length);
			PushByteArray(bytes, bytes.Length);
		}

		public Packet MakePacket()
		{
			return MakePacket(CMD.room_unlock_req);
		}
	}
}
