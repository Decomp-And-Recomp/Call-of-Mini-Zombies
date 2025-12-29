using System.Text;

namespace TNetSdk
{
	public class RoomLockReqCmd : RoomCmd
	{
		public RoomLockReqCmd(string key)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(key);
			PushUInt16((ushort)bytes.Length);
			PushByteArray(bytes, bytes.Length);
		}

		public Packet MakePacket()
		{
			return MakePacket(CMD.room_lock_req);
		}
	}
}
