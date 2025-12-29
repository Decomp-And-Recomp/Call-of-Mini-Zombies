namespace TNetSdk
{
	public class HeartBeatRequest : TNetRequest
	{
		public HeartBeatRequest(int ping)
			: base(RequestType.HeartBeat)
		{
			Init(ping);
		}

		private void Init(int ping)
		{
			packer = new SysHeartbeatCmd((ushort)ping);
			packet = ((SysHeartbeatCmd)packer).MakePacket();
		}
	}
}
