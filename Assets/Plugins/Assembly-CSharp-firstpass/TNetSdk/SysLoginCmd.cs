using System.Text;

namespace TNetSdk
{
	public class SysLoginCmd : SysCmd
	{
		public SysLoginCmd(string account, string pwd, string nickname)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(account);
			PushUInt16((ushort)bytes.Length);
			PushByteArray(bytes, bytes.Length);
			byte[] bytes2 = Encoding.ASCII.GetBytes(pwd);
			PushUInt16((ushort)bytes2.Length);
			PushByteArray(bytes2, bytes2.Length);
			byte[] bytes3 = Encoding.ASCII.GetBytes(nickname);
			PushUInt16((ushort)bytes3.Length);
			PushByteArray(bytes3, bytes3.Length);
		}

		public Packet MakePacket()
		{
			return MakePacket(CMD.sys_login);
		}
	}
}
