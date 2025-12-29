using System.Text;

namespace TNetSdk
{
	public class RoomCreateCmd : RoomCmd
	{
		public enum RoomType
		{
			open = 0,
			limit = 1
		}

		public enum RoomSwitchMasterType
		{
			None = 0,
			Auto = 1
		}

		public RoomCreateCmd(string room_name, string pwd, ushort group_id, ushort max_user, RoomType room_limit, RoomSwitchMasterType matertype, string parm)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(room_name);
			PushUInt16((ushort)bytes.Length);
			PushByteArray(bytes, bytes.Length);
			byte[] bytes2 = Encoding.ASCII.GetBytes(pwd);
			PushUInt16((ushort)bytes2.Length);
			PushByteArray(bytes2, bytes2.Length);
			PushUInt16(group_id);
			PushUInt16(max_user);
			PushUInt16((ushort)room_limit);
			PushUInt16((ushort)matertype);
			byte[] bytes3 = Encoding.ASCII.GetBytes(parm);
			PushUInt16((ushort)bytes3.Length);
			PushByteArray(bytes3, bytes3.Length);
		}

		public Packet MakePacket()
		{
			return MakePacket(CMD.sys_login);
		}
	}
}
