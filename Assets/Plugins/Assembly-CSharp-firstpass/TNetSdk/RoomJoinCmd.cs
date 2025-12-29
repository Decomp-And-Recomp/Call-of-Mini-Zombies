using System.Text;

namespace TNetSdk
{
	public class RoomJoinCmd : RoomCmd
	{
		public RoomJoinCmd(ushort room_id, string pwd)
		{
			PushUInt16(room_id);
			if (pwd == null || pwd.Length == 0)
			{
				PushUInt16(0);
				return;
			}
			byte[] bytes = Encoding.ASCII.GetBytes(pwd);
			PushUInt16((ushort)bytes.Length);
			PushByteArray(bytes, bytes.Length);
		}

		public Packet MakePacket()
		{
			return MakePacket(CMD.room_join);
		}
	}
}
