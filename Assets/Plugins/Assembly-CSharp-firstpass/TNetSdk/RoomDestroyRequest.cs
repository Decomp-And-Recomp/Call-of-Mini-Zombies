namespace TNetSdk
{
	public class RoomDestroyRequest : TNetRequest
	{
		public RoomDestroyRequest()
			: base(RequestType.DestroyRoom)
		{
			Init();
		}

		private void Init()
		{
			packer = new RoomDestroyCmd();
			packet = ((RoomDestroyCmd)packer).MakePacket();
		}
	}
}
