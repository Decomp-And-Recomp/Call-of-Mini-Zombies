namespace TNetSdk
{
	public class RoomStartRequest : TNetRequest
	{
		public RoomStartRequest()
			: base(RequestType.StartRoom)
		{
			Init();
		}

		private void Init()
		{
			packer = new RoomStartReqCmd();
			packet = ((RoomStartReqCmd)packer).MakePacket();
		}
	}
}
