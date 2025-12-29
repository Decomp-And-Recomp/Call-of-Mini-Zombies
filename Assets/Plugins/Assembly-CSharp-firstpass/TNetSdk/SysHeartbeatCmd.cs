namespace TNetSdk
{
	public class SysHeartbeatCmd : SysCmd
	{
		public SysHeartbeatCmd(ushort ping)
		{
			PushUInt16(ping);
		}

		public Packet MakePacket()
		{
			return MakePacket(CMD.sys_heartbeat);
		}
	}
}
